using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MoneyGenerationUpgrade", menuName = "ScriptableObjects/Upgrades/MoneyGeneration", order = 8)]
public class MoneyGenerationUpgradesScriptableObject : ScriptableObject
{
    [Serializable]
    public class BuildingMoneyGenerationUpgrade
    {
        public float m_cost;
        public float m_moneyGenerationAmount;
    }

    public List<BuildingMoneyGenerationUpgrade> m_moneyGenerationUpgradesList;
}
