using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ExitGameBanner : StartingBanner
{
    public override void OnPointerClick(PointerEventData eventData)
    {
        Application.Quit();
    }
}
