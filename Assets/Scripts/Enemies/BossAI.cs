using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAI : MonoBehaviour
{
    [SerializeField]
    private GameObject[] _turrets; // turrets are left to right, index 0 is left, index 3 is right
    [SerializeField]
    private GameObject[] _targetIndicators; // same indexing as turrets
    [SerializeField]
    private GameObject _basicLaser;
    [SerializeField]
    private GameObject _piercingLaser;
    [SerializeField]
    private GameObject _orbLaser;
    [SerializeField]
    private GameObject _orbSpread;
    [SerializeField]
    private GameObject _homingMissile;
    [SerializeField]
    private GameObject _explosion;

    private float _bossSpeed = 2.0f;
    private float _bossHealth = 200;
    private float[] _turretFireAngles = new float[4];
    private int _selectedTurret1;
    private int _selectedTurret2;
    private int _maxBossHealth = 200;
    private int _lastAttack = 4;
    private bool _canAttack = true;
    private GameObject _playerObj;
    private GameObject _projectileContainer;
    private GameManager _gameManager;
    private UIManager _uiManager;

    BoxCollider2D[] _bossColliders;

    // Start is called before the first frame update
    void Start()
    {
        _playerObj = GameObject.Find("Player");
        _projectileContainer = GameObject.Find("Enemy_Projectiles");
        _gameManager = GameObject.Find("Game_Manager").GetComponent<GameManager>();
        _uiManager = GameObject.Find("UI_Manager").GetComponent<UIManager>();
        _bossColliders = gameObject.GetComponents<BoxCollider2D>();

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
        if (_bossColliders == null)
        {
            Debug.LogError("Boss enemy collider reference is NULL!");
        }
        if (_uiManager == null)
        {
            Debug.LogError("Boss enemy UI manager reference is NULL!");
        }

        _gameManager.BossFight();
        StartCoroutine(IntroSequence());
    }

    // Update is called once per frame
    void Update()
    {
        CalculateFireAngle();     
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

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Laser"))
        {
            Damage(1);
            Destroy(other.gameObject);
        }
    }

    private void DeathSequence()
    {
        Debug.Log("Boss defeated!");
        StopAllCoroutines();
        _canAttack = false;

        foreach (var collider in _bossColliders)
        {
            collider.enabled = false;
        }
        Destroy(this.gameObject);
    }

    IEnumerator IntroSequence()
    {
        while (transform.position.y > 5.0f)
        {
            transform.Translate(Vector3.down * Time.deltaTime * _bossSpeed);
            yield return null;
        }
        yield return new WaitForSeconds(1.1f);
        foreach (var collider in _bossColliders)
        {
            collider.enabled = true;
        }

        StartCoroutine(SelectAttack(0.5f));
    }

    IEnumerator SelectAttack(float attackDelay)
    {
        yield return new WaitForSeconds(attackDelay);
        int selectedAttack = Random.Range(0, 4);

        while (selectedAttack == _lastAttack)
        {
            selectedAttack = Random.Range(0, 4);
        }
        if (_canAttack == true)
        {
            switch (selectedAttack)
            {
                case 0:
                    StartCoroutine(BasicRapidFire());
                    break;
                case 1:
                    StartCoroutine(OrbShotgun());
                    break;
                case 2:
                    StartCoroutine(MissileBarrage());
                    break;
                case 3:
                    StartCoroutine(PiercingRapidFire());
                    break;
                default:
                    Debug.LogError("Invalid attack ID selected");
                    break;
            }
        }

        _lastAttack = selectedAttack;
    }

    // rapid fire from two randomly selected turrets, attack ID = 0
    IEnumerator BasicRapidFire()
    {
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

        StartCoroutine(SelectAttack(1.0f));
    }

    // fire "shotgun" blasts from outer turrets, attack ID = 1
    IEnumerator OrbShotgun()
    {
        GameObject newBurst;

        for (int i = 0; i < 5; i++)
        {
            newBurst = Instantiate(_orbSpread, _turrets[0].transform.position, Quaternion.identity);
            newBurst.transform.parent = _projectileContainer.transform;
            newBurst = Instantiate(_orbSpread, _turrets[3].transform.position, Quaternion.identity);
            newBurst.transform.parent = _projectileContainer.transform;
            yield return new WaitForSeconds(0.75f);
        }

        StartCoroutine(SelectAttack(3.5f));
    }

    // fire several waves of homing missiles, attack ID = 2
    IEnumerator MissileBarrage()
    {
        GameObject newMissile;

        for (int i = 0; i < 3; i++)
        {
            newMissile = Instantiate(_homingMissile, _turrets[1].transform.position, Quaternion.Euler(Vector3.forward * 30));
            newMissile.transform.parent = _projectileContainer.transform;
            newMissile = Instantiate(_homingMissile, _turrets[2].transform.position, Quaternion.Euler(Vector3.forward * -30));
            newMissile.transform.parent = _projectileContainer.transform;
            yield return new WaitForSeconds(0.75f);
            newMissile = Instantiate(_homingMissile, _turrets[0].transform.position, Quaternion.Euler(Vector3.forward * 30));
            newMissile.transform.parent = _projectileContainer.transform;
            newMissile = Instantiate(_homingMissile, _turrets[3].transform.position, Quaternion.Euler(Vector3.forward * -30));
            newMissile.transform.parent = _projectileContainer.transform;
            yield return new WaitForSeconds(0.75f);
        }

        StartCoroutine(SelectAttack(2.0f));
    }

    // rapid fire a pierce laser from each turret, attack ID = 3
    IEnumerator PiercingRapidFire()
    {    
        for (int i = 0; i < 2; i++)
        {
            int activeTurret = 0;
            StartCoroutine(FirePierceLaser(activeTurret));
            activeTurret++;
            yield return new WaitForSeconds(0.5f);
            StartCoroutine(FirePierceLaser(activeTurret));
            activeTurret++;
            yield return new WaitForSeconds(0.5f);
            StartCoroutine(FirePierceLaser(activeTurret));
            activeTurret++;
            yield return new WaitForSeconds(0.5f);
            StartCoroutine(FirePierceLaser(activeTurret));
            yield return new WaitForSeconds(0.5f);
        }

        StartCoroutine(SelectAttack(1.5f));
    }

    IEnumerator FirePierceLaser(int turret)
    {
        GameObject newPierce;
        _targetIndicators[turret].SetActive(true);
        SpriteRenderer activeLaserSprite = _targetIndicators[turret].GetComponent<SpriteRenderer>();
        Color activeLaserColor = activeLaserSprite.color;
        _targetIndicators[turret].transform.rotation = Quaternion.Euler(Vector3.forward * _turretFireAngles[turret]);

        while(activeLaserColor.a < 1.0f)
        {
            activeLaserColor.a += 0.06f;
            activeLaserSprite.color = activeLaserColor;
            yield return new WaitForSeconds(0.05f);
        }
        for (int i = 0; i < 3; i++)
        {
            _targetIndicators[turret].SetActive(false);
            yield return new WaitForSeconds(0.1f);
            _targetIndicators[turret].SetActive(true);
            yield return new WaitForSeconds(0.1f);
        }

        newPierce = Instantiate(_piercingLaser, _turrets[turret].transform.position, _targetIndicators[turret].transform.rotation);
        newPierce.transform.parent = _projectileContainer.transform;
        activeLaserColor.a = 0.0f;
        activeLaserSprite.color = activeLaserColor;
        _targetIndicators[turret].SetActive(false);

        yield return null;
    }

    public void Damage(int damageTaken)
    {
        _bossHealth -= damageTaken;
        _uiManager.UpdateBossHealth(_bossHealth / _maxBossHealth);

        if (_bossHealth <= 0)
        {
            DeathSequence();
        }
    }
}
