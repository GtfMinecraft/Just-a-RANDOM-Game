using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FishingController : MonoBehaviour
{
    public Camera playerCam;
    public GameObject fishingCanvasPrefab;

    [Header("Fishing")]
    public UDictionaryIntInt fishedItemChance;
    public float[] successRange = new float[2]; //posX 9.5 ~ 149.5 width 0 ~ 140
    public float bufferRange;
    public float swipeSpeed;

    private Canvas canvas;
    private Slider slider;
    private float range;
    private float startPosition;
    private float elapsedTime;

    private void Start()
    {
        playerCam = PlayerController.instance.playerCam;
    }

    // Update is called once per frame
    void Update()
    {
        if(canvas != null && slider != null)
        {
            elapsedTime = 0;
            canvas.transform.rotation = Quaternion.LookRotation(-playerCam.transform.forward, playerCam.transform.up);
            elapsedTime += Time.deltaTime;
            slider.value = (Mathf.Sin(elapsedTime * swipeSpeed) + 1) / 2;
        }
    }

    // Start is called before the first frame update
    public void StartFishing(Vector3 bobberPos)
    {
        GameObject canvas = Instantiate(fishingCanvasPrefab, transform);
        canvas.GetComponent<Canvas>().worldCamera = playerCam;
        canvas.transform.position = bobberPos;
        slider = canvas.transform.GetChild(0).GetComponent<Slider>();
        range = Random.Range(successRange[0], successRange[1]);
        startPosition = Random.Range(9.5f, 149.5f - range);
        slider.transform.GetChild(1).transform.localPosition = new Vector3(startPosition, 0, 0);
        slider.transform.GetChild(1).GetComponent<RectTransform>().sizeDelta = new Vector2(range, 20);
    }

    public int StopFishing()
    {
        Destroy(canvas);

        if (slider.value >= (startPosition - 9.5)/140 - bufferRange && slider.value <= (startPosition + range) / 140 + bufferRange)
        {
            int totalWeight = 0;
            foreach(KeyValuePair<int, int> entry in fishedItemChance)
            {
                totalWeight += entry.Value;
            }

            int itemWeight = Random.Range(0, totalWeight);
            foreach (KeyValuePair<int, int> entry in fishedItemChance)
            {
                itemWeight -= entry.Value;
                if (itemWeight < 0)
                {
                    Debug.Log(entry.Key);
                    return entry.Key;
                }
            }
        }
        Debug.Log(0);
        return 0;
    }
}
