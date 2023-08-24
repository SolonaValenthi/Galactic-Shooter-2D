using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField]
    private AudioClip _laserSFX;
    [SerializeField]
    private AudioClip _explosionSFX;
    [SerializeField]
    private AudioClip _powerUpSFX;

    AudioSource _gameAudio;

    // Start is called before the first frame update
    void Start()
    {
        _gameAudio = gameObject.GetComponent<AudioSource>();

        if (_gameAudio == null)
        {
            Debug.LogError("Audio Manager audio source reference is NULL!");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Explosion()
    {
        _gameAudio.PlayOneShot(_explosionSFX);
    }

    public void PowerUp()
    {
        _gameAudio.PlayOneShot(_powerUpSFX);
    }
}
