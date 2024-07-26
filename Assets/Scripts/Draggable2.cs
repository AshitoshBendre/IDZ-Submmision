using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class Draggable2 : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    public ArrowDirection direction;
    public bool isDraggable = true;
    public static event Action<ArrowDirection> onObjectPlaced;
    public string destinationTag = "LoopButton";
    public string containerTag = "LoopContainer";
    public bool isLoopObject = false;
    public Transform placeholderParent; 
    public List<GameObject> placeholderList;
    bool droppedOnPlaceholder = false;

    private Vector3 offset;
    private RectTransform rectTransform;
    private Canvas canvas;
    private CanvasGroup canvasGroup;
    private Vector2 startPosition;
    private GameObject copiedObject; 

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

            // Create a copy of the GameObject and set it to the same position as the original one
            copiedObject = Instantiate(gameObject, rectTransform.position, Quaternion.identity, placeholderParent);
            copiedObject.transform.SetAsLastSibling(); // Ensure it is on top of other UI elements
            SetupCopiedObject(copiedObject);

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

        droppedOnPlaceholder = false;

        foreach (RaycastResult result in results)
        {
            if (result.gameObject.CompareTag("Placeholder"))
            {
                Debug.Log($"Found Placeholder: {result.gameObject.name}");
                if(result.gameObject.transform.childCount>0)
                {
                    foreach(Transform child in result.gameObject.transform)
                    {
                        Destroy(child.gameObject);
                    }
                }
                RectTransform placeholderRect = result.gameObject.GetComponent<RectTransform>();
                SequenceContainer sequenceContainer = result.gameObject.GetComponent<SequenceContainer>();
                if (copiedObject != null)
                {
                    copiedObject.transform.SetParent(placeholderRect);
                    copiedObject.transform.localPosition = Vector3.zero;
                    sequenceContainer.onObjectDropped(direction);
                }
                droppedOnPlaceholder = true;
                break;
            }
            Destroy(copiedObject.GetComponent<Draggable2>());
        }

        if (!droppedOnPlaceholder && copiedObject != null)
        {
            Destroy(copiedObject); // Destroy if not dropped on a placeholder
        }

        rectTransform.anchoredPosition = startPosition;

    }
    private void SetupCopiedObject(GameObject copiedObject)
    {
        // Add necessary components to the copied object to handle dragging
        var draggable = copiedObject.AddComponent<Draggable2Copied>();
        draggable.placeholderParent = placeholderParent;
        draggable.placeholderList = placeholderList;
        var copiedCanvasGroup = copiedObject.GetComponent<CanvasGroup>();
        if (copiedCanvasGroup == null)
        {
            copiedCanvasGroup = copiedObject.AddComponent<CanvasGroup>();
        }
        copiedCanvasGroup.blocksRaycasts = true;
        copiedCanvasGroup.interactable = true;
    }

    private Vector3 MouseWorldPosition(PointerEventData eventData)
    {
        Vector3 worldPoint;
        RectTransformUtility.ScreenPointToWorldPointInRectangle(rectTransform, eventData.position, eventData.pressEventCamera, out worldPoint);
        return worldPoint;
    }
}
