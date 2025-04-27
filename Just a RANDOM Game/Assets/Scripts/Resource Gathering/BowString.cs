using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

public class BowString : MonoBehaviour
{
    public Transform point1;
    public Transform point2;

    private LineRenderer lineRenderer;
    private Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        anim = PlayerController.instance.anim;
    }

    // Update is called once per frame
    void Update()
    {
        lineRenderer.SetPosition(0, point1.position);
        lineRenderer.SetPosition(2, point2.position);

        AnimatorClipInfo[] clipInfos = anim.GetCurrentAnimatorClipInfo(1);
        if (clipInfos.Length > 0 && clipInfos[0].clip.name == "Player Aim" && (int)(anim.GetCurrentAnimatorStateInfo(1).normalizedTime * clipInfos[0].clip.length * clipInfos[0].clip.frameRate) >= 12)
        {
            lineRenderer.SetPosition(1, PlayerItemController.instance.rightHandObj.transform.position);
        }
        else
        {
            lineRenderer.SetPosition(1, (point1.position + point2.position) / 2);
        }
    }
}
