using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class RangedBuilding : Building
{
    //float : send the cost of the next upgrade to show the correct price on the purchase button
    public static Action<string> OnShowUpgradeRangePrice;


    [Header("BUILDING ============================")]
    [SerializeField]
    protected SphereCollider m_colliderReference = null;

    [SerializeField]
    private GameObject m_rangeVisualFeedback = null;

    [SerializeField]
    private RangeUpgradesScriptableObject m_rangeUpgradesData = null;

    protected float m_turretRange;
    private int m_currentRangeLevel;
    private int m_maxRangeLevelIndex;

    protected override void OnEnable()
    {
        BuildingManager.OnShowSelectedTurretVisualFeedback += ManageRangeVisualFeedback;
        BuildingUpgradesAndModeManager.OnUpgradeRange += UpgradeRange;
        UIManager.OnShowCurrentBuildingUpgradeButtons += BroadcastOnShowUpgradeRangePriceEvent;
        
    }

    protected override void OnDisable()
    {
        BuildingManager.OnShowSelectedTurretVisualFeedback -= ManageRangeVisualFeedback;
        BuildingUpgradesAndModeManager.OnUpgradeRange -= UpgradeRange;
        UIManager.OnShowCurrentBuildingUpgradeButtons -= BroadcastOnShowUpgradeRangePriceEvent;
    }


    protected void InitializeRangedBuilding()
    {
        base.InitializeBuilding();

        if (m_rangeUpgradesData == null)
            Debug.LogError("The building upgrades SO is missing in the inspector!", gameObject);
        if (m_rangeVisualFeedback == null)
            Debug.LogError("The range visual feedback is missing in the inspector!", gameObject);
        if (m_colliderReference == null)
            Debug.LogError("The collider reference is missing in the inspector!");

        
        m_currentRangeLevel = 0;
        m_maxRangeLevelIndex = m_rangeUpgradesData.m_rangeUpgradesList.Count - 1;

        m_turretRange = m_rangeUpgradesData.m_rangeUpgradesList[m_currentRangeLevel].m_range;
        m_colliderReference.radius = m_turretRange;
        m_rangeVisualFeedback.transform.localScale = Vector3.one * m_turretRange;
        m_rangeVisualFeedback.SetActive(false);
    }

    private void ManageRangeVisualFeedback(GameObject turretSelected)
    {
        if (turretSelected == gameObject)
            m_rangeVisualFeedback.SetActive(true);
        else
            m_rangeVisualFeedback.SetActive(false);
    }

    private void UpdateRangeColliderAndVisualFeedback(float scale)
    {
        m_turretRange = scale;
        m_colliderReference.radius = m_turretRange;
        m_rangeVisualFeedback.transform.localScale = Vector3.one * m_turretRange;
    }

    protected void UpdateRangeValue(float newRadiusValue)
    {
        m_turretRange = newRadiusValue;
        UpdateRangeColliderAndVisualFeedback(m_turretRange);
    }

    private void UpgradeRange(GameObject buildingReference)
    {
        if (buildingReference == gameObject)
        {
            if (m_currentRangeLevel < m_maxRangeLevelIndex)
            {
                if (CurrencyManager.instance.CanPurchase(m_rangeUpgradesData.m_rangeUpgradesList[m_currentRangeLevel + 1].m_cost))
                {
                    BroadcastOnPurchaseUpgradeEvent(m_rangeUpgradesData.m_rangeUpgradesList[m_currentRangeLevel + 1].m_cost);
                    base.IncreaseBuildingValue(m_rangeUpgradesData.m_rangeUpgradesList[m_currentRangeLevel + 1].m_cost);
                    m_currentRangeLevel++;
                    UpdateRangeValue(m_rangeUpgradesData.m_rangeUpgradesList[m_currentRangeLevel].m_range);
                    BroadcastOnShowUpgradeRangePriceEvent(gameObject);
                }
                else
                {
                    GameManager.instance.PrintGeneralMessage("Not enough currency to improve building range...");
                }
            }
            else
            {
                GameManager.instance.PrintGeneralMessage("Can't Upgrade Range anymore...");
            }
        }
    }

    private void BroadcastOnShowUpgradeRangePriceEvent(GameObject buildingReference)
    {
        if (OnShowUpgradeRangePrice != null && buildingReference == gameObject)
        {
            if (m_currentRangeLevel < m_maxRangeLevelIndex)
                OnShowUpgradeRangePrice(m_rangeUpgradesData.m_rangeUpgradesList[m_currentRangeLevel + 1].m_cost.ToString("F0"));
            else
                OnShowUpgradeRangePrice("Maxed!");
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, m_turretRange);
    }

}
