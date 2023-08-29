using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDebris : MonoBehaviour
{
    [SerializeField]
    private int _debrisID; // 0 = left side of ship, 1 = top of ship, 2 = bottom of ship, 3 = right side of ship

    private float _rotationSpeed;
    private float _driftSpeed;
    private float _driftVariance;
    private Vector3 _driftDirection;
    
    // Start is called before the first frame update
    void Start()
    {
        _rotationSpeed = Random.Range(-8, 9);
        _driftSpeed = Random.Range(1.0f, 2.0f);
        _driftVariance = Random.Range(-1.0f, 1.0f);

        switch (_debrisID)
        {
            case 0:
                _driftDirection = new Vector3(-1, _driftVariance, 0);
                break;
            case 1:
                _driftDirection = new Vector3(_driftVariance, 1, 0);
                break;
            case 2:
                _driftDirection = new Vector3(_driftVariance, -1, 0);
                break;
            case 3:
                _driftDirection = new Vector3(1, _driftVariance, 0);
                break;
            default:
                Debug.LogError("Invalid Debris ID set");
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(_driftDirection * _driftSpeed * Time.deltaTime);
        transform.Rotate(0f, 0f, _rotationSpeed * Time.deltaTime);
    }
}
