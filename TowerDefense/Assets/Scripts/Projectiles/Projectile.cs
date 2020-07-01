using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour, IPoolable
{
    [SerializeField]
    protected float m_projectileSpeed = 10f;

    private Vector3 m_projectileDirection;
    private float m_expectedDistanceTraveledInNextFrame;

    protected GameObject m_target;


    protected virtual void OnEnable()
    {
        ProjectileLauncher.OnProjectileSent += GetTarget;
        GameManager.OnTitleScreen += ReturnToPool;
    }

    protected virtual void OnDisable()
    {
        ProjectileLauncher.OnProjectileSent -= GetTarget;
        GameManager.OnTitleScreen -= ReturnToPool;
    }

    protected virtual void Update()
    {
        if (m_target == null || m_target.activeInHierarchy == false)
        {
            ReturnToPool();
        }
        else
        {
            m_projectileDirection = m_target.transform.position - transform.position;
            m_expectedDistanceTraveledInNextFrame = m_projectileSpeed * Time.deltaTime;

            //check if we hit something (target)
            if (m_projectileDirection.magnitude <= m_expectedDistanceTraveledInNextFrame)
            {
                HitTarget();
            }

            //moves the bullet to the target
            transform.Translate(m_projectileDirection.normalized * m_expectedDistanceTraveledInNextFrame, Space.World);
            transform.LookAt(m_target.transform);
        }

    }


    private void GetTarget(GameObject bulletObjectReference, GameObject targetObjectReference)
    {
        if (gameObject == bulletObjectReference)
            m_target = targetObjectReference;
    }

    protected virtual void HitTarget()
    {
        
    }

    public void ReturnToPool()
    {
        gameObject.SetActive(false);
    }
}
