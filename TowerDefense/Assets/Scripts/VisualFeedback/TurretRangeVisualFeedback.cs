using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretRangeVisualFeedback : MonoBehaviour
{
    [SerializeField]
    private float m_rotationSpeed = 1f;

    void Update()
    {
        transform.Rotate(Vector3.up, Time.deltaTime * m_rotationSpeed);
    }
}
