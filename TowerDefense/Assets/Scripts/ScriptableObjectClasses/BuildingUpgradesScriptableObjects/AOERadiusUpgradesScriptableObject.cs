using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AOERadiusUpgrade", menuName = "ScriptableObjects/Upgrades/AOERadius", order = 2)]
public class AOERadiusUpgradesScriptableObject : ScriptableObject
{
    [Serializable]
    public class TurretDamageUpgrade
    {
        public float m_cost;
        public float m_AOERadius;
    }

    public List<TurretDamageUpgrade> m_AOERadiusUpgradesList;
}
