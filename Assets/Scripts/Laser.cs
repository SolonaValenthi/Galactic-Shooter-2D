using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    [SerializeField]
    private float _speed = 8.0f;
    [SerializeField]
    private int _projectileID; // 0 = regular laser, 1 = piercing laser

    // Update is called once per frame
    void Update()
    {
        if (_projectileID == 0)
        {
            BasicLaser();
        }

    }

    private void BasicLaser()
    {
        transform.Translate(Vector3.up * _speed * Time.deltaTime);

        if (transform.position.y > 15)
        {
            if (transform.parent != null && transform.parent.tag != "Container")
            {
                Destroy(transform.parent.gameObject);
            }
            Destroy(this.gameObject);
        }
        else if (transform.position.y < -6)
        {
            if (transform.parent != null && transform.parent.tag != "Container")
            {
                Destroy(transform.parent.gameObject);
            }
            Destroy(this.gameObject);
        }
    }
}
