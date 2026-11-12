
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class DraggableRecord : MonoBehaviour,
    IBeginDragHandler,
    IDragHandler,
    IEndDragHandler
{
    [Header("Record Settings")]
    [SerializeField, Range(1, 99)]
    private int digit = 1;

    [Header("References")]
    [SerializeField] private RectTransform dragArea;
    [SerializeField] private RecordDeskArea deskArea;
    [SerializeField] private TMP_Text recordValueText;

    public int Digit => digit;

    private RectTransform rectTransform;
    private Vector3 dragOffset;
    private bool isDragging;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();

        if (dragArea == null || deskArea == null)
        {
            Debug.LogError($"{name}: Missing references!");
            enabled = false;
            return;
        }

        if (rectTransform.parent != dragArea)
        {
            Debug.LogError(
                $"{name} must be a direct child of DragArea."
            );
        }

        recordValueText.text = digit.ToString();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (deskArea == null || deskArea.IsSolved())
            return;

        isDragging = true;

        // Remove this record from its previous stack, if any.
        deskArea.ReleaseRecord(this);

        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            dragArea,
            eventData.position,
            eventData.pressEventCamera,
            out Vector2 pointerPosition))
        {
            dragOffset = rectTransform.localPosition -
                new Vector3(pointerPosition.x, pointerPosition.y, 0);
        }

        rectTransform.SetAsLastSibling();
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!isDragging)
            return;

        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            dragArea,
            eventData.position,
            eventData.pressEventCamera,
            out Vector2 pointerPosition))
        {
            Vector3 targetPosition =
                new Vector3(pointerPosition.x, pointerPosition.y, 0)
                + dragOffset;

            rectTransform.localPosition = ClampToArea(targetPosition);
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!isDragging)
            return;

        isDragging = false;

        deskArea.TryPlaceRecord(this);
    }

    private Vector3 ClampToArea(Vector3 position)
    {
        Rect area = dragArea.rect;
        Rect item = rectTransform.rect;

        float scaleX = rectTransform.localScale.x;
        float scaleY = rectTransform.localScale.y;

        float minX = area.xMin +
            item.width * scaleX * rectTransform.pivot.x;

        float maxX = area.xMax -
            item.width * scaleX * (1 - rectTransform.pivot.x);

        float minY = area.yMin +
            item.height * scaleY * rectTransform.pivot.y;

        float maxY = area.yMax -
            item.height * scaleY * (1 - rectTransform.pivot.y);

        position.x = Mathf.Clamp(position.x, minX, maxX);
        position.y = Mathf.Clamp(position.y, minY, maxY);

        return position;
    }
}
