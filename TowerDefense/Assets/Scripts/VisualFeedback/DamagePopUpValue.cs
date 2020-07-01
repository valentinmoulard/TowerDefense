using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DamagePopUpValue : MonoBehaviour
{
    [SerializeField]
    private TMP_Text m_damageTMPComponent = null;


    private void OnEnable()
    {

    }

    private void OnDisable()
    {

    }

    private void Start()
    {
        if (m_damageTMPComponent == null)
            Debug.LogError("The TMP component is missing in the inspector!", gameObject);
    }

    private void UpdateDamageTextValue(GameObject popUpReference, float damage)
    {
        if (popUpReference == gameObject)
            m_damageTMPComponent.text = damage.ToString("F2");
    }

}
