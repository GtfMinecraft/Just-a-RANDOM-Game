using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutsceneController : MonoBehaviour
{
    public static CutsceneController instance;

    private int cutsceneIndex = -1;

    public void PlayNextCutscene()
    {
        cutsceneIndex++;
        //playcutscene
    }
}
