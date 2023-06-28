using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Powerup_Container : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> _powerups = new List<GameObject>();

    private int _powerupCount = 0;
    public int PowerupCount { get { return _powerupCount; } }

    public void addPowerup(GameObject powerup) 
    {
        _powerupCount++;
        _powerups.Add(powerup);
    }
    public void RemovePowerup(GameObject powerup) 
    {
        _powerupCount--;
        _powerups.Remove(powerup);
    }
}
