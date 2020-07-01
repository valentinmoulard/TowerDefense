using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Node : MonoBehaviour
{
    //EVENTS
    //to give information (node position, building on it, etc)
    //gameobject : this current node gameobject, node : node component on the current gameobject
    public static Action<GameObject, Node> OnNodeClicked;
    //gameobject : this current node gameobject, node : node component on the current gameobject, gameobject : default turret prefab to build before game starts
    public static Action<GameObject, Node, GameObject> OnDefaultTurretBroadcasted;


    //INSPECTOR VARIABLES
    [Header("Attributes & References")]
    [SerializeField]
    private GameObject m_defaultTurretOnCurrentNode = null;

    [SerializeField]
    private float m_nodeOffsetSize = 0.5f;

    [Header("Visual Feedback Effects")]
    [SerializeField]
    private Renderer m_currentRenderer = null;

    [SerializeField]
    private Color m_colorWhenActive = Color.yellow;

    [SerializeField]
    private Color m_colorWhenNotActive = Color.white;



    //PRIVATE VARIABLES
    private GameObject m_currentBuilding;
    public GameObject CurrentBuilding() { return m_currentBuilding; }


    private bool m_isNodeHighlightsSystemActivated;
    private bool m_isNodeOccupied;

    public Vector3 NodeBuildPosition { get => transform.position + m_nodeOffsetSize * Vector3.up; }
    public bool IsNodeOccupied { get => m_isNodeOccupied; set => m_isNodeOccupied = value; }

    private void OnEnable()
    {
        PurchaseTurretButton.OnPurchasableTurretButtonClick += ActivateHighlightForNode;
        BuildingManager.OnCurrentBuildingToBuildReset += DeactivateHighlightForNode;
        BuildingManager.OnBuildingBuilt += GetBuildingBuiltOnCurrentNode;

        GameManager.OnTitleScreen += ResetNode;
    }

    private void OnDisable()
    {
        PurchaseTurretButton.OnPurchasableTurretButtonClick -= ActivateHighlightForNode;
        BuildingManager.OnCurrentBuildingToBuildReset += DeactivateHighlightForNode;
        BuildingManager.OnBuildingBuilt -= GetBuildingBuiltOnCurrentNode;

        GameManager.OnTitleScreen -= ResetNode;
    }

    void Start()
    {
        if (m_currentRenderer == null)
            Debug.LogError("The renderer component reference is missing in the inspector!");

        ResetNode();

        //if the current node has a default building, send this event to build the building
        if (m_defaultTurretOnCurrentNode != null)
            BroadcastOnDefaultTurretBroadcastedEvent();
    }

    private void ResetNode()
    {
        m_isNodeHighlightsSystemActivated = false;
        m_currentBuilding = null;
        m_isNodeOccupied = false;
    }


    private void GetBuildingBuiltOnCurrentNode(GameObject buildingReference, GameObject node)
    {
        if (node == gameObject)
        {
            m_currentBuilding = buildingReference;
            m_isNodeOccupied = true;
        }
    }

    private void OnMouseEnter()
    {
        if (EventSystem.current.IsPointerOverGameObject() || !m_isNodeHighlightsSystemActivated)
            return;

        m_currentRenderer.material.color = m_colorWhenActive;
    }

    private void OnMouseDown()
    {
        if (EventSystem.current.IsPointerOverGameObject())
            return;

        BroadcastOnNodeClickedEvent();
    }

    private void OnMouseExit()
    {
        m_currentRenderer.material.color = m_colorWhenNotActive;
    }

    private void BroadcastOnNodeClickedEvent()
    {
        if (OnNodeClicked != null)
            OnNodeClicked(gameObject, this);
    }

    private void BroadcastOnDefaultTurretBroadcastedEvent()
    {
        if (OnDefaultTurretBroadcasted != null)
            OnDefaultTurretBroadcasted(gameObject, this, m_defaultTurretOnCurrentNode);
    }

    private void ActivateHighlightForNode(BuildingDataScriptableObject turretToBuild)
    {
        m_isNodeHighlightsSystemActivated = true;
    }

    private void DeactivateHighlightForNode(BuildingDataScriptableObject turretToBuild)
    {
        m_isNodeHighlightsSystemActivated = false;
    }
}
