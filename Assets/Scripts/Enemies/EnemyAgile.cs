using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAgile : MonoBehaviour
{
    [SerializeField]
    private float _enemySpeed = 6.0f;
    [SerializeField]
    private GameObject _laserPrefab;
    [SerializeField]
    private GameObject _explosionPrefab;
    [SerializeField]
    private GameObject _enemyShield;
    [SerializeField]
    private AudioClip _laserClip;
    [SerializeField]
    private Vector3 _laserOffset;

    private bool _flyingIn = true;
    private bool _isDead = false;
    private bool _atDestination = true;
    private bool _shieldActive = false;
    private float _xDestination; // -9.5 to 9.5
    private float _yDestination; // 3 to 5
    private float _canFire;
    private float _fireAngle;
    private float _shotVariance;
    private GameObject _playerObj;
    private GameObject _projectileContainer;
    private Player _player;
    private AudioManager _audioManager;
    private SpawnManager _spawnManager;
    private Vector3 _nextDestination;
    private Vector3 _flyInDirection;
    private Vector3 _flyInDestination;

    AudioSource _enemyAudio;
    BoxCollider2D[] _enemyCollider;

    // Start is called before the first frame update
    void Start()
    {
        _playerObj = GameObject.Find("Player");
        _projectileContainer = GameObject.Find("Enemy_Projectiles");
        _player = _playerObj.GetComponent<Player>();
        _audioManager = GameObject.Find("Audio_Manager").GetComponent<AudioManager>();
        _spawnManager = GameObject.Find("Spawn_Manager").GetComponent<SpawnManager>();
        _enemyAudio = gameObject.GetComponent<AudioSource>();
        _enemyCollider = gameObject.GetComponents<BoxCollider2D>();

        if (_playerObj == null)
        {
            Debug.LogError("Agile enemy player reference is NULL!");
        }
        if (_projectileContainer == null)
        {
            Debug.LogError("Agile enemy projectile container reference is NULL!");
        }
        if (_player == null)
        {
            Debug.LogError("Agile enemy player script reference is NULL!");
        }
        if (_audioManager == null)
        {
            Debug.LogError("Agile enemy audio manager reference is NULL!");
        }
        if (_spawnManager == null)
        {
            Debug.LogError("Agile enemy spawn manager reference is NULL!");
        }
        if (_enemyAudio == null)
        {
            Debug.LogError("Agile enemy audio source reference is NULL!");
        }
        if (_enemyCollider == null)
        {
            Debug.LogError("Agile enemy collider reference is NULL!");
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
        if (_isDead == false)
        {
            if (_flyingIn == true)
            {
                FlyIn();
            }

            if (_atDestination == false && _playerObj != null)
            {
                MoveToDestination(_nextDestination);
            }

            if (_flyingIn == false)
            {
                FacePlayer();
            }

            if (Time.time > _canFire && _flyingIn == false && _atDestination == true && _playerObj != null)
            {
                _canFire = Time.time + 2.0f;
                StartCoroutine(AgileFire());
            }
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
                _player.AddScore(25);
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
            StartCoroutine(AgileMovement());
        }
    }

    private void CalculateFlyIn()
    {
        if (transform.position.x > 0)
        {
            _flyInDirection = Vector3.left;
            _flyInDestination = new Vector3(transform.position.x - Random.Range(4.0f, 10.0f), 3, 0);
        }
        else if (transform.position.x < 0)
        {
            _flyInDirection = Vector3.right;
            _flyInDestination = new Vector3(transform.position.x + Random.Range(4.0f, 10.0f), 3, 0);
        }
    }

    private void FacePlayer()
    {
        if (_playerObj != null)
        {
            Vector3 lookTarget = _playerObj.transform.position - transform.position;
            float lookAngle = Mathf.Atan2(lookTarget.x, lookTarget.y) * Mathf.Rad2Deg - 180;
            transform.rotation = Quaternion.Euler(Vector3.back * lookAngle);
        }
    }

    private void SelectDesitnation()
    {
        _xDestination = Random.Range(-9.5f, 9.5f);
        _yDestination = Random.Range(3.0f, 5.0f);
        _nextDestination = new Vector3(_xDestination, _yDestination, 0);
    }

    private void MoveToDestination(Vector3 destination)
    {
        Vector3 targetPos = destination - transform.position;
        transform.position += (targetPos * Time.deltaTime);

        if (transform.position == destination)
        {
            _atDestination = true;
        }
    }

    private void DeathSequence()
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

    private void OnDestroy()
    {
        _spawnManager.OnEnemyDeath(this.gameObject);
    }

    IEnumerator AgileMovement()
    {
        while (true)
        {
            SelectDesitnation();
            _atDestination = true;
            yield return new WaitForSeconds(5.0f);
            _atDestination = false;
            yield return new WaitForSeconds(3.0f);
        }
    }

    IEnumerator AgileFire()
    {
        GameObject newLaser;
        int shotsToFire = Random.Range(3, 6);

        for (int i = 0; i < shotsToFire; i++)
        {
            _shotVariance = Random.Range(-10f, 10f);
            _fireAngle = transform.eulerAngles.z + 180;
            newLaser = Instantiate(_laserPrefab, transform.TransformPoint(_laserOffset), Quaternion.Euler(Vector3.forward * (_fireAngle + _shotVariance)));
            newLaser.transform.parent = _projectileContainer.transform;
            _enemyAudio.PlayOneShot(_laserClip);
            yield return new WaitForSeconds(0.1f);
        }
    }
}
