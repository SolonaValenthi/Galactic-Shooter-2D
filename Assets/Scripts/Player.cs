using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.XR;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    private float _speed;
    [SerializeField]
    private float _fireRate = 0.15f;
    [SerializeField]
    private float _bombRate = 0.6f;
    [SerializeField]
    private float _missileRate = 0.8f;
    [SerializeField]
    private GameObject _laserPrefab;
    [SerializeField]
    private GameObject _bombPrefab;
    [SerializeField]
    private GameObject _tripleShotPrefab;
    [SerializeField]
    private GameObject _playerShield;
    [SerializeField]
    private GameObject _rightDmg;
    [SerializeField]
    private GameObject _leftDmg;
    [SerializeField]
    private GameObject _explosion;
    [SerializeField]
    private GameObject _debrisPrefab;
    [SerializeField]
    private GameObject _thruster;
    [SerializeField]
    private GameObject _missilePrefab;
    [SerializeField]
    private AudioClip _laserClip;
    [SerializeField]
    private Vector3 _laserOffset;
    [SerializeField]
    private Vector3 _bombOffset;

    private int _lives = 3;
    private int _shieldStrength;
    private int _score = 0;
    private int _ammoCount = 15;
    private int _missileCount = 0;
    private float _fuel = 100;
    private float _canFire = -1f;
    private float _canMissile = -1f;
    private float _speedMulti;
    private float _thrustScale;
    private float _blueValue;
    private float _greenValue;
    private bool _tripleShotActive = false;
    private bool _infinAmmoActive = false;
    private bool _laserOverheat = false;
    private bool _bombsReady = false;
    private GameObject _projectileContainer;
    private SpawnManager _spawnManager;
    private UIManager _uiManager;
    private GameManager _gameManager;
    private CameraShake _cameraShake;

    public bool thrusterOverheat { get; private set; } = false;

    AudioSource _playerAudio;
    SpriteRenderer _shieldRenderer;

    // Start is called before the first frame update
    void Start()
    {
        transform.position = new Vector3(0, 0, 0);
        _thrustScale = 0.1f;
        _speedMulti = 1.0f;
        _projectileContainer = GameObject.Find("Player_Projectiles");
        _spawnManager = GameObject.Find("Spawn_Manager").GetComponent<SpawnManager>();
        _uiManager = GameObject.Find("UI_Manager").GetComponent<UIManager>();
        _gameManager = GameObject.Find("Game_Manager").GetComponent<GameManager>();
        _cameraShake = GameObject.Find("Main Camera").GetComponent<CameraShake>();
        _shieldRenderer = _playerShield.GetComponent<SpriteRenderer>();
        _playerAudio = gameObject.GetComponent<AudioSource>();

        if (_projectileContainer == null)
        {
            Debug.LogError("Player projectile container reference is NULL!");
        }
        if (_spawnManager == null)
        {
            Debug.LogError("Player spawn manager reference is NULL!");
        }
        if (_uiManager == null)
        {
            Debug.LogError("Player UI manager reference is NULL!");
        }
        if (_gameManager == null)
        {
            Debug.LogError("Player game manager reference is NULL!");
        }
        if (_cameraShake == null)
        {
            Debug.LogError("Player camera shake reference is NULL!");
        }
        if (_playerAudio == null)
        {
            Debug.LogError("Player audio source reference is NULL!");
        }
        if (_shieldRenderer == null)
        {
            Debug.LogError("Player shield renderer reference is NULL!");
        }
    }

    // Update is called once per frame
    void Update()
    {
        CalculateMovement();
        ThrusterControl();

        if (Input.GetKeyDown(KeyCode.Space) && Time.time > _canFire)
        {
            if (_bombsReady == true)
            {
                FireBomb();
            }
            else
            {
                FireLaser();
            }
        }

        if (Input.GetKeyDown(KeyCode.Q) && Time.time > _canMissile)
        {
            if (_missileCount > 0)
            {
                FireMissile();
            }
        }

        if (_fuel >= 100)
        {
            thrusterOverheat = false;
        }

        if (_ammoCount >= 15)
        {
            _laserOverheat = false;
        }
    }

    void CalculateMovement()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        Vector3 direction = new Vector3(horizontalInput, verticalInput, 0);

        transform.Translate(direction * _speed * _speedMulti * Time.deltaTime);

        if (transform.position.y >= 0)
        {
            transform.position = new Vector3(transform.position.x, 0, 0);
        }
        else if (transform.position.y <= -4.8f)
        {
            transform.position = new Vector3(transform.position.x, -4.8f, 0);
        }

        // horizontal wrap
        if (transform.position.x > 11.3f)
        {
            transform.position = new Vector3(-11.3f, transform.position.y, 0);
        }
        else if (transform.position.x < -11.3f)
        {
            transform.position = new Vector3(11.3f, transform.position.y, 0);
        }
    }

    void ThrusterControl()
    {
        _thrustScale = Mathf.Clamp(_thrustScale, 0.1f, 1.0f);
        _speedMulti = Mathf.Clamp(_speedMulti, 1.0f, 2.0f);
        _fuel = Mathf.Clamp(_fuel, 0.0f, 100.0f);

        if (_gameManager.isPaused == false)
        {
            if (Input.GetKey(KeyCode.LeftShift) && thrusterOverheat == false)
            {
                _thrustScale += 0.02f;
                _speedMulti += 0.02f;
                _fuel -= 0.5f;
                DrainFuel();
            }
            else
            {
                _thrustScale -= 0.02f;
                _speedMulti -= 0.02f;

                if (thrusterOverheat == true)
                {
                    _fuel += 0.05f;                
                }
                else
                {
                    _fuel += 0.1f;
                }

                _uiManager.UpdateFuel(_fuel);
            }
        }

        _thruster.transform.localScale = new Vector3(_thrustScale, 1, 1);
    }

    void FireLaser()
    {
        _canFire = Time.time + _fireRate;
        GameObject newLaser;

        if (_gameManager.isPaused == false && _ammoCount > 0 && _laserOverheat == false)
        {
            if (_tripleShotActive == true)
            {
                newLaser = Instantiate(_tripleShotPrefab, transform.position, Quaternion.identity);
            }
            else
            {
                newLaser = Instantiate(_laserPrefab, transform.position + _laserOffset, Quaternion.identity);
            }

            newLaser.transform.parent = _projectileContainer.transform;
            _playerAudio.PlayOneShot(_laserClip);

            if (_infinAmmoActive == false)
            {
                StopCoroutine("ReplenishAmmo");
                UpdateAmmo();
            }
        }
    }

    private void FireBomb()
    {
        _canFire = Time.time + _bombRate;

        if (_gameManager.isPaused == false)
        {
            Instantiate(_bombPrefab, transform.position + _bombOffset, Quaternion.identity);
        }
    }

    private void FireMissile()
    {
        _canMissile = Time.time + _missileRate;

        if (_gameManager.isPaused == false)
        {
            Instantiate(_missilePrefab, transform.position, Quaternion.identity);
            _missileCount--;
            _uiManager.UpdateMissiles(_missileCount);
        }
    }

    private void UpdateAmmo()
    {
        _ammoCount--;
        _ammoCount = Mathf.Clamp(_ammoCount, 0, 15);

        if (_ammoCount <= 0)
        {
            _laserOverheat = true;
            _uiManager.LaserOverheat();
        }

        _uiManager.UpdateAmmo(_ammoCount);
        StartCoroutine("ReplenishAmmo");
    }

    IEnumerator ReplenishAmmo()
    {
        yield return new WaitForSeconds(2.0f);
        while (_ammoCount < 15)
        {
            yield return new WaitForSeconds(0.2f);
            _ammoCount++;
            _uiManager.UpdateAmmo(_ammoCount);
        }
    }

    public void Damage(int damageDealt)
    {
        for (int i = 0; i < damageDealt; i++)
        {
            if (_shieldStrength > 0)
            {
                DamageShields();
                return;
            }

            _lives--;
            _uiManager.UpdateLives(_lives);
            DamageEngine();
            StartCoroutine(_cameraShake.ShakeCamera(0.2f, 0.2f));

            if (_lives < 1)
            {
                _spawnManager.OnPlayerDeath();
                _gameManager.GameOver();
                Instantiate(_explosion, transform.position, Quaternion.identity);
                Instantiate(_debrisPrefab, transform.position, Quaternion.identity);
                Destroy(this.gameObject);
            }
        }
    }

    public void DamageEngine()
    {
        // randomly select which engine is damaged first
        int _engineSelection = Random.Range(0, 2);

        if (_engineSelection == 0)
        {
            if (_rightDmg.activeInHierarchy == false)
            {
                _rightDmg.SetActive(true);
            }
            else
            {
                _leftDmg.SetActive(true);
            }
        }
        else if (_engineSelection == 1)
        {
            if (_leftDmg.activeInHierarchy == false)
            {
                _leftDmg.SetActive(true);
            }
            else
            {
                _rightDmg.SetActive(true);
            }
        }
    }

    public void ActivateTripleShot()
    {
        _tripleShotActive = true;
        StopCoroutine("DeactivateTripleShot");
        StartCoroutine("DeactivateTripleShot");
    }

    IEnumerator DeactivateTripleShot()
    {
        yield return new WaitForSeconds(5.0f);
        _tripleShotActive = false;
    }

    public void ReplenishFuel()
    {
        thrusterOverheat = false;
        _fuel = 100;
        _uiManager.UpdateFuel(_fuel);
    }

    public void ActivateShield()
    {
        _playerShield.SetActive(true);
        _shieldStrength = 3;
        _greenValue = 1;
        _blueValue = 1;
        _shieldRenderer.color = new Color(1, _greenValue, _blueValue, 1);
    }

    public void ActivateInfinAmmo()
    {
        _infinAmmoActive = true;
        _laserOverheat = false;
        _canFire = Time.time;
        _ammoCount = 15;
        StopCoroutine("ReplenishAmmo");
        _uiManager.UpdateAmmo(_ammoCount);
        StopCoroutine("DeactivateInfinAmmo");
        StartCoroutine("DeactivateInfinAmmo");
    }

    IEnumerator DeactivateInfinAmmo()
    {
        yield return new WaitForSeconds(5.0f);
        _infinAmmoActive = false;
    }

    public void HealPlayer()
    {
        if (_lives < 3)
        {
            _lives++;
            _uiManager.UpdateLives(_lives);

            // randomly select which engine is repaired first
            int _engineSelection = Random.Range(0, 2);

            if (_engineSelection == 0)
            {
                if (_rightDmg.activeInHierarchy == true)
                {
                    _rightDmg.SetActive(false);
                }
                else
                {
                    _leftDmg.SetActive(false);
                }
            }
            else if (_engineSelection == 1)
            {
                if (_leftDmg.activeInHierarchy == true)
                {
                    _leftDmg.SetActive(false);
                }
                else
                {
                    _rightDmg.SetActive(false);
                }
            }
        }
    }

    public void BombsReady()
    {
        _bombsReady = true;
        StopCoroutine("DeactivateBombs");
        StartCoroutine("DeactivateBombs");
    }

    IEnumerator DeactivateBombs()
    {
        yield return new WaitForSeconds(5.0f);
        _bombsReady = false;
    }

    public void LoadMissiles(int missilesToLoad)
    {
        for (int i = 0; i < missilesToLoad; i++)
        {
            if (_missileCount < 5)
            {
                _missileCount++;
            }
        }

        _uiManager.UpdateMissiles(_missileCount);
    }

    public void Jamming()
    {
        _shieldStrength = 0;
        _playerShield.SetActive(false);
        _ammoCount = 0;
        _infinAmmoActive = false;
        _bombsReady = false;
        _laserOverheat = true;
        StopCoroutine("ReplenishAmmo");
        UpdateAmmo();
        _fuel = 0.0f;
        thrusterOverheat = true;
        _uiManager.UpdateFuel(_fuel);
        _uiManager.ThrusterOverheat();
    }

    public void DrainFuel()
    {
        _fuel -= 0.5f;
        _uiManager.UpdateFuel(_fuel);
        if (_fuel <= 0.0f)
        {
            thrusterOverheat = true;
            _uiManager.ThrusterOverheat();
        }
    }

    private void DamageShields()
    {
        _shieldStrength--;

        switch (_shieldStrength)
        {
            case 0:
                _playerShield.SetActive(false);
                break;
            case 1:
                _greenValue = 0;
                break;
            case 2:
                _blueValue = 0;
                _greenValue = 0.5f;
                break;
            default:
                Debug.LogError("Invalid Shield Strength Detected.");
                break;
        }

        _shieldRenderer.color = new Color(1, _greenValue, _blueValue, 1);
    }

    public void AddScore(int points)
    {
        _score += points;
        _uiManager.UpdateScore(_score);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("EnemyLaser"))
        {
            if (other.name != "Piercing_Laser(Clone)")
            {
                Destroy(other.gameObject);
                Damage(1);
            }
            else
            {
                Damage(2);
            }
        }

        if (other.CompareTag("EnemyMissile"))
        {
            Damage(1);
            Destroy(other.gameObject);
        }
    }
}
