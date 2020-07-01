using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoneyGenerator : Building
{
    public static Action<string> OnShowMoneyGenerationUpgradePrice;

    [Header("MONEY GENERATOR =======================================")]
    [SerializeField]
    private MoneyGenerationUpgradesScriptableObject m_moneyGenerationUpgradesData = null;

    private const float MONEY_GAIN_REFRESH_RATE = 1.0f;

    private Coroutine m_moneyGenerationCoroutine;
    private float m_moneyToGenerate;
    private int m_currentMoneyGenerationLevel;
    private int m_maxMoneyGenerationLevelIndex;

    protected override void OnEnable()
    {
        base.OnEnable();
        BuildingUpgradesAndModeManager.OnUpgradeMoneyGeneration += UpgradeMoneyGeneration;
        UIManager.OnShowCurrentBuildingUpgradeButtons += BroadcastOnShowMoneyGenerationUpgradePriceEvent;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        BuildingUpgradesAndModeManager.OnUpgradeMoneyGeneration -= UpgradeMoneyGeneration;
        UIManager.OnShowCurrentBuildingUpgradeButtons -= BroadcastOnShowMoneyGenerationUpgradePriceEvent;
    }

    private void Start()
    {
        InitializeMoneyGenerator();
        m_moneyGenerationCoroutine = StartCoroutine(MoneyGenerationCoroutine());
    }

    private IEnumerator MoneyGenerationCoroutine()
    {
        while (m_isBuildingAlive)
        {
            yield return new WaitForSeconds(MONEY_GAIN_REFRESH_RATE);
            CurrencyManager.instance.AddCurrency(m_moneyToGenerate);
        }
    }

    private void InitializeMoneyGenerator()
    {
        base.InitializeBuilding();

        if (m_moneyGenerationUpgradesData == null)
            Debug.LogError("The money generation upgrades data is missing in the inspector!");

        m_currentMoneyGenerationLevel = 0;
        m_moneyToGenerate = m_moneyGenerationUpgradesData.m_moneyGenerationUpgradesList[m_currentMoneyGenerationLevel].m_moneyGenerationAmount;
        m_maxMoneyGenerationLevelIndex = m_moneyGenerationUpgradesData.m_moneyGenerationUpgradesList.Count - 1;
    }

    private void UpdateMoneyGenerationValue(float newValue)
    {
        m_moneyToGenerate = newValue;
    }

    private void UpgradeMoneyGeneration(GameObject buildingReference)
    {
        if (buildingReference == gameObject)
        {
            if (m_currentMoneyGenerationLevel < m_maxMoneyGenerationLevelIndex)
            {
                if (CurrencyManager.instance.CanPurchase(m_moneyGenerationUpgradesData.m_moneyGenerationUpgradesList[m_currentMoneyGenerationLevel + 1].m_cost))
                {
                    base.BroadcastOnPurchaseUpgradeEvent(m_moneyGenerationUpgradesData.m_moneyGenerationUpgradesList[m_currentMoneyGenerationLevel + 1].m_cost);
                    base.IncreaseBuildingValue(m_moneyGenerationUpgradesData.m_moneyGenerationUpgradesList[m_currentMoneyGenerationLevel + 1].m_cost);
                    m_currentMoneyGenerationLevel++;
                    UpdateMoneyGenerationValue(m_moneyGenerationUpgradesData.m_moneyGenerationUpgradesList[m_currentMoneyGenerationLevel].m_moneyGenerationAmount);
                    BroadcastOnShowMoneyGenerationUpgradePriceEvent(gameObject);
                }
                else
                {
                    GameManager.instance.PrintGeneralMessage("Not enough currency to improve building shoot speed...");
                }
            }
            else
            {
                GameManager.instance.PrintGeneralMessage("Can't Upgrade Shoot Speed anymore...");
            }
        }
    }

    private void BroadcastOnShowMoneyGenerationUpgradePriceEvent(GameObject buildingReference)
    {
        if (OnShowMoneyGenerationUpgradePrice != null && buildingReference == gameObject)
        {
            if (m_currentMoneyGenerationLevel < m_maxMoneyGenerationLevelIndex)
                OnShowMoneyGenerationUpgradePrice(m_moneyGenerationUpgradesData.m_moneyGenerationUpgradesList[m_currentMoneyGenerationLevel + 1].m_cost.ToString("F0"));
            else
                OnShowMoneyGenerationUpgradePrice("Maxed!");
        }
    }
}
