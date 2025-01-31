using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FishingController : MonoBehaviour
{
    public GameObject fishingCanvasPrefab;
    public UDictionaryIntInt fishedItemChance;
    public float[] fishHookTime = new float[2];
    public float[] successRange = new float[2]; //posX 9.5 ~ 149.5 width 0 ~ 140
    public float bufferRange;
    public float swipeSpeed;

    private Camera playerCam;

    private Canvas canvas;
    private Slider slider;
    private float range;
    private float startPosition;
    private float elapsedTime = 0;

    private void Start()
    {
        playerCam = PlayerController.instance.playerCam;
    }

    // Update is called once per frame
    void Update()
    {
        if(canvas != null && slider != null)
        {
            canvas.transform.rotation = Quaternion.LookRotation(-playerCam.transform.forward, playerCam.transform.up);
            elapsedTime += Time.deltaTime;
            slider.value = (Mathf.Sin(elapsedTime * swipeSpeed) + 1) / 2;
        }
    }

    public void ShowFishCanvas(Vector3 bobberPos)
    {
        //fish bubbling towards player vfx
        canvas = Instantiate(fishingCanvasPrefab, transform).GetComponent<Canvas>();
        canvas.worldCamera = playerCam;
        canvas.transform.position = bobberPos + Vector3.up * 2.5f;
        canvas.transform.localScale = Vector3.one * 0.015f;
        slider = canvas.transform.GetChild(0).GetComponent<Slider>();
        range = Random.Range(successRange[0], successRange[1]) * 140;
        startPosition = Random.Range(9.5f, 149.5f - range);
        slider.transform.GetChild(1).GetComponent<RectTransform>().anchoredPosition3D = new Vector3(startPosition, 0, 0);
        slider.transform.GetChild(1).GetComponent<RectTransform>().sizeDelta = new Vector2(range, 20);
    }

    public void StopFishing()
    {
        if (canvas == null || slider == null)
        {
            return;
        }

        if (slider.value >= (startPosition - 9.5) / 140 - bufferRange && slider.value <= (startPosition + range) / 140 + bufferRange)
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
                    //disable fish bubbling vfx
                    Debug.Log("success");//instantiate item through the drop controller and play add into inventory anim if auto pickup
                    break;
                }
            }
        }

        Destroy(canvas.gameObject);
    }
}
