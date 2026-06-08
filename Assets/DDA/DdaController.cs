using System;
using System.IO;
using UnityEngine;

public class DdaController : MonoBehaviour
{
    [Serializable]
    private class LinUcbState
    {
        public int d;
        public float[] Aflat;
        public float[] b;
        public int totalRuns;
        public float[] lastActionValues;
    }

    public string modelKey = "linucb_dda_v1";
    public LinUcbSettings settings;
    public BoxActionSpaceConfig actionSpace;
    public RewardFromJson reward;

    public DdaAction CurrentAction { get; private set; }

    private System.Random rng;
    private LinUcbState state;
    private bool initialized;

    void Awake()
    {
        InitRng();
    }

    public DdaAction BeginRun()
    {
        EnsureInitialized();
        if (actionSpace == null || settings == null)
            return null;

        DdaAction action = SelectAction();
        CurrentAction = action;
        return action;
    }

    public void EndRun(string telemetryJson)
    {
        EnsureInitialized();
        if (reward == null || CurrentAction == null)
            return;

        float r = Mathf.Clamp01(reward.Evaluate(telemetryJson));
        UpdateModel(CurrentAction, r);
        SaveState();
    }

    public void ResetCurrentPlayerModel()
    {
        EnsureInitialized();

        try
        {
            string path = GetStatePath();
            if (File.Exists(path))
                File.Delete(path);
        }
        catch
        {
        }

        state = CreateDefaultState();
        SaveState();
    }

    private void EnsureInitialized()
    {
        if (initialized && state != null)
            return;

        initialized = true;
        LoadState();

        if (state == null)
        {
            state = CreateDefaultState();
            SaveState();
        }
    }

    private void InitRng()
    {
        int seed = (settings != null) ? settings.seed : 0;
        rng = seed == 0 ? new System.Random() : new System.Random(seed);
    }

    private LinUcbState CreateDefaultState()
    {
        int k = actionSpace != null ? actionSpace.DimensionCount : 0;
        int d = 1 + k;

        float lambda = settings != null ? settings.lambda : 1f;
        var Aflat = new float[d * d];
        for (int i = 0; i < d; i++)
            Aflat[i * d + i] = lambda;

        return new LinUcbState
        {
            d = d,
            Aflat = Aflat,
            b = new float[d],
            totalRuns = 0,
            lastActionValues = new float[k]
        };
    }

    private DdaAction SelectAction()
    {
        int k = actionSpace.DimensionCount;
        int d = state.d;

        float[,] A = Unflatten(state.Aflat, d);
        float[,] Ainv = InvertMatrix(A);
        float[] theta = MulMatVec(Ainv, state.b);

        int K = Mathf.Max(1, settings.candidateCount);
        int globalCount = Mathf.Clamp(Mathf.RoundToInt(K * settings.globalCandidateRatio), 0, K);
        int localCount = Mathf.Clamp(Mathf.RoundToInt(K * settings.localCandidateRatio), 0, K - globalCount);
        int anchorCount = Mathf.Clamp(settings.anchorCandidateCount, 0, K - globalCount - localCount);
        int fillCount = K - globalCount - localCount - anchorCount;

        float bestScore = float.NegativeInfinity;
        float[] bestValues = null;

        TryCandidates(globalCount, () => SampleGlobal(k), theta, Ainv, ref bestScore, ref bestValues);
        TryCandidates(localCount, () => SampleLocal(k), theta, Ainv, ref bestScore, ref bestValues);
        TryCandidates(anchorCount, () => SampleAnchor(k), theta, Ainv, ref bestScore, ref bestValues);
        TryCandidates(fillCount, () => SampleGlobal(k), theta, Ainv, ref bestScore, ref bestValues);

        if (bestValues == null)
            bestValues = SampleGlobal(k);

        state.lastActionValues = (float[])bestValues.Clone();

        string[] keys = new string[k];
        for (int i = 0; i < k; i++)
            keys[i] = actionSpace.dimensions[i].key;

        return new DdaAction(keys, bestValues);
    }

    private void TryCandidates(int count, Func<float[]> sampler, float[] theta, float[,] Ainv, ref float bestScore, ref float[] bestValues)
    {
        if (count <= 0) return;

        for (int c = 0; c < count; c++)
        {
            float[] values = sampler();
            float[] x = BuildFeatureVector(values);

            float mean = Dot(theta, x);
            float var = QuadraticForm(Ainv, x);
            float score = mean + settings.alpha * Mathf.Sqrt(Mathf.Max(0f, var));

            if (score > bestScore)
            {
                bestScore = score;
                bestValues = values;
            }
        }
    }

    private float[] BuildFeatureVector(float[] values)
    {
        int k = actionSpace.DimensionCount;
        var x = new float[1 + k];
        x[0] = 1f;

        for (int i = 0; i < k; i++)
        {
            var dim = actionSpace.dimensions[i];
            float v = Mathf.Clamp(values[i], dim.min, dim.max);
            if (dim.normalizeToUnit)
                v = (v - dim.min) / Mathf.Max(0.0001f, dim.max - dim.min);
            x[1 + i] = v;
        }

        return x;
    }

    private float[] SampleGlobal(int k)
    {
        var values = new float[k];
        for (int i = 0; i < k; i++)
        {
            var dim = actionSpace.dimensions[i];
            values[i] = Lerp(dim.min, dim.max, (float)rng.NextDouble());
        }
        return values;
    }

    private float[] SampleLocal(int k)
    {
        if (state.lastActionValues == null || state.lastActionValues.Length != k)
            return SampleGlobal(k);

        var values = new float[k];
        for (int i = 0; i < k; i++)
        {
            var dim = actionSpace.dimensions[i];
            float baseV = state.lastActionValues[i];
            float noise = NextGaussian() * dim.localStdDev;
            values[i] = Mathf.Clamp(baseV + noise, dim.min, dim.max);
        }
        return values;
    }

    private float[] SampleAnchor(int k)
    {
        if (actionSpace != null && actionSpace.anchors != null && actionSpace.anchors.Length > 0)
        {
            int tries = 8;
            for (int t = 0; t < tries; t++)
            {
                int idx = rng.Next(0, actionSpace.anchors.Length);
                var a = actionSpace.anchors[idx];
                if (a == null || a.values == null || a.values.Length != k)
                    continue;

                var values = new float[k];
                for (int i = 0; i < k; i++)
                {
                    var dim = actionSpace.dimensions[i];
                    values[i] = Mathf.Clamp(a.values[i], dim.min, dim.max);
                }
                return values;
            }
        }

        var fallback = new float[k];
        for (int i = 0; i < k; i++)
        {
            var dim = actionSpace.dimensions[i];
            fallback[i] = Mathf.Clamp(1.0f, dim.min, dim.max);
        }
        return fallback;
    }

    private void UpdateModel(DdaAction action, float rewardValue)
    {
        int k = actionSpace.DimensionCount;
        int d = state.d;

        float[] values = new float[k];
        for (int i = 0; i < k; i++)
            values[i] = action.Get(actionSpace.dimensions[i].key, 1f);

        float[] x = BuildFeatureVector(values);

        float gamma = settings != null ? settings.forgettingFactor : 1f;
        gamma = Mathf.Clamp(gamma, 0.90f, 1.0f);

        if (gamma < 0.999999f)
        {
            for (int i = 0; i < state.Aflat.Length; i++)
                state.Aflat[i] *= gamma;

            for (int i = 0; i < state.b.Length; i++)
                state.b[i] *= gamma;
        }

        for (int row = 0; row < d; row++)
        {
            for (int col = 0; col < d; col++)
            {
                state.Aflat[row * d + col] += x[row] * x[col];
            }
        }

        for (int i = 0; i < d; i++)
            state.b[i] += rewardValue * x[i];

        state.totalRuns++;
    }

    private void LoadState()
    {
        state = null;

        try
        {
            string path = GetStatePath();
            if (!File.Exists(path))
                return;

            string json = File.ReadAllText(path);
            state = JsonUtility.FromJson<LinUcbState>(json);
        }
        catch
        {
            state = null;
        }

        if (state == null)
            return;

        int expectedD = 1 + (actionSpace != null ? actionSpace.DimensionCount : 0);
        if (state.d != expectedD || state.Aflat == null || state.b == null)
        {
            state = null;
            return;
        }

        if (state.Aflat.Length != state.d * state.d || state.b.Length != state.d)
        {
            state = null;
            return;
        }
    }

    private void SaveState()
    {
        try
        {
            string path = GetStatePath();
            Directory.CreateDirectory(Path.GetDirectoryName(path));
            string json = JsonUtility.ToJson(state, true);
            File.WriteAllText(path, json);
        }
        catch
        {
        }
    }

    private string GetStatePath()
    {
        string folder = Path.Combine(Application.persistentDataPath, "dda", modelKey);
        return Path.Combine(folder, "model.json");
    }

    private static float Dot(float[] a, float[] b)
    {
        int n = Mathf.Min(a.Length, b.Length);
        float s = 0f;
        for (int i = 0; i < n; i++)
            s += a[i] * b[i];
        return s;
    }

    private static float QuadraticForm(float[,] M, float[] x)
    {
        int d = x.Length;
        float s = 0f;
        for (int i = 0; i < d; i++)
        {
            float row = 0f;
            for (int j = 0; j < d; j++)
                row += M[i, j] * x[j];
            s += x[i] * row;
        }
        return s;
    }

    private static float[] MulMatVec(float[,] M, float[] v)
    {
        int d = v.Length;
        var outV = new float[d];
        for (int i = 0; i < d; i++)
        {
            float s = 0f;
            for (int j = 0; j < d; j++)
                s += M[i, j] * v[j];
            outV[i] = s;
        }
        return outV;
    }

    private static float[,] Unflatten(float[] flat, int d)
    {
        var M = new float[d, d];
        for (int i = 0; i < d; i++)
            for (int j = 0; j < d; j++)
                M[i, j] = flat[i * d + j];
        return M;
    }

    private static float[,] InvertMatrix(float[,] A)
    {
        int n = A.GetLength(0);
        var aug = new float[n, n * 2];

        for (int i = 0; i < n; i++)
        {
            for (int j = 0; j < n; j++)
                aug[i, j] = A[i, j];
            aug[i, n + i] = 1f;
        }

        for (int col = 0; col < n; col++)
        {
            int pivotRow = col;
            float pivotAbs = Mathf.Abs(aug[pivotRow, col]);
            for (int r = col + 1; r < n; r++)
            {
                float v = Mathf.Abs(aug[r, col]);
                if (v > pivotAbs)
                {
                    pivotAbs = v;
                    pivotRow = r;
                }
            }

            if (pivotAbs < 1e-8f)
                return Identity(n);

            if (pivotRow != col)
            {
                for (int j = 0; j < n * 2; j++)
                {
                    float tmp = aug[col, j];
                    aug[col, j] = aug[pivotRow, j];
                    aug[pivotRow, j] = tmp;
                }
            }

            float pivot = aug[col, col];
            for (int j = 0; j < n * 2; j++)
                aug[col, j] /= pivot;

            for (int r = 0; r < n; r++)
            {
                if (r == col) continue;
                float factor = aug[r, col];
                if (Mathf.Abs(factor) < 1e-8f) continue;
                for (int j = 0; j < n * 2; j++)
                    aug[r, j] -= factor * aug[col, j];
            }
        }

        var inv = new float[n, n];
        for (int i = 0; i < n; i++)
            for (int j = 0; j < n; j++)
                inv[i, j] = aug[i, n + j];
        return inv;
    }

    private static float[,] Identity(int n)
    {
        var I = new float[n, n];
        for (int i = 0; i < n; i++)
            I[i, i] = 1f;
        return I;
    }

    private static float Lerp(float a, float b, float t)
    {
        return a + (b - a) * t;
    }

    private float NextGaussian()
    {
        double u1 = 1.0 - rng.NextDouble();
        double u2 = 1.0 - rng.NextDouble();
        double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2);
        return (float)randStdNormal;
    }
}
