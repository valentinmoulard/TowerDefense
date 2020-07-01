using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static Action<GameObject> OnShowCurrentBuildingUpgradeButtons;

    [Header("General UI =====================================")]
    [SerializeField]
    private GameObject m_gameOverUI = null;

    [SerializeField]
    private GameObject m_levelClearedUI = null;

    [SerializeField]
    private GameObject m_readyScreenUI = null;

    [SerializeField]
    private GameObject m_inGameUI = null;

    [SerializeField]
    private GameObject m_pauseUI = null;


    [Header("Buildings UI ====================================")]
    [SerializeField]
    private GameObject m_upgradeBuildingUI = null;

    [SerializeField]
    private GameObject m_laserBeamerModeUI = null;

    [Header("Other UI ========================================")]
    [SerializeField]
    private GameObject m_generalMessageGameObject = null;

    [SerializeField]
    private TMP_Text m_generalMessageTMP = null;

    [SerializeField]
    private Animator m_generaMessageAnimatorReference = null;

    private Coroutine m_showGeneralMessageCoroutine;
    private float m_generalMessageDisplayTime;

    private void OnEnable()
    {
        GameManager.OnGameOver += ShowGameoverUI;
        GameManager.OnGameStart += ShowInGameUI;
        GameManager.OnLevelCleared += ShowLevelClearedUI;

        BuildingManager.OnShowCurrentBuildingUI += ShowUpgradeUI;
        LaserBeamer.OnShowLaserBeamerModeUI += ShowLaserBeamerModeUI;
        BuildingManager.OnHideCurrentBuildingUI += HideUpgradeUI;
        BuildingManager.OnHideCurrentBuildingUI += HideLaserBeamerModeUI;

        GameManager.OnGeneralMessagePrint += PrintGeneralMessage;
        GameManager.OnPauseGame += ShowPauseMenu;
        GameManager.OnResumeGame += HidePauseMenu;
    }

    private void OnDisable()
    {
        GameManager.OnGameOver -= ShowGameoverUI;
        GameManager.OnGameStart -= ShowInGameUI;
        GameManager.OnLevelCleared -= ShowLevelClearedUI;

        BuildingManager.OnShowCurrentBuildingUI -= ShowUpgradeUI;
        LaserBeamer.OnShowLaserBeamerModeUI -= ShowLaserBeamerModeUI;
        BuildingManager.OnHideCurrentBuildingUI -= HideUpgradeUI;
        BuildingManager.OnHideCurrentBuildingUI -= HideLaserBeamerModeUI;

        GameManager.OnGeneralMessagePrint -= PrintGeneralMessage;
        GameManager.OnPauseGame -= ShowPauseMenu;
        GameManager.OnResumeGame -= HidePauseMenu;
    }

    private void Start()
    {
        if (m_gameOverUI == null || m_inGameUI == null ||
            m_readyScreenUI == null || m_upgradeBuildingUI == null ||
            m_laserBeamerModeUI == null || m_levelClearedUI == null ||
            m_generalMessageTMP == null || m_generalMessageGameObject == null ||
            m_generaMessageAnimatorReference == null)
        {
            Debug.LogError("One or more references are missing in the inspector!");
        }
        InitializeUI();
    }

    private void InitializeUI()
    {
        ShowReadyScreenUI();
        m_generalMessageDisplayTime = m_generaMessageAnimatorReference.runtimeAnimatorController.animationClips[0].length;
    }

    #region PAUSE MENU UI
    private void ShowPauseMenu()
    {
        m_pauseUI.SetActive(true);
    }

    private void HidePauseMenu()
    {
        m_pauseUI.SetActive(false);
    }
    #endregion

    #region BUILDING UPGRADES UI
    private void ShowUpgradeUI(GameObject buildingReference, Vector3 UIPosition)
    {
        m_upgradeBuildingUI.transform.position = UIPosition;
        m_upgradeBuildingUI.SetActive(true);

        //sent so the building components activates the adapted buttons in upgrade panel
        BroadcastOnShowCorretBuildingUpgradeButtonsEvent(buildingReference);
    }

    private void HideUpgradeUI()
    {
        m_upgradeBuildingUI.SetActive(false);
    }
    #endregion

    #region LASER BEAMER MODE UI
    private void ShowLaserBeamerModeUI(GameObject buildingReference)
    {
        m_laserBeamerModeUI.SetActive(true);
        m_laserBeamerModeUI.transform.position = buildingReference.transform.position;
    }

    private void HideLaserBeamerModeUI()
    {
        m_laserBeamerModeUI.SetActive(false);
    }
    #endregion

    #region GAME OVER UI
    private void ShowGameoverUI()
    {
        m_gameOverUI.SetActive(true);
        HideReadyScreenUI();
        HideInGameUI();
        HidePauseMenu();
        HideLevelClearedUI();
        HideUpgradeUI();
        HideLaserBeamerModeUI();
        HideGeneralMessage();
    }

    private void HideGameOverUI()
    {
        m_gameOverUI.SetActive(false);
    }
    #endregion

    #region LEVEL CLEARED UI
    private void ShowLevelClearedUI()
    {
        m_levelClearedUI.SetActive(true);
        HideReadyScreenUI();
        HideGameOverUI();
        HideInGameUI();
        HidePauseMenu();
        HideUpgradeUI();
        HideLaserBeamerModeUI();
        HideGeneralMessage();
    }

    private void HideLevelClearedUI()
    {
        m_levelClearedUI.SetActive(false);
    }
    #endregion

    #region IN GAME UI
    private void ShowInGameUI()
    {
        m_inGameUI.SetActive(true);
        HideReadyScreenUI();
        HideGameOverUI();
        HidePauseMenu();
        HideLevelClearedUI();
        HideUpgradeUI();
        HideLaserBeamerModeUI();
        HideGeneralMessage();
    }

    private void HideInGameUI()
    {
        m_inGameUI.SetActive(false);
    }
    #endregion

    #region READY SCREEN UI
    private void ShowReadyScreenUI()
    {
        m_readyScreenUI.SetActive(true);
        HideInGameUI();
        HideGameOverUI();
        HidePauseMenu();
        HideLevelClearedUI();
        HideUpgradeUI();
        HideLaserBeamerModeUI();
        HideGeneralMessage();
    }

    private void HideReadyScreenUI()
    {
        m_readyScreenUI.SetActive(false);
    }
    #endregion

    #region GENERAL MESSAGE
    public void PrintGeneralMessage(string UIManagerMessage)
    {
        m_generalMessageGameObject.SetActive(true);
        m_generalMessageTMP.text = UIManagerMessage;

        if (m_showGeneralMessageCoroutine == null)
            m_showGeneralMessageCoroutine = StartCoroutine(ShowGeneralMessageCoroutine());
    }

    private void HideGeneralMessage()
    {
        if (m_showGeneralMessageCoroutine != null)
            StopCoroutine(m_showGeneralMessageCoroutine);

        m_generalMessageTMP.text = "";
        m_generalMessageGameObject.SetActive(false);
    }
    #endregion

    private IEnumerator ShowGeneralMessageCoroutine()
    {
        yield return new WaitForSeconds(m_generalMessageDisplayTime);
        m_generalMessageGameObject.SetActive(false);
        m_showGeneralMessageCoroutine = null;
    }

    private void BroadcastOnShowCorretBuildingUpgradeButtonsEvent(GameObject buildingReference)
    {
        if (OnShowCurrentBuildingUpgradeButtons != null)
            OnShowCurrentBuildingUpgradeButtons(buildingReference);
    }
}
