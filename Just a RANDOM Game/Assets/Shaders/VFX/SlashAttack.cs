using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class SlashAttack : MonoBehaviour
{
    public VisualEffect vfx;

    private void Update()
    {
        if(Input.GetKeyUp(KeyCode.Space))
        {
            Slash();
        }
    }

    [ContextMenu("Slash")]
    void Slash()
    {
        vfx.Play();
    }
}
