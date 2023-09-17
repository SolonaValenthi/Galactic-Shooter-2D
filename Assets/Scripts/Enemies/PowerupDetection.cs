using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerupDetection : MonoBehaviour
{
    private int _parentID; // 0 = basic enemy, 1 = ambush enemy
    private GameObject _powerupTarget;
    private GameObject _incomingLaser;
    private EnemyAmbush _ambushParent;
    private Enemy _enemyParent;

    // Start is called before the first frame update
    void Start()
    {
        if (transform.parent.name == "Enemy(Clone)")
        {
            _enemyParent = transform.parent.GetComponent<Enemy>();
            _parentID = 0;
        }
        if (transform.parent.name == "Enemy_Ambush(Clone)")
        {
            _ambushParent = transform.parent.GetComponent<EnemyAmbush>();
            _parentID = 1;
        }

        if (_enemyParent == null && _parentID == 0)
        {
            Debug.LogError("Powerup Detection parent script reference is NULL!");
        }
        if (_ambushParent == null && _parentID == 1)
        {
            Debug.LogError("Powerup Detection parent script reference is NULL!");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (_powerupTarget != null && _parentID == 0)
        {
            _enemyParent.AttackPowerup(_powerupTarget.transform.position);
        }

        if (_incomingLaser != null && _parentID == 1)
        {
            _ambushParent.IncomingLaser(_incomingLaser.transform.position);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("PowerUp"))
        {
            if (_powerupTarget == null)
            {
                _powerupTarget = other.gameObject;
            }
        }

        if (other.CompareTag("Laser"))
        {
            if (_incomingLaser == null)
            {
                _incomingLaser = other.gameObject;
                StartCoroutine(_ambushParent.AfterDodge());
            }
        }
    }

    public void ClearTarget()
    {
        _powerupTarget = null;
        _incomingLaser = null;
    }
}
