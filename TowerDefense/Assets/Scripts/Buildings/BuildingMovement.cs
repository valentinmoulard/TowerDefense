using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingMovement : MonoBehaviour
{
    [Header("Attributes")]
    [SerializeField]
    private float m_offensiveBuildingRotationSpeed = 7f;


    [Header("References")]
    [SerializeField]
    private GameObject m_offensiveBuildingPartToRotate = null;

    private GameObject m_currentTarget;
    private Quaternion m_turretLookRotation;
    private Vector3 m_turretDirection;
    private Vector3 m_rotationBuffer;

    private void OnEnable()
    {
        OffensiveBuilding.OnTargetChanged += GetTarget;
    }

    private void OnDisable()
    {
        OffensiveBuilding.OnTargetChanged -= GetTarget;
    }

    private void Start()
    {
        if (m_offensiveBuildingPartToRotate == null)
            Debug.LogError("The turret part to rotate is missing in the inspector!");
    }

    private void Update()
    {
        RotateTurretToTarget();
    }

    private void RotateTurretToTarget()
    {
        if (m_currentTarget != null && m_currentTarget.activeInHierarchy == true)
        {
            m_turretDirection = m_currentTarget.transform.position - m_offensiveBuildingPartToRotate.transform.position;
            m_turretLookRotation = Quaternion.LookRotation(m_turretDirection);
            m_rotationBuffer = Quaternion.Lerp(m_offensiveBuildingPartToRotate.transform.rotation, m_turretLookRotation, Time.deltaTime * m_offensiveBuildingRotationSpeed).eulerAngles;
            m_offensiveBuildingPartToRotate.transform.rotation = Quaternion.Euler(0.0f, m_rotationBuffer.y, 0.0f);
        }
    }


    private void GetTarget(GameObject newTarget, GameObject buildingReference)
    {
        if (buildingReference == gameObject)
            m_currentTarget = newTarget;
    }
}
