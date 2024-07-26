using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoopContainer : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI iNo;
    [SerializeField] RectTransform imageRect;
    public int counter = 0;
    public ArrowDirection arrowDirection;
    private Button upperButton;

    

    void Awake()
    {

        foreach (Transform t in transform)
        {
            if (t.gameObject.name.Equals("Upper"))
            {
                iNo = t.gameObject.GetComponentInChildren<TextMeshProUGUI>();
                upperButton = t.GetComponentInChildren<Button>();
                if (upperButton != null)
                {
                    upperButton.onClick.AddListener(OnUpperButtonClick);
                }
            }
            if (t.gameObject.name.Equals("Image (1)"))
            {
                imageRect = t.GetComponent<RectTransform>();
            }
        }
        UpdateCounterText();

    }

    private void OnUpperButtonClick()
    {
        counter++;
        UpdateCounterText();
    }

    private void UpdateCounterText()
    {
        if (iNo != null)
        {
            iNo.text = counter.ToString();
        }
    }

    public void onObjectDropped(ArrowDirection dir)
    {
        arrowDirection = dir;
        float rot = GetDirectionFloat(dir);
        Debug.Log(rot);
        Vector3 pos = imageRect.localEulerAngles;
        pos.z = rot;
        imageRect.localEulerAngles = pos; 
    }

    private float GetDirectionFloat(ArrowDirection dir)
    {
        switch (dir)
        {
            case ArrowDirection.Up:
                return 90f;
            case ArrowDirection.Down:
                return -90f;
            case ArrowDirection.Left:
                return 180f;
            case ArrowDirection.Right:
                return 0f;
            default:
                throw new System.ArgumentException("Invalid direction", nameof(dir));
        }
    }
}
