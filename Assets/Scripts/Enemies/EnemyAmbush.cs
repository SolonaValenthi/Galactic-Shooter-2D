using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAmbush : MonoBehaviour
{
    [SerializeField]
    private float _enemySpeed = 4.0f;
    [SerializeField]
    private GameObject _primaryLaserPrefab;
    [SerializeField]
    private GameObject _ambushTargetIndicator;
    [SerializeField]
    private GameObject _ambushLaser;
    [SerializeField]
    private AudioClip _primaryLaserClip;
    [SerializeField]
    private Vector3 _laserOffset;

    private float _spawnRange;
    private float _canFire;
    private float _fireRate = 4.5f;
    private float _playerDeviation;
    private bool _isAmbushing;
    private bool _flyingIn = true;
    private bool _isDead = false;
    private Player _player;
    private GameObject _playerObj;
    private GameObject _projectileContainer;
    private Vector3 _flyInDirection;
    private Vector3 _flyInDestination;

    AudioSource _enemyAudio;

    /// ambush enemy behavior outline
    /// Spawns in and moves like a normal enemy (done)
    /// can detect and will try to avoid player attacks
    /// actively tries to avoid collision with player to use ambush weapon
    /// upon reaching bottom screen bound stop moving
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
        _enemyAudio = gameObject.GetComponent<AudioSource>();

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
        if (_enemyAudio == null)
        {
            Debug.LogError("Ambush enemy audio source reference is NULL!");
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
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        
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
            _canFire = Time.time + Random.Range(3, 6);
            GameObject newLaser = Instantiate(_primaryLaserPrefab, transform.position + _laserOffset, Quaternion.Euler(Vector3.forward * fireAngle));
            newLaser.transform.parent = _projectileContainer.transform;
            _enemyAudio.PlayOneShot(_primaryLaserClip);
        }
    }

    private void Ambush()
    {

    }
}
