using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    [SerializeField]
    private int _damage = 1;
    public int Damage { get { return _damage; } }
    [SerializeField]
    private float _speed = 10f;
    [SerializeField]
    private GameObject _laserContainer;
    [SerializeField]
    private bool _isEnemyLaser = false;
    public bool IsEnemyLaser { get { return _isEnemyLaser; } }
    [SerializeField]
    private bool _isDestroyedOnContact = true;

    
    [SerializeField]
    private GameObject _prefabExplosion;
    private SpriteRenderer _renderer;
    public bool IsDestroyedOnContact { get { return _isDestroyedOnContact; } }
    private bool _movesDown = false;

   
    
    private void Start()
    {

        _renderer = GetComponent<SpriteRenderer>();
       
      
        
    }
    // Update is called once per frame
    void Update()
    {

        CalculateMovement();
        
    }
    void CalculateMovement() 
    {
        if (!IsEnemyLaser)
        {
            transform.Translate(Vector3.up * _speed * Time.deltaTime);

            if (transform.position.y > 10f)
            {

                Destroy(gameObject);

            }
        }
        else
        {
            transform.Translate(Vector3.down * _speed * Time.deltaTime);

            if (transform.position.y < -10f)
            {

                Destroy(gameObject);

            }
        }

    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player") 
        {
            Player player = other.GetComponent<Player>();
            if (player != null && _isEnemyLaser) 
            {
                player.Damage(1);
                Destroy(gameObject);
            }
            
        }
        if (other.tag == "Sensor") 
        {
            Physics2D.IgnoreCollision(other,GetComponent<Collider2D>());
        }
        
    }
    public void SetSpeed(float speed) 
    {
        _speed = speed; 
    }
    public void MakeEnemyLaser() 
    {
        _isEnemyLaser = true;
    }
    
    private void OnTriggerEnter(Collider other)
    {
        
    }
    public void BlowUp() 
    {
        _speed = 0;
        GameObject explosion = Instantiate(_prefabExplosion, transform.position, Quaternion.identity);
        _renderer.enabled = false;
        Destroy(gameObject);
        //gameObject.SetActive(false);
        //Destroy(explosion, .5f);
    }
    IEnumerator WaitABit(float seconds) 
    {
        yield return new WaitForSeconds(seconds);
    }
    private void OnDisable()
    {
        if (false)
        {
            BlowUp();
        }
    }
}
