
using UnityEngine;
using UnityEngine.EventSystems;

public class DraggableUIItem : MonoBehaviour,
    IBeginDragHandler,
    IDragHandler,
    IEndDragHandler
{
    [Header("References")]
    [SerializeField] private RectTransform dragArea;
    [SerializeField] private GearPuttingArea puttingArea;
    [SerializeField] private ColorChangerForItems colorChangerForItems;

    private RectTransform rectTransform;

    private Vector3 dragOffset;
    private bool isDragging;

    public void Initialize(RectTransform area, GearPuttingArea area1)
    {
        dragArea = area;
        puttingArea = area1;
    }

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();

        if (dragArea == null || puttingArea == null)
        {
            Debug.Log($"{name}: Missing drag references!");
        }

        // This script expects the gear to be a direct child
        // of the drag area.
        if (rectTransform.parent != dragArea)
        {
            Debug.Log(
                $"{name} must be a direct child of Drag Area!"
            );
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (dragArea == null || puttingArea == null)
            return;

        isDragging = true;

        // Free the pivot if this gear was previously snapped.
        puttingArea.ReleaseGear(this);

        // Keep the exact point where the player grabbed the gear.
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            dragArea,
            eventData.position,
            eventData.pressEventCamera,
            out Vector2 localPointer))
        {
            dragOffset = rectTransform.localPosition - 
                         new Vector3(localPointer.x, localPointer.y, 0);
        }

        // Draw the gear above other UI elements while dragging.
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
            out Vector2 localPointer))
        {
            Vector3 targetPosition =
                new Vector3(localPointer.x, localPointer.y, 0)
                + dragOffset;

            targetPosition = ClampToArea(targetPosition);

            rectTransform.localPosition = targetPosition;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!isDragging)
            return;

        isDragging = false;

        // Ask the putting area to find the nearest pivot.
        puttingArea.TrySnapGear(this);
    }

    private Vector3 ClampToArea(Vector3 position)
    {
        Rect areaRect = dragArea.rect;
        Rect itemRect = rectTransform.rect;

        float scaleX = rectTransform.localScale.x;
        float scaleY = rectTransform.localScale.y;

        float minX = areaRect.xMin +
                     itemRect.width * scaleX * rectTransform.pivot.x;

        float maxX = areaRect.xMax -
                     itemRect.width * scaleX * (1 - rectTransform.pivot.x);

        float minY = areaRect.yMin +
                     itemRect.height * scaleY * rectTransform.pivot.y;

        float maxY = areaRect.yMax -
                     itemRect.height * scaleY * (1 - rectTransform.pivot.y);

        position.x = Mathf.Clamp(position.x, minX, maxX);
        position.y = Mathf.Clamp(position.y, minY, maxY);

        return position;
    }

    public void OnSnapped()
    {
        colorChangerForItems.SetSnapStateColor(true);
    }

    public void OnDesnapped()
    {
        colorChangerForItems.SetSnapStateColor(false);
    }
}
