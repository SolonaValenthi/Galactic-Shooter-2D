using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAmbush : MonoBehaviour
{
    [SerializeField]
    private float _enemySpeed = 4.0f;
    [SerializeField]
    private GameObject _primaryLaserPrefab;
    [SerializeField]
    private GameObject _ambushTargetIndicator;
    [SerializeField]
    private GameObject _ambushLaser;
    [SerializeField]
    private AudioClip _primaryLaserClip;
    [SerializeField]
    private Vector3 _laserOffset;

    private float _spawnRange;
    private float _canFire;
    private bool _isAmbushing;
    private bool _flyingIn = true;
    private GameObject _playerObj;
    private Vector3 _flyInDirection;
    private Vector3 _flyInDestination;

    /// ambush enemy behavior outline
    /// Spawns in and moves like a normal enemy (done)
    /// can detect and will try to avoid player attacks
    /// actively tries to avoid collision with player to use ambush weapon
    /// upon reaching bottom screen bound stop moving
    /// turn towards and begin tracking the player
    /// display a target indicator
    /// cease tracking after a few seconds
    /// fire a large piercing laser at the final position, shortly after tracking ends
    /// resume moving off screen to "respawn" at the top bound

    // Start is called before the first frame update
    void Start()
    {


        CalculateFlyIn();
    }

    // Update is called once per frame
    void Update()
    {
        if (_flyingIn == true)
        {
            FlyIn();
        }

        if (_flyingIn == false && _isAmbushing == false)
        {
            AmbushMovement();
            
            if (transform.position.y <= -5.3f)
            {
                _isAmbushing = true;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        
    }

    private void FlyIn()
    {
        float vertSpeed = Mathf.Abs(transform.position.y - _flyInDestination.y);

        transform.Translate((Vector3.down * vertSpeed + (_flyInDirection * _enemySpeed)) * Time.deltaTime);

        if (vertSpeed < 1.5f)
        {
            _flyingIn = false;
        }
    }

    private void CalculateFlyIn()
    {
        if (transform.position.x > 0)
        {
            _flyInDirection = Vector3.left;
            _flyInDestination = new Vector3(transform.position.x - Random.Range(4.0f, 10.0f), 1, 0);
        }
        else if (transform.position.x < 0)
        {
            _flyInDirection = Vector3.right;
            _flyInDestination = new Vector3(transform.position.x + Random.Range(4.0f, 10.0f), 1, 0);
        }
    }

    private void AmbushMovement()
    {
        transform.Translate(Vector3.down * _enemySpeed * Time.deltaTime);

        if (transform.position.y < -7)
        {
            _spawnRange = Random.Range(-9.0f, 9.0f);
            transform.position = new Vector3(_spawnRange, 8, 0);
            CalculateFlyIn();
            _flyingIn = true;
        }
    }
}
