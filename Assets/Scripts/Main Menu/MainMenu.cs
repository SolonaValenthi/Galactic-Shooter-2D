using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField]
    private GameObject _titleDisplay;
    [SerializeField]
    private GameObject _controlDisplay;
    [SerializeField]
    private GameObject _loadingScreen;
    [SerializeField]
    private Image _loadingSpinner;
    [SerializeField]
    private Text _loadingDots;

    private void Update()
    {
        if (_loadingSpinner.enabled == true)
        {
            _loadingSpinner.rectTransform.Rotate(Vector3.up);
        }
    }

    public void LoadGame()
    {
        _titleDisplay.SetActive(false);
        _loadingScreen.SetActive(true);
        StartCoroutine(LoadMainGameScene()); // begin async scene loading
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

    IEnumerator LoadMainGameScene()
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(1);
        asyncLoad.allowSceneActivation = false;

        while (asyncLoad.isDone == false)
        {
            _loadingDots.text = ". ";
            yield return new WaitForSeconds(0.2f);
            _loadingDots.text = ". . ";
            yield return new WaitForSeconds(0.2f);
            _loadingDots.text = ". . .";
            yield return new WaitForSeconds(0.2f);

            if (asyncLoad.progress >= 0.9f)
            {
                asyncLoad.allowSceneActivation = true;
            }

            yield return null;
        }
    }
}
