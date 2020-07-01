using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WaveNumberUI : MonoBehaviour
{
    [SerializeField]
    private TMP_Text m_tmpComponent = null;

    private void OnEnable()
    {
        EnemySpawnManager.OnNewWave += UpdateWaveNumber;
        EnemySpawnManager.OnWaveSurvived += UpdateWaveNumber;
    }

    private void OnDisable()
    {
        EnemySpawnManager.OnNewWave -= UpdateWaveNumber;
        EnemySpawnManager.OnWaveSurvived -= UpdateWaveNumber;
    }

    private void Start()
    {
        if (m_tmpComponent == null)
            Debug.LogError("TMP component reference is missing in the inspector!");
    }

    private void UpdateWaveNumber(int newWaveValue)
    {
        m_tmpComponent.text = newWaveValue.ToString();
    }
}
