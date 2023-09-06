using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField]
    private GameObject _enemyBasic;
    [SerializeField]
    private GameObject _enemyContainer;
    [SerializeField]
    private GameObject[] _powerups;

    // arrange _powerups array according to this table
    private int[] _powerupTable =
    {
        30, // fuel
        30, // ammo
        15, // shields
        15, // triple shot
        5,  // heal
        5   // bomb
    };

    private int _totalWeight;
    private int _randomPowerup;
    private bool _stopSpawning = false;
    private WaitForSeconds _spawnTime = new WaitForSeconds(5.0f);

    void Start()
    {
        foreach (var item in _powerupTable)
        {
            _totalWeight += item;
        }
    }

    public void StartSpawning()
    {
        StartCoroutine(SpawnEnemyRoutine());
        StartCoroutine(SpawnPowerupRoutine());
    }

    IEnumerator SpawnEnemyRoutine()
    {
        yield return new WaitForSeconds(3.0f);
        while (_stopSpawning == false)
        {
            Vector3 spawnPos = new Vector3(Random.Range(-9.0f, 9.0f), 8, 0);
            GameObject newEnemy = Instantiate(_enemyBasic, spawnPos, Quaternion.identity);
            newEnemy.transform.parent = _enemyContainer.transform;
            yield return _spawnTime;
        }
    }

    IEnumerator SpawnPowerupRoutine()
    {
        yield return new WaitForSeconds(4.0f);
        while (_stopSpawning == false)
        {
            int powerupToSpawn = 0;
            Vector3 powerupPos = new Vector3(Random.Range(-9.0f, 9.0f), 8, 0);
            _randomPowerup = Random.Range(0, _totalWeight);

            foreach(var weight in _powerupTable)
            {
                if (_randomPowerup <= weight)
                {
                    Instantiate(_powerups[powerupToSpawn], powerupPos, Quaternion.identity);
                    break;
                }
                else
                {
                    _randomPowerup -= weight;
                    powerupToSpawn++;
                }
            }

            yield return new WaitForSeconds(8f);
        }
    }

    public void OnPlayerDeath()
    {
        _stopSpawning = true;
    }
}
