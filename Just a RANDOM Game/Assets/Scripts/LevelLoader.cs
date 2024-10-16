using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Newtonsoft.Json;

public class LevelLoader : MonoBehaviour
{
    [SerializeField] private int maxConcurrentLoad;
    [SerializeField] private string assetListName;
    [SerializeField] private GameObject loadingScreenPrefab;

    public LevelDirector director;
    private GameObject progressBarInstance;
    private LoadingProgressDirector progressDirector;
    private LinkedList<(int id, AsyncOperationHandle<GameObject> handle)> handles = new LinkedList<(int id, AsyncOperationHandle<GameObject> handle)>();
    private List<string> assetList;
    private List<GameObject> rootGameObjects;
    private int nextAssetIndex = 0;

    private void Awake()
    {
        // get all root objects in scene
        Scene activeScene = SceneManager.GetActiveScene();
        rootGameObjects = new List<GameObject>();
        activeScene.GetRootGameObjects(rootGameObjects);

        // disable all other objects in scene while loading assets
        foreach (GameObject obj in rootGameObjects)
            obj.SetActive(obj == gameObject);

        progressBarInstance = Instantiate(loadingScreenPrefab);
        progressDirector = progressBarInstance.GetComponent<LoadingProgressDirector>();
        assetList = JsonConvert.DeserializeObject<List<string>>(File.ReadAllText(Path.Combine(Application.persistentDataPath, assetListName + ".dat")));
    }

    private void Update()
    {
        float loadingProgressSum = 0f;
        LoadAssets();
        for(LinkedListNode<(int id, AsyncOperationHandle<GameObject> handle)> node = handles.First; node != null; node = node.Next)
        {
            loadingProgressSum = 0f;

            if (node.Value.handle.Status == AsyncOperationStatus.Succeeded)
            {
                // remove operation from list if asset has finished loading
                LevelInfo.assets.Add(assetList[node.Value.id], node.Value.handle.Result);
                handles.Remove(node);
            }
            else if (node.Value.handle.Status == AsyncOperationStatus.Failed)
                Debug.Log($"asset failed to load: {node.Value.handle}");
            else
                loadingProgressSum += node.Value.handle.PercentComplete;
        }

        progressDirector.SetProgress((loadingProgressSum + nextAssetIndex) / assetList.Count);

        // commit suicide if assets have finished loading
        if (nextAssetIndex >= assetList.Count && handles.Count == 0) {
            director.GenerateLevel();
            // re-enable objects in scene
            foreach (GameObject obj in rootGameObjects)
                obj.SetActive(true);
            Destroy(progressBarInstance);
            Destroy(gameObject);
        }
    }

    private void LoadAssets()
	{
		while (handles.Count <= maxConcurrentLoad && nextAssetIndex < assetList.Count)
        {
            // load next asset
            handles.AddLast((nextAssetIndex, Addressables.LoadAssetAsync<GameObject>(assetList[nextAssetIndex])));
            nextAssetIndex++;
        }
	}
}
