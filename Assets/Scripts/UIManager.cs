using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    private Text _scoreText;
    [SerializeField]
    private Sprite[] _liveSprites;
    [SerializeField]
    private Sprite[] _missileSprites;
    [SerializeField]
    private Sprite[] _ammoSprites;
    [SerializeField]
    private Image _livesDisplay;
    [SerializeField]
    private Image _missileDisplay;
    [SerializeField]
    private Image _ammoDisplay;
    [SerializeField]
    private Text _gameOverText;
    [SerializeField]
    private Text _restartText;
    [SerializeField]
    private Slider _fuelSlider;
    [SerializeField]
    private Image _fuelWarning;
    [SerializeField]
    private Image _ammoWarning;
    [SerializeField]
    private Text _currentWaveText;
    [SerializeField]
    private Slider _bossHPSlider;
    [SerializeField]
    private GameObject _bossDisplay;

    private Color _fuelColor;
    private Color _ammoColor;

    // Start is called before the first frame update
    void Start()
    {
        _gameOverText.gameObject.SetActive(false);
        _restartText.gameObject.SetActive(false);
        _currentWaveText.gameObject.SetActive(false);
        _livesDisplay.sprite = _liveSprites[3];
        _ammoDisplay.sprite = _ammoSprites[15];
        _scoreText.text = "Score: " + 0;
        _missileDisplay.sprite = _missileSprites[0];
        _fuelSlider.value = 100;
        _fuelColor = _fuelWarning.color;
        _ammoColor = _ammoWarning.color;
    }

    public void UpdateScore(int scoreToAdd)
    {
        _scoreText.text = "Score: " + scoreToAdd;
    }

    public void UpdateLives(int currentLives)
    {
        _livesDisplay.sprite = _liveSprites[currentLives];

        if (currentLives == 0)
        {
            GameOverSequence();
        }
    }

    public void UpdateAmmo(int currentAmmo)
    {
        _ammoDisplay.sprite = _ammoSprites[currentAmmo];

        if (currentAmmo >= 15)
        {
            StopCoroutine("LaserHeatSequence");
            _ammoColor.r = 0f;
            _ammoWarning.color = _ammoColor;
        }
    }

    public void UpdateFuel(float currentFuel)
    {
        _fuelSlider.value = currentFuel;

        if (currentFuel >= 100)
        {
            StopCoroutine("ThrusterHeatSequence");
            _fuelColor.r = 0f;
            _fuelWarning.color = _fuelColor;
        }
    }

    public void UpdateMissiles(int currentMissiles)
    {
        _missileDisplay.sprite = _missileSprites[currentMissiles];
    }

    public void UpdateBossHealth(float healthPercent)
    {
        _bossHPSlider.value = healthPercent * 100;
    }

    void GameOverSequence()
    {
        _gameOverText.gameObject.SetActive(true);
        _restartText.gameObject.SetActive(true);
        StartCoroutine(GameOverRoutine());
    }

    IEnumerator GameOverRoutine()
    {
        while (true)
        {
            if (_gameOverText.gameObject.activeInHierarchy == true)
            {
                _gameOverText.gameObject.SetActive(false);
            }
            else
            {
                _gameOverText.gameObject.SetActive(true);
            }
            yield return new WaitForSeconds(0.5f);
        }
    }

    public void ThrusterOverheat()
    {
        StartCoroutine("ThrusterHeatSequence");
    }

    public void LaserOverheat()
    {
        StartCoroutine("LaserHeatSequence");
    }

    public IEnumerator AnnounceWave(int currentWave)
    {
        _currentWaveText.text = "WAVE " + currentWave;

        for (int i = 0; i < 3; i++)
        {
            _currentWaveText.gameObject.SetActive(true);
            yield return new WaitForSeconds(0.5f);
            _currentWaveText.gameObject.SetActive(false);
            yield return new WaitForSeconds(0.2f);
        }
    }

    public IEnumerator Intermission()
    {
        _currentWaveText.text = "WAVE CLEARED!";
        
        for (int i = 0; i < 3; i++)
        {
            _currentWaveText.gameObject.SetActive(true);
            yield return new WaitForSeconds(0.5f);
            _currentWaveText.gameObject.SetActive(false);
            yield return new WaitForSeconds(0.2f);
        }
    }

    IEnumerator ThrusterHeatSequence()
    {
        while (true)
        {
            if (_fuelWarning.color.r != 1.0f)
            {
                _fuelColor.r = 1.0f;
            }
            else
            {
                _fuelColor.r = 0f; 
            }
            _fuelWarning.color = _fuelColor;
            yield return new WaitForSeconds(0.2f);
        }
    }

    IEnumerator LaserHeatSequence()
    {
        while (true)
        {
            if (_ammoWarning.color.r != 1.0f)
            {
                _ammoColor.r = 1.0f;
            }
            else
            {
                _ammoColor.r = 0f;
            }
            _ammoWarning.color = _ammoColor;
            yield return new WaitForSeconds(0.2f);
        }
    }

    public IEnumerator BossSpawn()
    {
        yield return new WaitForSeconds(2.5f);
        _bossDisplay.SetActive(true);

        while (_bossHPSlider.value < _bossHPSlider.maxValue)
        {
            _bossHPSlider.value++;
            yield return new WaitForSeconds(0.01f);
        }

    }
}
