using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossCentralTurret : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(RotateTurret());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator RotateTurret()
    {
        float elapsed;
        while (true)
        {
            elapsed = 0.0f;
            while (elapsed < 6.0f)
            {
                transform.Rotate(Vector3.back * Time.deltaTime * 10);
                elapsed += Time.deltaTime;
                yield return null;
            }
            elapsed = 0.0f;
            while (elapsed < 6.0f)
            {
                transform.Rotate(Vector3.forward * Time.deltaTime * 10);
                elapsed += Time.deltaTime;
                yield return null;
            }
        }
    }
}
