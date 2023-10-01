using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldDrone : MonoBehaviour
{
    [SerializeField]
    private GameObject _basicLaser;
    [SerializeField]
    private GameObject _spriteController;

    private float _speed = 4;
    private float _fireAngle;
    private bool _isRotating = false;
    private GameObject _boss;
    private GameObject _playerObj;
    private BossAI _bossAI;

    // Start is called before the first frame update
    void Start()
    {
        _boss = GameObject.FindGameObjectWithTag("EnemyBoss");
        _playerObj = GameObject.Find("Player");
        _bossAI = _boss.GetComponent<BossAI>();

        if (_boss == null)
        {
            Debug.LogError("Shield drone boss object reference is NULL!");
        }
        if (_bossAI == null)
        {
            Debug.LogError("Shield drone boss AI reference is NULL!");
        }

        StartCoroutine(OnSpawn());
    }

    // Update is called once per frame
    void Update()
    {
        CaclulateFireAngle();

        if (_isRotating == true)
        {
            transform.RotateAround(_boss.transform.position, Vector3.forward, 36 * Time.deltaTime);
            _spriteController.transform.rotation = Quaternion.Euler(Vector3.back * _fireAngle);
        }       
    }

    private void CaclulateFireAngle()
    {
        Vector3 targetPos = _playerObj.transform.position - transform.position;
        _fireAngle = Mathf.Atan2(targetPos.x, targetPos.y) * Mathf.Rad2Deg - 180;
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
}
