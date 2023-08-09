using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    // Initialize speed variable
    [SerializeField]
    private float _speed = 3.5f;

    // Start is called before the first frame update
    void Start()
    {
        // Set current position = new position (0, 0, 0)
        transform.position = new Vector3(0, 0, 0);
    }

    // Update is called once per frame
    void Update()
    {
        // fetch horizontal input from the input manager
        float horizontalInput = Input.GetAxis("Horizontal");
        // fetch vertical input from the input manager
        float verticalInput = Input.GetAxis("Vertical");
        // Vector3 to store player movement input
        Vector3 direction = new Vector3(horizontalInput, verticalInput, 0);

        // new Vector3(1, 0, 0) * player input * 5 * real time
        //transform.Translate(Vector3.right * horizontalInput * _speed * Time.deltaTime);
        // new Vector3(0, 1, 0) * player input * 5 * real time
        //transform.Translate(Vector3.up * verticalInput * _speed * Time.deltaTime);
        transform.Translate(direction * _speed * Time.deltaTime);
    }
}
