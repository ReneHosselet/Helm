using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class OnMouseOverMainMenu : MonoBehaviour,IPointerEnterHandler, IPointerExitHandler
{

    public float maxFontSize = 60f;
    public float minFontSize = 30f;
    private TextMeshProUGUI tmp;

    private void Start()
    {
        tmp = this.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        tmp.fontSize = maxFontSize;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        tmp.fontSize = minFontSize;
    }
}
