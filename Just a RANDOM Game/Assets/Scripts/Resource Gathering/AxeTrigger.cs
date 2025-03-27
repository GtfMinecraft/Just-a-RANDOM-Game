using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AxeTrigger : MonoBehaviour
{
    [HideInInspector]
    public bool detect = false;
    [HideInInspector]
    public float damage;

    private void OnCollisionEnter(Collision collision)
    {
        TreeEntity entity = collision.gameObject.GetComponent<TreeEntity>();
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
