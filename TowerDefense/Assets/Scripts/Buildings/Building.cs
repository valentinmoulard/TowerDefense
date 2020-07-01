using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : MonoBehaviour
{
    public static Action<string> OnShowBuildingSellingValue;
    public static Action<string> OnBuildingValueIncreased;
    //event used by this andd all children of this class to notify the currency manager that an upgrade is bought
    public static Action<float> OnPurchaseUpgrade;

    private const float BUILDING_SELLING_VALUE_RATIO = 0.4f;

    protected float m_buildingValue;
    protected bool m_isBuildingAlive;

    protected virtual void OnEnable()
    {
        BuildingManager.OnBuildingSold += SellBuilding;
        BuildingManager.OnSendBuildingBuiltPrice += SetBuildingInitialValue;
        UIManager.OnShowCurrentBuildingUpgradeButtons += BroadcastOnShowBuildingSellingValueEvent;
    }

    protected virtual void OnDisable()
    {
        BuildingManager.OnBuildingSold -= SellBuilding;
        BuildingManager.OnSendBuildingBuiltPrice -= SetBuildingInitialValue;
        UIManager.OnShowCurrentBuildingUpgradeButtons -= BroadcastOnShowBuildingSellingValueEvent;
    }

    protected void InitializeBuilding()
    {
        m_isBuildingAlive = true;
    }

    private void SetBuildingInitialValue(float value)
    {
        m_buildingValue = value * BUILDING_SELLING_VALUE_RATIO;
    }

    protected void IncreaseBuildingValue(float value)
    {
        m_buildingValue += value * BUILDING_SELLING_VALUE_RATIO;
        BroadcastOnBuildingValueIncreasedEvent();
    }

    private void SellBuilding(GameObject buildingReference)
    {
        if (buildingReference == gameObject)
        {
            CurrencyManager.instance.AddCurrency(m_buildingValue);
            GameManager.instance.PrintGeneralMessage("Building sold for : " + m_buildingValue);
            Destroy(gameObject);
        }
    }

    protected void BroadcastOnPurchaseUpgradeEvent(float value)
    {
        if (OnPurchaseUpgrade != null)
            OnPurchaseUpgrade(value);
    }

    private void BroadcastOnShowBuildingSellingValueEvent(GameObject buildingReference)
    {
        if (OnShowBuildingSellingValue != null && buildingReference == gameObject)
            OnShowBuildingSellingValue(m_buildingValue.ToString("F0"));
    }

    private void BroadcastOnBuildingValueIncreasedEvent()
    {
        if (OnBuildingValueIncreased != null)
            OnBuildingValueIncreased(m_buildingValue.ToString("F0"));
    }
}
