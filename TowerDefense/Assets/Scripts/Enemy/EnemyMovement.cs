using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    public static Action<GameObject> OnEnemyReachedEndPoint;


    [SerializeField]
    private float m_initialMovementSpeed = 10f;

    [SerializeField]
    private float m_minimalDistanceCheckBeforeNextWaypoint = 0.3f;


    private Transform m_targetDirection = null;
    private Vector3 m_currentDirection;
    private float m_currentMoveSpeed;
    private int m_currentWaypointIndex;


    private void OnEnable()
    {
        Enemy.OnEnemyKilled += EnemyMovementResetOnDeath;
        Laser.OnTargetHitBySlowingLaser += DecreaseSpeed;
        Laser.OnTargetFreed += RestoreMovementSpeed;

        InitializeEnemyMovement();
    }

    private void OnDisable()
    {
        Enemy.OnEnemyKilled -= EnemyMovementResetOnDeath;
        Laser.OnTargetHitBySlowingLaser -= DecreaseSpeed;
        Laser.OnTargetFreed -= RestoreMovementSpeed;
    }

    private void Update()
    {
        m_currentDirection = (m_targetDirection.position - transform.position).normalized;
        transform.Translate(m_currentDirection * m_currentMoveSpeed * Time.deltaTime, Space.World);

        float lol = Vector3.Distance(transform.position, m_targetDirection.position);
        if (Vector3.Distance(transform.position, m_targetDirection.position) < m_minimalDistanceCheckBeforeNextWaypoint)
        {
            m_targetDirection = GetNextTargetWaypoint();
        }
    }

    private void InitializeEnemyMovement()
    {
        m_currentWaypointIndex = 0;
        m_targetDirection = WaypointsManager.instance.GetWaypoint(m_currentWaypointIndex);
        RestoreMovementSpeed(gameObject);
        if (m_targetDirection == null)
            Debug.LogError("Unable to get waypoint transform in list!", gameObject);
    }

    private Transform GetNextTargetWaypoint()
    {
        m_currentWaypointIndex++;
        if (m_currentWaypointIndex < WaypointsManager.instance.GetWaypointCount())
            return WaypointsManager.instance.GetWaypoint(m_currentWaypointIndex);
        else
        {
            BroadcastOnEnemyReachedEndPoint();
            return null;
        }
    }

    private void EnemyMovementResetOnDeath(GameObject currentObjectReference)
    {
        if (currentObjectReference == gameObject)
            InitializeEnemyMovement();
    }


    /// <summary>
    /// Method to slow the enemy down. Only keeps the best slow (so place your turret wisely)
    /// </summary>
    /// <param name="slowFactor"></param>
    private void DecreaseSpeed(GameObject objectReference, float slowFactor)
    {
        if (objectReference == gameObject)
        {
            if (m_currentMoveSpeed > m_initialMovementSpeed * (1 - slowFactor))
                m_currentMoveSpeed = m_initialMovementSpeed * (1 - slowFactor);
        }
    }

    private void StopObjectMovement(GameObject objectReference)
    {
        if (objectReference == gameObject)
            m_currentMoveSpeed = 0f;
    }

    private void RestoreMovementSpeed(GameObject objectReference, GameObject buildingReference = null)
    {
        if (objectReference == gameObject)
            m_currentMoveSpeed = m_initialMovementSpeed;
    }


    private void BroadcastOnEnemyReachedEndPoint()
    {
        if (OnEnemyReachedEndPoint != null)
            OnEnemyReachedEndPoint(gameObject);
    }

}
