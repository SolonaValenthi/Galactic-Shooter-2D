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
    private AudioClip _laserClip;

    private float _spawnRange;
    private float _canFire;
    private float _offset = -1.3f;
    private float _playerDeviation;
    private bool _isDead = false;
    private Player _player;
    private AudioManager _audioManager;
    private GameObject _playerObj;
    private Vector3 _laserOffset;

    Animator _deathAnim;
    AudioSource _enemyAudio;
    BoxCollider2D _enemyCollider;

    private void Start()
    {
        _player = GameObject.Find("Player").GetComponent<Player>();
        _deathAnim = gameObject.GetComponent<Animator>();
        _enemyCollider = gameObject.GetComponent<BoxCollider2D>();
        _enemyAudio = gameObject.GetComponent<AudioSource>();
        _audioManager = GameObject.Find("Audio_Manager").GetComponent<AudioManager>();
        _canFire = Random.Range(3, 6);
        _playerObj = GameObject.Find("Player");
        _laserOffset = new Vector3(0, _offset, 0);

        if (_player == null)
        {
            Debug.LogError("Enemy player reference is NULL!");
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
        if (_enemyAudio == null)
        {
            Debug.LogError("Enemy audio source reference is NULL!");
        }
    }

    // Update is called once per frame
    void Update()
    {
        EnemyMovement();

        if (_playerObj != null)
        {
            _playerDeviation = transform.position.y - _playerObj.transform.position.y;
        }      

        if (Time.time > _canFire && _isDead == false && _playerDeviation >= 1.3)
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

            DeathSequence();
        }

        if (other.CompareTag("Laser"))
        {
            if (_player != null)
            {
                _player.AddScore(10);
            }

            DeathSequence();
            Destroy(other.gameObject);
        }
    }

    private void DeathSequence()
    {
        Destroy(this._enemyCollider);
        _isDead = true;
        _deathAnim.SetTrigger("OnEnemyDeath");
        _enemySpeed = 0f;
        _audioManager.Explosion();
        Destroy(this.gameObject, 2.4f);
    }

    public void EnemyFire()
    {
        if (_playerObj != null)
        {
            Vector3 targetPos = _playerObj.transform.position - transform.position;
            float fireAngle = Mathf.Atan2(targetPos.y, targetPos.x) * Mathf.Rad2Deg - 90;
            _canFire = Time.time + Random.Range(3, 6);
            Instantiate(_laserPrefab, transform.position + _laserOffset, Quaternion.Euler(Vector3.forward * fireAngle));
            _enemyAudio.PlayOneShot(_laserClip);
        }
    }
}
