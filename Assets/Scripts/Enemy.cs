using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    private float _speed = 5f;
    [SerializeField]
    private float _upperbound = 6.5f, 
                  _lowerbound = -6.5f;
    

    private float _xPos;
    // Start is called before the first frame update
    void Start()
    {
         getNewPos();
    }

    // Update is called once per frame
    void Update()
    {
      transform.Translate(-Vector3.up * _speed * Time.deltaTime);

        if (transform.position.y < _lowerbound) 
        {
            getNewPos();
        }
    }
    private void getNewPos() 
    {
        _xPos = Random.RandomRange(-9.5f, 9.5f);
        transform.position = new Vector3(_xPos, _upperbound, 0);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Laser") 
        {
            Destroy(gameObject);
        }
        if (other.tag == "Player") 
        {
            Destroy(gameObject);
        }
    }
}
