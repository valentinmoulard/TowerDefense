using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaypointsManager : MonoBehaviour
{
    /*Memo
    public static Action OnPlayerDeath;
    public delegate void ControllerEvent(Vector3 cursorPosition);
    */
    public static WaypointsManager instance;

    [SerializeField]
    private List<Transform> m_waypointList = null;

    private int m_waypointCount;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);
    }

    private void Start()
    {
        if (m_waypointList == null || m_waypointList.Count == 0)
            Debug.LogError("List of waypoints is null or empty!");
        m_waypointCount = m_waypointList.Count;
    }

    public Transform GetWaypoint(int index)
    {
        if (index >= m_waypointList.Count)
        {
            Debug.LogError("int index is out of waypoint list range!");
        }
        else
            return m_waypointList[index];

        return null;
    }

    public Transform GetLastWaypoint()
    {
        return m_waypointList[m_waypointCount - 1];
    }

    public int GetWaypointCount()
    {
        return m_waypointCount;
    }
}
