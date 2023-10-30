using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldDrone : MonoBehaviour
{
    [SerializeField]
    private GameObject _orbLaser;
    [SerializeField]
    private GameObject _spriteController;

    private float _speed = 4;
    private float _lookAngle;
    private float _canFire;
    private float _fireRate = 0.5f;
    private int _droneID;
    private bool _isRotating = false;
    private bool _isDead = false;
    private GameObject _boss;
    private GameObject _playerObj;
    private GameObject _projectileContainer;
    private BossAI _bossAI;

    private static float _rotationSpeed = 36;
    private static int _killedDrones = 0;

    // Start is called before the first frame update
    void Start()
    {
        _boss = GameObject.FindGameObjectWithTag("EnemyBoss");
        _playerObj = GameObject.Find("Player");
        _bossAI = _boss.GetComponent<BossAI>();
        _projectileContainer = GameObject.Find("Enemy_Projectiles");

        if (_boss == null)
        {
            Debug.LogError("Shield drone boss object reference is NULL!");
        }
        if (_playerObj == null)
        {
            Debug.LogError("Shield drone player object reference is NULL!");
        }
        if (_bossAI == null)
        {
            Debug.LogError("Shield drone boss AI reference is NULL!");
        }
        if (_projectileContainer == null)
        {
            Debug.LogError("Shield drone projectile container reference is NULL!");
        }

        StartCoroutine(OnSpawn());
        _canFire = Time.time + 2.0f;
    }

    // Update is called once per frame
    void Update()
    {
        CaclulateLookAngle();

        if (_isRotating == true && _isDead == false)
        {
            transform.RotateAround(_boss.transform.position, Vector3.forward, _rotationSpeed * Time.deltaTime);
            _spriteController.transform.rotation = Quaternion.Euler(Vector3.back * _lookAngle);
        }    
        
        if (Time.time > _canFire && transform.position.y < 1.0f)
        {
            StartCoroutine(FireLaser());
        }
    }

    private void CaclulateLookAngle()
    {
        if (_playerObj != null)
        {
            Vector3 targetPos = _playerObj.transform.position - transform.position;
            _lookAngle = Mathf.Atan2(targetPos.x, targetPos.y) * Mathf.Rad2Deg - 180;
        }
    }

    private void OnDestroy()
    {
        ShieldDrone.RotateFaster();
        ShieldDrone._killedDrones++;
        _bossAI.DroneDestroyed();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Laser"))
        {
            Laser hitBy = other.GetComponent<Laser>();
            hitBy.HitEnemy();

            Destroy(this.gameObject);
        }
    }

    IEnumerator OnSpawn()
    {
        while (transform.position.y > 1)
        {
            transform.Translate(Vector3.down * _speed * Time.deltaTime);
            yield return null;
        }
        _isRotating = true;
    }

    IEnumerator FireLaser()
    {
        _canFire = Time.time + _fireRate;

        for (int i = 0; i <= _killedDrones; i++)
        {
            float fireAngle = _spriteController.transform.eulerAngles.z + 180;
            Instantiate(_orbLaser, transform.position, Quaternion.Euler(Vector3.forward * fireAngle));
            Instantiate(_orbLaser, transform.position, Quaternion.Euler(Vector3.forward * (fireAngle - 30)));
            Instantiate(_orbLaser, transform.position, Quaternion.Euler(Vector3.forward * (fireAngle + 30)));
            yield return new WaitForSeconds(_fireRate);
        }
    }

    public void SetID(int id)
    {
        _droneID = id;
    }

    public static void RotateFaster()
    {
        _rotationSpeed += 18;
    }
}
