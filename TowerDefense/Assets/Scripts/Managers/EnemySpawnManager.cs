using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnManager : MonoBehaviour
{
    public static Action<int> OnNewWave;
    public static Action<int> OnWaveSurvived;
    public static Action OnLevelCleared;

    private const float TIMER_CHECK_LEVEL_CLEARED = 2f;

    [SerializeField]
    private WaveDataScriptableObject m_waveData = null;
    [SerializeField]
    private Transform m_spawnPosition = null;

    private List<GameObject> m_enemiesList;
    private Coroutine m_spawnCoroutine;
    private Coroutine m_checkLevelClearedCoroutine;
    private WaveDataScriptableObject.Wave m_currentWaveData;
    private WaveDataScriptableObject.EnemyGroup m_currentEnemyGroupData;
    private int m_currentWaveIndex;
    private int m_waveCount;
    private int m_currentEnemyGroupIndex;
    private int m_enemyGroupCount;

    private void OnEnable()
    {
        GameManager.OnGameStart += StartEnemySpawn;
        GameManager.OnGameOver += BroadcastOnWaveSurvivedEvent;
        GameManager.OnTitleScreen += ResetEnemySpawnManager;
        Enemy.OnEnemyKilled += RemoveEnemyFromList;

        m_enemiesList = new List<GameObject>();
    }

    private void OnDisable()
    {
        GameManager.OnGameStart -= StartEnemySpawn;
        GameManager.OnGameOver -= BroadcastOnWaveSurvivedEvent;
        GameManager.OnTitleScreen -= ResetEnemySpawnManager;
        Enemy.OnEnemyKilled -= RemoveEnemyFromList;
    }

    private void ResetEnemySpawnManager()
    {
        if (m_spawnCoroutine != null)
            StopCoroutine(m_spawnCoroutine);
    }

    private void StartEnemySpawn()
    {
        if (m_waveData == null || m_waveData.m_waveList.Count == 0)
            Debug.LogError("The wave list is null or empty!");
        if (m_spawnPosition == null)
            Debug.LogError("Spawn position is null or missing in the inspector!");

        m_currentWaveIndex = 0;
        m_currentWaveData = m_waveData.m_waveList[m_currentWaveIndex];
        m_waveCount = m_waveData.m_waveList.Count;

        m_currentEnemyGroupIndex = 0;
        m_currentEnemyGroupData = m_currentWaveData.m_enemyGroupList[m_currentEnemyGroupIndex];
        m_enemyGroupCount = m_currentWaveData.m_enemyGroupList.Count;

        BroadcastOnNewWaveEvent();
        m_spawnCoroutine = StartCoroutine(SpawnDelay());
    }

    private IEnumerator SpawnDelay()
    {
        while (m_currentWaveIndex < m_waveCount)
        {
            while (m_currentEnemyGroupIndex < m_enemyGroupCount)
            {
                for (int i = 0; i < m_currentEnemyGroupData.m_number; i++)
                {
                    m_enemiesList.Add(PoolManager.instance.SpawnPooledObject(m_currentEnemyGroupData.m_enemyPrefabReference, m_spawnPosition.position, Quaternion.identity));
                    yield return new WaitForSeconds(m_currentEnemyGroupData.m_timeBetweenEnemies);
                }

                GetNextEnemyGroup();
            }

            yield return new WaitForSeconds(m_currentWaveData.m_timeBetweenWaves);
            GetNextWave();
        }

        //when all the enemies of the levels have spawned, we check if they are alive in this coroutine to know if the player won the level or not
        m_checkLevelClearedCoroutine = StartCoroutine(CheckLevelCleared());
    }

    private void RemoveEnemyFromList(GameObject targetReference)
    {
        m_enemiesList.Remove(targetReference);
    }

    private IEnumerator CheckLevelCleared()
    {
        while(m_enemiesList.Count > 0)
        {
            yield return new WaitForSeconds(TIMER_CHECK_LEVEL_CLEARED);
        }
        BroadcastOnLevelClearedEvent();
        BroadcastOnWaveSurvivedEvent();
    }

    private void GetNextEnemyGroup()
    {
        m_currentEnemyGroupIndex++;
        if (m_currentEnemyGroupIndex < m_enemyGroupCount)
            m_currentEnemyGroupData = m_currentWaveData.m_enemyGroupList[m_currentEnemyGroupIndex];
    }

    private void GetNextEnemyGroupOnNextWave()
    {
        m_currentEnemyGroupIndex = 0;
        m_currentEnemyGroupData = m_currentWaveData.m_enemyGroupList[m_currentEnemyGroupIndex];
        m_enemyGroupCount = m_currentWaveData.m_enemyGroupList.Count;
    }

    private void GetNextWave()
    {
        m_currentWaveIndex++;
        if (m_currentWaveIndex < m_waveCount)
            m_currentWaveData = m_waveData.m_waveList[m_currentWaveIndex];

        GetNextEnemyGroupOnNextWave();

        BroadcastOnNewWaveEvent();
    }

    private void StopSpawnCoroutine()
    {
        StopCoroutine(m_spawnCoroutine);
    }

    private void BroadcastOnNewWaveEvent()
    {
        if (OnNewWave != null)
            OnNewWave(m_currentWaveIndex);
    }

    private void BroadcastOnWaveSurvivedEvent()
    {
        if (OnWaveSurvived != null)
            OnWaveSurvived(m_currentWaveIndex);
    }

    private void BroadcastOnLevelClearedEvent()
    {
        if (OnLevelCleared != null)
            OnLevelCleared();
    }
}
