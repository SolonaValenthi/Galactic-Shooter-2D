using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    private AudioManager _audioManager;

    // Start is called before the first frame update
    void Start()
    {
        _audioManager = GameObject.Find("Audio_Manager").GetComponent<AudioManager>();

        if (_audioManager == null)
        {
            Debug.LogError("Explosion audio manager referece is NULL!");
        }

        _audioManager.Explosion();
        Destroy(this.gameObject, 2.37f);
    }
}
