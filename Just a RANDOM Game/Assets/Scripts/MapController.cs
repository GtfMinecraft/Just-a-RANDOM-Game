using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    void PlaceBeacon()
    {
        GameObject newBeacon = Instantiate(Beacon, Icon.transform.position, Quaternion.identity);
        newBeacon.transform.SetParent(mapCanvas);
        beacons.Add(newBeacon); 
    }
    // Update is called once per frame
    void Update()
    {
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
        }
        
    }
}

