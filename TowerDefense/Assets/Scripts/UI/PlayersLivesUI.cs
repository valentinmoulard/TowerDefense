using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayersLivesUI : MonoBehaviour
{
    [SerializeField]
    private TMP_Text m_tmpComponent = null;

    private void OnEnable()
    {
        PlayerManager.OnPlayersHealthChanged += UpdatePlayersLivesValue;
    }

    private void OnDisable()
    {
        PlayerManager.OnPlayersHealthChanged -= UpdatePlayersLivesValue;
    }

    private void Start()
    {
        if (m_tmpComponent == null)
            Debug.LogError("TMP component reference is missing in the inspector!");
    }

    private void UpdatePlayersLivesValue(float newPlayersLivesValue)
    {
        m_tmpComponent.text = newPlayersLivesValue.ToString("f0");
    }
}
