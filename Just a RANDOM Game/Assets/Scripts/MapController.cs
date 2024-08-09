using System.Collections;
using System.Collections.Generic;
using UnityEngine;
<<<<<<< Updated upstream

public class MapController : MonoBehaviour
{
    public Transform mapCanvas;
    public GameObject Map, Icon, Beacon;
    private List<GameObject> beacons = new List<GameObject>();
    // Start is called before the first frame update
    void Start()
    { 
        Map.SetActive(false);
        Icon.SetActive(false);
        Beacon.SetActive(false);
    }

=======
using UnityEngine.UI;
using TMPro;

public class MapController : MonoBehaviour
{
    public CanvasScaler canvasScaler;
    public float zoomSpeed = 0.1f;
    public float minScale = 0.5f;
    public float maxScale = 2.0f;
    public Transform mapCanvas;
    public GameObject Map, Icon, Beacon, choicePanel;
    public List<GameObject> beacons = new List<GameObject>();
    public bool[] unlockedChunks;
    public Button confirmButton;
    public Button cancelButton;
    public Button removeButton;
    public TMP_Dropdown dropdown;
    public Color highlight = Color.yellow;
    private GameObject highlightedBeacon;
    private Color originalColor;
    private string confirmbuttonText = "Confirm", cancelbuttonText = "Cancel", removebuttonText = "Remove";
    // Start is called before the first frame update
    void Start()
    { 
        // mapCamera = Camera.main;
        Map.SetActive(false);
        Icon.SetActive(false);
        Beacon.SetActive(false);
        choicePanel.SetActive(false);
        confirmButton.onClick.AddListener(OnConfirmButtonClick);
        cancelButton.onClick.AddListener(OnCancelButtonClick);
        removeButton.onClick.AddListener(OnRemoveButtonClick);
        dropdown.onValueChanged.AddListener(OnDropdownValueChanged);
    }
    void ShowChoiceBox()
    {
        choicePanel.SetActive(true);
        confirmButton.transform.position = new Vector3(70, 40, 0);
        cancelButton.transform.position = new Vector3(70, 10, 0);
        removeButton.transform.position = new Vector3(70, 70, 0);
        confirmButton.GetComponentInChildren<TextMeshProUGUI>().text = confirmbuttonText;
        cancelButton.GetComponentInChildren<TextMeshProUGUI>().text = cancelbuttonText;
        removeButton.GetComponentInChildren<TextMeshProUGUI>().text = removebuttonText;
        PopulateDropdown();
    }
    void PopulateDropdown()
    {
        dropdown.ClearOptions();
        List<string> options = new List<string>();

        for (int i = 1; i <= beacons.Count; i++)
        {
            options.Add("Beacon " + i);
        }

        dropdown.AddOptions(options);

    }
    void OnDropdownValueChanged(int value)
    {
        int selectedIndex = dropdown.value;
        GameObject selectedBeacon = beacons[selectedIndex];
        HighlightBeacon(selectedBeacon);
    }
    void OnConfirmButtonClick()
    {
        int selectedIndex = dropdown.value;
        GameObject selectedBeacon = beacons[selectedIndex];
        ConfirmSelection(selectedBeacon);
    }
    void OnRemoveButtonClick()
    {
        if(highlightedBeacon != null)
        {
            beacons.Remove(highlightedBeacon);
            Destroy(highlightedBeacon);
            highlightedBeacon = null;
            PopulateDropdown();
        }
    }
    void OnCancelButtonClick()
    {
        choicePanel.SetActive(false); 

        if (highlightedBeacon != null)
        {
            RawImage prevImage = highlightedBeacon.GetComponent<RawImage>();
            if (prevImage != null)
            {
                prevImage.color = originalColor;
            }
            highlightedBeacon = null;
        }
    }
    void HighlightBeacon(GameObject beacon)
    {
        if (highlightedBeacon != null)
        {
            RawImage prevImage = highlightedBeacon.GetComponent<RawImage>();
            if (prevImage != null)
            {
                prevImage.color = originalColor;
            }
        }

        highlightedBeacon = beacon;
        RawImage rawImage = beacon.GetComponent<RawImage>();
        if (rawImage != null)
        {
            originalColor = rawImage.color;
            rawImage.color = highlight;
        }
    }
    private void ConfirmSelection(GameObject beacon)
    {
        choicePanel.SetActive(false); 
        Icon.transform.position = beacon.transform.position;
        Debug.Log("you've selected a beacon");
        if (highlightedBeacon != null)
        {
            RawImage prevImage = highlightedBeacon.GetComponent<RawImage>();
            if (prevImage != null)
            {
                prevImage.color = originalColor;
            }
            highlightedBeacon = null;
        }
    }
>>>>>>> Stashed changes
    void PlaceBeacon()
    {
        GameObject newBeacon = Instantiate(Beacon, Icon.transform.position, Quaternion.identity);
        newBeacon.transform.SetParent(mapCanvas);
        beacons.Add(newBeacon); 
    }
    // Update is called once per frame
    void Update()
    {
<<<<<<< Updated upstream
=======
        Vector3 mousePosition = Input.mousePosition;
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(mousePosition);
>>>>>>> Stashed changes
        // present or hide the map
        if (Input.GetKeyDown(KeyCode.M))
        {
            Map.SetActive(!Map.activeSelf);
            Icon.SetActive(!Icon.activeSelf);
        }
        // control the icon
        int x = (int)Icon.transform.position.x, y = (int)Icon.transform.position.y;
        if (Icon.activeSelf == true)
        {
<<<<<<< Updated upstream
=======
            float scrollData = Input.GetAxis("Mouse ScrollWheel");
            if (scrollData != 0.0f)
            {
                Debug.Log(mousePosition);
                canvasScaler.GetComponent<RectTransform>().pivot = mousePosition;
                float newScaleFactor = canvasScaler.scaleFactor - scrollData * zoomSpeed;
                canvasScaler.scaleFactor = Mathf.Clamp(newScaleFactor, minScale, maxScale);
            }

>>>>>>> Stashed changes
            if (Input.GetKeyDown(KeyCode.W) && y < 1020)
            {
                Icon.transform.Translate(0, 20, 0);
            }
            if (Input.GetKeyDown(KeyCode.A) && x > 60)
            {
                Icon.transform.Translate(-20, 0, 0);
            }
            if (Input.GetKeyDown(KeyCode.S) && y > 70)
            {
                Icon.transform.Translate(0, -20, 0);
            }
            if (Input.GetKeyDown(KeyCode.D) && x < 1860)
            {
                Icon.transform.Translate(20, 0, 0);
            }
            // place the beacon
            if (Input.GetKeyDown(KeyCode.P))
            {
                Beacon.SetActive(true);
                Beacon.transform.position = Icon.transform.position;
                PlaceBeacon();
                Beacon.SetActive(false);
            }
<<<<<<< Updated upstream
        }
        
=======
            // select beacon
            if (Input.GetKeyDown(KeyCode.Y) && beacons.Count != 0)
            {
                ShowChoiceBox();
            }
        }  
    }

    public void LoadData(GameData data)
    {
        beacons = data.mapData.beacons;
        highlightedBeacon = data.mapData.selectedBeacon;
        unlockedChunks = data.mapData.unlockedChunks;
    }
    public void SaveData(GameData data)
    {
        data.mapData.beacons = beacons;
        data.mapData.selectedBeacon = highlightedBeacon;
        data.mapData.unlockedChunks = unlockedChunks;
>>>>>>> Stashed changes
    }
}

