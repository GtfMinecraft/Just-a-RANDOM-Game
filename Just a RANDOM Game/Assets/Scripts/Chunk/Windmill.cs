using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Windmill : MonoBehaviour
{
    public Vector3 spinDirection = new Vector3(0.079f, 0.007f, 37.697f);
    public float speed;

    // Update is called once per frame
    void Update()
    {
        transform.rotation *= Quaternion.Euler(spinDirection * speed * Time.deltaTime);
    }
}
