using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingUpgradesAndModeManager : MonoBehaviour
{
    public delegate void OnBuildingUpgradesManagerEvent(GameObject buildingReference);
    public static OnBuildingUpgradesManagerEvent OnUpgradeRange;
    public static OnBuildingUpgradesManagerEvent OnUpgradeDamage;
    public static OnBuildingUpgradesManagerEvent OnUpgradeShootSpeed;
    public static OnBuildingUpgradesManagerEvent OnUpgradeAOERadiusEffect;
    public static OnBuildingUpgradesManagerEvent OnUpgradeSlowPower;
    public static OnBuildingUpgradesManagerEvent OnUpgradeMoneyGeneration;
    public static OnBuildingUpgradesManagerEvent OnUpgradeBuildingBooster;


    public delegate void OnLaserModeManagerEvent(GameObject buildingReference);
    public static OnLaserModeManagerEvent OnLaserBeamerDamageModeActivated;
    public static OnLaserModeManagerEvent OnLaserBeamerSlowModeActivated;


    public delegate void OnBoostModeManagerEvent(GameObject buildingReference, BuildingBooster.BoosterMode boostMode);
    public static OnBoostModeManagerEvent OnBoostRangeBuildingBooster;
    public static OnBoostModeManagerEvent OnBoostDamageBuildingBooster;
    public static OnBoostModeManagerEvent OnBoostShootSpeedBuildingBooster;
    public static OnBoostModeManagerEvent OnBoostSlowPowerBuildingBooster;
    public static OnBoostModeManagerEvent OnBoostAOERangeBuildingBooster;
    public static OnBoostModeManagerEvent OnBoostMoneyGenerationBuildingBooster;

    private GameObject m_buildingSelectedBuffer;


    /// <summary>
    /// The following mothods are called by buttons
    /// </summary>
    public void UpgradeSelectedBuildingRange()
    {
        m_buildingSelectedBuffer = BuildingManager.instance.BuildingSelected;
        if (m_buildingSelectedBuffer != null && OnUpgradeRange != null)
            OnUpgradeRange(m_buildingSelectedBuffer);
    }

    public void UpgradeSelectedBuildingDamage()
    {
        m_buildingSelectedBuffer = BuildingManager.instance.BuildingSelected;
        if (m_buildingSelectedBuffer != null && OnUpgradeDamage != null)
            OnUpgradeDamage(m_buildingSelectedBuffer);
    }

    public void UpgradeSelectedBuildingShootSpeed()
    {
        m_buildingSelectedBuffer = BuildingManager.instance.BuildingSelected;
        if (m_buildingSelectedBuffer != null && OnUpgradeShootSpeed != null)
            OnUpgradeShootSpeed(m_buildingSelectedBuffer);
    }

    public void UpgradeSelectedBuildingAOERadiusEffect()
    {
        m_buildingSelectedBuffer = BuildingManager.instance.BuildingSelected;
        if (m_buildingSelectedBuffer != null && OnUpgradeAOERadiusEffect != null)
            OnUpgradeAOERadiusEffect(m_buildingSelectedBuffer);
    }

    public void UpgradeSelectedBuildingSlowPower()
    {
        m_buildingSelectedBuffer = BuildingManager.instance.BuildingSelected;
        if (m_buildingSelectedBuffer != null && OnUpgradeSlowPower != null)
            OnUpgradeSlowPower(m_buildingSelectedBuffer);
    }

    public void UpgradeSelectedBuildingMoneyGeneration()
    {
        m_buildingSelectedBuffer = BuildingManager.instance.BuildingSelected;
        if (m_buildingSelectedBuffer != null && OnUpgradeMoneyGeneration != null)
            OnUpgradeMoneyGeneration(m_buildingSelectedBuffer);
    }

    public void UpgradeSelectedBuildingOnUpgradeBuildingBooster()
    {
        m_buildingSelectedBuffer = BuildingManager.instance.BuildingSelected;
        if (m_buildingSelectedBuffer != null && OnUpgradeBuildingBooster != null)
            OnUpgradeBuildingBooster(m_buildingSelectedBuffer);
    }
    
    public void SetLaserBeamerToDamageMode()
    {
        m_buildingSelectedBuffer = BuildingManager.instance.BuildingSelected;
        if (m_buildingSelectedBuffer != null && OnLaserBeamerDamageModeActivated != null)
            OnLaserBeamerDamageModeActivated(m_buildingSelectedBuffer);
    }

    public void SetLaserBeamerToSlowMode()
    {
        m_buildingSelectedBuffer = BuildingManager.instance.BuildingSelected;
        if (m_buildingSelectedBuffer != null && OnLaserBeamerSlowModeActivated != null)
            OnLaserBeamerSlowModeActivated(m_buildingSelectedBuffer);
    }

    public void SetBuildingBoosterToBoostRange()
    {
        m_buildingSelectedBuffer = BuildingManager.instance.BuildingSelected;
        if (m_buildingSelectedBuffer != null && OnBoostRangeBuildingBooster != null)
            OnBoostRangeBuildingBooster(m_buildingSelectedBuffer, BuildingBooster.BoosterMode.BoostRange);
    }

    public void SetBuildingBoosterToBoostDamage()
    {
        m_buildingSelectedBuffer = BuildingManager.instance.BuildingSelected;
        if (m_buildingSelectedBuffer != null && OnBoostDamageBuildingBooster != null)
            OnBoostDamageBuildingBooster(m_buildingSelectedBuffer, BuildingBooster.BoosterMode.BoostDamage);
    }

    public void SetBuildingBoosterToBoostShootSpeed()
    {
        m_buildingSelectedBuffer = BuildingManager.instance.BuildingSelected;
        if (m_buildingSelectedBuffer != null && OnBoostShootSpeedBuildingBooster != null)
            OnBoostShootSpeedBuildingBooster(m_buildingSelectedBuffer, BuildingBooster.BoosterMode.BoostShootSpeed);
    }

    public void SetBuildingBoosterToBoostSlowPower()
    {
        m_buildingSelectedBuffer = BuildingManager.instance.BuildingSelected;
        if (m_buildingSelectedBuffer != null && OnBoostSlowPowerBuildingBooster != null)
            OnBoostSlowPowerBuildingBooster(m_buildingSelectedBuffer, BuildingBooster.BoosterMode.BoostSlowPower);
    }

    public void SetBuildingBoosterToBoostAOERange()
    {
        m_buildingSelectedBuffer = BuildingManager.instance.BuildingSelected;
        if (m_buildingSelectedBuffer != null && OnBoostAOERangeBuildingBooster != null)
            OnBoostAOERangeBuildingBooster(m_buildingSelectedBuffer, BuildingBooster.BoosterMode.BoostAOERange);
    }

    public void SetBuildingBoosterToBoostMoneyGeneration()
    {
        m_buildingSelectedBuffer = BuildingManager.instance.BuildingSelected;
        if (m_buildingSelectedBuffer != null && OnBoostMoneyGenerationBuildingBooster != null)
            OnBoostMoneyGenerationBuildingBooster(m_buildingSelectedBuffer, BuildingBooster.BoosterMode.BoostMoneyGeneration);
    }

}
