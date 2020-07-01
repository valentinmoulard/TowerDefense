using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour, IPoolable
{
    //EVENTS
    //GameObject : current object reference
    public static Action<GameObject> OnEnemyKilled;

    public static Action<float> OnEnemyDamagedPlayer;
    //float : object score value
    public static Action<float> OnEnemyScoreBroadcasted;

    [Header("Attributes")]
    [SerializeField, Range(1, 100)]
    private float m_enemyMaxHealth = 10f;

    [SerializeField]
    private float m_enemyScoreValue = 30f;

    [SerializeField]
    private float m_enemyDamage = 1f;

    [Header("References")]
    [SerializeField]
    private GameObject m_damagePopUpPrefabReference = null;

    [SerializeField]
    private Image m_imageHealthBar = null;

    private GameObject m_pooledDamagePopUpReference;

    private float m_enemyCurrentHealth;

    private void OnEnable()
    {
        Bullet.OnTargetHitByBullet += GetHit;
        Missile.OnTargetHitByMissile += GetHit;
        Laser.OnTargetHitByDamagingLaser += GetHit;
        EnemyMovement.OnEnemyReachedEndPoint += DamagePlayer;

        GameManager.OnTitleScreen += KillEnemy;

        InitializeEnemy();
    }

    private void OnDisable()
    {
        Bullet.OnTargetHitByBullet -= GetHit;
        Missile.OnTargetHitByMissile -= GetHit;
        Laser.OnTargetHitByDamagingLaser -= GetHit;
        EnemyMovement.OnEnemyReachedEndPoint -= DamagePlayer;

        GameManager.OnTitleScreen -= KillEnemy;
    }


    private void Start()
    {
        if (m_enemyScoreValue < 0)
        {
            Debug.Log("Can't set a negative value for enemy score value... setting to default value : 30");
            m_enemyScoreValue = 30f;
        }

        if (m_enemyDamage < 0)
        {
            Debug.Log("Can't set a negative value for enemy damage... setting to default value : 1");
            m_enemyDamage = 1f;
        }

        if (m_damagePopUpPrefabReference == null)
            Debug.LogError("The damage pop up object reference is missing in the inspector!");

        if (m_imageHealthBar == null)
            Debug.LogError("The health bar is missing in the inspector!", gameObject);
    }

    private void InitializeEnemy()
    {
        m_enemyCurrentHealth = m_enemyMaxHealth;
        m_imageHealthBar.fillAmount = m_enemyCurrentHealth / m_enemyMaxHealth;
    }

    private void GetHit(GameObject target, float damage)
    {
        if (gameObject == target)
        {
            m_enemyCurrentHealth -= damage;

            m_imageHealthBar.fillAmount = m_enemyCurrentHealth / m_enemyMaxHealth;

            if (m_enemyCurrentHealth <= 0)
            {
                //the enemy will return to the pool, send the enemy score value to increase the players currency,
                BroadcastOnEnemyScoreBroadcastedEvent();
                KillEnemy();
            }
        }
    }


    private void DamagePlayer(GameObject objectReference)
    {
        if (gameObject == objectReference)
        {
            BroadcastOnEnemyDamagedPlayerEvent();
            Kill(gameObject);
        }
    }

    private void KillEnemy()
    {
        BroadcastOnEnemyKilledEvent();
        Kill(gameObject);
    }

    private void Kill(GameObject objectReference)
    {
        if (gameObject == objectReference)
            ReturnToPool();
    }

    public void ReturnToPool()
    {
        gameObject.SetActive(false);
    }


    private void BroadcastOnEnemyDamagedPlayerEvent()
    {
        if (OnEnemyDamagedPlayer != null)
            OnEnemyDamagedPlayer(m_enemyDamage);
    }

    private void BroadcastOnEnemyKilledEvent()
    {
        if (OnEnemyKilled != null)
            OnEnemyKilled(gameObject);
    }

    private void BroadcastOnEnemyScoreBroadcastedEvent()
    {
        if (OnEnemyScoreBroadcasted != null)
            OnEnemyScoreBroadcasted(m_enemyScoreValue);
    }
}
