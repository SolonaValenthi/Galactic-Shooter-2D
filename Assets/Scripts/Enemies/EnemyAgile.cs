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
    private AudioClip _laserClip;

    private bool _flyingIn = true;
    private bool _isDead = false;
    private bool _atDestination = true;
    private float _xDestination; // -9.5 to 9.5
    private float _yDestination; // 3 to 5
    private float _canFire;
    private float _fireAngle;
    private float _shotVariance;
    private GameObject _playerObj;
    private Player _player;
    private AudioManager _audioManager;
    private Vector3 _nextDestination;
    private Vector3 _flyInDirection;
    private Vector3 _flyInDestination;

    AudioSource _enemyAudio;
    BoxCollider2D _enemyCollider;

    // Start is called before the first frame update
    void Start()
    {
        _playerObj = GameObject.Find("Player");
        _player = _playerObj.GetComponent<Player>();
        _audioManager = GameObject.Find("Audio_manager").GetComponent<AudioManager>();
        _enemyAudio = gameObject.GetComponent<AudioSource>();
        _enemyCollider = gameObject.GetComponent<BoxCollider2D>();

        if (_playerObj == null)
        {
            Debug.LogError("Agile enemy player reference is NULL!");
        }
        if (_player == null)
        {
            Debug.LogError("Agile enemy player script reference is NULL!");
        }
        if (_audioManager == null)
        {
            Debug.LogError("Agile enemy audio manager reference is NULL!");
        }
        if (_enemyAudio == null)
        {
            Debug.LogError("Agile enemy audio source reference is NULL!");
        }
        if (_enemyCollider == null)
        {
            Debug.LogError("Agile enemy collider reference is NULL!");
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

        if (_atDestination == false && _isDead == false)
        {
            MoveToDestination(_nextDestination);
        }

        if (_flyingIn == false)
        {
            FacePlayer();
        }

        if (Time.time > _canFire && _flyingIn == false && _atDestination == true && _isDead == false)
        {
            _canFire = Time.time + 2.0f;
            StartCoroutine(AgileFire());
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        
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
            _flyInDestination = new Vector3(transform.position.x - Random.Range(4.0f, 10.0f), 1, 0);
        }
        else if (transform.position.x < 0)
        {
            _flyInDirection = Vector3.right;
            _flyInDestination = new Vector3(transform.position.x + Random.Range(4.0f, 10.0f), 1, 0);
        }
    }

    private void FacePlayer()
    {
        Vector3 lookTarget = _playerObj.transform.position - transform.position;
        float lookAngle = Mathf.Atan2(lookTarget.x, lookTarget.y) * Mathf.Rad2Deg - 180;
        transform.rotation = Quaternion.Euler(Vector3.back * lookAngle);
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
        _fireAngle = transform.eulerAngles.z + 180;
        Instantiate(_laserPrefab, transform.TransformPoint(Vector3.down * 1.3f), Quaternion.Euler(Vector3.forward * _fireAngle));
        yield return new WaitForSeconds(0.1f);
        _shotVariance = Random.Range(3f, 10f);
        Instantiate(_laserPrefab, transform.TransformPoint(Vector3.down * 1.3f), Quaternion.Euler(Vector3.forward * (_fireAngle - _shotVariance)));
        yield return new WaitForSeconds(0.1f);
        _shotVariance = Random.Range(3f, 10f);
        Instantiate(_laserPrefab, transform.TransformPoint(Vector3.down * 1.3f), Quaternion.Euler(Vector3.forward * (_fireAngle + _shotVariance)));
    }
}
