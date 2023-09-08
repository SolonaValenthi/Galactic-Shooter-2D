using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Asteroid : MonoBehaviour
{
    [SerializeField]
    private GameObject _explosion;

    private float _rotationSpeed = 4.0f;
    private SpawnManager _spawnManager;

    CircleCollider2D _asteroidCollider;

    // Start is called before the first frame update
    void Start()
    {
        _asteroidCollider = gameObject.GetComponent<CircleCollider2D>();
        _spawnManager = GameObject.Find("Spawn_Manager").GetComponent<SpawnManager>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(0f, 0f, _rotationSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Laser"))
        {
            _asteroidCollider.enabled = false;
            Destroy(other.gameObject);
            Instantiate(_explosion, transform.position, Quaternion.identity);
            _spawnManager.StartSpawning();
            Destroy(this.gameObject, 0.25f);          
        }
    }
}
