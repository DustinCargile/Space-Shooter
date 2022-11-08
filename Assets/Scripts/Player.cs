using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    private float _speed = 10f;

    [SerializeField]
    private GameObject _prefabLaser;
    private Vector3 _laserOffset = new Vector3(0, .7f, 0);

    [SerializeField]
    private float _fireRate = 2f;
    private float _fireTimer = -1f;

    [SerializeField]
    private int _lives = 3;
 
    private SpawnManager _spawnManager;
    // Start is called before the first frame update
    void Start()
    {
        _spawnManager = FindObjectOfType<SpawnManager>();
        //take the current position = starting position(0,0,0)
        transform.position = new Vector3(0, 0, 0);
        Debug.Log(Screen.width + "," + Screen.height);
    }

    // Update is called once per frame
    void Update()
    {
        Calculate_Movement();

        
        if (Input.GetKeyDown(KeyCode.Space) && Time.time > _fireTimer)
        {
            FireLaser();
        }




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
    private void FireLaser() 
    {
        _fireTimer = Time.time + _fireRate;
        Instantiate(_prefabLaser, transform.position + _laserOffset, Quaternion.identity);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Enemy") 
        {

            Damage(1);
        }
    }

    public void Damage(int dmg) 
    {
        _lives--;
        if (_lives <= 0) 
        {
            Kill();
        }
    }
    public void Kill() 
    {
        //Communicate with SpawnManager
        if (_spawnManager != null)
        {
            _spawnManager.OnPlayerDeath();
        }
        else 
        {
            Debug.LogError("Spawn Manager did not load!");
        }
        Destroy(gameObject);
    }
}
