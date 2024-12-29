using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ToolWheelUIHover : MonoBehaviour
{
    private Animator anim;
    private bool selected = false;
    [HideInInspector]
    public bool hovered = false;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!selected && hovered)
        {
            selected = true;
            anim.SetBool("Hover", true);
            transform.GetChild(0).gameObject.SetActive(true);
        }
        else if(selected && !hovered)
        {
            selected = false;
            anim.SetBool("Hover", false);
            transform.GetChild(0).gameObject.SetActive(false);
        }
    }
}
