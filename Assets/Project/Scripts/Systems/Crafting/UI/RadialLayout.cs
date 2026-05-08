using UnityEngine;

public class RadialLayout : MonoBehaviour
{
    [SerializeField] private float radius = 100f;
    [SerializeField] private float startAngle = 0f;
    [SerializeField] private float angleStep = 90f; // 4 botones = 360 / 4

    private void Start()
    {
        Arrange();
    }

    public void Arrange()
    {
        int count = transform.childCount;

        for (int i = 0; i < count; i++)
        {
            RectTransform child = transform.GetChild(i) as RectTransform;

            float angle = startAngle + (angleStep * i);
            float rad = angle * Mathf.Deg2Rad;

            Vector2 pos = new Vector2(
                Mathf.Cos(rad) * radius,
                Mathf.Sin(rad) * radius
            );

            child.anchoredPosition = pos;
        }
    }
}