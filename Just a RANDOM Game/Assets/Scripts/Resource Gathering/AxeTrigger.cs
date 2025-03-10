using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AxeTrigger : MonoBehaviour
{
    [HideInInspector]
    public bool detect = false;
    public float damage;

    private void OnCollisionEnter(Collision collision)
    {
        if (detect && collision.gameObject.GetComponent<TreeEntity>() != null)
        {
            detect = false;
            collision.gameObject.GetComponent<TreeEntity>().health -= damage;
        }
    }

    public void StopDetecting()
    {
        detect = false;
    }
}
