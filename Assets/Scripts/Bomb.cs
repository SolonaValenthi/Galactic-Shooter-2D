using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    private float _rotationSpeed;

    // Start is called before the first frame update
    void Start()
    {
        _rotationSpeed = Random.Range(-1.5f, 1.5f);
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(0, 0, _rotationSpeed);
    }
}
