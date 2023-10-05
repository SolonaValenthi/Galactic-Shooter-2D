using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMissile : MonoBehaviour
{
    private float _speed = 4.0f;
    private float _distanceToPlayer;
    private float _homingSpeed = 10.0f;
    private bool _homingOn = false;
    private GameObject _playerObj;
    private Vector3 _playerPos;
    
    // Start is called before the first frame update
    void Start()
    {
        _playerObj = GameObject.Find("Player");

        if (_playerObj == null)
        {
            Debug.LogError("Enemy missile player reference is NULL!");
        }

        StartCoroutine(HomingDelay());
    }

    // Update is called once per frame
    void Update()
    {
        if (_playerObj != null)
        {
            _playerPos = _playerObj.transform.position;
            _distanceToPlayer = Vector3.Distance(_playerPos, transform.position);
            transform.Translate(Vector3.up * _speed * Time.deltaTime);

            if (_homingOn == true)
            {
                Homing();
            }

            if (_distanceToPlayer < 3)
            {
                _homingOn = false;
            }
        }

        if (transform.position.y > 15)
        {
            if (transform.parent != null && transform.parent.tag != "Container")
            {
                Destroy(transform.parent.gameObject);
            }
            Destroy(this.gameObject);
        }
        else if (transform.position.y < -10)
        {
            if (transform.parent != null && transform.parent.tag != "Container")
            {
                Destroy(transform.parent.gameObject);
            }
            Destroy(this.gameObject);
        }
    }

    private void Homing()
    {
        Vector3 targetPos = _playerPos - transform.position;
        float homingAngle = Mathf.Atan2(targetPos.y, targetPos.x) * Mathf.Rad2Deg - 90;
        Quaternion targetRotation = Quaternion.Euler(Vector3.forward * homingAngle);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * _homingSpeed);
    }

    IEnumerator HomingDelay()
    {
        yield return new WaitForSeconds(1.0f);
        _homingOn = true;
        _speed = 8.0f;
        yield return new WaitForSeconds(1.3f);
        _homingOn = false;
    }
}
