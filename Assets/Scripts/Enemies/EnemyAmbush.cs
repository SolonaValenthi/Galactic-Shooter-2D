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
    private GameObject _ambushTargetIndicator;
    [SerializeField]
    private GameObject _ambushLaser;
    [SerializeField]
    private GameObject _explosionPrefab;
    [SerializeField]
    private AudioClip _primaryLaserClip;
    [SerializeField]
    private Vector3 _laserOffset;

    private float _spawnRange;
    private float _canFire;
    private float _fireRate = 3.0f;
    private float _playerDeviation;
    private float _distanceToPlayer;
    private bool _isAmbushing;
    private bool _flyingIn = true;
    private bool _isDead = false;
    private bool _incomingAttack = false;
    private bool _dodgeCD = false;
    private Player _player;
    private AudioManager _audioManager;
    private PowerupDetection _powerupDetection;
    private GameObject _playerObj;
    private GameObject _projectileContainer;
    private Vector3 _flyInDirection;
    private Vector3 _flyInDestination;
    private Vector3 _projToDodge;

    AudioSource _enemyAudio;
    BoxCollider2D[] _enemyCollider;

    /// ambush enemy behavior outline
    /// Spawns in and moves like a normal enemy (done)
    /// can detect and will try to avoid player attacks
    /// actively tries to avoid collision with player to use ambush weapon (done)
    /// upon reaching bottom screen bound stop moving (done)
    /// turn towards and begin tracking the player
    /// display a target indicator
    /// cease tracking after a few seconds
    /// fire a large piercing laser at the final position, shortly after tracking ends
    /// resume moving off screen to "respawn" at the top bound

    // Start is called before the first frame update
    void Start()
    {
        _playerObj = GameObject.Find("Player");
        _projectileContainer = GameObject.Find("Enemy_Projectiles");
        _player = _playerObj.GetComponent<Player>();
        _audioManager = GameObject.Find("Audio_Manager").GetComponent<AudioManager>();
        _powerupDetection = gameObject.GetComponentInChildren<PowerupDetection>();
        _enemyAudio = gameObject.GetComponent<AudioSource>();
        _enemyCollider = gameObject.GetComponents<BoxCollider2D>();

        if (_playerObj == null)
        {
            Debug.LogError("Ambush enemy player object reference is NULL!");
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
        if (_enemyAudio == null)
        {
            Debug.LogError("Ambush enemy audio source reference is NULL!");
        }
        if (_enemyCollider == null)
        {
            Debug.LogError("Ambush enemy collider reference is NULL!");
        }

        CalculateFlyIn();
    }

    // Update is called once per frame
    void Update()
    {
        _distanceToPlayer = Vector3.Distance(transform.position, _playerObj.transform.position);
        
        if (_flyingIn == true)
        {
            FlyIn();
        }

        if (_playerObj != null)
        {
            _playerDeviation = transform.position.y - _playerObj.transform.position.y;
        }

        if (_flyingIn == false && _isAmbushing == false)
        {
            EnemyMovement();

            if (Time.time > _canFire && _isDead == false && _playerDeviation > 1.3)
            {
                EnemyFire();
            }
            if (transform.position.y <= -5.3f)
            {
                _isAmbushing = true;
            }
            if (_distanceToPlayer <= 2.5)
            {
                AvoidPlayer();
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
            _player.Damage();
            Deathsequence();
        }
        
        if (other.CompareTag("Laser"))
        {
            Destroy(other.gameObject);
            Deathsequence();
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

        if (transform.position.y < -7)
        {
            _spawnRange = Random.Range(-9.0f, 9.0f);
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

    private void AvoidPlayer()
    {
        if (_playerObj.transform.position.x >= transform.position.x)
        {
            transform.Translate(Vector3.left * _dodgeSpeed * Time.deltaTime);
        }
        else if (_playerObj.transform.position.x < transform.position.x)
        {
            transform.Translate(Vector3.right * _dodgeSpeed * Time.deltaTime);
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

    private void Ambush()
    {

    }

    private void Deathsequence()
    {
        foreach (var collider in _enemyCollider)
        {
            collider.enabled = false;
        }
        _isDead = true;
        _audioManager.Explosion();
        Instantiate(_explosionPrefab, transform.position, transform.rotation);
        Destroy(this.gameObject, 1.0f);
    }

    public void IncomingLaser(Vector3 attackPosition)
    { 
        _projToDodge = attackPosition;
        _incomingAttack = true;
    }

     public IEnumerator AfterDodge()
    {
        yield return new WaitForSeconds(0.15f);
        _incomingAttack = false;
        _powerupDetection.ClearTarget();
        _dodgeCD = true;
        yield return new WaitForSeconds(2.0f);
        _dodgeCD = false;
    }
}
