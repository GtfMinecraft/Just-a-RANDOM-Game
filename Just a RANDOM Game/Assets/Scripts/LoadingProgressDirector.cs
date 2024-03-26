using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingProgressDirector : MonoBehaviour
{
    [SerializeField] private Slider progressBar;

    public void SetProgress(float value) {
        progressBar.value = value;
    }
}
