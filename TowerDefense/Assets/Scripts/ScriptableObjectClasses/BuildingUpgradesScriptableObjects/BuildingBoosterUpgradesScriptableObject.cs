using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BuildingBoosterUpgrade", menuName = "ScriptableObjects/Upgrades/BuildingBoosterUpgrade", order = 10)]
public class BuildingBoosterUpgradesScriptableObject : ScriptableObject
{
    [Serializable]
    public class BuildingBoosterUpgrade
    {
        public float m_cost;
        public float m_rangeBoostValue;
        public float m_damageBoostValue;
        public float m_shootSpeedBoostValue;
        public float m_slowPowerBoostValue;
        public float m_AOERangeBoostValue;
        public float m_moneyGenerationBoostValue;
    }

    public List<BuildingBoosterUpgrade> m_buildingBoosterUpgradeList;
}
