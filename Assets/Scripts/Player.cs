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
        // Fetch horizontal input from the input manager
        float horizontalInput = Input.GetAxis("Horizontal");
        // Fetch vertical input from the input manager
        float verticalInput = Input.GetAxis("Vertical");
        // Vector3 to store player movement input
        Vector3 direction = new Vector3(horizontalInput, verticalInput, 0);

        // Vector3(playerinput, playerInput, 0) * 3.5f * real time
        transform.Translate(direction * _speed * Time.deltaTime);

        // Restrict player movement on y axis
        if (transform.position.y >= 2)
        {
            transform.position = new Vector3(transform.position.x, 2, 0);
        }
        else if (transform.position.y <= -3.8f)
        {
            transform.position = new Vector3(transform.position.x, -3.8f, 0);
        }

        // Wrap player to other side when moving off-screen
        if (transform.position.x > 11.3f)
        {
            transform.position = new Vector3(-11.3f, transform.position.y, 0);
        }
        else if (transform.position.x < -11.3f)
        {
            transform.position = new Vector3(11.3f, transform.position.y, 0);
        }
    }
}
