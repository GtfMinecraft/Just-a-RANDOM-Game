using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EscController : MonoBehaviour
{
    public static EscController instance;

    public Image[] resumeButton;

    private bool exited = false;

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
        foreach(Image image in resumeButton)
        {
            image.alphaHitTestMinimumThreshold = 0.1f;
        }
    }

    public void Resume()
    {
        InterfaceHandler.instance.CloseAllInterface();
    }

    public void ReturnToTitle()
    {
        if (!exited)
        {
            exited = true;
            Time.timeScale = 1f;
            PlayerController.instance.canRotate = true;
            DataPersistenceManager.instance.SaveGame();
            StartCoroutine(LoadStartingScene());
        }
    }

    private IEnumerator LoadStartingScene()
    {
        yield return new WaitForEndOfFrame();
        //DestoryDontDestroyOnLoad();
        SceneManager.LoadSceneAsync("Starting");
    }

    private void DestoryDontDestroyOnLoad()
    {
        DontDestroyOnLoad[] dontDestroyOnLoadObjs = FindObjectsByType<DontDestroyOnLoad>(FindObjectsSortMode.None);
        foreach (var obj in dontDestroyOnLoadObjs)
        {
            Destroy(obj.gameObject);
        }
    }
}
