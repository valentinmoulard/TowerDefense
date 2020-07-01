using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SlowPowerUpgrade", menuName = "ScriptableObjects/Upgrades/SlowPower", order = 5)]
public class SlowPowerUpgradesScriptableObject : ScriptableObject
{   
    [Serializable]
    public class BuildingSlowPowerUpgrade
    {
        public float m_cost;
        public float m_slowPower;
    }

    public List<BuildingSlowPowerUpgrade> m_slowPowerUpgradesList;
}
