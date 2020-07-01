using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Wave", menuName = "ScriptableObjects/WaveData", order = 1)]
public class WaveDataScriptableObject : ScriptableObject
{
    [Serializable]
    public class Wave{
        public List<EnemyGroup> m_enemyGroupList;
        public float m_timeBetweenWaves;
    }


    [Serializable]
    public class EnemyGroup
    {
        public GameObject m_enemyPrefabReference;
        public float m_timeBetweenEnemies;
        public int m_number;
    }

    public List<Wave> m_waveList;
}
