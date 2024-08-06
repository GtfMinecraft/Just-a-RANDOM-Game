using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlashAttack : MonoBehaviour
{
    public GameObject slash;
    private bool isSlashing = true;

    private void Start()
    {
        slash.SetActive(false);
        isSlashing = false;
    }

    private void Update()
    {
        if(Input.GetKeyUp(KeyCode.Space) && isSlashing == false)
        {
            slash.SetActive(true);
            isSlashing = true;
            StartCoroutine(DisableSlash());
        }
    }

    [ContextMenu("Slash")]
    void Slash()
    {
        slash.SetActive(false);
        isSlashing = false;
        StartCoroutine(DisableSlash());
    }

    IEnumerator DisableSlash()
    {
        yield return new WaitForSeconds(1);
        if (isSlashing == true)
        {
            slash.SetActive(false);
            isSlashing = false;
        }
        else
        {
            slash.SetActive(true);
            isSlashing = true;
        }
    }
}
