using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoeTrigger : MonoBehaviour
{
    [HideInInspector]
    public bool detect = false;

    private void OnCollisionEnter(Collision collision)
    {
        if(detect && collision.gameObject.GetComponent<FarmingController>() != null)
        {
            detect = false;
            PlayerItemController.instance.StartFarming();
        }
    }
}
