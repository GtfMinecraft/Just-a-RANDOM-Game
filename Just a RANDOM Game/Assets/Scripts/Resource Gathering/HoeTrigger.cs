using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoeTrigger : MonoBehaviour
{
    [HideInInspector]
    public bool detect = false;
    [HideInInspector]
    public Vector3 range;

    private void OnCollisionEnter(Collision collision)
    {
        if(detect && collision.gameObject.GetComponent<FarmingController>() != null)
        {
            detect = false;
            PlayerItemController.instance.StartFarming(range);
        }
    }

    public void StopDetecting()
    {
        detect = false;
    }
}
