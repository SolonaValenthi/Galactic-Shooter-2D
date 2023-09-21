using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMissile : MonoBehaviour
{
    [SerializeField]
    private float _speed = 4.0f;

    private float _distanceToTarget = 100;
    private float _homingSpeed = 10.0f;
    private bool _homingOn = false;
    private GameObject _target;
    private SpawnManager _spawnManager;

    // Start is called before the first frame update
    void Start()
    {
        _spawnManager = GameObject.Find("Spawn_Manager").GetComponent<SpawnManager>();

        if (_spawnManager == null)
        {
            Debug.LogError("Player missile spawn manager reference is NULL!");
        }
        
        if (_spawnManager.enemies.Count > 0)
        {
            SetTarget();
        }

        StartCoroutine(HomingDelay());
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.up * _speed * Time.deltaTime);

        if (_target != null)
        {
            Homing();
        }      
    }

    private void SetTarget()
    {
        foreach (var enemy in _spawnManager.enemies)
        {
            float distance = Vector3.Distance(transform.position, enemy.transform.position);

            if (distance < _distanceToTarget)
            {
                _distanceToTarget = distance;
                _target = enemy;
            }
        }
    }

    private void Homing()
    {
        if (_homingOn == true)
        {
            Vector3 targetPos = _target.transform.position - transform.position;
            float homingAngle = Mathf.Atan2(targetPos.y, targetPos.x) * Mathf.Rad2Deg - 90;
            Quaternion targetAngle = Quaternion.Euler(Vector3.forward * homingAngle);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetAngle, Time.deltaTime * _homingSpeed);
        }
    }

    IEnumerator HomingDelay()
    {
        yield return new WaitForSeconds(0.5f);
        _homingOn = true;
        _speed = 8.0f;
    }
}
