using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    //gameobject : target
    public static Action<GameObject, float> OnTargetHitByDamagingLaser;
    //gameobject : target
    public static Action<GameObject, float> OnTargetHitBySlowingLaser;
    //gameobject : target, gameobject : turret reference
    public static Action<GameObject, GameObject> OnTargetFreed;


    [Header("References")]
    [SerializeField]
    private LineRenderer m_laserLineRenderer = null;

    [SerializeField]
    private GameObject m_firePosition = null;

    [SerializeField]
    private GameObject m_turretReference = null;

    [SerializeField]
    private LaserBeamer m_laserBeamerReference = null;

    private GameObject m_targetBuffer;
    private bool m_isLaserActive;

    private void OnEnable()
    {
        Laser.OnTargetFreed += CheckIfAffectedByOtherTurrets;
        OffensiveBuilding.OnTargetChanged += FreeTargetFromSlow;
        LaserBeamer.OnLaserSlowModeActivated += ApplySlowEffectToTarget;
        LaserBeamer.OnLaserDamageModeActivated += RemoveSlowEffectOfTarget;
    }

    private void OnDisable()
    {
        Laser.OnTargetFreed -= CheckIfAffectedByOtherTurrets;
        OffensiveBuilding.OnTargetChanged -= FreeTargetFromSlow;
        LaserBeamer.OnLaserSlowModeActivated -= ApplySlowEffectToTarget;
        LaserBeamer.OnLaserDamageModeActivated -= RemoveSlowEffectOfTarget;
    }

    private void Start()
    {
        if (m_laserLineRenderer == null)
            Debug.LogError("The line renderer component is missing in the inspector!", gameObject);
        if (m_firePosition == null)
            Debug.LogError("The fire position gameobject is missing in the inspector!", gameObject);
        if (m_turretReference == null)
            Debug.LogError("The turret object reference is missing in the inspector!", gameObject);
        if (m_laserBeamerReference == null)
            Debug.LogError("The laser beamer component of the building is missing in the inspetor!", gameObject);

        m_isLaserActive = false;
    }

    private void Update()
    {
        SetLaser();
    }

    private void SetLaser()
    {
        if (m_laserBeamerReference.Target != null && m_laserBeamerReference.Target.activeInHierarchy)
        {
            m_isLaserActive = true;
            if (m_targetBuffer != m_laserBeamerReference.Target && m_laserBeamerReference.CurrentLaserMode == LaserBeamer.LaserMode.Slow)
            {
                ApplySlowEffectToTarget();
            }

            PositionLaser(m_firePosition.transform.position, m_laserBeamerReference.Target.transform.position);

            if (m_laserBeamerReference.CurrentLaserMode == LaserBeamer.LaserMode.Damage)
                ApplyLaserDamageToTarget();

            m_targetBuffer = m_laserBeamerReference.Target;
        }
        else
        {
            //IMPORTANT NOTE : not a bug, but sometimes, the laser resets its position even if the target is not null and active in hierarchy
            ResetLaserPosition();
        }
    }




    private void ApplySlowEffectToTarget()
    {
        BroadcastOnTargetFreedEvent(m_laserBeamerReference.Target);
        BroadcastOnTargetHitBySlowingLaserEvent(m_laserBeamerReference.Target);
    }

    private void RemoveSlowEffectOfTarget()
    {
        BroadcastOnTargetFreedEvent(m_laserBeamerReference.Target);
    }

    private void FreeTargetFromSlow(GameObject target, GameObject currentTurret)
    {
        if (m_laserBeamerReference.gameObject == currentTurret && target != m_targetBuffer)
            BroadcastOnTargetFreedEvent(m_targetBuffer);
    }





    private void ApplyLaserDamageToTarget()
    {
        if (m_laserBeamerReference.Target != null)
            BroadcastOnTargetHitByDamagingLaserEvent();
    }


    private void PositionLaser(Vector3 startPosition, Vector3 endPosition)
    {
        m_laserLineRenderer.SetPosition(0, startPosition);
        m_laserLineRenderer.SetPosition(1, endPosition);
    }

    private void ResetLaserPosition()
    {
        if (m_isLaserActive)
        {
            PositionLaser(m_firePosition.transform.position, m_firePosition.transform.position);
            m_isLaserActive = false;
        }
    }


    /// <summary>
    /// This method is called when a turret set an enemy free. If an other turret is still applying an effect to this enemy, we don't want to free it.
    /// So if an enemy is affected by 2 or more turrets, and one still affects the enemy, the first turret will free the enemy while the second will reapply its effect
    /// Here the methods listen to other Laser enemy free event. If an other laser try to set free the same enemy than the current Laser. The enemy is set free and the current turret reapply its effect.
    /// </summary>
    /// <param name="enemy"></param>
    /// <param name="turretReference"></param>
    private void CheckIfAffectedByOtherTurrets(GameObject enemy, GameObject turretReference)
    {
        if (enemy == m_laserBeamerReference.Target && turretReference != m_turretReference)
        {
            if (m_laserBeamerReference.CurrentLaserMode == LaserBeamer.LaserMode.Slow)
                BroadcastOnTargetHitBySlowingLaserEvent(m_laserBeamerReference.Target);
        }
    }

    #region BROADCASTERS
    private void BroadcastOnTargetHitByDamagingLaserEvent()
    {
        if (OnTargetHitByDamagingLaser != null)
            OnTargetHitByDamagingLaser(m_laserBeamerReference.Target, m_laserBeamerReference.LaserDamage * Time.deltaTime);
    }

    private void BroadcastOnTargetHitBySlowingLaserEvent(GameObject target)
    {
        if (OnTargetHitBySlowingLaser != null)
            OnTargetHitBySlowingLaser(target, m_laserBeamerReference.LaserSlowPower);
    }

    private void BroadcastOnTargetFreedEvent(GameObject targetToSetFree)
    {
        if (OnTargetFreed != null)
            OnTargetFreed(targetToSetFree, gameObject);
    }
    #endregion
}
