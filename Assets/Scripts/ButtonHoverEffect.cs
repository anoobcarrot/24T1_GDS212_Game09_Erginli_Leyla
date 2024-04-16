using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

public class ButtonHoverEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    private TextMeshProUGUI buttonText;
    private Color originalColor;
    [SerializeField] private Color hoverColor;
    [SerializeField] private Color pressedColor;

    void Start()
    {
        buttonText = GetComponentInChildren<TextMeshProUGUI>();
        if (buttonText != null)
        {
            originalColor = buttonText.color;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (buttonText != null)
        {
            buttonText.color = hoverColor;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (buttonText != null)
        {
            buttonText.color = originalColor;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (buttonText != null)
        {
            buttonText.color = pressedColor;
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (buttonText != null)
        {
            buttonText.color = hoverColor;
        }
    }
}
