using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RangeUpgrade", menuName = "ScriptableObjects/Upgrades/Range", order = 1)]
public class RangeUpgradesScriptableObject : ScriptableObject
{
    [Serializable]
    public class BuildingRangeUpgrade
    {
        public float m_cost;
        public float m_range;
    }
    
    public List<BuildingRangeUpgrade> m_rangeUpgradesList;
}
