using Cinemachine;
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
    public float combatRotationSpeed;

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

    private void Update()
    {
        if(player.GetComponent<PlayerController>().canMove)
        {
            if(thirdPersonCam.GetComponent<CinemachineFreeLook>().enabled != true)
            {
                thirdPersonCam.GetComponent<CinemachineFreeLook>().enabled = true;
                combatCam.GetComponent<CinemachineFreeLook>().enabled = true;
                topDownCam.GetComponent<CinemachineFreeLook>().enabled = true;
            }

            // switch styles
            if (Input.GetKeyDown(KeyCode.Alpha1)) SwitchCameraStyle(CameraStyle.Basic);
            if (Input.GetKeyDown(KeyCode.Alpha2)) SwitchCameraStyle(CameraStyle.Combat);
            if (Input.GetKeyDown(KeyCode.Alpha3)) SwitchCameraStyle(CameraStyle.Topdown);

            // rotate orientation
            Vector3 viewDir = player.position - new Vector3(transform.position.x, player.position.y, transform.position.z);
            orientation.forward = viewDir.normalized;

            // roate player object
            if (currentStyle == CameraStyle.Basic || currentStyle == CameraStyle.Topdown)
            {
                float horizontalInput = Input.GetAxis("Horizontal");
                float verticalInput = Input.GetAxis("Vertical");
                Vector3 inputDir = orientation.forward * verticalInput + orientation.right * horizontalInput;

                if (inputDir != Vector3.zero)
                    playerObj.forward = Vector3.Slerp(playerObj.forward, inputDir.normalized, Time.deltaTime * rotationSpeed);
            }

            else if (currentStyle == CameraStyle.Combat)
            {
                Vector3 dirToCombatLookAt = combatLookAt.position - new Vector3(transform.position.x, combatLookAt.position.y, transform.position.z);
                orientation.forward = dirToCombatLookAt.normalized;

                playerObj.forward = dirToCombatLookAt.normalized;
            }
        }
        else if(thirdPersonCam.GetComponent<CinemachineFreeLook>().enabled == true)
        {
            thirdPersonCam.GetComponent<CinemachineFreeLook>().enabled = false;
            combatCam.GetComponent<CinemachineFreeLook>().enabled = false;
            topDownCam.GetComponent<CinemachineFreeLook>().enabled = false;
        }
    }

    private void SwitchCameraStyle(CameraStyle newStyle)
    {
        combatCam.SetActive(false);
        thirdPersonCam.SetActive(false);
        topDownCam.SetActive(false);

        if (newStyle == CameraStyle.Basic) thirdPersonCam.SetActive(true);
        if (newStyle == CameraStyle.Combat) combatCam.SetActive(true);
        if (newStyle == CameraStyle.Topdown) topDownCam.SetActive(true);

        currentStyle = newStyle;
    }
}
