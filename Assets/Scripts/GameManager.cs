using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private GameObject _pauseMenu;
    [SerializeField]
    private GameObject _bgm;

    private bool _isGameOver = false;
    private Player _player;
    private SpawnManager _spawnManager;
    private UIManager _uiManager;

    public bool isPaused { get; private set; }
    public bool bossActive { get; private set; } = false;

    private void Start()
    {
        Time.timeScale = 1;
        _pauseMenu.SetActive(false);
        _player = GameObject.Find("Player").GetComponent<Player>();
        _spawnManager = GameObject.Find("Spawn_Manager").GetComponent<SpawnManager>();
        _uiManager = GameObject.Find("UI_Manager").GetComponent<UIManager>();
        isPaused = false;

        if (_spawnManager == null)
        {
            Debug.LogError("Game Manager spawn manager reference is NULL!");
        }
        if (_uiManager == null)
        {
            Debug.LogError("Game Manager UI manager reference is NULL!");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (_isGameOver == true && Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(1); // load game scene
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            PauseGame();
        }
    }

    public void BossFight()
    {
        bossActive = true;
        _player.SetBounds();
    }

    public void OnBossDeath()
    {
        bossActive = false;
        _player.SetBounds();
    }

    public void GameOver()
    {
        _isGameOver = true;
    }

    void PauseGame()
    {
        Time.timeScale = 0;
        _bgm.GetComponent<AudioSource>().volume = 0.25f;
        _pauseMenu.SetActive(true);
        isPaused = true;
    }

    public void ResumeGame()
    {
        Time.timeScale = 1;
        _bgm.GetComponent<AudioSource>().volume = 1;
        _pauseMenu.SetActive(false);
        isPaused = false;
    }

    public void ToMainMenu()
    {
        SceneManager.LoadScene(0); // load main menu scene
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
