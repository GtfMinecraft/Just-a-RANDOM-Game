using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAction : MonoBehaviour
{
    public GameObject BOT;
    public Item hold = null;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {

        }
        if(Input.GetKeyDown(KeyCode.C))
        {
            BOT.GetComponent<BOTMovement>().Call();
        }
        if (Input.GetKeyDown(KeyCode.Q) && hold != null)
        {
            Drop();
            hold = null;
        }
    }

    public void PickUp()
    {
        //pickup animation
    }

    public void Drop()
    {
        //drop animation
    }
}
