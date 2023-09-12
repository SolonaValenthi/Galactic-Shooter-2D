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
    private Player _player;
    private Vector3 _flyInDirection;
    private Vector3 _flyInDestination;
    private Vector3 _retreatDestination;
    private Vector3 _playerPos;

    /// Aggressive enemy behavior outline
    /// alternate between firing and moving, similar to agile enemy
    /// move directly towards player after initial fly-in
    /// attemp to "ram" the player when in close proximity
    /// after ram attempt retreat to top screen bound
    /// does not die when colliding with player
    /// enter fire mode
    /// fire a volley of homing projectiles at the player (missiles probably)
    /// homing function deactivates once projectiles are near the player
    /// enter move phase again 

    // Start is called before the first frame update
    void Start()
    {
        _playerObj = GameObject.Find("Player");
        _player = _playerObj.GetComponent<Player>();

        if (_playerObj == null)
        {
            Debug.LogError("Aggressiive enemy player reference is NULL!");
        }
        if (_player == null)
        {
            Debug.LogError("Aggressive enemy player script reference is NULL!");
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
        Vector3 targetPos = destination - transform.position;
        float retreatDistance = Vector3.Distance(destination, transform.position);
        transform.position += (targetPos * Time.deltaTime);

        if (retreatDistance <= 0.3)
        {
            _retreatPhase = false;
            _firePhase = true;
        }
    }

    IEnumerator ExitFirePhase()
    {
        yield return new WaitForSeconds(0.5f);
        if (_playerObj != null)
        {
            Instantiate(_missilePrefab, transform.position, transform.rotation);
        }
        yield return new WaitForSeconds(2.0f);
        if (_playerObj != null)
        {
            Instantiate(_missilePrefab, transform.position, transform.rotation);
        }
        yield return new WaitForSeconds(0.5f);
        _movePhase = true;
    }
}
