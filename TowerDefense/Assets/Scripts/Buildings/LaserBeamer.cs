using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserBeamer : OffensiveBuilding
{
    //float : send the cost of the next upgrade to show the correct price on the purchase button
    public static Action<string> OnShowDamageUpgradePrice;
    public static Action<string> OnShowSlowPowerUpgradePrice;
    public static Action OnLaserSlowModeActivated;
    public static Action OnLaserDamageModeActivated;
    public static Action<GameObject> OnShowLaserBeamerModeUI;


    public enum LaserMode
    {
        Damage,
        Slow
    }


    [Header("LASER BEAMER")]
    [SerializeField]
    private GameObject m_laserReference = null;

    [SerializeField]
    private SlowPowerUpgradesScriptableObject m_slowPowerUpgradesData = null;

    [SerializeField]
    private DamageUpgradesScriptableObject m_damageUpgradesData = null;

    private LaserMode m_laserMode;

    private int m_maxDamageLevelIndex;
    private int m_currentDamageLevel;
    private float m_laserDamage;


    private int m_maxSlowPowerLevelIndex;
    private int m_currentSlowPowerLevel;
    private float m_laserSlowPower;

    public GameObject Target { get => m_target; }
    public LaserMode CurrentLaserMode { get => m_laserMode; }
    public float LaserDamage { get => m_laserDamage; }
    public float LaserSlowPower { get => m_laserSlowPower; }

    private GameObject m_initialLaserPrefabReference;


    protected override void OnEnable()
    {
        base.OnEnable();
        BuildingUpgradesAndModeManager.OnUpgradeDamage += UpgradeDamage;
        BuildingUpgradesAndModeManager.OnUpgradeSlowPower += UpgradeSlowPower;
        BuildingUpgradesAndModeManager.OnLaserBeamerDamageModeActivated += SetLaserModeToDamage;
        BuildingUpgradesAndModeManager.OnLaserBeamerSlowModeActivated += SetLaserModeToSlow;

        UIManager.OnShowCurrentBuildingUpgradeButtons += ShowLaserBeamerInterface;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        BuildingUpgradesAndModeManager.OnUpgradeDamage -= UpgradeDamage;
        BuildingUpgradesAndModeManager.OnUpgradeSlowPower -= UpgradeSlowPower;
        BuildingUpgradesAndModeManager.OnLaserBeamerDamageModeActivated -= SetLaserModeToDamage;
        BuildingUpgradesAndModeManager.OnLaserBeamerSlowModeActivated -= SetLaserModeToSlow;

        UIManager.OnShowCurrentBuildingUpgradeButtons -= ShowLaserBeamerInterface;
    }

    private void Start()
    {
        InitializeLaserBeamer();
    }

    private void InitializeLaserBeamer()
    {
        base.InitializeOffensiveBuilding();

        m_initialLaserPrefabReference = m_laserReference;

        if (m_slowPowerUpgradesData == null)
            Debug.LogError("The slow power upgrade SO is missing in the inspector!", gameObject);
        if (m_damageUpgradesData == null)
            Debug.LogError("The damage upgrade SO is missing in the inspector!", gameObject);

        m_maxDamageLevelIndex = m_damageUpgradesData.m_damageUpgradesList.Count - 1;
        m_currentDamageLevel = 0;
        m_laserDamage = m_damageUpgradesData.m_damageUpgradesList[m_currentDamageLevel].m_damage;

        m_maxSlowPowerLevelIndex = m_slowPowerUpgradesData.m_slowPowerUpgradesList.Count - 1;
        m_currentSlowPowerLevel = 0;
        m_laserSlowPower = m_slowPowerUpgradesData.m_slowPowerUpgradesList[m_currentSlowPowerLevel].m_slowPower;

        m_laserMode = LaserMode.Damage;
    }

    private void SetLaserModeToDamage(GameObject buildingReference)
    {
        if (buildingReference == gameObject && m_laserMode != LaserMode.Damage)
        {
            m_laserMode = LaserMode.Damage;
            BroadcastOnLaserDamageModeActivatedEvent();
        }
    }

    private void SetLaserModeToSlow(GameObject buildingReference)
    {
        if (buildingReference == gameObject && m_laserMode != LaserMode.Slow)
        {
            m_laserMode = LaserMode.Slow;
            BroadcastOnLaserSlowModeActivatedEvent();
        }
    }


    #region UPGRADES
    private void UpdateDamageValue(float newValue)
    {
        m_laserDamage = newValue;
    }

    private void UpgradeDamage(GameObject buildingReference)
    {
        if (buildingReference == gameObject)
        {
            if (m_currentDamageLevel < m_maxDamageLevelIndex)
            {
                if (CurrencyManager.instance.CanPurchase(m_damageUpgradesData.m_damageUpgradesList[m_currentDamageLevel + 1].m_cost))
                {
                    BroadcastOnPurchaseUpgradeEvent(m_damageUpgradesData.m_damageUpgradesList[m_currentDamageLevel + 1].m_cost);
                    base.IncreaseBuildingValue(m_damageUpgradesData.m_damageUpgradesList[m_currentDamageLevel + 1].m_cost);
                    m_currentDamageLevel++;
                    UpdateDamageValue(m_damageUpgradesData.m_damageUpgradesList[m_currentDamageLevel].m_damage);
                    BroadcastOnShowDamageUpgradePriceEvent();
                }
                else
                {
                    GameManager.instance.PrintGeneralMessage("Not enough currency to improve building damage...");
                }
            }
            else
            {
                GameManager.instance.PrintGeneralMessage("Can't Upgrade Damage anymore...");
            }
        }
    }

    private void UpdateSlowPowerValue(float newValue)
    {
        m_laserSlowPower = newValue;
    }

    private void UpgradeSlowPower(GameObject buildingReference)
    {
        if (buildingReference == gameObject)
        {
            if (m_currentSlowPowerLevel < m_maxSlowPowerLevelIndex)
            {
                if (CurrencyManager.instance.CanPurchase(m_slowPowerUpgradesData.m_slowPowerUpgradesList[m_currentSlowPowerLevel + 1].m_cost))
                {
                    BroadcastOnPurchaseUpgradeEvent(m_slowPowerUpgradesData.m_slowPowerUpgradesList[m_currentSlowPowerLevel + 1].m_cost);
                    base.IncreaseBuildingValue(m_slowPowerUpgradesData.m_slowPowerUpgradesList[m_currentSlowPowerLevel + 1].m_cost);
                    m_currentSlowPowerLevel++;
                    UpdateSlowPowerValue(m_slowPowerUpgradesData.m_slowPowerUpgradesList[m_currentSlowPowerLevel].m_slowPower);
                    BroadcastOnShowSlowPowerUpgradePriceEvent();
                }
                else
                {
                    GameManager.instance.PrintGeneralMessage("Not enough currency to improve building slow power...");
                }
            }
            else
            {
                GameManager.instance.PrintGeneralMessage("Can't Upgrade Slow Power anymore...");
            }
        }
    }
    #endregion

    #region BROADCASTERS
    private void BroadcastOnLaserSlowModeActivatedEvent()
    {
        if (OnLaserSlowModeActivated != null)
            OnLaserSlowModeActivated();
    }


    private void BroadcastOnLaserDamageModeActivatedEvent()
    {
        if (OnLaserDamageModeActivated != null)
            OnLaserDamageModeActivated();
    }
    private void ShowLaserBeamerInterface(GameObject buildingReference)
    {
        if (buildingReference == gameObject)
        {
            BroadcastOnShowDamageUpgradePriceEvent();
            BroadcastOnShowSlowPowerUpgradePriceEvent();
            BroadcastOnShowLaserBeamerModeUIEvent();
        }
    }

    private void BroadcastOnShowLaserBeamerModeUIEvent()
    {
        if (OnShowLaserBeamerModeUI != null)
            OnShowLaserBeamerModeUI(gameObject);
    }

    private void BroadcastOnShowDamageUpgradePriceEvent()
    {
        if (OnShowDamageUpgradePrice != null)
        {
            if (m_currentDamageLevel < m_maxDamageLevelIndex)
                OnShowDamageUpgradePrice(m_damageUpgradesData.m_damageUpgradesList[m_currentDamageLevel + 1].m_cost.ToString("F0"));
            else
                OnShowDamageUpgradePrice("Maxed");
        }
    }

    private void BroadcastOnShowSlowPowerUpgradePriceEvent()
    {
        if (OnShowSlowPowerUpgradePrice != null)
        {
            if (m_currentSlowPowerLevel < m_maxSlowPowerLevelIndex)
                OnShowSlowPowerUpgradePrice(m_slowPowerUpgradesData.m_slowPowerUpgradesList[m_currentSlowPowerLevel + 1].m_cost.ToString("F0"));
            else
                OnShowSlowPowerUpgradePrice("Maxed");
        }
    }
    #endregion
}
