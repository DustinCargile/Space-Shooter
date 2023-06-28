using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrusterDamage : MonoBehaviour
{
    [SerializeField]
    private bool _isBoosted = false;


    public void BoostActivated() 
    {
        _isBoosted = true;
    }
    public void BoostDeactivated() 
    {
        _isBoosted=false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_isBoosted) 
        {
            if (other.tag == "Enemy")
            {
                IDamagable enemy = other.GetComponent<IDamagable>();
                enemy.Damage(1);
            }
        }
    }
}
