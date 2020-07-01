using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Missile : Projectile
{
    public static Action<GameObject, float> OnTargetHitByMissile;

    [SerializeField]
    private LayerMask m_targetLayerToImpact = 0;


    private float m_missileDamage;
    private float m_damageRadius;
    private Collider[] m_enemiesCollidersInAOE;

    protected override void OnEnable()
    {
        base.OnEnable();
        MissileLauncher.OnProjectileAOERadiusSent += GetMissileAOERadius;
        MissileLauncher.OnProjectileDamageSent += GetMissileDamage;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        MissileLauncher.OnProjectileAOERadiusSent -= GetMissileAOERadius;
        MissileLauncher.OnProjectileDamageSent += GetMissileDamage;
    }

    protected override void Update()
    {
        base.Update();
    }

    protected override void HitTarget()
    {
        m_enemiesCollidersInAOE = Physics.OverlapSphere(transform.position, m_damageRadius, m_targetLayerToImpact);

        for (int i = 0; i < m_enemiesCollidersInAOE.Length; i++)
        {
            BroadcastOnTargetHitByMissileEvent(m_enemiesCollidersInAOE[i].gameObject);
        }

        base.ReturnToPool();
    }

    private void GetMissileDamage(float newMissileDamage)
    {
        m_missileDamage = newMissileDamage;
    }

    private void GetMissileAOERadius(float newRadius)
    {
        m_damageRadius = newRadius;
    }

    private void BroadcastOnTargetHitByMissileEvent(GameObject target)
    {
        if (OnTargetHitByMissile != null)
            OnTargetHitByMissile(target, m_missileDamage);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, m_damageRadius);
    }
}
