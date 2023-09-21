using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Powerup : MonoBehaviour
{
    [SerializeField]
    private float _moveSpeed = 3.0f;
    [SerializeField] // 0 = triple shot, 1 = fuel, 2 = shields, 3 = ammo, 4 = health, 5 = bomb alt fire, 6 = missiles, 7 = jamming (negative)
    private int _powerupID;
    [SerializeField]
    private AudioManager _audioManager;

    private PowerupDetection _detectedEnemy;
    private Color _powerupColor;

    SpriteRenderer _powerupSprite;

    private void Start()
    {
        _audioManager = GameObject.Find("Audio_Manager").GetComponent<AudioManager>();
        _powerupSprite = gameObject.GetComponent<SpriteRenderer>();
        _powerupColor = _powerupSprite.color;

        if (_audioManager == null)
        {
            Debug.LogError("Powerup audio manager reference is NULL!");
        }
        if (_powerupSprite == null)
        {
            Debug.LogError("Powerup sprite renderer reference is NULL!");
        }

        // bomb color change
        if (_powerupID == 5)
        {
            StartCoroutine(BombColorChange());
        }
        // missile color change
        if (_powerupID == 6)
        {
            StartCoroutine(MissileColorChange());
        }
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.down * _moveSpeed * Time.deltaTime);
        if (transform.position.y < -7)
        {
            Destroy(this.gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Player player = other.GetComponent<Player>();

            if (player != null)
            {
                switch (_powerupID)
                {
                    case 0:
                        player.ActivateTripleShot();
                        break;
                    case 1:
                        player.ReplenishFuel();
                        break;
                    case 2:
                        player.ActivateShield();
                        break;
                    case 3:
                        player.ActivateInfinAmmo();
                        break;
                    case 4:
                        player.HealPlayer();
                        break;
                    case 5:
                        player.BombsReady();
                        break;
                    case 6:
                        player.LoadMissiles(2); // the player may hold up to 5 missiles
                        break;
                    case 7:
                        break;
                    default:
                        Debug.LogError("Invalid ID assigned");
                        break;
                }
            }
            _audioManager.PowerUp();   
            Destroy(this.gameObject);
        }

        if (other.CompareTag("PowerupDetection"))
        {
            _detectedEnemy = other.GetComponent<PowerupDetection>();
        }

        if (other.CompareTag("EnemyLaser"))
        {
            Destroy(other.gameObject);
            Destroy(this.gameObject);
        }
    }

    private void OnDestroy()
    {
        if (_detectedEnemy != null)
        {
            _detectedEnemy.ClearTarget();
        }
    }

    IEnumerator BombColorChange()
    {
        while (true)
        {
            while (_powerupColor.g < 0.8f)
            {
                _powerupColor.g += 0.06f;
                _powerupSprite.color = _powerupColor;
                yield return new WaitForSeconds(0.1f);
            }
            while (_powerupColor.g > 0.4f)
            {
                _powerupColor.g -= 0.06f;
                _powerupSprite.color = _powerupColor;
                yield return new WaitForSeconds(0.1f);
            }
        }
    }

    IEnumerator MissileColorChange()
    {
        while (true)
        {
            while (_powerupColor.b < 1.0f)
            {
                _powerupColor.b += 0.06f;
                _powerupSprite.color = _powerupColor;
                yield return new WaitForSeconds(0.1f);
            }
            while (_powerupColor.b > 0f)
            {
                _powerupColor.b -= 0.06f;
                _powerupSprite.color = _powerupColor;
                yield return new WaitForSeconds(0.1f);
            }
        }
    }
}
