using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EscController : MonoBehaviour
{
    public EscController instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(this);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Canvas>().enabled = false;
    }
}
