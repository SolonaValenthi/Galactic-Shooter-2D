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
    private int _enemiesKilled = 0;
    private int _totalEnemies;
    private bool _stopSpawning = false;
    private UIManager _uiManager;
    private WaitForSeconds _spawnTime = new WaitForSeconds(5.0f);

    public int _currentWave { get; private set; } = 1;

    void Start()
    {
        _uiManager = GameObject.Find("UI_Manager").GetComponent<UIManager>();

        if (_uiManager == null)
        {
            Debug.LogError("Spawn manager ui manager reference is NULL!");
        }

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
        _uiManager.UpdateWave(_currentWave);
    }

    private void ResetEnemyCount()
    {
        _basicSpawned = 0;
        _agileSpawned = 0;
        _aggressiveSpawned = 0;
        _ambushSpawned = 0;
        _enemiesSpawned = 0;
        _enemiesKilled = 0;
    }

    IEnumerator SpawnEnemyRoutine()
    {
        ResetEnemyCount();
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
                        _enemiesSpawned++;
                        yield return _spawnTime;
                    }                 
                    break;
                case 1:
                    if (_agileSpawned < _agileToSpawn)
                    {
                        newEnemy = Instantiate(_enemyTypes[selectedEnemy], spawnPos, Quaternion.identity);
                        newEnemy.transform.parent = _enemyContainer.transform;
                        _agileSpawned++;
                        _enemiesSpawned++;
                        yield return _spawnTime;
                    }
                    break;
                case 2:
                    if (_aggressiveSpawned < _aggressiveToSpawn)
                    {
                        newEnemy = Instantiate(_enemyTypes[selectedEnemy], spawnPos, Quaternion.identity);
                        newEnemy.transform.parent = _enemyContainer.transform;
                        _aggressiveSpawned++;
                        _enemiesSpawned++;
                        yield return _spawnTime;
                    }
                    break;
                case 3:
                    if (_ambushSpawned < _ambushToSpawn)
                    {
                        newEnemy = Instantiate(_enemyTypes[selectedEnemy], spawnPos, Quaternion.identity);
                        newEnemy.transform.parent = _enemyContainer.transform;
                        _ambushSpawned++;
                        _enemiesSpawned++;
                        yield return _spawnTime;
                    }
                    break;
                default:
                    Debug.LogError("Invalid enemy type selected.");
                    break;
            }
            if (_enemiesSpawned >= _totalEnemies)
            {
                _stopSpawning = true;
            }
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

    IEnumerator WaveDelay()
    {
        StartCoroutine(_uiManager.AnnounceWave());
        yield return new WaitForSeconds(1.0f);
        StartCoroutine(SpawnEnemyRoutine());
        StartCoroutine(SpawnPowerupRoutine());
    }

    public void StartSpawning()
    {
        _stopSpawning = false;
        StartCoroutine(WaveDelay());
    }

    public void OnPlayerDeath()
    {
        _stopSpawning = true;
    }

    public void OnEnemyDeath()
    {
        _enemiesKilled++;

        if (_enemiesKilled >= _totalEnemies)
        {
            int spawnBound = Random.Range(0, 3); // 0 = top, 1 = left, 2 = right
            float ySpawn;
            float xSpawn;
            Vector3 spawnPos;

            switch (spawnBound)
            {
                case 0:
                    ySpawn = 7.5f;
                    xSpawn = Random.Range(-12f, 12f);
                    spawnPos = new Vector3(xSpawn, ySpawn, 0);
                    Instantiate(_asteroidPrefab, spawnPos, Quaternion.identity);
                    break;
                case 1:
                    ySpawn = Random.Range(2.5f, 6.5f);
                    xSpawn = -12f;
                    spawnPos = new Vector3(xSpawn, ySpawn, 0);
                    Instantiate(_asteroidPrefab, spawnPos, Quaternion.identity);
                    break;
                case 2:
                    ySpawn = Random.Range(2.5f, 6.5f);
                    xSpawn = 12f;
                    spawnPos = new Vector3(xSpawn, ySpawn, 0);
                    Instantiate(_asteroidPrefab, spawnPos, Quaternion.identity);
                    break;
                default:
                    Debug.LogError("Invalid asteroid spawn bound detected");
                    break;
            }
            CalculateWave();
        }
    }
}
