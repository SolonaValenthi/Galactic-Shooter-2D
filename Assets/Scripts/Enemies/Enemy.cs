using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    private float _enemySpeed = 4.0f;
    [SerializeField]
    private GameObject _laserPrefab;
    [SerializeField]
    private GameObject _enemyShield;
    [SerializeField]
    private AudioClip _laserClip;
    [SerializeField]
    private Vector3 _laserOffset;

    private float _spawnRange;
    private float _canFire;
    private float _powerupAttackCD;
    private float _playerDeviation;
    private bool _isDead = false;
    private bool _flyingIn = true;
    private bool _shieldActive = false;
    private Player _player;
    private AudioManager _audioManager;
    private SpawnManager _spawnManager;
    private GameObject _playerObj;
    private GameObject _projectileContainer;
    private Vector3 _flyInDirection;
    private Vector3 _flyInDestination;

    Animator _deathAnim;
    AudioSource _enemyAudio;
    BoxCollider2D[] _enemyCollider;

    private void Start()
    {
        _playerObj = GameObject.Find("Player");
        _projectileContainer = GameObject.Find("Enemy_Projectiles");
        _player = _playerObj.GetComponent<Player>();
        _deathAnim = gameObject.GetComponent<Animator>();
        _enemyCollider = gameObject.GetComponents<BoxCollider2D>();
        _enemyAudio = gameObject.GetComponent<AudioSource>();
        _audioManager = GameObject.Find("Audio_Manager").GetComponent<AudioManager>();
        _spawnManager = GameObject.Find("Spawn_Manager").GetComponent<SpawnManager>();
        _canFire = Random.Range(3, 6);

        if (_player == null)
        {
            Debug.LogError("Enemy player script reference is NULL!");
        }
        if (_projectileContainer == null)
        {
            Debug.LogError("Enemy projectile container reference is NULL!");
        }
        if (_deathAnim == null)
        {
            Debug.LogError("Enemy animator reference is NULL!");
        }
        if (_enemyCollider == null)
        {
            Debug.LogError("Enemy collider reference is NULL!");
        }
        if (_audioManager == null)
        {
            Debug.LogError("Enemy audio manager reference is NULL!");
        }
        if (_spawnManager == null)
        {
            Debug.LogError("Enemy spawn manager reference is NULL!");
        }
        if (_enemyAudio == null)
        {
            Debug.LogError("Enemy audio source reference is NULL!");
        }
        if (_playerObj == null)
        {
            Debug.LogError("Enemy player object reference is NULL!");
        }

        int shieldGen = Random.Range(0, 3);
        if (shieldGen == 0)
        {
            _shieldActive = true;
            _enemyShield.SetActive(true);
        }

        CalculateFlyIn();
    }

    // Update is called once per frame
    void Update()
    {
        if (_flyingIn == true)
        {
            FlyIn();
        }
        else
        {
            EnemyMovement();
        }

        if (_playerObj != null)
        {
            _playerDeviation = transform.position.y - _playerObj.transform.position.y;
        }      

        if (Time.time > _canFire && _isDead == false && _playerDeviation >= 1.3 && _flyingIn == false)
        {
            EnemyFire();
        }
    }

    private void EnemyMovement()
    {
        transform.Translate(Vector3.down * _enemySpeed * Time.deltaTime);

        if (transform.position.y < -7)
        {
            _spawnRange = Random.Range(-9.0f, 9.0f);
            transform.position = new Vector3(_spawnRange, 8, 0);
            CalculateFlyIn();
            _flyingIn = true;
        } 
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (_player != null)
            {
                _player.Damage();
            }

            if (_shieldActive == true)
            {
                _shieldActive = false;
                _enemyShield.SetActive(false);
                return;
            }

            DeathSequence();
        }

        if (other.CompareTag("Laser"))
        {
            Destroy(other.gameObject);

            if (_shieldActive == true)
            {
                _shieldActive = false;
                _enemyShield.SetActive(false);
                return;
            }

            if (_player != null)
            {
                _player.AddScore(10);
            }

            DeathSequence();
        }
    }

    private void DeathSequence()
    {
        foreach (var collider in _enemyCollider)
        {
            collider.enabled = false;
        }
        _isDead = true;
        _deathAnim.SetTrigger("OnEnemyDeath");
        _audioManager.Explosion();
        StopMoving();
        Destroy(this.gameObject, 2.4f);
    }

    public void AttackPowerup(Vector3 powerupPos)
    {
        if (Time.time > _powerupAttackCD && _isDead == false)
        {
            Vector3 targetPos = powerupPos - transform.position;
            float fireAngle = Mathf.Atan2(targetPos.y, targetPos.x) * Mathf.Rad2Deg - 90;
            float powerupDeviation = transform.position.y - powerupPos.y;
            if (powerupDeviation > 3.0f)
            {
                _powerupAttackCD = Time.time + 3.0f;
                GameObject newLaser = Instantiate(_laserPrefab, transform.position + _laserOffset, Quaternion.Euler(Vector3.forward * fireAngle));
                newLaser.transform.parent = _projectileContainer.transform;
                _enemyAudio.PlayOneShot(_laserClip);
            }
        }
    }

    private void StopMoving()
    {
        _enemySpeed = 0;
    }

    private void EnemyFire()
    {
        if (_playerObj != null)
        {
            Vector3 targetPos = _playerObj.transform.position - transform.position;
            float fireAngle = Mathf.Atan2(targetPos.y, targetPos.x) * Mathf.Rad2Deg - 90;
            _canFire = Time.time + Random.Range(3, 6);
            GameObject newLaser = Instantiate(_laserPrefab, transform.position + _laserOffset, Quaternion.Euler(Vector3.forward * fireAngle));
            newLaser.transform.parent = _projectileContainer.transform;
            _enemyAudio.PlayOneShot(_laserClip);
        }
    }

    private void FlyIn()
    {
        float vertSpeed = Mathf.Abs(transform.position.y - _flyInDestination.y);

        transform.Translate((Vector3.down * vertSpeed + (_flyInDirection * _enemySpeed)) * Time.deltaTime);
        
        if (vertSpeed < 1.5f)
        {
            _flyingIn = false;
        }
    }

    private void CalculateFlyIn()
    {
        if (transform.position.x > 0)
        {
            _flyInDirection = Vector3.left;
            _flyInDestination = new Vector3(transform.position.x - Random.Range(4.0f, 10.0f), 1, 0);
        }
        else if (transform.position.x < 0)
        {
            _flyInDirection = Vector3.right;
            _flyInDestination = new Vector3(transform.position.x + Random.Range(4.0f, 10.0f), 1, 0);
        }
    }

    private void OnDestroy()
    {
        _spawnManager.OnEnemyDeath(this.gameObject);
    }
}
