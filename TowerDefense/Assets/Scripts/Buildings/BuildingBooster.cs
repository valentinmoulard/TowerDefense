using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingBooster : RangedBuilding
{
    public delegate void OnBuildingBoosterEvent(GameObject buildingReference, float boostValue);
    public static OnBuildingBoosterEvent OnBoostedRange;
    public static OnBuildingBoosterEvent OnBoostedDamage;
    public static OnBuildingBoosterEvent OnBoostedShootSpeed;
    public static OnBuildingBoosterEvent OnBoostedSlowPower;
    public static OnBuildingBoosterEvent OnBoostedAOERange;
    public static OnBuildingBoosterEvent OnBoostedMoneyGeneration;

    public static Action<string> OnShowBuildingBoosterUpgradePrice;

    public enum BoosterMode
    {
        BoostRange,
        BoostDamage,
        BoostShootSpeed,
        BoostSlowPower,
        BoostAOERange,
        BoostMoneyGeneration
    }

    [Header("BUILDING BOOSTER =======================================")]
    [SerializeField]
    private BuildingBoosterUpgradesScriptableObject m_buildingBoosterUpgradesData = null;

    private List<GameObject> m_turretInRangeList;
    private BoosterMode m_currentBoosterMode;

    private float m_rangeBoostValue;
    private float m_damageBoostValue;
    private float m_shootSpeedBoostValue;
    private float m_slowPowerBoostValue;
    private float m_AOERangeBoostValue;
    private float m_moneyGenerationBoostValue;

    private int m_currentBuildingBoosterLevel;
    private int m_maxBuildingBoosterLevelIndex;

    protected override void OnEnable()
    {
        //when upgrading the building booster range, re check if there is more turret to affect
        //when the bulding booster is upgraded, update the booster value to all the building in range

        //send event when building booster is destroyed to remove the booster effect

        BuildingManager.OnBuildingBuilt += TryAddBuilginInRangeList;

        BuildingUpgradesAndModeManager.OnUpgradeBuildingBooster += UpgradeBuildingBooster;
        BuildingUpgradesAndModeManager.OnBoostRangeBuildingBooster += ChangeBoostMode;
        BuildingUpgradesAndModeManager.OnBoostDamageBuildingBooster += ChangeBoostMode;
        BuildingUpgradesAndModeManager.OnBoostShootSpeedBuildingBooster += ChangeBoostMode;
        BuildingUpgradesAndModeManager.OnBoostSlowPowerBuildingBooster += ChangeBoostMode;
        BuildingUpgradesAndModeManager.OnBoostAOERangeBuildingBooster += ChangeBoostMode;
        BuildingUpgradesAndModeManager.OnBoostMoneyGenerationBuildingBooster += ChangeBoostMode;

        UIManager.OnShowCurrentBuildingUpgradeButtons += BroadcastOnShowBuildingBoosterUpgradePriceEvent;
    }

    protected override void OnDisable()
    {
        BuildingManager.OnBuildingBuilt -= TryAddBuilginInRangeList;

        BuildingUpgradesAndModeManager.OnUpgradeBuildingBooster -= UpgradeBuildingBooster;
        BuildingUpgradesAndModeManager.OnBoostRangeBuildingBooster -= ChangeBoostMode;
        BuildingUpgradesAndModeManager.OnBoostDamageBuildingBooster -= ChangeBoostMode;
        BuildingUpgradesAndModeManager.OnBoostShootSpeedBuildingBooster -= ChangeBoostMode;
        BuildingUpgradesAndModeManager.OnBoostSlowPowerBuildingBooster -= ChangeBoostMode;
        BuildingUpgradesAndModeManager.OnBoostAOERangeBuildingBooster -= ChangeBoostMode;
        BuildingUpgradesAndModeManager.OnBoostMoneyGenerationBuildingBooster -= ChangeBoostMode;

        UIManager.OnShowCurrentBuildingUpgradeButtons -= BroadcastOnShowBuildingBoosterUpgradePriceEvent;
    }

    private void Start()
    {
        InitializeBuildingBooster();
    }

    private void InitializeBuildingBooster()
    {
        base.InitializeRangedBuilding();
        m_turretInRangeList = new List<GameObject>();
        m_currentBoosterMode = BoosterMode.BoostRange;

        m_currentBuildingBoosterLevel = 0;
        m_maxBuildingBoosterLevelIndex = m_buildingBoosterUpgradesData.m_buildingBoosterUpgradeList.Count - 1;
        GetBoostValues(m_currentBuildingBoosterLevel);

        UpdateAndApplyEffectOnInRangeBuildings();
    }



    private void UpdateAndApplyEffectOnInRangeBuildings()
    {
        GetInRangeBuildings();
        ApplyEffectOnInRangeBuildings();
    }

    private void GetInRangeBuildings()
    {
        foreach (Transform turretTransform in BuildingManager.instance.TurretParentTransform)
        {
            TryAddBuilginInRangeList(turretTransform.gameObject);
        }
    }

    private void TryAddBuilginInRangeList(GameObject turretToCheck, GameObject buildingNodeReference = null)
    {
        if (Vector3.Distance(turretToCheck.transform.position, gameObject.transform.position) < base.m_turretRange)
            m_turretInRangeList.Add(turretToCheck);
    }


    private void ApplyEffectOnInRangeBuildings()
    {
        for (int i = 0; i < m_turretInRangeList.Count; i++)
        {
            BroadcastBoostEffect(m_turretInRangeList[i]);
            Debug.Log("Boosting " + m_turretInRangeList[i].name + " with " + m_currentBoosterMode.ToString());
        }
    }

    private void BroadcastBoostEffect(GameObject buildingToAffect)
    {
        switch (m_currentBoosterMode)
        {
            case BoosterMode.BoostRange:
                BroadcastOnBoostedRangeEvent(buildingToAffect);
                break;
            case BoosterMode.BoostDamage:
                BroadcastOnBoostedDamageEvent(buildingToAffect);
                break;
            case BoosterMode.BoostShootSpeed:
                BroadcastOnBoostedShootSpeedEvent(buildingToAffect);
                break;
            case BoosterMode.BoostSlowPower:
                BroadcastOnBoostedSlowPowerEvent(buildingToAffect);
                break;
            case BoosterMode.BoostAOERange:
                BroadcastOnBoostedAOERangeEvent(buildingToAffect);
                break;
            case BoosterMode.BoostMoneyGeneration:
                BroadcastOnBoostedMoneyGenerationEvent(buildingToAffect);
                break;
            default:
                break;
        }
    }

    private void UpgradeBuildingBooster(GameObject buildingReference)
    {
        if (buildingReference == gameObject)
        {
            if (m_currentBuildingBoosterLevel < m_maxBuildingBoosterLevelIndex)
            {
                if (CurrencyManager.instance.CanPurchase(m_buildingBoosterUpgradesData.m_buildingBoosterUpgradeList[m_currentBuildingBoosterLevel + 1].m_cost))
                {
                    base.BroadcastOnPurchaseUpgradeEvent(m_buildingBoosterUpgradesData.m_buildingBoosterUpgradeList[m_currentBuildingBoosterLevel + 1].m_cost);
                    base.IncreaseBuildingValue(m_buildingBoosterUpgradesData.m_buildingBoosterUpgradeList[m_currentBuildingBoosterLevel + 1].m_cost);
                    m_currentBuildingBoosterLevel++;
                    GetBoostValues(m_currentBuildingBoosterLevel);
                    BroadcastOnShowBuildingBoosterUpgradePriceEvent(gameObject);
                }
                else
                {
                    GameManager.instance.PrintGeneralMessage("Not enough currency to improve building boost power...");
                }
            }
            else
            {
                GameManager.instance.PrintGeneralMessage("Can't Upgrade Boost Power anymore...");
            }
        }
        UpdateAndApplyEffectOnInRangeBuildings();
    }

    private void GetBoostValues(int currentLevel)
    {
        m_rangeBoostValue = m_buildingBoosterUpgradesData.m_buildingBoosterUpgradeList[currentLevel].m_rangeBoostValue;
        m_damageBoostValue = m_buildingBoosterUpgradesData.m_buildingBoosterUpgradeList[currentLevel].m_damageBoostValue;
        m_shootSpeedBoostValue = m_buildingBoosterUpgradesData.m_buildingBoosterUpgradeList[currentLevel].m_shootSpeedBoostValue;
        m_slowPowerBoostValue = m_buildingBoosterUpgradesData.m_buildingBoosterUpgradeList[currentLevel].m_slowPowerBoostValue;
        m_AOERangeBoostValue = m_buildingBoosterUpgradesData.m_buildingBoosterUpgradeList[currentLevel].m_AOERangeBoostValue;
        m_moneyGenerationBoostValue = m_buildingBoosterUpgradesData.m_buildingBoosterUpgradeList[currentLevel].m_moneyGenerationBoostValue;
    }

    #region BOOST MODE MANAGEMENT
    private void ChangeBoostMode(GameObject buildingReference, BoosterMode boostMode)
    {
        if (buildingReference == gameObject)
        {
            switch (boostMode)
            {
                case BoosterMode.BoostRange:
                    m_currentBoosterMode = BoosterMode.BoostRange;
                    break;
                case BoosterMode.BoostDamage:
                    m_currentBoosterMode = BoosterMode.BoostDamage;
                    break;
                case BoosterMode.BoostShootSpeed:
                    m_currentBoosterMode = BoosterMode.BoostShootSpeed;
                    break;
                case BoosterMode.BoostSlowPower:
                    m_currentBoosterMode = BoosterMode.BoostSlowPower;
                    break;
                case BoosterMode.BoostAOERange:
                    m_currentBoosterMode = BoosterMode.BoostAOERange;
                    break;
                case BoosterMode.BoostMoneyGeneration:
                    m_currentBoosterMode = BoosterMode.BoostMoneyGeneration;
                    break;
                default:
                    break;
            }

            ApplyEffectOnInRangeBuildings();
        }
    }
    #endregion


    #region BROADCASTERS
    private void BroadcastOnShowBuildingBoosterUpgradePriceEvent(GameObject buildingReference)
    {
        if (OnShowBuildingBoosterUpgradePrice != null && buildingReference == gameObject)
        {
            if (m_currentBuildingBoosterLevel < m_maxBuildingBoosterLevelIndex)
                OnShowBuildingBoosterUpgradePrice(m_buildingBoosterUpgradesData.m_buildingBoosterUpgradeList[m_currentBuildingBoosterLevel + 1].m_cost.ToString("F0"));
            else
                OnShowBuildingBoosterUpgradePrice("Maxed!");
        }
    }

    private void BroadcastOnBoostedRangeEvent(GameObject buildingToAffect)
    {
        if (OnBoostedRange != null)
            OnBoostedRange(buildingToAffect, m_rangeBoostValue);
    }

    private void BroadcastOnBoostedDamageEvent(GameObject buildingToAffect)
    {
        if (OnBoostedDamage != null)
            OnBoostedDamage(buildingToAffect, m_damageBoostValue);
    }

    private void BroadcastOnBoostedShootSpeedEvent(GameObject buildingToAffect)
    {
        if (OnBoostedShootSpeed != null)
            OnBoostedShootSpeed(buildingToAffect, m_shootSpeedBoostValue);
    }

    private void BroadcastOnBoostedSlowPowerEvent(GameObject buildingToAffect)
    {
        if (OnBoostedSlowPower != null)
            OnBoostedSlowPower(buildingToAffect, m_slowPowerBoostValue);
    }

    private void BroadcastOnBoostedAOERangeEvent(GameObject buildingToAffect)
    {
        if (OnBoostedAOERange != null)
            OnBoostedAOERange(buildingToAffect, m_AOERangeBoostValue);
    }

    private void BroadcastOnBoostedMoneyGenerationEvent(GameObject buildingToAffect)
    {
        if (OnBoostedMoneyGeneration != null)
            OnBoostedMoneyGeneration(buildingToAffect, m_moneyGenerationBoostValue);
    }
    #endregion

}
