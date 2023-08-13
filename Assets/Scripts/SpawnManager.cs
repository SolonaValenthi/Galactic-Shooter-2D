using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField]
    private GameObject _enemyBasic;
    [SerializeField]
    private GameObject _enemyContainer;

    private bool _stopSpawning = false;

    private WaitForSeconds _spawnTime = new WaitForSeconds(5.0f);

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(SpawnRoutine());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator SpawnRoutine()
    {
        while (_stopSpawning == false)
        {        
            GameObject newEnemy = Instantiate(_enemyBasic);
            newEnemy.transform.parent = _enemyContainer.transform;
            yield return _spawnTime;
        }
    }

    public void OnPlayerDeath()
    {
        _stopSpawning = true;
    }
}
