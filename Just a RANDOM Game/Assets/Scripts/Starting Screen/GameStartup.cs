using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStartup : MonoBehaviour
{
    [Header("Camera")]
    public float turnSpeed=10;
    public float fadeInSpeed=0.5f;

    [Header("Start")]
    public Canvas canvas;
    public Canvas starter;
    public AudioSource music;
    public GameObject scene;

    [HideInInspector]
    public bool played = false;
    private bool once = true;
    private bool returning = false;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = false;
        starter.enabled = true;
        canvas.enabled = false;

        if (Time.time > 10)
        {
            returning = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if ((Input.anyKeyDown&&played || returning)&&once)
        {
            music.Play();
            starter.GetComponent<StarterController>().enabled = false;
            Cursor.visible = true;
            once = false;
            starter.enabled = false;
            scene.SetActive(true);
            canvas.enabled = true;
            canvas.GetComponent<CanvasGroup>().alpha = 0;
        }
        if (canvas.enabled == true && canvas.GetComponent<CanvasGroup>().alpha < 1)
        {
            canvas.GetComponent<CanvasGroup>().alpha += Time.deltaTime*fadeInSpeed;
        }
        if(once==false)
            transform.rotation *= Quaternion.Euler(0, turnSpeed*Time.deltaTime, 0);
    }
}
