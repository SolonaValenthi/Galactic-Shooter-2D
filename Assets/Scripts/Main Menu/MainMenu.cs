using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField]
    private GameObject _titleDisplay;
    [SerializeField]
    private GameObject _controlDisplay;

    public void LoadGame()
    {
        SceneManager.LoadScene(1); //load the game scene
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void DisplayControls()
    {
        _titleDisplay.SetActive(false);
        _controlDisplay.SetActive(true);
    }

    public void ReturnToTitle()
    {
        _titleDisplay.SetActive(true);
        _controlDisplay.SetActive(false);
    }
}
