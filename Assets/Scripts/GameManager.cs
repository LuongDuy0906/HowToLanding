using System;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get ; private set; }

    private static int levelNumber = 1;

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

        LoadCurrentLevel();
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
        foreach (GameLevel level in gameLevelList)
        {
            if(level.GetLevelNumber() == levelNumber)
            {
                GameLevel spawnGameLevel = Instantiate(level, Vector3.zero, Quaternion.identity);
                Lander.Instance.transform.position = spawnGameLevel.GetLanderStartPosition();
                cinemachineCamera.Target.TrackingTarget = spawnGameLevel.GetCameraTransformTarget();
                CinemachineZoom2D.Instance.SetTargetOrthographicSize(spawnGameLevel.GetZoomedOutOrthigraphicSize());
            }
        }
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

        if(e.state == Lander.State.Normal)
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

    public void GoToNextLevel()
    {
        levelNumber++;
        SceneManager.LoadScene(0);
    }

    public void RetryThisLevel()
    {
        SceneManager.LoadScene(0);
    }
}
