using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private const int BORDER_OFFSET = 100;

    [SerializeField]
    private float m_cameraSpeed = 30f;

    [SerializeField]
    private float m_cameraScrollSpeed = 5f;

    [SerializeField]
    private float m_minCameraHeight = 10f;

    [SerializeField]
    private float m_maxCameraHeight = 100f;

    private Vector3 m_correctedDirection;
    private bool m_isCameraLocked;

    private void Start()
    {
        m_correctedDirection = Vector3.zero;
        m_isCameraLocked = true;
    }

    void Update()
    {
        if (GameManager.instance.CurrentGameState == GameManager.GameState.InGame)
        {
            CheckCameraLock();

            if (!m_isCameraLocked)
                MoveCamera();
        }
    }

    private void CheckCameraLock()
    {
        if (Input.GetKeyDown(KeyCode.Y))
            m_isCameraLocked = !m_isCameraLocked;
    }

    private void MoveCamera()
    {
        MoveCameraOnZAxis();
        MoveCameraOnXAxis();
        CameraZoom();
    }

    private void MoveCameraOnZAxis()
    {
        //checking on local Z axis (if moving positively or negatively)
        if (Input.GetKey(KeyCode.Z) || Input.mousePosition.y > Screen.height - BORDER_OFFSET)
        {
            MoveCameraWithCorrectedDirection(true, true);
        }
        if (Input.GetKey(KeyCode.S) || Input.mousePosition.y < BORDER_OFFSET)
        {
            MoveCameraWithCorrectedDirection(true, false);
        }
    }

    private void MoveCameraOnXAxis()
    {
        //checking on local X axis (if moving positively or negatively)
        if (Input.GetKey(KeyCode.D) || Input.mousePosition.x > Screen.width - BORDER_OFFSET)
        {
            MoveCameraWithCorrectedDirection(false, true);
        }
        if (Input.GetKey(KeyCode.Q) || Input.mousePosition.x < BORDER_OFFSET)
        {
            MoveCameraWithCorrectedDirection(false, false);
        }
    }

    private void CameraZoom()
    {
        if (Input.GetAxis("Mouse ScrollWheel") < 0 && transform.position.y < m_maxCameraHeight)
        {
            transform.Translate(-Vector3.forward * m_cameraScrollSpeed);
        }
        else if (Input.GetAxis("Mouse ScrollWheel") > 0 && transform.position.y > m_minCameraHeight)
        {
            transform.Translate(Vector3.forward * m_cameraScrollSpeed);
        }
    }

    /// <param name="first"> first bool determines if we are moving positively on the Z axis </param>
    /// <param name="second"> second bool determines if we are moving positively on the X axis</param>
    private void MoveCameraWithCorrectedDirection(bool first, bool second)
    {
        if (first)
        {
            m_correctedDirection.x = transform.forward.x;
            m_correctedDirection.z = transform.forward.z;
        }
        else
        {
            m_correctedDirection.x = transform.right.x;
            m_correctedDirection.z = transform.right.z;
        }

        if (second)
            transform.Translate(m_correctedDirection * m_cameraSpeed * Time.deltaTime, Space.World);
        else
        {
            transform.Translate(-m_correctedDirection * m_cameraSpeed * Time.deltaTime, Space.World);
        }
    }
}
