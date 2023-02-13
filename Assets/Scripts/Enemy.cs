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

    [SerializeField]
    private float _timeDelay = 0.3f;
    private float _xPos;

    private float _leftbound = -9.5f,
                  _rightbound = 9.5f;
    [SerializeField]
    private Player _player;
    // Start is called before the first frame update
    void Start()
    {
         _player = FindObjectOfType<Player>();
         getNewPos();
    }

    // Update is called once per frame
    void Update()
    {
        MoveDown();
    }
   /* private void MoveLeft() 
    {
        transform.Translate(-Vector3.left * _speed * Time.deltaTime);
        if (Mathf.Abs(transform.position.x) > _rightbound) 
        {
            getNewPos();
        }
    }
    private void MoveRight()
    {
        transform.Translate(-Vector3.right * _speed * Time.deltaTime);
        if (Mathf.Abs(transform.position.x) > _rightbound)
        {
            getNewPos();
        }
    }*/
    private void MoveDown() 
    {
        transform.Translate(Vector3.down * _speed * Time.deltaTime);
        if (transform.position.y < _lowerbound)
        {
            getNewPos();
        }

    }
    private void getNewPos() 
    {
        _xPos = Random.Range(-9.5f, 9.5f);
        transform.position = new Vector3(_xPos, _upperbound, 0);
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Laser") 
        {
            Destroy(other.gameObject);
            if (_player == null) { Debug.Log("Could not find Player Object!"); } else 
            { _player.AdjustScore(10); }
            Kill();
        }
        if (other.tag == "Player") 
        {
            Player player = other.GetComponent<Player>();
            player.Damage(1);
            Kill();
        }
    }
    private void Kill() 
    {
        Destroy(gameObject);
    }
}
