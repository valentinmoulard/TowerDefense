using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public static Action OnPlayersDeath;
    public static Action<float> OnPlayersHealthChanged;


    [SerializeField]
    private float m_playerInitialHealth = 10f;

    private float m_playerCurrentHealth;

    private bool m_isPlayerAlive;

    private void OnEnable()
    {
        GameManager.OnGameStart += InitializePlayersHealth;
        Enemy.OnEnemyDamagedPlayer += DeacreasePlayersHealth;
    }

    private void OnDisable()
    {
        GameManager.OnGameStart -= InitializePlayersHealth;
        Enemy.OnEnemyDamagedPlayer -= DeacreasePlayersHealth;
    }

    void Start()
    {
        if (m_playerInitialHealth < 0)
        {
            Debug.Log("The players health can't be negative. Setting the value to 10...");
            m_playerInitialHealth = 10;
        }
        BroadcastOnPlayersHealthChangedEvent();
    }

    private void InitializePlayersHealth()
    {
        m_isPlayerAlive = true;
        m_playerCurrentHealth = m_playerInitialHealth;
        BroadcastOnPlayersHealthChangedEvent();
    }

    private void DeacreasePlayersHealth(float damage)
    {
        if (m_isPlayerAlive)
        {
            m_playerCurrentHealth -= damage;
            if (m_playerCurrentHealth <= 0)
            {
                m_playerCurrentHealth = 0;
                m_isPlayerAlive = false;
                BroadcastOnPlayersDeathEvent();
            }
            BroadcastOnPlayersHealthChangedEvent();
        }
    }

    private void BroadcastOnPlayersDeathEvent()
    {
        if (OnPlayersDeath != null)
            OnPlayersDeath();
    }

    private void BroadcastOnPlayersHealthChangedEvent()
    {
        if (OnPlayersHealthChanged != null)
            OnPlayersHealthChanged(m_playerCurrentHealth);
    }
}
