using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAI : MonoBehaviour
{
    private float _bossSpeed = 3.0f;
    private GameManager _gameManager;

    // Start is called before the first frame update
    void Start()
    {
        _gameManager = GameObject.Find("Game_Manager").GetComponent<GameManager>();
        
        if (_gameManager == null)
        {
            Debug.LogError("Boss enemy game manager reference is NULL!");
        }

        _gameManager.BossFight();
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.y > 5.0f)
        {
            transform.Translate(Vector3.down * Time.deltaTime);
        }
    }

    private void OnDestroy()
    {
        _gameManager.OnBossDeath();
    }
}
