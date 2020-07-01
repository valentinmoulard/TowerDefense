using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PurchaseTurretButton : MonoBehaviour
{
    public static Action<BuildingDataScriptableObject> OnPurchasableTurretButtonClick;

    [Header("Attributes")]
    [SerializeField]
    private BuildingDataScriptableObject m_buildingData = null;

    [Header("References")]
    [SerializeField]
    private TMP_Text m_nameTMP = null;
    [SerializeField]
    private TMP_Text m_priceTMP = null;

    private void Start()
    {
        if (m_buildingData == null)
            Debug.LogError("The building data is missing in the inspector!", gameObject);

        if (m_priceTMP == null || m_nameTMP == null)
            Debug.LogError("A TMP component reference is missing in the inspector!", gameObject);

        m_priceTMP.text = m_buildingData.m_buildingPrice.ToString("F2");
        m_nameTMP.text = m_buildingData.m_name;
    }

    /// <summary>
    /// Called by purchase button
    /// </summary>
    public void PurchaseTurret()
    {
        if (CurrencyManager.instance.CanPurchase(m_buildingData.m_buildingPrice))
        {
            BroadcastOnPurchasableTurretButtonClickEvent();
        }
        else
        {
            GameManager.instance.PrintGeneralMessage("Cannot buy the building : " + m_buildingData.m_name);
        }
    }

    private void BroadcastOnPurchasableTurretButtonClickEvent()
    {
        if (OnPurchasableTurretButtonClick != null)
            OnPurchasableTurretButtonClick(m_buildingData);
    }

}
