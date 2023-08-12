using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    private float _enemySpeed = 4.0f;

    private float _spawnRange;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.down * _enemySpeed * Time.deltaTime);
        
        if (transform.position.y < -6)
        {
            _spawnRange = Random.Range(-12, 12);
            transform.position = new Vector3(_spawnRange, 8, 0);

            Debug.Log("x spawn pos set to" + _spawnRange);
        }
    }
}
