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

public class StoryModeBanner : StartingBanner, IDataPersistence
{
    public TMP_Text loading;
    public float loadingDotSpeed;

    private AsyncOperationHandle<SceneInstance> gameScene;
    private bool finishLoading = false;
    private bool activateOnce = true;
    AsyncOperation activation;
    private float time = 0;

    private ChunkTypes chunk = ChunkTypes.Logging;

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
            if (time >= loadingDotSpeed)
            {
                time = 0;
                if (loading.text == "Loading..." || loading.text == "")
                    loading.text = "Loading";
                else
                    loading.text += '.';
            }

            if (finishLoading && activateOnce)
            {
                activateOnce = false;
                activation = gameScene.Result.ActivateAsync();
                DataPersistenceManager.firstLoad = true;
            }

            if (activation.isDone)
            {
                SceneManager.SetActiveScene(gameScene.Result.Scene);
                DataPersistenceManager.instance.SceneLoad();
                SceneManager.UnloadSceneAsync("Starting");
            }
        }
    }

    async void LoadScenes()
    {
        //load chunk
        //await Addressables.LoadSceneAsync("Maps", LoadSceneMode.Additive).Task;

        gameScene = Addressables.LoadSceneAsync("PlayerScene", LoadSceneMode.Additive, false);
        await gameScene.Task;

        finishLoading = true;
    }

    public override void OnPointerClick(PointerEventData eventData)
    {
        //TODO: swipe left to the save files -> fade black
        //TODO: set DataPersistenceManager saveFileIndex
        loading.enabled = true;
    }

    public void LoadData(GameData data)
    {
        chunk = data.statisticsData.playerChunk;
    }

    public void SaveData(GameData data)
    {

    }
}
