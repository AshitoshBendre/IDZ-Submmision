

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class Draggable : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    public ArrowDirection direction;
    public float rot;
    public bool isDraggable = true;
    public UnityEvent onObjectPlaced;
    public string destinationTag = "LoopButton";
    public string containerTag = "LoopContainer";
    public bool isLoopObject = false;

    private Vector3 offset;
    private RectTransform rectTransform;
    private Canvas canvas;
    private CanvasGroup canvasGroup;
    public Vector2 startPosition;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        canvasGroup = GetComponent<CanvasGroup>();
        startPosition = rectTransform.anchoredPosition;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("Pointer Down");
        offset = rectTransform.position - MouseWorldPosition(eventData);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (isDraggable)
        {
            Debug.Log("Begin Drag");
            canvasGroup.blocksRaycasts = false;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (isDraggable)
        {
            Debug.Log("Dragging");
            rectTransform.position = MouseWorldPosition(eventData) + offset;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log("End Drag");
        canvasGroup.blocksRaycasts = true;
        // Get all UI elements under the drop position
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);

        
        
        //Collider2D[] hits = Physics2D.OverlapPointAll(Camera.main.ScreenToWorldPoint(eventData.position));
        
        foreach (RaycastResult result in results)
        {
            if (result.gameObject.CompareTag(destinationTag) && !isLoopObject)
            {
                Debug.Log($"Found Drop Zone: {result.gameObject.name}");
                rectTransform.position = result.gameObject.GetComponent<RectTransform>().position;
                LoopContainer container = result.gameObject.GetComponent<LoopContainer>();
                container.onObjectDropped(direction);
                //onObjectPlaced.Invoke();
                break;
            }
            if (isLoopObject && result.gameObject.CompareTag(containerTag))
            {
                Debug.Log($"Found Game Object: {result.gameObject.name}");
                LoopManager lm = result.gameObject.GetComponent<LoopManager>();
                lm.EnableObject();
                break;
            }
        }
        rectTransform.anchoredPosition = startPosition;
    }

    private Vector3 MouseWorldPosition(PointerEventData eventData)
    {
        Vector3 worldPoint;
        RectTransformUtility.ScreenPointToWorldPointInRectangle(rectTransform, eventData.position, eventData.pressEventCamera, out worldPoint);
        return worldPoint;
    }
}
