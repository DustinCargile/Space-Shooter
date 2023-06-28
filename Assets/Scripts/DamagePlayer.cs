using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamagePlayer : MonoBehaviour
{
    [SerializeField]
    private int _damage = 3;
    [SerializeField]
    private bool _hasDamage = true;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player" && _hasDamage) 
        {
            Player player = other.GetComponent<Player>();
            player.Damage(_damage);
            Debug.Log("Collided with Player!");
        }
    }
}
