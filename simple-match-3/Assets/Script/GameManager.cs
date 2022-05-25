using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance { get; private set; }
    
    public Button pauseButton;
    public Button reloadButton;
    public Button resumeButton;
    public GameObject pauseMenu;

    public Button resetButton;
    public GameObject endGameScreen;

    public Image timeBar;

    private float timeRemain;

    private void Awake()
    {
        instance = this;
        
        timeRemain = 60f;
        
        pauseMenu.SetActive(false);
        endGameScreen.SetActive(false);
        pauseButton.onClick.AddListener(()=>Pause());
        resumeButton.onClick.AddListener(()=>Resume());
        reloadButton.onClick.AddListener(()=>Reload());
        resetButton.onClick.AddListener(()=>Reset());
    }

    private void Update()
    {
        timeRemain -= Time.deltaTime;
        timeBar.fillAmount = timeRemain / 60f;
        
        if (timeBar.fillAmount > 0.66f) timeBar.color = Color.green;
        else if (timeBar.fillAmount > 0.33f) timeBar.color = Color.yellow;
        else timeBar.color = Color.red;

        if (timeRemain < 0)
        {
            Endgame();
        }
    }

    public void IncreaseTimeRemain()
    {
        timeRemain = Mathf.Clamp(timeRemain + 1f, 0f, 60f);
    }

    private void Pause()
    {
        Time.timeScale = 0;
        pauseMenu.SetActive(true);
    }

    private void Resume()
    {
        Time.timeScale = 1;
        pauseMenu.SetActive(false);
    }

    private void Reload()
    {
        Board.instance.GenerateBoard();
        ScoreCounter.instance.Score = (int)(ScoreCounter.instance.Score * 0.75f);
    }

    private void Reset()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void Endgame()
    {
        endGameScreen.SetActive(true);
    }
}
