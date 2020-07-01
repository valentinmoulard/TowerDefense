using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class OffensiveBuilding : RangedBuilding
{
    //gameobject : target, gameobject : current turret
    public static Action<GameObject, GameObject> OnTargetChanged;

    [SerializeField, Range(0.1f, 0.5f)]
    private float m_refreshReserchTargetRate = 0.3f;

    protected List<GameObject> m_enemiesInRangeList;
    protected GameObject m_target;
    protected GameObject m_nearestEnemyReferenceBuffer;
    private Coroutine m_targetSearchingCoroutine;
    private float m_nearestEnemyDistanceBuffer;
    private float m_distanceBuffer;

    protected override void OnEnable()
    {
        base.OnEnable();
        Enemy.OnEnemyKilled += RemoveTargetFromInRangeList;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        Enemy.OnEnemyKilled -= RemoveTargetFromInRangeList;
    }


    protected void InitializeOffensiveBuilding()
    {
        base.InitializeRangedBuilding();

        //initializing target searching, target aiming, target shooting and shoot speed
        m_enemiesInRangeList = new List<GameObject>();
        m_target = null;
        m_targetSearchingCoroutine = StartCoroutine(SearchTargetCoroutine());
    }

    private IEnumerator SearchTargetCoroutine()
    {
        while (m_isBuildingAlive)
        {
            yield return new WaitForSeconds(m_refreshReserchTargetRate);
            UpdateTarget();
        }
    }


    private void UpdateTarget()
    {
        //En fonction de l'IA de la tour, choisir
        m_target = GetNearestEnemy();
    }


    private GameObject GetNearestEnemy()
    {
        if (m_enemiesInRangeList.Count == 0)
        {
            BroadcastOnTargetChangedEvent(null, gameObject);
            return null;
        }

        m_nearestEnemyDistanceBuffer = Mathf.Infinity;
        m_nearestEnemyReferenceBuffer = null;
        for (int i = 0; i < m_enemiesInRangeList.Count; i++)
        {
            m_distanceBuffer = Vector3.Distance(m_enemiesInRangeList[i].transform.position, transform.position);
            if (m_distanceBuffer < m_nearestEnemyDistanceBuffer)
            {
                m_nearestEnemyDistanceBuffer = m_distanceBuffer;
                m_nearestEnemyReferenceBuffer = m_enemiesInRangeList[i];
            }
        }

        //check if the target has changed due to death or out of range
        if (m_target != m_nearestEnemyReferenceBuffer)
            BroadcastOnTargetChangedEvent(m_nearestEnemyReferenceBuffer, gameObject);

        return m_nearestEnemyReferenceBuffer;
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
            AddTargetToInRangeList(other.gameObject);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Enemy"))
            RemoveTargetFromInRangeList(other.gameObject);
    }

    private void AddTargetToInRangeList(GameObject objectToAdd)
    {
        m_enemiesInRangeList.Add(objectToAdd);
    }

    private void RemoveTargetFromInRangeList(GameObject objectToRemove)
    {
        m_enemiesInRangeList.Remove(objectToRemove);
        m_target = null;
    }


    #region BROADCASTERS
    private void BroadcastOnTargetChangedEvent(GameObject newTargetReference, GameObject currentTurretReference)
    {
        if (OnTargetChanged != null)
            OnTargetChanged(newTargetReference, currentTurretReference);
    }
    #endregion

}
