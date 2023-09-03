using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Powerup : MonoBehaviour
{
    [SerializeField]
    private float _moveSpeed = 3.0f;
    [SerializeField] // 0 = triple shot, 1 = speed boost, 2 = shields, 3 = ammo, 4 = health
    private int _powerupID;
    [SerializeField]
    private AudioManager _audioManager;

    private void Start()
    {
        _audioManager = GameObject.Find("Audio_Manager").GetComponent<AudioManager>();

        if (_audioManager == null)
        {
            Debug.LogError("Powerup audio manager reference is NULL!");
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
                        player.ActivateSpeedBoost();
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
                    default:
                        Debug.LogError("Invalid ID assigned");
                        break;
                }
            }
            _audioManager.PowerUp();   
            Destroy(this.gameObject);
        }
    }
}
