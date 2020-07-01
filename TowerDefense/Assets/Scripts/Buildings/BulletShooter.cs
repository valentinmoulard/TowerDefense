using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletShooter : ProjectileLauncher
{
    //float : projectile damage
    public static Action<float> OnProjectileDamageSent;
    //float : send the cost of the next upgrade to show the correct price on the purchase button
    public static Action<string> OnShowDamageUpgradePrice;

    [Header("BULLET SHOOTER")]
    [SerializeField]
    protected DamageUpgradesScriptableObject m_damageUpgradesData = null;

    private float m_projectileDamage;
    private int m_currentDamageLevel;
    private int m_maxDamageLevelIndex;


    protected override void OnEnable()
    {
        base.OnEnable();
        BuildingUpgradesAndModeManager.OnUpgradeDamage += UpgradeDamage;
        ProjectileLauncher.OnProjectileSent += BroadcastProjectileStats;
        UIManager.OnShowCurrentBuildingUpgradeButtons += BroadcastOnShowDamageUpgradePriceEvent;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        BuildingUpgradesAndModeManager.OnUpgradeDamage -= UpgradeDamage;
        ProjectileLauncher.OnProjectileSent -= BroadcastProjectileStats;
        UIManager.OnShowCurrentBuildingUpgradeButtons -= BroadcastOnShowDamageUpgradePriceEvent;
    }


    protected virtual void Start()
    {
        InitializeBulletShooter();
    }


    protected void InitializeBulletShooter()
    {
        base.InitializeProjectileLauncher();

        if (m_damageUpgradesData == null)
            Debug.LogError("The damage upgrade SO is missing in the inspector!", gameObject);

        m_maxDamageLevelIndex = m_damageUpgradesData.m_damageUpgradesList.Count - 1;
        m_currentDamageLevel = 0;
        m_projectileDamage = m_damageUpgradesData.m_damageUpgradesList[m_currentDamageLevel].m_damage;
    }

    private void UpdateDamageValue(float newValue)
    {
        m_projectileDamage = newValue;
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
                    BroadcastOnShowDamageUpgradePriceEvent(gameObject);
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

    protected virtual void BroadcastProjectileStats(GameObject projectileReference, GameObject target)
    {
        BroadcastOnProjectileDamageSentEvent(m_projectileDamage);
    }

    private void BroadcastOnProjectileDamageSentEvent(float projectileDamage)
    {
        if (OnProjectileDamageSent != null)
            OnProjectileDamageSent(projectileDamage);
    }

    private void BroadcastOnShowDamageUpgradePriceEvent(GameObject buildingReference)
    {
        if (OnShowDamageUpgradePrice != null && buildingReference == gameObject)
        {
            if (m_currentDamageLevel < m_maxDamageLevelIndex)
                OnShowDamageUpgradePrice(m_damageUpgradesData.m_damageUpgradesList[m_currentDamageLevel + 1].m_cost.ToString("F0"));
            else
                OnShowDamageUpgradePrice("Maxed!");
        }
    }
}
