using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAmbush : MonoBehaviour
{
    [SerializeField]
    private float _enemySpeed = 4.0f;
    [SerializeField]
    private float _dodgeSpeed = 8.0f;
    [SerializeField]
    private GameObject _primaryLaserPrefab;
    [SerializeField]
    private GameObject _ambushLaser;
    [SerializeField]
    private GameObject _explosionPrefab;
    [SerializeField]
    private GameObject _enemyShield;
    [SerializeField]
    private AudioClip _primaryLaserClip;
    [SerializeField]
    private Vector3 _laserOffset;

    private float _spawnRange;
    private float _canFire;
    private float _fireRate = 3.0f;
    private float _trackingSpeed = 5.0f;
    private float _playerDeviation;
    private float _distanceToPlayer;
    private bool _trackingOn;
    private bool _isAmbushing;
    private bool _flyingIn = true;
    private bool _isDead = false;
    private bool _incomingAttack = false;
    private bool _dodgeCD = false;
    private bool _shieldActive = false;
    private Player _player;
    private AudioManager _audioManager;
    private PowerupDetection _powerupDetection;
    private SpawnManager _spawnManager;
    private GameObject _playerObj;
    private GameObject _guideLaser;
    private GameObject _projectileContainer;
    private Vector3 _flyInDirection;
    private Vector3 _flyInDestination;
    private Vector3 _projToDodge;
    private Color _GuideColor;

    AudioSource _enemyAudio;
    BoxCollider2D[] _enemyCollider;
    SpriteRenderer _guideSprite;

    // Start is called before the first frame update
    void Start()
    {
        _playerObj = GameObject.Find("Player");
        _guideLaser = transform.GetChild(1).gameObject;
        _projectileContainer = GameObject.Find("Enemy_Projectiles");
        _player = _playerObj.GetComponent<Player>();
        _audioManager = GameObject.Find("Audio_Manager").GetComponent<AudioManager>();
        _powerupDetection = gameObject.GetComponentInChildren<PowerupDetection>();
        _spawnManager = GameObject.Find("Spawn_Manager").GetComponent<SpawnManager>();
        _enemyAudio = gameObject.GetComponent<AudioSource>();
        _enemyCollider = gameObject.GetComponents<BoxCollider2D>();

        if (_playerObj == null)
        {
            Debug.LogError("Ambush enemy player object reference is NULL!");
        }
        if (_guideLaser == null)
        {
            Debug.LogError("Ambush enemy guide laser object reference is NULL!");
        }
        if (_projectileContainer == null)
        {
            Debug.LogError("Ambush enemy projectile container reference is NULL!");
        }
        if (_player == null)
        {
            Debug.LogError("Ambush enemy player script reference is NULL!");
        }
        if (_audioManager == null)
        {
            Debug.LogError("Ambush enemy audio manager reference is NULL!");
        }
        if (_powerupDetection == null)
        {
            Debug.LogError("Ambush enemy powerup detection reference is NULL!");
        }
        if (_spawnManager == null)
        {
            Debug.LogError("Ambush enemy spawn manager reference is NULL!");
        }
        if (_enemyAudio == null)
        {
            Debug.LogError("Ambush enemy audio source reference is NULL!");
        }
        if (_enemyCollider == null)
        {
            Debug.LogError("Ambush enemy collider reference is NULL!");
        }

        if (_guideLaser != null)
        {
            _guideSprite = _guideLaser.GetComponent<SpriteRenderer>();
            _GuideColor = _guideSprite.color;
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

        if (_playerObj != null)
        {
            _playerDeviation = transform.position.y - _playerObj.transform.position.y;
            _distanceToPlayer = Vector3.Distance(transform.position, _playerObj.transform.position);
        }

        if (_flyingIn == false)
        {
            if (_isAmbushing == false && _trackingOn == false)
            {
                EnemyMovement();

                if (Time.time > _canFire && _isDead == false && _playerDeviation > 1.3)
                {
                    EnemyFire();
                }
                if (transform.position.y <= -5.3f)
                {
                    _trackingOn = true;
                }
                if (_distanceToPlayer <= 2.5)
                {
                    AvoidPlayer();
                }
            }

            if (_trackingOn == true)
            {
                FacePlayer();
                if (_isAmbushing == false)
                {
                    StartCoroutine(Ambush());
                }
            }
        }

        if (_incomingAttack == true)
        {
            AvoidAttack(_projToDodge);
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
                _player.AddScore(40);
            }

            DeathSequence();
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

    private void EnemyMovement()
    {
        transform.Translate(Vector3.down * _enemySpeed * Time.deltaTime);

        if (transform.position.y < -7f)
        {
            _spawnRange = Random.Range(-9.0f, 9.0f);
            transform.rotation = Quaternion.identity;
            transform.position = new Vector3(_spawnRange, 8, 0);
            CalculateFlyIn();
            _flyingIn = true;
        }
    }

    private void EnemyFire()
    {
        if (_playerObj != null)
        {
            Vector3 targetPos = _playerObj.transform.position - transform.position;
            float fireAngle = Mathf.Atan2(targetPos.y, targetPos.x) * Mathf.Rad2Deg - 90;
            _canFire = Time.time + _fireRate;
            GameObject newLaser = Instantiate(_primaryLaserPrefab, transform.position + _laserOffset, Quaternion.Euler(Vector3.forward * fireAngle));
            newLaser.transform.parent = _projectileContainer.transform;
            _enemyAudio.PlayOneShot(_primaryLaserClip);
        }
    }

    private void FacePlayer()
    {
        if (_playerObj != null)
        {
            Vector3 lookTarget = _playerObj.transform.position - transform.position;
            float lookAngle = Mathf.Atan2(lookTarget.x, lookTarget.y) * Mathf.Rad2Deg - 180;
            Quaternion targetAngle = Quaternion.Euler(Vector3.back * lookAngle);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetAngle, Time.deltaTime * _trackingSpeed);
        }
    }

    private void AvoidPlayer()
    {
        if (_playerObj != null)
        {
            _flyingIn = false;
            if (_playerObj.transform.position.x >= transform.position.x)
            {
                transform.Translate(Vector3.left * _dodgeSpeed * Time.deltaTime);
            }
            else if (_playerObj.transform.position.x < transform.position.x)
            {
                transform.Translate(Vector3.right * _dodgeSpeed * Time.deltaTime);
            }
        }
    }

    private void AvoidAttack(Vector3 attackPosition)
    {
        if (_dodgeCD == false)
        {
            if (attackPosition.x >= transform.position.x)
            {
                transform.Translate(Vector3.left * _dodgeSpeed * Time.deltaTime);
            }
            else if (attackPosition.x < transform.position.x)
            {
                transform.Translate(Vector3.right * _dodgeSpeed * Time.deltaTime);
            }
        }
    }

    private void DeathSequence()
    {
        foreach (var collider in _enemyCollider)
        {
            collider.enabled = false;
        }
        _isDead = true;
        StopMoving();
        _audioManager.Explosion();
        Instantiate(_explosionPrefab, transform.position, transform.rotation);
        Destroy(this.gameObject, 1.0f);
    }

    private void StopMoving()
    {
        _enemySpeed = 0;
        _dodgeSpeed = 0;
    }

    public void IncomingLaser(Vector3 attackPosition)
    { 
        _projToDodge = attackPosition;
        _incomingAttack = true;
    }

    private void OnDestroy()
    {
        _spawnManager.OnEnemyDeath();
    }

    public IEnumerator AfterDodge()
    {
        yield return new WaitForSeconds(0.15f);
        _incomingAttack = false;
        _powerupDetection.ClearTarget();
        _dodgeCD = true;
        yield return new WaitForSeconds(1.5f);
        _dodgeCD = false;
    }

    IEnumerator Ambush()
    {
        _isAmbushing = true;
        _guideLaser.SetActive(true);
        while (_guideSprite.color.a < 1.0f)
        {
            _GuideColor.a += 0.06f;
            _guideSprite.color = _GuideColor;
            yield return new WaitForSeconds(0.1f);
        }
        for (int i = 0; i < 3; i++)
        {
            _trackingOn = false;
            _guideLaser.SetActive(false);
            yield return new WaitForSeconds(0.1f);
            _guideLaser.SetActive(true);
            yield return new WaitForSeconds(0.1f);
        }
        yield return new WaitForSeconds(0.5f);
        GameObject newLaser = Instantiate(_ambushLaser, transform.position, _guideLaser.transform.rotation);
        newLaser.transform.parent = _projectileContainer.transform;
        _GuideColor.a = 0.0f;
        _guideSprite.color = _GuideColor;
        _guideLaser.SetActive(false);
        yield return new WaitForSeconds(0.2f);
        while (transform.position.y > -7.5f)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.identity, Time.deltaTime);
            transform.position += Vector3.down * Time.deltaTime;
            yield return null;
        }
        EnemyMovement();
        _isAmbushing = false;
    }
}
