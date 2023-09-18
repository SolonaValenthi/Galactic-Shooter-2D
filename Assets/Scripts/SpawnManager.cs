using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField]
    private GameObject _enemyContainer;
    [SerializeField]
    private GameObject _asteroidPrefab;
    [SerializeField]
    private GameObject[] _powerups;
    [SerializeField]
    private GameObject[] _enemyTypes; // 0 = basic, 1 = agile, 2 = aggressive, 3 = ammbush

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
    private int _basicToSpawn;
    private int _agileToSpawn;
    private int _aggressiveToSpawn;
    private int _ambushToSpawn;
    private int _basicSpawned = 0;
    private int _agileSpawned = 0;
    private int _aggressiveSpawned = 0;
    private int _ambushSpawned = 0;
    private int _enemiesSpawned = 0;
    private int _totalEnemies;
    private bool _stopSpawning = false;
    private WaitForSeconds _spawnTime = new WaitForSeconds(5.0f);

    public int _currentWave { get; private set; } = 1;

    void Start()
    {
        foreach (var item in _powerupTable)
        {
            _totalWeight += item;
        }

        CalculateWave();
    }

    private void CalculateWave()
    {
        _basicToSpawn = _currentWave * 2 + 3;
        _agileToSpawn = Mathf.RoundToInt(_currentWave * 1.5f);
        _aggressiveToSpawn = Mathf.RoundToInt(_currentWave * 0.75f);
        _ambushToSpawn = Mathf.RoundToInt(_currentWave * 0.5f);
        _totalEnemies = _basicToSpawn + _agileToSpawn + _aggressiveToSpawn + _ambushToSpawn;
    }

    IEnumerator SpawnEnemyRoutine()
    {
        yield return new WaitForSeconds(3.0f);
        while (_stopSpawning == false)
        {
            Vector3 spawnPos = new Vector3(Random.Range(-9.0f, 9.0f), 8, 0);
            int selectedEnemy = Random.Range(0, 4);
            GameObject newEnemy;
            switch (selectedEnemy)
            {
                case 0:
                    if (_basicSpawned < _basicToSpawn)
                    {
                        newEnemy = Instantiate(_enemyTypes[selectedEnemy], spawnPos, Quaternion.identity);
                        newEnemy.transform.parent = _enemyContainer.transform;
                        _basicSpawned++;
                    }                 
                    break;
                case 1:
                    if (_agileSpawned < _agileToSpawn)
                    {
                        newEnemy = Instantiate(_enemyTypes[selectedEnemy], spawnPos, Quaternion.identity);
                        newEnemy.transform.parent = _enemyContainer.transform;
                        _agileSpawned++;
                    }
                    break;
                case 2:
                    if (_aggressiveSpawned < _aggressiveToSpawn)
                    {
                        newEnemy = Instantiate(_enemyTypes[selectedEnemy], spawnPos, Quaternion.identity);
                        newEnemy.transform.parent = _enemyContainer.transform;
                        _aggressiveSpawned++;
                    }
                    break;
                case 3:
                    if (_ambushSpawned < _ambushToSpawn)
                    {
                        newEnemy = Instantiate(_enemyTypes[selectedEnemy], spawnPos, Quaternion.identity);
                        newEnemy.transform.parent = _enemyContainer.transform;
                        _ambushSpawned++;
                    }
                    break;
                default:
                    Debug.LogError("Invalid enemy type selected.");
                    break;
            }
            _enemiesSpawned = _basicSpawned + _agileSpawned + _aggressiveSpawned + _ambushSpawned;
            if (_enemiesSpawned >= _totalEnemies)
            {
                _stopSpawning = true;
            }
            yield return _spawnTime;
        }
        _currentWave++;
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
                if (_randomPowerup < weight)
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

    public void StartSpawning()
    {
        StartCoroutine(SpawnEnemyRoutine());
        StartCoroutine(SpawnPowerupRoutine());
    }


    public void OnPlayerDeath()
    {
        _stopSpawning = true;
    }
}
