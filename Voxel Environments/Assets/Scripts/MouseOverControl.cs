using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MouseOverControl : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public FreeFlyCamera cameraScript;

    public void OnPointerEnter(PointerEventData eventData)
    {
        if(cameraScript != null)
        {
            cameraScript.enabled = false;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (cameraScript != null)
        {
            cameraScript.enabled = true;
        }
    }
}
