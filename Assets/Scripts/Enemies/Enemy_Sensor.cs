using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Sensor : MonoBehaviour
{
    private bool _playerDetected = false;
    public bool PlayerDetected { get { return _playerDetected; } }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player") 
        {
            _playerDetected = true;
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        _playerDetected = false;
    }
}
