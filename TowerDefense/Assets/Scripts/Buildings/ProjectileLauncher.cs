using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileLauncher : OffensiveBuilding
{
    //gameobject : target, gameobject : projectile reference
    public static Action<GameObject, GameObject> OnProjectileSent;
    //float : send the cost of the next upgrade to show the correct price on the purchase button
    public static Action<string> OnShowShootSpeedUpgradePrice;

    [Header("PROJECTILE LAUNCHER =======================================")]
    [SerializeField]
    private ShootSpeedUpgradesScriptableObject m_shootSpeedUpgradesData = null;

    [SerializeField]
    private GameObject m_projectilePrefabReference = null;

    [SerializeField]
    protected GameObject m_projectileSpawnLocation = null;


    private bool m_canShoot;
    private int m_currentShootSpeedLevel;
    private int m_maxShootSpeedLevelIndex;

    private float m_turretShootSpeed;
    private float m_shootCountdownBuffer;

    private GameObject m_pooledBulletReferenceBuffer;
    private GameObject m_initialProjectilePrefabReference;

    protected override void OnEnable()
    {
        base.OnEnable();
        BuildingUpgradesAndModeManager.OnUpgradeShootSpeed += UpgradeShootSpeed;
        UIManager.OnShowCurrentBuildingUpgradeButtons += BroadcastOnShowShootSpeedUpgradePriceEvent;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        BuildingUpgradesAndModeManager.OnUpgradeShootSpeed -= UpgradeShootSpeed;
        UIManager.OnShowCurrentBuildingUpgradeButtons -= BroadcastOnShowShootSpeedUpgradePriceEvent;
    }


    private void Update()
    {
        if (m_target != null)
            ShootProjectileToTarget();
    }

    protected void InitializeProjectileLauncher()
    {
        base.InitializeOffensiveBuilding();

        if (m_shootSpeedUpgradesData == null)
            Debug.LogError("The turret upgrade SO is missing in the inspector!", gameObject);
        if (m_projectilePrefabReference == null)
            Debug.LogError("The projectile prefab reference is missing in the inspector!");
        if (m_projectileSpawnLocation == null)
            Debug.LogError("The projectile spawn position object is missing in the inspector!");

        m_currentShootSpeedLevel = 0;
        m_turretShootSpeed = m_shootSpeedUpgradesData.m_shootSpeedUpgradesList[m_currentShootSpeedLevel].m_shootspeed;
        m_maxShootSpeedLevelIndex = m_shootSpeedUpgradesData.m_shootSpeedUpgradesList.Count - 1;

        m_initialProjectilePrefabReference = m_projectilePrefabReference;
        m_canShoot = true;
        m_shootCountdownBuffer = 1 / m_turretShootSpeed;

    }

    private void ShootProjectileToTarget()
    {
        if (m_target != null && m_target.activeInHierarchy == true && m_canShoot)
        {
            m_canShoot = false;
            Shoot();
        }

        if (!m_canShoot)
        {
            m_shootCountdownBuffer -= Time.deltaTime;
            if (m_shootCountdownBuffer < 0)
            {
                m_canShoot = true;
                m_shootCountdownBuffer = 1 / m_turretShootSpeed;
            }
        }
    }

    private void Shoot()
    {
        m_pooledBulletReferenceBuffer = PoolManager.instance.SpawnPooledObject(m_projectilePrefabReference, m_projectileSpawnLocation.transform.position, Quaternion.identity);

        BroadcastOnBulletShootEvent(m_pooledBulletReferenceBuffer, m_target);
    }

    private void UpdateShootSpeedValue(float newValue)
    {
        m_turretShootSpeed = newValue;
    }

    private void UpgradeShootSpeed(GameObject buildingReference)
    {
        if (buildingReference == gameObject)
        {
            if (m_currentShootSpeedLevel < m_maxShootSpeedLevelIndex)
            {
                if (CurrencyManager.instance.CanPurchase(m_shootSpeedUpgradesData.m_shootSpeedUpgradesList[m_currentShootSpeedLevel + 1].m_cost))
                {
                    BroadcastOnPurchaseUpgradeEvent(m_shootSpeedUpgradesData.m_shootSpeedUpgradesList[m_currentShootSpeedLevel + 1].m_cost);
                    base.IncreaseBuildingValue(m_shootSpeedUpgradesData.m_shootSpeedUpgradesList[m_currentShootSpeedLevel + 1].m_cost);
                    m_currentShootSpeedLevel++;
                    UpdateShootSpeedValue(m_shootSpeedUpgradesData.m_shootSpeedUpgradesList[m_currentShootSpeedLevel].m_shootspeed);
                    BroadcastOnShowShootSpeedUpgradePriceEvent(gameObject);
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

    #region BROADCASTERS
    private void BroadcastOnBulletShootEvent(GameObject projectileObjectReference, GameObject targetObjectReference)
    {
        if (OnProjectileSent != null)
            OnProjectileSent(projectileObjectReference, targetObjectReference);
    }

    private void BroadcastOnShowShootSpeedUpgradePriceEvent(GameObject buildingReference)
    {
        if (OnShowShootSpeedUpgradePrice != null && buildingReference == gameObject)
        {
            if (m_currentShootSpeedLevel < m_maxShootSpeedLevelIndex)
                OnShowShootSpeedUpgradePrice(m_shootSpeedUpgradesData.m_shootSpeedUpgradesList[m_currentShootSpeedLevel + 1].m_cost.ToString("F0"));
            else
                OnShowShootSpeedUpgradePrice("Maxed!");
        }
    }

    #endregion
}
