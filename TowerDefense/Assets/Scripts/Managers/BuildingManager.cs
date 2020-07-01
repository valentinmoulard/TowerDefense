using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingManager : MonoBehaviour
{
    //EVENTS ==================================================================================
    public static Action<BuildingDataScriptableObject> OnCurrentBuildingToBuildReset;
    //gameobject : building built reference, gameobject : node gameobject reference
    public static Action<GameObject, GameObject> OnBuildingBuilt;
    public static Action<GameObject> OnBuildingSold;
    public static Action<GameObject> OnShowSelectedTurretVisualFeedback;
    public static Action<float> OnSendBuildingBuiltPrice;
    
    //GO : building reference, V3 :  building position
    public static Action<GameObject, Vector3> OnShowCurrentBuildingUI;
    public static Action OnHideCurrentBuildingUI;

    //test
    public static Action<GameObject> OnShowCurrentBuildingInterfaceUI;

    //SINGLETON ===============================================================================
    public static BuildingManager instance;

    [SerializeField, Tooltip("To store all the instanciated turret as child")]
    private Transform m_turretParentTransform = null;
    public Transform TurretParentTransform { get => m_turretParentTransform; }


    //PRIVATE VARIABLES =======================================================================
    //to build building
    private BuildingDataScriptableObject m_buildingDataToBuild;
    private GameObject m_currentBuildingToBuildPrefabReference;

    //to manage building
    private GameObject m_buildingSelected;
    public GameObject BuildingSelected { get => m_buildingSelected; }

    private List<GameObject> m_instanciatedTurretsReferenceList;


    private bool m_isInPurchaseMode;
    private bool m_isInManageMode;


    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);
        m_instanciatedTurretsReferenceList = new List<GameObject>();
    }

    private void OnEnable()
    {
        //when a purchase building button is pressed, enters the purchase mode...
        PurchaseTurretButton.OnPurchasableTurretButtonClick += GetBuildingDataToBuild;
        //When a node is clicked while we are in purchase mode, try to build building
        Node.OnNodeClicked += ManageBuildingOnSelectedNode;
        //If we have a default building to build before the start of the game
        Node.OnDefaultTurretBroadcasted += TryBuildBuilding;

        GameManager.OnTitleScreen += ResetBuildingManager;
    }

    private void OnDisable()
    {
        PurchaseTurretButton.OnPurchasableTurretButtonClick -= GetBuildingDataToBuild;
        Node.OnNodeClicked -= ManageBuildingOnSelectedNode;
        Node.OnDefaultTurretBroadcasted -= TryBuildBuilding;

        GameManager.OnTitleScreen -= ResetBuildingManager;
    }

    private void Start()
    {
        InitializeBuildingManager();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
            ExitCurrentMode();
    }


    private void InitializeBuildingManager()
    {
        if (m_turretParentTransform == null)
            Debug.LogError("The gameobject storing the turrets as child is missing in the inspector!!!");

        m_buildingDataToBuild = null;
        m_currentBuildingToBuildPrefabReference = null;
        m_isInPurchaseMode = false;
        m_isInManageMode = false;
    }


    /// <summary>
    /// After a purchasable building is selected, and after choosing a node to build on, try to build the building on top of it
    /// If a node is clicked and we are not in puchase mode, it means we try to access the upgrade mode.
    /// If the node is occupied, shows the upgrade UI
    /// </summary>
    /// <param name="node"></param>
    private void ManageBuildingOnSelectedNode(GameObject node, Node nodeComponentReference)
    {
        if (nodeComponentReference != null)
        {
            if (m_isInPurchaseMode)
            {
                if (nodeComponentReference.IsNodeOccupied)
                    Debug.Log("There is already a building on occupying this area!");

                else if (m_buildingDataToBuild != null)
                    BuildBuilding(node, nodeComponentReference.NodeBuildPosition);
            }

            else
            {
                if (nodeComponentReference.IsNodeOccupied)
                {
                    m_isInManageMode = true;
                    m_buildingSelected = nodeComponentReference.CurrentBuilding();
                    BroadcastOnShowUpgradeUIEvent(m_buildingSelected, nodeComponentReference.NodeBuildPosition);
                    BroadcastOnShowSelectedTurretVisualFeedbackEvent(m_buildingSelected);
                }
                else
                {
                    ExitCurrentMode();
                }
            }
        }
    }


    #region BUILD BUILDINGS
    /// <summary>
    /// When a puchasable turret button is pressed, get the building data of the concerned building
    /// </summary>
    /// <param name="buildingData"></param>
    private void GetBuildingDataToBuild(BuildingDataScriptableObject buildingData)
    {
        BroadcastOnHideUpgradeUIEvent();
        m_isInPurchaseMode = true;
        m_buildingDataToBuild = buildingData;
    }


    private void TryBuildBuilding(GameObject node, Node nodeComponentReference, GameObject defaultTurretPrefabReference)
    {
        if (nodeComponentReference != null)
        {
            if (nodeComponentReference.IsNodeOccupied)
                Debug.Log("There is already a building '"+ nodeComponentReference.CurrentBuilding().name + "' occupying this area!", gameObject);

            else
                BuildBuilding(node, nodeComponentReference.NodeBuildPosition, defaultTurretPrefabReference);
        }
    }


    /// <summary>
    /// If we start the game with some default turrets on some nodes, we instantiate then in the 'else' part of the code.
    /// This method is used to instantiate buildings from in game mode (decrease currency etc) and also used to instantiate the default turrets if there are default turrets on the map before the game starts
    /// If the node is available, builds the building on the node. Then broadcasts the needed events (decrease currency, etc)
    /// </summary>
    /// <param name="nodeTransform"></param>
    /// <param name="buildingPosition"></param>
    private void BuildBuilding(GameObject node, Vector3 buildingPosition, GameObject defaultTurretPrefabReference = null)
    {
        if (defaultTurretPrefabReference == null)
        {
            //m_currentBuildingToBuildPrefabReference = PoolManager.instance.SpawnPooledObject(m_buildingDataToBuild.m_buildingPrefabReference, buildingPosition, Quaternion.identity);
            m_currentBuildingToBuildPrefabReference = (GameObject)Instantiate(m_buildingDataToBuild.m_buildingPrefabReference, buildingPosition, Quaternion.identity, m_turretParentTransform);
            m_instanciatedTurretsReferenceList.Add(m_currentBuildingToBuildPrefabReference);

            //to decrease currency
            BroadcastOnSendBuildingBuiltPriceEvent(m_buildingDataToBuild.m_buildingPrice);
        }
        else
        {
            m_currentBuildingToBuildPrefabReference = (GameObject)Instantiate(defaultTurretPrefabReference, buildingPosition, Quaternion.identity);
        }

        BroadcastOnBuildingBuiltEvent(m_currentBuildingToBuildPrefabReference, node);
        ExitCurrentMode();
    }
    #endregion


    #region MANAGE BUILDINGS
    public void SellSelectedBuilding()
    {
        ExitCurrentMode();

        BroadcastOnBuildingSoldEvent(m_buildingSelected);
    }
    #endregion


    /// <summary>
    /// if we cancel the purchase or finish a purchase
    /// </summary>
    private void ExitCurrentMode()
    {
        if (m_buildingDataToBuild != null && m_isInPurchaseMode)
        {
            BroadcastOnCurrentBuildingToBuildResetEvent(m_buildingDataToBuild);
            m_buildingDataToBuild = null;
            m_currentBuildingToBuildPrefabReference = null;
            m_isInPurchaseMode = false;
        }

        if (m_isInManageMode)
        {
            m_isInManageMode = false;
            BroadcastOnHideUpgradeUIEvent();
            BroadcastOnShowSelectedTurretVisualFeedbackEvent(null);
        }
    }

    private void ResetBuildingManager()
    {
        BroadcastOnCurrentBuildingToBuildResetEvent(m_buildingDataToBuild);
        m_isInPurchaseMode = false;
        m_isInManageMode = false;
        m_currentBuildingToBuildPrefabReference = null;
        m_buildingDataToBuild = null;
        DestroyAllTurrets();
    }

    private void DestroyAllTurrets()
    {
        for (int i = 0; i < m_instanciatedTurretsReferenceList.Count; i++)
        {
            Destroy(m_instanciatedTurretsReferenceList[i].gameObject);
        }
    }

    #region BROADCASTERS
    private void BroadcastOnBuildingBuiltEvent(GameObject buildingReference, GameObject node)
    {
        if (OnBuildingBuilt != null)
            OnBuildingBuilt(buildingReference, node);
    }

    private void BroadcastOnSendBuildingBuiltPriceEvent(float buildingPrice)
    {
        if (OnSendBuildingBuiltPrice != null)
            OnSendBuildingBuiltPrice(buildingPrice);
    }

    private void BroadcastOnCurrentBuildingToBuildResetEvent(BuildingDataScriptableObject buildingToBuildToReset)
    {
        if (OnCurrentBuildingToBuildReset != null)
            OnCurrentBuildingToBuildReset(buildingToBuildToReset);
    }

    private void BroadcastOnShowUpgradeUIEvent(GameObject buildingReference, Vector3 buildingPosition)
    {
        if (OnShowCurrentBuildingUI != null)
            OnShowCurrentBuildingUI(buildingReference, buildingPosition);
    }

    private void BroadcastOnHideUpgradeUIEvent()
    {
        if (OnHideCurrentBuildingUI != null)
            OnHideCurrentBuildingUI();
    }

    private void BroadcastOnShowSelectedTurretVisualFeedbackEvent(GameObject selectedTurret)
    {
        if (OnShowSelectedTurretVisualFeedback != null)
            OnShowSelectedTurretVisualFeedback(selectedTurret);
    }

    private void BroadcastOnBuildingSoldEvent(GameObject buildingReference)
    {
        if (OnBuildingSold != null)
            OnBuildingSold(buildingReference);
    }
    #endregion


}
