using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RodTrigger : MonoBehaviour
{
    public float maxFishingDistance = 12f;

    [Header("Fishing Reel")]
    public int vertexCount = 12;
    public Transform rodTop;
    public Transform middlePoint;

    private LineRenderer lineRenderer;
    private Animation anim;
    private bool playAnim = false;

    [HideInInspector]
    public bool detect = false;

    private Transform parent;

    private void Start()
    {
        parent = transform.parent;
        lineRenderer = GetComponent<LineRenderer>();
        anim = GetComponent<Animation>();
    }

    private void Update()
    {
        List<Vector3> points = new List<Vector3>();
        for (float ratio = 0; ratio <= 1; ratio += 1f / vertexCount)
        {
            var vert1Tan = Vector3.Lerp(transform.position, middlePoint.position, ratio);
            var vert2Tan = Vector3.Lerp(middlePoint.position, rodTop.position, ratio);
            var point = Vector3.Lerp(vert1Tan, vert2Tan, ratio);
            points.Add(point);
        }

        lineRenderer.positionCount = points.Count;
        lineRenderer.SetPositions(points.ToArray());

        PlayerItemController controller = PlayerItemController.instance;

        if (detect && !playAnim && !controller.isFishing)
        {
            playAnim = true;
            anim.Play("Casting");
            Invoke("SetParentToRoot", controller.toolUseTime[5]);
        }
        else if (!detect && playAnim && !controller.isFishing)
        {
            playAnim = false;
            anim.Play("Reeling");
            transform.parent = parent;
            transform.SetAsFirstSibling();
        }

        if(playAnim && Vector3.Distance(transform.position, controller.transform.position) > maxFishingDistance)
        {
            playAnim = detect = false;
            anim.Play("Reeling");
            transform.parent = parent;
            transform.SetAsFirstSibling();
            PlayerItemController.instance.StopFishing();
        }
    }

    private void SetParentToRoot()
    {
        anim.Stop();
        transform.parent = null;
    }

    void OnTriggerStay(Collider hit)
    {
        if(detect && hit.GetComponent<FishingController>() != null)
        {
            PlayerItemController.instance.StartFishing(hit.GetComponent<FishingController>(), transform.position);
            detect = false;
        }
    }
}
