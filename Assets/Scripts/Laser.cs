using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    [SerializeField]
    private float _speed = 10f;
    
    // Update is called once per frame
    void Update()
    {
        
        transform.Translate(Vector3.up * _speed * Time.deltaTime);
      
        if (transform.position.y > 6f) 
        {
            Destroy(gameObject);
        }
        
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Enemy") 
        {
            Destroy (gameObject);
        }
    }
}
