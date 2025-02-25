using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StoryModeBanner : StartingBanner
{
    public override void OnPointerClick(PointerEventData eventData)
    {
        //TODO: swipe left to the save files -> fade black
        //TODO: set DataPersistenceManager saveFileIndex
        SceneManager.LoadScene("Game");
    }
}
