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
    private GameObject _enemyBoss;
    [SerializeField]
    private GameObject[] _powerups;
    [SerializeField]
    private GameObject[] _enemyTypes; // 0 = basic, 1 = agile, 2 = aggressive, 3 = ammbush

    // arrange _powerups array according to this table
    private int[] _powerupTable =
    {
        30, // fuel
        30, // ammo
        10, // shields
        10, // triple shot
        7,  // jamming
        7,  // heal
        3,  // bomb
        3   // missile
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
    private int _maxEnemies = 5;
    private int _totalEnemies;
    private bool _spawnEnemies = true;
    private bool _spawnPowerups = true;
    private bool _bossWave = false;
    private bool _waveSkipped = false; //for testing purposes only, remove from final version
    private UIManager _uiManager;
    private CameraShake _cameraShake;
    private WaitForSeconds _spawnTime = new WaitForSeconds(3.0f);

    public int currentWave { get; private set; }
    public List<GameObject> enemies { get; private set; } = new List<GameObject>();

    void Start()
    {
        _uiManager = GameObject.Find("UI_Manager").GetComponent<UIManager>();
        _cameraShake = GameObject.Find("Main Camera").GetComponent<CameraShake>();

        if (_uiManager == null)
        {
            Debug.LogError("Spawn manager ui manager reference is NULL!");
        }
        if (_cameraShake == null)
        {
            Debug.LogError("Spawn manager camera shake reference is NULL!");
        }

        foreach (var item in _powerupTable)
        {
            _totalWeight += item;
        }

        currentWave = 1;
        CalculateWave();
    }

    private void Update()
    {
        /*
        if (Input.GetKeyDown(KeyCode.O) && _waveSkipped == false)
        {
            _waveSkipped = true;
            SkipToBoss();
        }
        */
    }

    private void CalculateWave()
    {
        _basicToSpawn = currentWave * 2 + 3;
        _agileToSpawn = Mathf.RoundToInt(currentWave * 1.5f);
        _aggressiveToSpawn = Mathf.RoundToInt(currentWave * 0.75f);
        _ambushToSpawn = Mathf.RoundToInt(currentWave * 0.5f);
        _totalEnemies = _basicToSpawn + _agileToSpawn + _aggressiveToSpawn + _ambushToSpawn;

        if (currentWave % 5 == 0)
        {
            _bossWave = true;
        }
        else
        {
            _bossWave = false;
        }
    }

    // for testing purposes only, disable in final version
    private void SkipToBoss()
    {
        currentWave = 5;
        _bossWave = true;
        _basicToSpawn = 1;
        _agileToSpawn = 0;
        _aggressiveToSpawn = 0;
        _ambushToSpawn = 0;
        _totalEnemies = _basicToSpawn + _agileToSpawn + _aggressiveToSpawn + _ambushToSpawn;
        Debug.Log("skipped to boss wave");
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
        while (_spawnEnemies == true)
        {
            Vector3 spawnPos = new Vector3(Random.Range(-9.0f, 9.0f), 10, 0);
            int selectedEnemy = Random.Range(0, 4);
            GameObject newEnemy;
            switch (selectedEnemy)
            {
                case 0:
                    if (_basicSpawned < _basicToSpawn && enemies.Count < _maxEnemies)
                    {
                        newEnemy = Instantiate(_enemyTypes[selectedEnemy], spawnPos, Quaternion.identity);
                        newEnemy.transform.parent = _enemyContainer.transform;
                        enemies.Add(newEnemy);
                        _basicSpawned++;
                        _enemiesSpawned++;
                        yield return _spawnTime;
                    }                 
                    break;
                case 1:
                    if (_agileSpawned < _agileToSpawn && enemies.Count < _maxEnemies)
                    {
                        newEnemy = Instantiate(_enemyTypes[selectedEnemy], spawnPos, Quaternion.identity);
                        newEnemy.transform.parent = _enemyContainer.transform;
                        enemies.Add(newEnemy);
                        _agileSpawned++;
                        _enemiesSpawned++;
                        yield return _spawnTime;
                    }
                    break;
                case 2:
                    if (_aggressiveSpawned < _aggressiveToSpawn && enemies.Count < _maxEnemies)
                    {
                        newEnemy = Instantiate(_enemyTypes[selectedEnemy], spawnPos, Quaternion.identity);
                        newEnemy.transform.parent = _enemyContainer.transform;
                        enemies.Add(newEnemy);
                        _aggressiveSpawned++;
                        _enemiesSpawned++;
                        yield return _spawnTime;
                    }
                    break;
                case 3:
                    if (_ambushSpawned < _ambushToSpawn && enemies.Count < _maxEnemies)
                    {
                        newEnemy = Instantiate(_enemyTypes[selectedEnemy], spawnPos, Quaternion.identity);
                        newEnemy.transform.parent = _enemyContainer.transform;
                        enemies.Add(newEnemy);
                        _ambushSpawned++;
                        _enemiesSpawned++;
                        yield return _spawnTime;
                    }
                    break;
                default:
                    Debug.LogError("Invalid enemy type selected.");
                    break;
            }
            yield return null;
            if (_enemiesSpawned >= _totalEnemies)
            {
                yield return null;
                _spawnEnemies = false;
            }
        }
    }

    IEnumerator SpawnPowerupRoutine()
    {
        yield return new WaitForSeconds(4.0f);
        while (_spawnPowerups == true)
        {
            int powerupToSpawn = 0;
            Vector3 powerupPos = new Vector3(Random.Range(-9.0f, 9.0f), 10, 0);
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
        if (_waveSkipped == false)
        {
            CalculateWave();
        }
        StartCoroutine(_uiManager.AnnounceWave(currentWave));
        yield return new WaitForSeconds(1.0f);
        StartCoroutine(SpawnEnemyRoutine());
        StartCoroutine(SpawnPowerupRoutine());
    }

    IEnumerator WaveCleared()
    {
        StartCoroutine(_uiManager.Intermission());
        yield return new WaitForSeconds(1.0f);
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
    }

    IEnumerator BossSpawnSequence()
    {
        StartCoroutine(_cameraShake.BossCameraShift());
        yield return new WaitForSeconds(4.0f);
        Instantiate(_enemyBoss, new Vector3(0, 11, 0), Quaternion.identity);
    }

    public void StartSpawning()
    {
        _spawnEnemies = true;
        _spawnPowerups = true;
        StartCoroutine(WaveDelay());
    }

    public void OnPlayerDeath()
    {
        _spawnEnemies = false;
        _spawnPowerups = false;
    }

    public void OnEnemyDeath(GameObject enemy)
    {
        _enemiesKilled++;
        enemies.Remove(enemy);

        if (_enemiesKilled >= _totalEnemies)
        {
            if (_bossWave == false)
            {
                currentWave++;
                _spawnPowerups = false;
                StartCoroutine(WaveCleared());
            }
            else
            {
                StartCoroutine(BossSpawnSequence());
            }
        }
    }
}
