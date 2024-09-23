using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems; // Required for Event Systems
using TMPro; // Required for TextMeshPro

public class NoAccountHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private TextMeshProUGUI buttonText; // Assign this in the inspector
    [SerializeField] private Color normalColor = Color.blue; // Default color
    [SerializeField] private Color hoverColor = Color.red; // Color on hover

    public void OnPointerEnter(PointerEventData eventData)
    {
        // Change the text color to hover color
        buttonText.color = hoverColor;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // Revert the text color to normal
        buttonText.color = normalColor;
    }
}
