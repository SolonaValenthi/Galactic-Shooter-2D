using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAggressive : MonoBehaviour
{
    [SerializeField]
    private float _enemySpeed = 4.0f;
    [SerializeField]
    private float _rammingSpeed = 6.0f;
    [SerializeField]
    private GameObject _missilePrefab;
    [SerializeField]
    private GameObject _explosionPrefab;

    private float _distanceToPlayer;
    private bool _isDead = false;
    private bool _flyingIn = true;
    private bool _movePhase;
    private bool _ramPhase;
    private bool _retreatPhase;
    private bool _firePhase;
    private GameObject _playerObj;
    private GameObject _projectileContainer;
    private AudioManager _audioManager;
    private Player _player;
    private Vector3 _flyInDirection;
    private Vector3 _flyInDestination;
    private Vector3 _retreatDestination;
    private Vector3 _playerPos;

    AudioSource _enemyAudio;
    BoxCollider2D _enemyCollider;

    // Start is called before the first frame update
    void Start()
    {
        _playerObj = GameObject.Find("Player");
        _projectileContainer = GameObject.Find("Enemy_Projectiles");
        _audioManager = GameObject.Find("Audio_Manager").GetComponent<AudioManager>();
        _player = _playerObj.GetComponent<Player>();
        _enemyAudio = gameObject.GetComponent<AudioSource>();
        _enemyCollider = gameObject.GetComponent<BoxCollider2D>();

        if (_playerObj == null)
        {
            Debug.LogError("Aggressiive enemy player reference is NULL!");
        }
        if (_projectileContainer == null)
        {
            Debug.LogError("Aggressive enemy projectile container reference is NULL!");
        }
        if (_audioManager == null)
        {
            Debug.LogError("Aggressive enemy audio manager reference is NULL!");
        }
        if (_player == null)
        {
            Debug.LogError("Aggressive enemy player script reference is NULL!");
        }
        if (_enemyAudio == null)
        {
            Debug.LogError("Aggressive enemy audio source reference is NULL!");
        }
        if (_enemyCollider == null)
        {
            Debug.LogError("Aggressive enemy collider reference is NULL!");
        }

        CalculateFlyIn();
    }

    // Update is called once per frame
    void Update()
    {
        if (_isDead == false)
        {
            if (_playerObj != null)
            {
                _playerPos = _playerObj.transform.position - transform.position;
                _distanceToPlayer = Vector3.Distance(_playerObj.transform.position, transform.position);
            }

            if (_flyingIn == true)
            {
                FlyIn();
            }

            if (_flyingIn == false && _ramPhase == false)
            {
                FacePlayer();
            }

            if (_movePhase == true)
            {
                MoveToPlayer();
            }

            if (_ramPhase == true)
            {
                RamPlayer();
            }

            if (_retreatPhase == true)
            {
                Retreat(_retreatDestination);
            }

            if (_firePhase == true)
            {
                StartCoroutine(ExitFirePhase());
                _firePhase = false;
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
                _ramPhase = false;
                SelectDestination();
            }
        }

        if (other.CompareTag("Laser"))
        {
            if (_player != null)
            {
                _player.AddScore(40);
            }

            Destroy(other.gameObject);
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
            _movePhase = true;
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

    private void MoveToPlayer()
    {
        if (_playerObj != null)
        {
            transform.Translate(Vector3.down * _enemySpeed * Time.deltaTime);
        }

        if (_distanceToPlayer <= 5.0f)
        {
            _movePhase = false;
            _ramPhase = true;
        }
    }

    private void FacePlayer()
    {
        float lookAngle = Mathf.Atan2(_playerPos.x, _playerPos.y) * Mathf.Rad2Deg + 180;
        transform.rotation = Quaternion.Euler(Vector3.back * lookAngle);
    }

    private void RamPlayer()
    {
        if (_playerObj != null)
        {
            transform.Translate(Vector3.down * _rammingSpeed * Time.deltaTime);
        }

        if (transform.position.y <= -4.5f)
        {
            _ramPhase = false;
            SelectDestination();
        }
    }

    private void SelectDestination()
    {
        float xPos = Random.Range(-9.5f, 9.5f);
        float yPos = Random.Range(3.5f, 5.0f);
        _retreatDestination = new Vector3(xPos, yPos, 0);
        _retreatPhase = true;
    }

    private void Retreat(Vector3 destination)
    {
        _enemyCollider.enabled = false;
        Vector3 targetPos = destination - transform.position;
        float retreatDistance = Vector3.Distance(destination, transform.position);
        transform.position += (targetPos * Time.deltaTime);

        if (retreatDistance <= 0.3)
        {
            _enemyCollider.enabled = true;
            _retreatPhase = false;
            _firePhase = true;
        }
    }

    private void DeathSequence()
    {
        _enemyCollider.enabled = false;
        _isDead = true;
        Instantiate(_explosionPrefab, transform.position, transform.rotation);
        _audioManager.Explosion();
        Destroy(this.gameObject, 1.0f);
    }

    IEnumerator ExitFirePhase()
    {
        GameObject newMissile;
        yield return new WaitForSeconds(0.5f);
        if (_playerObj != null)
        {
            newMissile = Instantiate(_missilePrefab, transform.position, transform.rotation);
            newMissile.transform.parent = _projectileContainer.transform;
        }
        yield return new WaitForSeconds(2.0f);
        if (_playerObj != null)
        {
            newMissile = Instantiate(_missilePrefab, transform.position, transform.rotation);
            newMissile.transform.parent = _projectileContainer.transform;
        }
        yield return new WaitForSeconds(0.5f);
        _movePhase = true;
    }
}
