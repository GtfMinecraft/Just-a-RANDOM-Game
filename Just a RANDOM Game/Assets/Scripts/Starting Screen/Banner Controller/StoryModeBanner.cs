using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.EventSystems;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StoryModeBanner : StartingBanner
{
    public TMP_Text loading;
    public float loadingDotSpeed;

    private AsyncOperationHandle<SceneInstance> gameScene;
    private bool finishLoading = false;
    private bool activateOnce = false;
    private float time = 0;

    private void Start()
    {
        loading.enabled = false;
        LoadScenes();
    }

    private void Update()
    {
        if (loading.enabled)
        {
            time += Time.deltaTime;
            if(time >= loadingDotSpeed)
            {
                time = 0;
                if (loading.text == "Loading...")
                    loading.text = "Loading";
                else
                    loading.text += '.';
            }

            if (finishLoading && activateOnce)
            {
                activateOnce = false;
                gameScene.Result.ActivateAsync();
            }
        }
    }

    async void LoadScenes()
    {
        //var mapsScene = Addressables.LoadSceneAsync("Maps", LoadSceneMode.Additive);
        //await mapsScene.Task;

        gameScene = Addressables.LoadSceneAsync("PlayerScene", LoadSceneMode.Single, false);
        await gameScene.Task;

        finishLoading = true;
    }

    public override void OnPointerClick(PointerEventData eventData)
    {
        //TODO: swipe left to the save files -> fade black
        //TODO: set DataPersistenceManager saveFileIndex
        loading.enabled = true;
    }
}
