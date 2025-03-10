using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickaxeTrigger : MonoBehaviour
{
    [HideInInspector]
    public bool detect = false;
    public float damage;

    private void OnCollisionEnter(Collision collision)
    {
        if (detect && collision.gameObject.GetComponent<OreEntity>() != null)
        {
            detect = false;
            collision.gameObject.GetComponent<OreEntity>().health -= damage;
        }
    }

    public void StopDetecting()
    {
        detect = false;
    }
}
