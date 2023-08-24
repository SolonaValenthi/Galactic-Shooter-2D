using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    private float _enemySpeed = 4.0f;

    private float _spawnRange;
    private Player _player;
    private AudioManager _audioManager;

    Animator _deathAnim;
    BoxCollider2D _enemyCollider;

    private void Start()
    {
        _player = GameObject.Find("Player").GetComponent<Player>();
        _deathAnim = gameObject.GetComponent<Animator>();
        _enemyCollider = gameObject.GetComponent<BoxCollider2D>();
        _audioManager = GameObject.Find("Audio_Manager").GetComponent<AudioManager>();

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
    }

    // Update is called once per frame
    void Update()
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
            //Player player = other.GetComponent<Player>();

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
        _deathAnim.SetTrigger("OnEnemyDeath");
        _enemySpeed = 0f;
        _audioManager.Explosion();
        Destroy(this.gameObject, 2.4f);
    }
}
