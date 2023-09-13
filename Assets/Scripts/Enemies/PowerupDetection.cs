using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerupDetection : MonoBehaviour
{
    private bool _hasTarget = false;
    private GameObject _powerupTarget;
    private Enemy _enemyParent;

    // Start is called before the first frame update
    void Start()
    {
        _enemyParent = transform.parent.GetComponent<Enemy>();

        if (_enemyParent == null)
        {
            Debug.LogError("Powerup Detection enemy script reference is NULL!");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (_hasTarget == true)
        {
            _enemyParent.AttackPowerup(_powerupTarget.transform.position);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("PowerUp"))
        {
            if (_powerupTarget == null)
            {
                _powerupTarget = other.gameObject;
                _hasTarget = true;
            }
        }
    }

    public void ClearTarget()
    {
        _powerupTarget = null;
        _hasTarget = false;
    }
}
