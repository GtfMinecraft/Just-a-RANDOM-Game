using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonCam : MonoBehaviour
{
    [Header("References")]
    public Transform orientation;
    public Transform player;
    public Transform playerObj;

    public float rotationSpeed;

    public Transform combatLookAt;

    public GameObject thirdPersonCam;
    public GameObject combatCam;
    public GameObject topDownCam;

    public CameraStyle currentStyle;
    public enum CameraStyle
    {
        Basic,
        Combat,
        Topdown
    }

    private CinemachineCore.AxisInputDelegate origianlInput = CinemachineCore.GetInputAxis;
    private bool changed = false;

    private float switchTime = 0;
    private Vector3 previousOrientation;

    private void Update()
    {
        if (player.GetComponent<PlayerController>().canRotate)
        {
            if (changed)
            {
                changed = false;
                CinemachineCore.GetInputAxis = origianlInput;
            }
        }
        else if (!changed)
        {
            changed = true;
            CinemachineCore.GetInputAxis = temp => { return 0; };
        }

        if (player.GetComponent<PlayerController>().MovementIsEnable())
        {
            // switch styles
            if (Input.GetKeyDown(KeyCode.Alpha1)) SwitchCameraStyle(CameraStyle.Basic);
            if (Input.GetKeyDown(KeyCode.Alpha2)) SwitchCameraStyle(CameraStyle.Combat);
            if (Input.GetKeyDown(KeyCode.Alpha3)) SwitchCameraStyle(CameraStyle.Topdown);

            // roate player object
            if (currentStyle == CameraStyle.Basic || currentStyle == CameraStyle.Topdown)
            {
                // rotate orientation
                Vector3 viewDir = player.position - new Vector3(transform.position.x, player.position.y, transform.position.z);
                if (switchTime > 0)
                {
                    switchTime -= Time.deltaTime;
                    orientation.forward = Vector3.Lerp(viewDir.normalized, previousOrientation, switchTime / 2);
                }
                else
                    orientation.forward = viewDir.normalized;

                float horizontalInput = player.GetComponent<PlayerController>().currentMovement.x;
                float verticalInput = player.GetComponent<PlayerController>().currentMovement.y;
                Vector3 inputDir = orientation.forward * verticalInput + orientation.right * horizontalInput;

                if (inputDir != Vector3.zero)
                    playerObj.forward = Vector3.Slerp(playerObj.forward, inputDir.normalized, Time.deltaTime * rotationSpeed);
            }

            else if (currentStyle == CameraStyle.Combat)
            {
                Vector3 dirToCombatLookAt = combatLookAt.position - new Vector3(transform.position.x, combatLookAt.position.y, transform.position.z);
                if (switchTime > 0)
                {
                    switchTime -= Time.deltaTime;
                    orientation.forward = Vector3.Lerp(dirToCombatLookAt.normalized, previousOrientation, switchTime / 2);
                    playerObj.forward = Vector3.Slerp(playerObj.forward, dirToCombatLookAt.normalized, Time.deltaTime * rotationSpeed);
                }
                else
                {
                    orientation.forward = dirToCombatLookAt.normalized;
                    playerObj.forward = dirToCombatLookAt.normalized;
                }
            }
        }
    }

    public void SwitchCameraStyle(CameraStyle newStyle)
    {
        thirdPersonCam.SetActive(newStyle == CameraStyle.Basic);
        combatCam.SetActive(newStyle == CameraStyle.Combat);
        topDownCam.SetActive(newStyle == CameraStyle.Topdown);

        switchTime = 2 - switchTime;
        previousOrientation = orientation.forward;

        currentStyle = newStyle;
    }
}
