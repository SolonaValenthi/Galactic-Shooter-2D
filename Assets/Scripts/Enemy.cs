using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    private float _enemySpeed = 4.0f;

    private float _spawnRange;
    private Player _player;

    Animator _deathAnim;
    BoxCollider2D _enemyCollider;

    private void Start()
    {
        _player = GameObject.Find("Player").GetComponent<Player>();
        _deathAnim = gameObject.GetComponent<Animator>();
        _enemyCollider = gameObject.GetComponent<BoxCollider2D>();

        if (_player == null)
        {
            Debug.Log("Unable to set player");
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
        Destroy(this.gameObject, 2.4f);
    }
}
