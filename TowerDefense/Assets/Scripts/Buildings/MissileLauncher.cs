using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileLauncher : BulletShooter
{
    //float : projectile damage
    public static Action<float> OnProjectileAOERadiusSent;
    //float : send the cost of the next upgrade to show the correct price on the purchase button
    public static Action<string> OnShowAOERangeUpgradePrice;

    [Header("MISSILE LAUNCHER")]
    [SerializeField]
    protected AOERadiusUpgradesScriptableObject m_AOERadiusUpgradesData = null;

    private float m_AOERadius;
    private int m_currentAOERadiusLevel;
    private int m_maxAOERadiusLevelIndex;

    protected override void OnEnable()
    {
        base.OnEnable();
        BuildingUpgradesAndModeManager.OnUpgradeAOERadiusEffect += UpgradeAOERadius;
        UIManager.OnShowCurrentBuildingUpgradeButtons += BroadcastOnShowAOERangeUpgradePriceEvent;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        BuildingUpgradesAndModeManager.OnUpgradeAOERadiusEffect -= UpgradeAOERadius;
        UIManager.OnShowCurrentBuildingUpgradeButtons -= BroadcastOnShowAOERangeUpgradePriceEvent;
    }


    protected override void Start()
    {
        InitializeMissileLauncher();
    }


    private void InitializeMissileLauncher()
    {
        base.InitializeBulletShooter();

        if (m_AOERadiusUpgradesData == null)
            Debug.LogError("The AOE radius data upgrade SO is missing in the inspector!", gameObject);

        m_maxAOERadiusLevelIndex = m_AOERadiusUpgradesData.m_AOERadiusUpgradesList.Count - 1;
        m_currentAOERadiusLevel = 0;
        m_AOERadius = m_AOERadiusUpgradesData.m_AOERadiusUpgradesList[m_currentAOERadiusLevel].m_AOERadius;
    }

    private void UpdateAOERadiusValue(float newValue)
    {
        m_AOERadius = newValue;
    }

    private void UpgradeAOERadius(GameObject buildingReference)
    {
        if (buildingReference == gameObject)
        {
            if (m_currentAOERadiusLevel < m_maxAOERadiusLevelIndex)
            {
                if (CurrencyManager.instance.CanPurchase(m_AOERadiusUpgradesData.m_AOERadiusUpgradesList[m_currentAOERadiusLevel + 1].m_cost))
                {
                    BroadcastOnPurchaseUpgradeEvent(m_AOERadiusUpgradesData.m_AOERadiusUpgradesList[m_currentAOERadiusLevel + 1].m_cost);
                    base.IncreaseBuildingValue(m_AOERadiusUpgradesData.m_AOERadiusUpgradesList[m_currentAOERadiusLevel + 1].m_cost);
                    m_currentAOERadiusLevel++;
                    UpdateAOERadiusValue(m_AOERadiusUpgradesData.m_AOERadiusUpgradesList[m_currentAOERadiusLevel].m_AOERadius);
                    BroadcastOnShowAOERangeUpgradePriceEvent(gameObject);
                }
                else
                {
                    GameManager.instance.PrintGeneralMessage("Not enough currency to improve building AOE radius...");
                }
            }
            else
            {
                GameManager.instance.PrintGeneralMessage("Can't Upgrade AOE radius anymore...");
            }
        }
    }

    protected override void BroadcastProjectileStats(GameObject projectileReference, GameObject target)
    {
        base.BroadcastProjectileStats(projectileReference, target);
        BroadcastOnProjectileAOERadiusSentEvent(m_AOERadius);
    }

    private void BroadcastOnProjectileAOERadiusSentEvent(float projectileAOERadius)
    {
        if (OnProjectileAOERadiusSent != null)
            OnProjectileAOERadiusSent(projectileAOERadius);
    }

    private void BroadcastOnShowAOERangeUpgradePriceEvent(GameObject buildingReference)
    {
        if (OnShowAOERangeUpgradePrice != null && buildingReference == gameObject)
        {
            if (m_currentAOERadiusLevel < m_maxAOERadiusLevelIndex)
                OnShowAOERangeUpgradePrice(m_AOERadiusUpgradesData.m_AOERadiusUpgradesList[m_currentAOERadiusLevel + 1].m_cost.ToString("F0"));
            else
                OnShowAOERangeUpgradePrice("Maxed!");
        }
    }
}
