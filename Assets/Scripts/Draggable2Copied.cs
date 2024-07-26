using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;

public class Draggable2Copied : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    public Transform placeholderParent;
    public List<GameObject> placeholderList;

    private Vector3 offset;
    private RectTransform rectTransform;
    private Canvas canvas;
    private CanvasGroup canvasGroup;
    private bool droppedOnPlaceholder = false;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("Pointer Down Copied Object");
        offset = rectTransform.position - MouseWorldPosition(eventData);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log("Begin Drag Copied Object");
        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        Debug.Log("Dragging Copied Object");
        rectTransform.position = MouseWorldPosition(eventData) + offset;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log("End Drag Copied Object");
        canvasGroup.blocksRaycasts = true;

        // Get all UI elements under the drop position
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);

        droppedOnPlaceholder = false;

        foreach (RaycastResult result in results)
        {
            if (result.gameObject.CompareTag("Placeholder"))
            {
                Debug.Log($"Found Placeholder for Copied Object: {result.gameObject.name}");
                RectTransform placeholderRect = result.gameObject.GetComponent<RectTransform>();
                transform.SetParent(placeholderRect);
                transform.localPosition = Vector3.zero; // Center the copied object in the placeholder
                droppedOnPlaceholder = true;
                break;
            }
        }

        if (!droppedOnPlaceholder)
        {
            Destroy(gameObject); // Destroy if not dropped on a placeholder
        }
    }

    private Vector3 MouseWorldPosition(PointerEventData eventData)
    {
        Vector3 worldPoint;
        RectTransformUtility.ScreenPointToWorldPointInRectangle(rectTransform, eventData.position, eventData.pressEventCamera, out worldPoint);
        return worldPoint;
    }
}