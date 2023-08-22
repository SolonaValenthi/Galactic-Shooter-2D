using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    private float _speed = 5.0f;
    [SerializeField]
    private GameObject _laserPrefab;
    [SerializeField]
    private float _fireRate = 0.15f;
    [SerializeField]
    private int _lives = 3;
    [SerializeField]
    private GameObject _tripleShotPrefab;
    [SerializeField]
    private bool _tripleShotActive = false;
    [SerializeField]
    private bool _shieldsActive = false;
    [SerializeField]
    private GameObject _playerShield;
    [SerializeField]
    private int _score = 0;
    [SerializeField]
    private GameObject _rightDmg;
    [SerializeField]
    private GameObject _leftDmg;

    private float _offset = 1.05f;
    private float _canFire = -1f;
    private SpawnManager _spawnManager;
    private UIManager _uiManager;
    private GameManager _gameManager;
    private Vector3 _laserOffset;
    // Start is called before the first frame update
    void Start()
    {
        transform.position = new Vector3(0, 0, 0);
        _spawnManager = GameObject.Find("Spawn_Manager").GetComponent<SpawnManager>();
        _uiManager = GameObject.Find("UI_Manager").GetComponent<UIManager>();
        _gameManager = GameObject.Find("Game_Manager").GetComponent<GameManager>();
        _laserOffset = new Vector3(0, _offset, 0);
    }

    // Update is called once per frame
    void Update()
    {
        CalculateMovement();

        if (Input.GetKeyDown(KeyCode.Space) && Time.time > _canFire)
        {
            FireLaser();
        }
    }

    void CalculateMovement()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        Vector3 direction = new Vector3(horizontalInput, verticalInput, 0);

        transform.Translate(direction * _speed * Time.deltaTime);

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

    void FireLaser()
    {
        _canFire = Time.time + _fireRate;

        if (_tripleShotActive == true)
        {
            Instantiate(_tripleShotPrefab, transform.position, Quaternion.identity);
        }
        else
        {
            Instantiate(_laserPrefab, transform.position + _laserOffset, Quaternion.identity);
        }     
    }

    public void Damage()
    {
        if (_shieldsActive == true)
        {
            _shieldsActive = false;
            _playerShield.SetActive(false);
            return;
        }

        _lives--;
        _uiManager.UpdateLives(_lives);
        DamageEngine();

        if ( _lives < 1)
        {
            _spawnManager.OnPlayerDeath();
            _gameManager.GameOver();
            Destroy(this.gameObject);
        }
    }

    public void DamageEngine()
    {
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
        StartCoroutine(DeactivateTripleShot());
    }

    IEnumerator DeactivateTripleShot()
    {
        yield return new WaitForSeconds(5.0f);
        _tripleShotActive = false;
    }

    public void ActivateSpeedBoost()
    {
        _speed = 8.5f;
        StartCoroutine(DeactivateSpeedBoost());
    }

    IEnumerator DeactivateSpeedBoost()
    {
        yield return new WaitForSeconds(5.0f);
        _speed = 5.0f;
    }

    public void ActivateShield()
    {
        _playerShield.SetActive(true);
        _shieldsActive = true;
    }

    public void AddScore(int points)
    {
        _score += 10;
        _uiManager.UpdateScore(_score);
    }
}
