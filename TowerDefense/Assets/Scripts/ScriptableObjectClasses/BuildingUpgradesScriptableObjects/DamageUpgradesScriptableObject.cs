using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DamageUpgrade", menuName = "ScriptableObjects/Upgrades/Damage", order = 2)]
public class DamageUpgradesScriptableObject : ScriptableObject
{
    [Serializable]
    public class TurretDamageUpgrade
    {
        public float m_cost;
        public float m_damage;
    }

    public List<TurretDamageUpgrade> m_damageUpgradesList;
}
