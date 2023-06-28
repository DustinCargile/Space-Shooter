using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sensor : MonoBehaviour
{
    private int _aggroLevel = 0;
    public int AggroLevel { get { return _aggroLevel; } }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player") 
        {
            _aggroLevel++;
            
            Debug.Log("Aggro Level: " + _aggroLevel);

        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == "Player") 
        {
            _aggroLevel--;
            if (_aggroLevel < 0) 
            {
                _aggroLevel = 0;
            }
            Debug.Log("Aggro Level: " + _aggroLevel);
        }
    }
}
