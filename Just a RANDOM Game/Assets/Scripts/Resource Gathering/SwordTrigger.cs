using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordTrigger : MonoBehaviour
{
    [HideInInspector]
    public bool detect = false;
    [HideInInspector]
    public float damage;

    private void OnCollisionEnter(Collision collision)
    {
        Entity entity = collision.gameObject.GetComponent<Entity>();
        if (detect && entity != null && entity.IsMob)
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
