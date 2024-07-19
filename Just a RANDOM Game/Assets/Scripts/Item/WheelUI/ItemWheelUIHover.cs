using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemWheelUIHover : MonoBehaviour
{
    private Animator anim;
    private Animator[] subsectionAnim = new Animator[5];
    private bool selected = false;
    [HideInInspector] public bool hovered = false;
    private int subsectionSelected = -1;
    [HideInInspector] public int subsectionHovered = -1;

    private void Start()
    {
        anim = GetComponent<Animator>();
        for (int i = 0; i < subsectionAnim.Length; i++)
        {
            subsectionAnim[i] = transform.GetChild(1).GetChild(i).GetComponent<Animator>();
        }
    }

    private void Update()
    {
        if (!selected && hovered)
        {
            selected = true;
            anim.SetBool("Hover", true);
            transform.GetChild(1).gameObject.SetActive(true);
        }
        else if (selected && !hovered)
        {
            selected = false;
            anim.SetBool("Hover", false);
            transform.GetChild(1).gameObject.SetActive(false);
        }
        else if (selected && hovered)
        {
            if (subsectionSelected != subsectionHovered && subsectionSelected != -1)
            {
                subsectionAnim[subsectionSelected].SetBool("Hover", false);
            }
            subsectionSelected = subsectionHovered;
            if (subsectionSelected != -1)
            {
                subsectionAnim[subsectionSelected].SetBool("Hover", true);
            }
        }
        else if (subsectionSelected != -1)
        {
            subsectionAnim[subsectionSelected].SetBool("Hover", false);
            subsectionSelected = -1;
        }
    }
}
