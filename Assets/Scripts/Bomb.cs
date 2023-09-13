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
    private GameObject _projectileContainer;

    // Start is called before the first frame update
    void Start()
    {
        _rotateDirection = Random.Range(0, 2);
        _projectileContainer = GameObject.Find("Player_Projectiles");

        if (_projectileContainer == null)
        {
            Debug.LogError("Bomb projectile container reference is NULL!");
        }

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
        GameObject newLaser;

        while (elapsed < _duration)
        {
            newLaser = Instantiate(_laserPrefab, transform.position, transform.rotation);
            newLaser.transform.parent = _projectileContainer.transform;
            elapsed += 0.1f;
            yield return new WaitForSeconds(0.1f);
        }
        newLaser = Instantiate(_laserBurstPrefab, transform.position, transform.rotation);
        newLaser.transform.parent = _projectileContainer.transform;
        Destroy(this.gameObject);      
    }
}
