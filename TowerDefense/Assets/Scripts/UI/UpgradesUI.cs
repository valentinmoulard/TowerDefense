using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UpgradesUI : MonoBehaviour
{
    [SerializeField]
    private GameObject m_rangeUpgradeButton = null;
    [SerializeField]
    private TMP_Text m_rangeTextPrice = null;
    [SerializeField]
    private GameObject m_shootSpeedUpgradeButton = null;
    [SerializeField]
    private TMP_Text m_shootSpeedTextPrice = null;
    [SerializeField]
    private GameObject m_damageUpgradeButton = null;
    [SerializeField]
    private TMP_Text m_damageTextPrice = null;
    [SerializeField]
    private GameObject m_AOERadiusUpgradeButton = null;
    [SerializeField]
    private TMP_Text m_AOERadiusTextPrice = null;
    [SerializeField]
    private GameObject m_slowPowerUpgradeButton = null;
    [SerializeField]
    private TMP_Text m_slowPowerTextPrice = null;
    [SerializeField]
    private GameObject m_moneyGenerationUpgradeButton = null;
    [SerializeField]
    private TMP_Text m_moneyGenerationTextPrice = null;

    [SerializeField]
    private TMP_Text m_buildingValueSellingPriceText = null;

    private void OnEnable()
    {
        DisableAllButtons();
        RangedBuilding.OnShowUpgradeRangePrice += ShowUpgradeRangeButton;
        ProjectileLauncher.OnShowShootSpeedUpgradePrice += ShowUpgradeShootSpeedButton;
        BulletShooter.OnShowDamageUpgradePrice += ShowUpgradeDamageButton;
        MissileLauncher.OnShowAOERangeUpgradePrice += ShowUpgradeAOERadiusButton;
        LaserBeamer.OnShowDamageUpgradePrice += ShowUpgradeDamageButton;
        LaserBeamer.OnShowSlowPowerUpgradePrice += ShowUpgradeSlowPowerButton;
        MoneyGenerator.OnShowMoneyGenerationUpgradePrice += ShowMoneyGenerationButton;

        RangedBuilding.OnShowBuildingSellingValue += UpdateBuildingSellingValue;
        RangedBuilding.OnBuildingValueIncreased += UpdateBuildingSellingValue;
    }

    private void OnDisable()
    {
        RangedBuilding.OnShowUpgradeRangePrice -= ShowUpgradeRangeButton;
        ProjectileLauncher.OnShowShootSpeedUpgradePrice -= ShowUpgradeShootSpeedButton;
        BulletShooter.OnShowDamageUpgradePrice -= ShowUpgradeDamageButton;
        MissileLauncher.OnShowAOERangeUpgradePrice -= ShowUpgradeAOERadiusButton;
        LaserBeamer.OnShowDamageUpgradePrice -= ShowUpgradeDamageButton;
        LaserBeamer.OnShowSlowPowerUpgradePrice -= ShowUpgradeSlowPowerButton;
        MoneyGenerator.OnShowMoneyGenerationUpgradePrice -= ShowMoneyGenerationButton;

        RangedBuilding.OnShowBuildingSellingValue -= UpdateBuildingSellingValue;
        RangedBuilding.OnBuildingValueIncreased -= UpdateBuildingSellingValue;
    }


    private void Start()
    {
        if (m_rangeUpgradeButton == null || m_shootSpeedUpgradeButton == null ||
            m_damageUpgradeButton == null || m_AOERadiusUpgradeButton == null ||
            m_slowPowerUpgradeButton == null || m_moneyGenerationUpgradeButton == null)
        {
            Debug.LogError("An upgrade button GO reference is missing in the inspector!");
        }

        if (m_rangeTextPrice == null || m_shootSpeedTextPrice == null ||
            m_damageTextPrice == null || m_AOERadiusTextPrice == null ||
            m_slowPowerTextPrice == null || m_moneyGenerationTextPrice == null ||
            m_buildingValueSellingPriceText == null)
        {
            Debug.LogError("An upgrade TMP text reference is missing in the inspector!");
        }
    }

    private void DisableAllButtons()
    {
        m_rangeUpgradeButton.SetActive(false);
        m_shootSpeedUpgradeButton.SetActive(false);
        m_damageUpgradeButton.SetActive(false);
        m_AOERadiusUpgradeButton.SetActive(false);
        m_slowPowerUpgradeButton.SetActive(false);
        m_moneyGenerationUpgradeButton.SetActive(false);
    }

    private void UpdateBuildingSellingValue(string sellingValue)
    {
        m_buildingValueSellingPriceText.text = sellingValue;
    }

    private void ShowUpgradeRangeButton(string upgradePrice)
    {
        m_rangeUpgradeButton.SetActive(true);
        m_rangeTextPrice.text = upgradePrice;
    }

    private void ShowUpgradeShootSpeedButton(string upgradePrice)
    {
        m_shootSpeedUpgradeButton.SetActive(true);
        m_shootSpeedTextPrice.text = upgradePrice;
    }

    private void ShowUpgradeDamageButton(string upgradePrice)
    {
        m_damageUpgradeButton.SetActive(true);
        m_damageTextPrice.text = upgradePrice;
    }

    private void ShowUpgradeAOERadiusButton(string upgradePrice)
    {
        m_AOERadiusUpgradeButton.SetActive(true);
        m_AOERadiusTextPrice.text = upgradePrice;
    }

    private void ShowUpgradeSlowPowerButton(string upgradePrice)
    {
        m_slowPowerUpgradeButton.SetActive(true);
        m_slowPowerTextPrice.text = upgradePrice;
    }

    private void ShowMoneyGenerationButton(string upgradePrice)
    {
        m_moneyGenerationUpgradeButton.SetActive(true);
        m_moneyGenerationTextPrice.text = upgradePrice;
    }
}
