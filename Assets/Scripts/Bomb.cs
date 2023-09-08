using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    [SerializeField]
    private GameObject _laserPrefab;
    [SerializeField]
    private GameObject _laserBurstPrefab;
    
    private float _rotationSpeed;
    private float _speed = 2f;
    private float _duration = 2.0f;
    private int _rotateDirection;

    // Start is called before the first frame update
    void Start()
    {
        _rotateDirection = Random.Range(0, 2);
        if (_rotateDirection == 0)
        {
            _rotationSpeed = -2;
        }
        else
        {
            _rotationSpeed = 2;
        }
        StartCoroutine(BombSequence());
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += Vector3.up * _speed * Time.deltaTime;
        transform.Rotate(0, 0, _rotationSpeed);
    }

    IEnumerator BombSequence()
    {
        float elapsed = 0f;

        while (elapsed < _duration)
        {
            Instantiate(_laserPrefab, transform.position, transform.rotation);
            elapsed += 0.1f;
            yield return new WaitForSeconds(0.1f);
        }
        Instantiate(_laserBurstPrefab, transform.position, transform.rotation);
        Destroy(this.gameObject);      
    }
}
