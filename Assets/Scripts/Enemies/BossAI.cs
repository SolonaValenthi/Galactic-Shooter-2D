using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAI : MonoBehaviour
{
    [SerializeField]
    private GameObject[] _turrets; // turrets are left to right, index 0 is left, index 3 is right
    [SerializeField]
    private GameObject _basicLaser;
    [SerializeField]
    private GameObject _piercingLaser;
    [SerializeField]
    private GameObject _orbLaser;
    [SerializeField]
    private GameObject _orbSpread;

    private float _bossSpeed = 2.0f;
    private float[] _turretFireAngles = new float[4];
    private int _selectedTurret1;
    private int _selectedTurret2;
    private bool _isAttacking = false;
    private GameObject _playerObj;
    private GameObject _projectileContainer;
    private GameManager _gameManager;

    // Start is called before the first frame update
    void Start()
    {
        _playerObj = GameObject.Find("Player");
        _projectileContainer = GameObject.Find("Enemy_Projectiles");
        _gameManager = GameObject.Find("Game_Manager").GetComponent<GameManager>();

        if (_playerObj == null)
        {
            Debug.LogError("Boss enemy player object reference is NULL!");
        }
        if (_projectileContainer == null)
        {
            Debug.LogError("Boss enemy projectile container reference is NULL!");
        }
        if (_gameManager == null)
        {
            Debug.LogError("Boss enemy game manager reference is NULL!");
        }

        _gameManager.BossFight();
    }

    // Update is called once per frame
    void Update()
    {
        CalculateFireAngle();
        
        if (transform.position.y > 5.0f)
        {
            transform.Translate(Vector3.down * Time.deltaTime * _bossSpeed);
        }

        if (Input.GetKeyDown(KeyCode.Alpha1) && _isAttacking == false)
        {
            StartCoroutine(BasicRapidFire());
        }

        if (Input.GetKeyDown(KeyCode.Alpha2) && _isAttacking == false)
        {
            StartCoroutine(OrbShotgun());
        }
    }

    // randomly select two of the boss' turrets
    private void TwoTurretSelect()
    {
        _selectedTurret1 = Random.Range(0, 4);
        _selectedTurret2 = Random.Range(0, 4);
        while (_selectedTurret2 == _selectedTurret1)
        {
            _selectedTurret2 = Random.Range(0, 4);
        }    
    }

    private void CalculateFireAngle()
    {
        if (_playerObj != null)
        {
            int currentIndex = 0;
            foreach (var turret in _turrets)
            {
                Vector3 targetPos = _playerObj.transform.position - turret.transform.position;
                _turretFireAngles[currentIndex] = Mathf.Atan2(targetPos.y, targetPos.x) * Mathf.Rad2Deg - 90;
                currentIndex++;
            }
        }
    }

    private void OnDestroy()
    {
        _gameManager.OnBossDeath();
    }

    // rapid fire from two randomly selected turrets
    IEnumerator BasicRapidFire()
    {
        _isAttacking = true;
        TwoTurretSelect();
        int shotsToFire = Random.Range(20, 31);
        GameObject newLaser;

        for (int i = 0; i < shotsToFire; i++)
        {
            float shotVariance = Random.Range(-15f, 15f);
            newLaser = Instantiate(_basicLaser, _turrets[_selectedTurret1].transform.position, Quaternion.Euler(Vector3.forward * (_turretFireAngles[_selectedTurret1] + shotVariance)));
            newLaser.transform.parent = _projectileContainer.transform;
            newLaser = Instantiate(_basicLaser, _turrets[_selectedTurret2].transform.position, Quaternion.Euler(Vector3.forward * (_turretFireAngles[_selectedTurret2] + shotVariance)));
            newLaser.transform.parent = _projectileContainer.transform;
            yield return new WaitForSeconds(0.1f);
        }

        _isAttacking = false;
    }

    // fire "shotgun" blasts from outer turrets
    IEnumerator OrbShotgun()
    {
        _isAttacking = true;
        GameObject newBurst;

        for (int i = 0; i < 5; i++)
        {
            newBurst = Instantiate(_orbSpread, _turrets[0].transform.position, Quaternion.identity);
            newBurst.transform.parent = _projectileContainer.transform;
            newBurst = Instantiate(_orbSpread, _turrets[3].transform.position, Quaternion.identity);
            newBurst.transform.parent = _projectileContainer.transform;
            yield return new WaitForSeconds(0.75f);
        }

        _isAttacking = false;
    }
}
