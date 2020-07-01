using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Building", menuName = "ScriptableObjects/BuildingData", order = 1)]
public class BuildingDataScriptableObject : ScriptableObject
{
    public string m_name;
    public GameObject m_buildingPrefabReference;
    public float m_buildingPrice;
}
