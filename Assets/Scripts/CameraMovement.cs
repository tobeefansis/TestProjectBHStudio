using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [SerializeField] private Transform cameraTarget;
    [SerializeField] private Transform playerTransform;
    [SerializeField] [Range(0, 360)] private float minimumAngle;
    [SerializeField] [Range(0, 360)] private float maximumAngle;
    [SerializeField] [Range(0, 5)] private float mouseSensitivity;
    [SerializeField] private bool stickCamera;
    [SerializeField] private CinemachineVirtualCamera virtualCamera;


    public void SetTargetPlayer(PlayerInfo playerInfo)
    {
        if (playerInfo.hasAuthority)
        {
            cameraTarget = playerInfo.CameraTarget;
            playerTransform = playerInfo.PlayerTransform;
            virtualCamera.Follow = cameraTarget;
        }
    }

    private void Update()
    {
        if (cameraTarget && playerTransform)
        {
            float _mouseX = Input.GetAxis("Mouse X");
            float _mouseY = Input.GetAxis("Mouse Y");
            cameraTarget.rotation *= Quaternion.AngleAxis(_mouseX * mouseSensitivity, Vector3.up);
            cameraTarget.rotation *= Quaternion.AngleAxis(-_mouseY * mouseSensitivity, Vector3.right);


            var angleX = cameraTarget.localEulerAngles.x;
            if (angleX > 180 && angleX < maximumAngle)
            {
                angleX = maximumAngle;
            }
            else if (angleX < 180 && angleX > minimumAngle)
            {
                angleX = minimumAngle;
            }

            cameraTarget.localEulerAngles = new Vector3(angleX, cameraTarget.localEulerAngles.y, 0);

            if (stickCamera)
            {
                playerTransform.rotation = Quaternion.Euler(0, cameraTarget.rotation.eulerAngles.y, 0);
            }
        }
    }
}
