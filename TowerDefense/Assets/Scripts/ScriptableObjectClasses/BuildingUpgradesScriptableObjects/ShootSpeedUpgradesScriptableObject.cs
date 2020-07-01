using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ShootSpeedUpgrade", menuName = "ScriptableObjects/Upgrades/ShootSpeed", order = 3)]
public class ShootSpeedUpgradesScriptableObject : ScriptableObject
{
    [Serializable]
    public class TurretShootSpeedUpgrade
    {
        public float m_cost;
        public float m_shootspeed;
    }

    public List<TurretShootSpeedUpgrade> m_shootSpeedUpgradesList;
}