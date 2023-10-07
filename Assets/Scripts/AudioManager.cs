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
    [SerializeField]
    private AudioSource _BGM;

    AudioSource _gameAudio;

    // Start is called before the first frame update
    void Start()
    {
        _gameAudio = gameObject.GetComponent<AudioSource>();

        if (_gameAudio == null)
        {
            Debug.LogError("Audio Manager audio source reference is NULL!");
        }

        StartCoroutine(FadeMusicIn(_BGM));
    }

    public void Explosion()
    {
        _gameAudio.PlayOneShot(_explosionSFX);
    }

    public void PowerUp()
    {
        _gameAudio.PlayOneShot(_powerUpSFX);
    }

    IEnumerator FadeMusicIn(AudioSource fadeTarget)
    {
        while (fadeTarget.volume < 1.0f)
        {
            fadeTarget.volume += 0.05f;
            yield return new WaitForSeconds(0.05f);
        }
    }
}
