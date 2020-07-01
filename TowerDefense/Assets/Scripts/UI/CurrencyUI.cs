using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CurrencyUI : MonoBehaviour
{
    [SerializeField]
    private TMP_Text m_tmpComponent = null;

    private void OnEnable()
    {
        CurrencyManager.OnCurrencyValueChanged += UpdateCurrencyText;
    }

    private void OnDisable()
    {
        CurrencyManager.OnCurrencyValueChanged -= UpdateCurrencyText;
    }

    private void Start()
    {
        if (m_tmpComponent == null)
            Debug.LogError("TMP component reference is missing in the inspector!");

        m_tmpComponent.text = CurrencyManager.instance.GetCurrency().ToString("f0");
    }

    private void UpdateCurrencyText(float newCurrencyValue)
    {
        m_tmpComponent.text = newCurrencyValue.ToString("f0");
    }

}
