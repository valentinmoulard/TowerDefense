using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurrencyManager : MonoBehaviour
{
    public static Action<float> OnCurrencyValueChanged;

    public static CurrencyManager instance;

    [Header("Attributes")]
    [SerializeField]
    private float m_currencyInitialValue = 400f;


    private float m_currency;


    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);
    }


    private void OnEnable()
    {
        GameManager.OnGameStart += InitializeCurrency;
        Enemy.OnEnemyScoreBroadcasted += AddCurrency;
        BuildingManager.OnSendBuildingBuiltPrice += DecreaseCurrency;
        RangedBuilding.OnPurchaseUpgrade += DecreaseCurrency;
    }

    private void OnDisable()
    {
        GameManager.OnGameStart -= InitializeCurrency;
        Enemy.OnEnemyScoreBroadcasted -= AddCurrency;
        BuildingManager.OnSendBuildingBuiltPrice -= DecreaseCurrency;
        RangedBuilding.OnPurchaseUpgrade -= DecreaseCurrency;
    }

    private void InitializeCurrency()
    {
        m_currency = m_currencyInitialValue; //later, get this value from save
        BroadcastOnCurrencyValueChangedEvent();
    }

    public bool CanPurchase(float purchaseAmount)
    {
        if (purchaseAmount <= m_currency)
            return true;

        return false;
    }


    public void DecreaseCurrency(float amountToDecrease)
    {
        m_currency -= amountToDecrease;
        BroadcastOnCurrencyValueChangedEvent();
    }


    public void AddCurrency(float amountToAdd)
    {
        m_currency += amountToAdd;
        BroadcastOnCurrencyValueChangedEvent();
    }

    public float GetCurrency()
    {
        return m_currency;
    }

    private void BroadcastOnCurrencyValueChangedEvent()
    {
        if (OnCurrencyValueChanged != null)
            OnCurrencyValueChanged(m_currency);
    }
}
