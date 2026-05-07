using UnityEngine;
using UnityEngine.UI;

public class InventoryDragHandler : MonoBehaviour
{
    [SerializeField] private Image ghostIcon;

    private Canvas canvas;

    private void Awake()
    {
        canvas = GetComponentInParent<Canvas>();
        ghostIcon.gameObject.SetActive(false);
    }

    public void StartDrag(Sprite icon)
    {
        ghostIcon.transform.SetAsLastSibling();
        ghostIcon.sprite = icon;
        ghostIcon.gameObject.SetActive(true);
    }

        public void UpdateDrag(Vector2 position)
        {
            ghostIcon.rectTransform.position = position;
        }

    public void EndDrag()
    {
        ghostIcon.gameObject.SetActive(false);
    }
    
}