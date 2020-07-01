using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu(fileName = "LevelsData", menuName = "ScriptableObjects/LevelsData", order = 99)]
public class LevelsDataScriptableObject : ScriptableObject
{
    public List<Scene> m_levelsSceneList;

    public int GetLevelsCount()
    {
        return m_levelsSceneList.Count;
    }

}
