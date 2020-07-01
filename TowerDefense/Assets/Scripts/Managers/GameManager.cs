using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static Action OnGameOver;
    public static Action OnLevelCleared;
    public static Action OnGameStart;
    public static Action OnTitleScreen;
    public static Action OnPauseGame;
    public static Action OnResumeGame;
    public static Action<string> OnGeneralMessagePrint;

    public enum GameState
    {
        GetReady,
        InGame,
        GameOver,
        Pause
    }
    public static GameManager instance;
    private GameState m_currentGameState;
    public int CurrentLevel { get => SceneManager.GetActiveScene().buildIndex ; }
    public GameState CurrentGameState { get => m_currentGameState; set => m_currentGameState = value; }

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);
    }

    private void OnEnable()
    {
        PlayerManager.OnPlayersDeath += OnPlayersDeathEvent;
        EnemySpawnManager.OnLevelCleared += OnLevelClearedEvent;
    }

    private void OnDisable()
    {
        PlayerManager.OnPlayersDeath -= OnPlayersDeathEvent;
        EnemySpawnManager.OnLevelCleared -= OnLevelClearedEvent;
    }

    private void Start()
    {
        m_currentGameState = GameState.GetReady;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.P))
        {
            if (m_currentGameState == GameState.InGame)
                PauseGame();

            else if (m_currentGameState == GameState.Pause)
                ResumeGame();
        }

    }

    private void OnLevelClearedEvent()
    {
        BroadcastOnLevelClearedEvent();
    }

    private void OnPlayersDeathEvent()
    {
        m_currentGameState = GameState.GameOver;
        BroadcastOnGameOverEvent();
    }


    public void StartGame()
    {
        m_currentGameState = GameState.InGame;
        BroadcastOnGameStartEvent();
    }

    public void ToMainMenu()
    {
        Time.timeScale = 1f;
        BroadcastOnTitleScreenEvent();
        SceneManager.LoadScene(0);
    }

    private void PauseGame()
    {
        BroadcastOnPauseGameEvent();
        m_currentGameState = GameState.Pause;
        Time.timeScale = 0f;
    }


    public void ResumeGame()
    {
        Time.timeScale = 1f;
        BroadcastOnResumeEvent();
        m_currentGameState = GameState.InGame;
    }

    public void RetryLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void GetToNextLevel()
    {
        Time.timeScale = 1f;
        if (SceneManager.GetActiveScene().buildIndex + 1 >= Tags.FIRST_LEVEL_INDEX && SceneManager.GetActiveScene().buildIndex + 1 <= Tags.LAST_LEVEL_INDEX)
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void PrintGeneralMessage(string message)
    {
        BroadcastOnGeneralMessagePrintEvent(message);
    }

    #region BROADCASTERS
    private void BroadcastOnGameOverEvent()
    {
        if (OnGameOver != null)
            OnGameOver();
    }

    private void BroadcastOnGameStartEvent()
    {
        if (OnGameStart != null)
            OnGameStart();
    }

    private void BroadcastOnTitleScreenEvent()
    {
        if (OnTitleScreen != null)
            OnTitleScreen();
    }

    private void BroadcastOnGeneralMessagePrintEvent(string message)
    {
        if (OnGeneralMessagePrint != null)
            OnGeneralMessagePrint(message);
    }

    private void BroadcastOnPauseGameEvent()
    {
        if (OnPauseGame != null)
            OnPauseGame();
    }

    private void BroadcastOnResumeEvent()
    {
        if (OnResumeGame != null)
            OnResumeGame();
    }

    private void BroadcastOnLevelClearedEvent()
    {
        if (OnLevelCleared != null)
            OnLevelCleared();
    }
    #endregion
}
