using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    private Vector3 _originalPos;
    private Vector3 _bossCameraPos;

    // Start is called before the first frame update
    void Start()
    {
        _originalPos = transform.position;
    }

    public IEnumerator ShakeCamera(float duration, float magnitude)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float xShake = Random.Range(-1f, 1f) * magnitude;
            float yShake = Random.Range(-1f, 1f) * magnitude;

            transform.position = new Vector3(xShake, yShake, _originalPos.z);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = _originalPos;
    }

    public IEnumerator BossCameraShift()
    {
        while (transform.position.z > -15.0f)
        {
            transform.Translate(Vector3.back * Time.deltaTime);
            yield return null;
        }
    }
}
