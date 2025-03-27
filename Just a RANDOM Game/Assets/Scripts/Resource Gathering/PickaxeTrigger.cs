using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickaxeTrigger : MonoBehaviour
{
    [HideInInspector]
    public bool detect = false;
    [HideInInspector]
    public float damage;

    private void OnCollisionEnter(Collision collision)
    {
        OreEntity entity = collision.gameObject.GetComponent<OreEntity>();
        if (detect && entity != null)
        {
            detect = false;
            entity.TakeDamage(new Damage(damage));
        }
    }

    public void StopDetecting()
    {
        detect = false;
    }
}
