using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    private float _speed = 10f;
 
    // Start is called before the first frame update
    void Start()
    {
        //take the current position = starting position(0,0,0)
        transform.position = new Vector3(0, 0, 0);
        Debug.Log(Screen.width + "," + Screen.height);
    }

    // Update is called once per frame
    void Update()
    {
        Calculate_Movement();
    }
    private void Calculate_Movement()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        float upperbounds = 0,
                lowerbounds = -5.02f,
                leftbounds = -11.88f,
                rightbounds = 11.88f;

        Vector3 direction = new Vector3(horizontalInput, verticalInput, 0);
        transform.Translate(direction * _speed * Time.deltaTime);

        transform.position = new Vector3(transform.position.x, Mathf.Clamp(transform.position.y, lowerbounds, upperbounds));
        if (transform.position.y > upperbounds)
        {
            transform.position = new Vector3(transform.position.x, 0, 0);
        }
        if (transform.position.y < lowerbounds)
        {
            transform.position = new Vector3(transform.position.x, lowerbounds, 0);
        }
        if (transform.position.x < leftbounds)
        {
            transform.position = new Vector3(rightbounds, transform.position.y, 0);
        }
        if (transform.position.x > rightbounds)
        {
            transform.position = new Vector3(leftbounds, transform.position.y, 0);
        }
    }
}
