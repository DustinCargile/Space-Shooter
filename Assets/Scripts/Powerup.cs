using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Powerup : MonoBehaviour
{
    [SerializeField]
    private float _speed = 3f;

    //ID for Powerups
    //0 = Triple Shot
    //1 = Speed
    //2 = Shields
    [SerializeField]
    private int _powerupID;
   
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector2.down * Time.deltaTime * _speed);

        if (transform.position.y <= -10f) 
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player") 
        {
            
            Player player = other.gameObject.GetComponent<Player>();

            switch (_powerupID) 
            {
                case 0:
                    player.setTripleShot_Enabled();
                    Debug.Log("Collected Triple Shot");
                    break;
                case 1:
                    player.setSpeed_Enabled();
                    Debug.Log("Collected Speed");
                    break;
            
                case 2:
                    player.setShieldActive();
                    Debug.Log("Collected Shields");
                    break;
                default:
                    Debug.Log("Powerup ID not found");
                    break;
            }
            Destroy(gameObject);
        }
            
        }
    }

    

