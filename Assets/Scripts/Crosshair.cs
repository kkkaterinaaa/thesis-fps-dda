using UnityEngine;

public class Crosshair : MonoBehaviour
{
    public RectTransform top;
    public RectTransform bottom;
    public RectTransform left;
    public RectTransform right;

    private float baseDistance = 10f;
    private float spread = 0f;
    private float spreadSpeed = 5f;

    void Update()
    {
        // плавное возвращение к исходному размеру прицела
        spread = Mathf.Lerp(spread, 0, Time.deltaTime * spreadSpeed);

        top.anchoredPosition = new Vector2(0, baseDistance + spread);
        bottom.anchoredPosition = new Vector2(0, -(baseDistance + spread));
        left.anchoredPosition = new Vector2(-(baseDistance + spread), 0);
        right.anchoredPosition = new Vector2(baseDistance + spread, 0);
    }

    public void AddSpread(float amount)
    {
        spread += amount;
    }
}