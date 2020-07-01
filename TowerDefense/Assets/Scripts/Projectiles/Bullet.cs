using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : Projectile
{
    public static Action<GameObject, float> OnTargetHitByBullet;

    private float m_bulletDamage;

    protected override void OnEnable()
    {
        base.OnEnable();
        BulletShooter.OnProjectileDamageSent += GetProjectileDamage;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        BulletShooter.OnProjectileDamageSent -= GetProjectileDamage;
    }

    protected override void Update()
    {
        base.Update();
    }

    protected override void HitTarget()
    {
        BroadcastOnTargetHitByBulletEvent();
        base.ReturnToPool();
    }

    private void GetProjectileDamage(float projectileDamageValue)
    {
        m_bulletDamage = projectileDamageValue;
    }

    private void BroadcastOnTargetHitByBulletEvent()
    {
        if (OnTargetHitByBullet != null)
            OnTargetHitByBullet(m_target, m_bulletDamage);
    }

}
