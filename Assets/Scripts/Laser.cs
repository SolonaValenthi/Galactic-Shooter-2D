using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    [SerializeField]
    private float _speed = 8.0f;
    [SerializeField]
    private int _projectileID; // 0 = regular laser, 1 = piercing laser, 2 = giga laser
    [SerializeField]
    private bool _playerLaser = false;

    private int _sweepDirection; // 0 = right-left, 1 = left-right
    private float _sweepSpeed = 30.0f;

    BoxCollider2D _laserCollider;

    private void Start()
    {
        _laserCollider = gameObject.GetComponent<BoxCollider2D>();

        if (_projectileID == 1)
        {
            StartCoroutine(PiercingLaser());
        }

        if (_projectileID == 2)
        {
            StartCoroutine(GigaDamage());
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (_projectileID == 0)
        {
            BasicLaser();
        }
        
        if (_projectileID == 2)
        {
            GigaLaser();
        }
    }

    private void BasicLaser()
    {
        transform.Translate(Vector3.up * _speed * Time.deltaTime);

        CullLasers();
    }

    private void CullLasers()
    {
        if (transform.position.y > 15)
        {
            if (_playerLaser == false)
            {
                if (transform.parent != null && transform.parent.tag != "Container")
                {
                    Destroy(transform.parent.gameObject);
                }
                Destroy(this.gameObject);
            }
            else
            {
                gameObject.SetActive(false);
            }
        }
        else if (transform.position.y < -20)
        {
            if (_playerLaser == false)
            {
                if (transform.parent != null && transform.parent.tag != "Container")
                {
                    Destroy(transform.parent.gameObject);
                }
                Destroy(this.gameObject);
            }
            else
            {
                gameObject.SetActive(false);
            }
        }
    }

    public void HitEnemy()
    {
        if (_playerLaser == false)
        {
            Destroy(this.gameObject);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    private void GigaLaser()
    {
        if (_sweepDirection == 0)
        {
            transform.Rotate(Vector3.back * _sweepSpeed * Time.deltaTime);
        }
        else if (_sweepDirection == 1)
        {
            transform.Rotate(Vector3.forward * _sweepSpeed * Time.deltaTime);
        }
    }

    public void SetSweep(Vector3 playerPos)
    {
        if (playerPos.x >= 0)
        {
            _sweepDirection = 0;
            transform.rotation = Quaternion.Euler(Vector3.forward * 240);
        }
        else if (playerPos.x < 0)
        {
            _sweepDirection = 1;
            transform.rotation = Quaternion.Euler(Vector3.forward * 120);
        }
    }

    IEnumerator PiercingLaser()
    {
        yield return new WaitForSeconds(0.05f);
        _laserCollider.enabled = true;
        yield return new WaitForSeconds(0.2f);
        Destroy(this.gameObject);
    }

    IEnumerator GigaDamage()
    {
        Destroy(this.gameObject, 4.0f);
        while (true)
        {
            yield return new WaitForSeconds(0.05f);
            _laserCollider.enabled = false;
            yield return new WaitForSeconds(0.05f);
            _laserCollider.enabled = true;
        }
    }
}
