using System;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private static int levelNumber = 1;
    private static int totalScore = 0;

    public static void ResetStaticData()
    {
        levelNumber = 1;
        totalScore = 0;
    }

    public event EventHandler OnGamePaused;
    public event EventHandler OnGameUnPause;

    [SerializeField] private List<GameLevel> gameLevelList;
    [SerializeField] private CinemachineCamera cinemachineCamera;

    private int score;
    private float time;
    private bool isTimerActive;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        Lander.Instance.OnCoinPickup += Lander_OnCoinPickup;
        Lander.Instance.OnLanded += Lander_OnLanded;
        Lander.Instance.OnStateChange += Lander_OnStateChange;

        GameInput.Instance.OnMenuButtonPressed += GameInput_OnMenuButtonPressed;
        LoadCurrentLevel();
    }

    private void GameInput_OnMenuButtonPressed(object sender, EventArgs e)
    {
        PauseUnPauseGame();
    }

    private void Update()
    {
        if (isTimerActive)
        {
            time += Time.deltaTime;
        }
    }

    private void LoadCurrentLevel()
    {
        GameLevel gameLevel = GetGameLevel();
        GameLevel spawnGameLevel = Instantiate(gameLevel, Vector3.zero, Quaternion.identity);
        Lander.Instance.transform.position = spawnGameLevel.GetLanderStartPosition();
        cinemachineCamera.Target.TrackingTarget = spawnGameLevel.GetCameraTransformTarget();
        CinemachineZoom2D.Instance.SetTargetOrthographicSize(spawnGameLevel.GetZoomedOutOrthigraphicSize());
    }

    private GameLevel GetGameLevel()
    {
        foreach (GameLevel level in gameLevelList)
        {
            if (level.GetLevelNumber() == levelNumber)
            {
                return level;
            }
        }
        return null;
    }

    private void Lander_OnLanded(object sender, Lander.OnLandedEventArgs e)
    {
        AddScore(e.score);
    }

    private void Lander_OnCoinPickup(object sender, EventArgs e)
    {
        AddScore(50);
    }
    private void Lander_OnStateChange(object sender, Lander.OnStateChangeEventArgs e)
    {
        isTimerActive = e.state == Lander.State.Normal;

        if (e.state == Lander.State.Normal)
        {
            cinemachineCamera.Target.TrackingTarget = Lander.Instance.transform;
            CinemachineZoom2D.Instance.SetNormalOrthographicSize();
        }
    }

    public void AddScore(int addScoreAmount)
    {
        score += addScoreAmount;
        Debug.Log(score);
    }

    public int GetScore()
    {
        return score;
    }

    public float GetTime()
    {
        return time;
    }

    public int GetLevelNumber()
    {
        return levelNumber;
    }

    public int GetTotalScore()
    {
        return totalScore;
    }

    public void GoToNextLevel()
    {
        levelNumber++;
        totalScore += score;
        
        if(GetGameLevel() == null)
        {
            SceneLoader.LoadScene(SceneLoader.Scene.GameOverScene);
        } 
        else
        {
            SceneLoader.LoadScene(SceneLoader.Scene.GameScene);
        }
    }

    public void RetryThisLevel()
    {
        SceneLoader.LoadScene(SceneLoader.Scene.GameScene);
    }

    public void PauseUnPauseGame()
    {
        if(Time.timeScale == 1f)
        {
            PauseGame();
        }
        else
        {
            UnPauseGame();
        }
    }

    public void PauseGame()
    {
        Time.timeScale = 0f;
        OnGamePaused?.Invoke(this, EventArgs.Empty);
    }

    public void UnPauseGame()
    {
        Time.timeScale = 1f;
        OnGameUnPause?.Invoke(this, EventArgs.Empty);
    }
}
