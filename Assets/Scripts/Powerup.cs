using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

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

    private Powerup_Container _container;

    [SerializeField]
    private float _spawnWeight;
    public float SpawnWeight { get { return _spawnWeight; } }

    private AudioSource _audioSource;

    [SerializeField]
    private AudioClip _powerupSound, _destroySound;
    [SerializeField]
    private GameObject _prefabDestroyAnim;

    [SerializeField]
    private GameObject _gravityVisualizer;
    private GravityArrow _activeGV;
    private bool _hasGravityConnection = false;

    private Player _player;
    private float _distanceToPlayer;


    // Start is called before the first frame update
    void Start()
    {
        _audioSource = GetComponent<AudioSource>();
        _container = FindObjectOfType<Powerup_Container>();
        _container.addPowerup(gameObject);
        _player = FindObjectOfType<Player>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_player != null) 
        {
            _distanceToPlayer = Vector3.Distance(_player.transform.position, transform.position);

            if (_hasGravityConnection && _player != null && _distanceToPlayer <= 8)
            {
                transform.position = Vector3.MoveTowards(transform.position,
                    _player.transform.position,
                    _speed * (8 - _activeGV.Distance) * Time.deltaTime);
            }
            else
            {
                transform.Translate(Vector2.down * Time.deltaTime * _speed);
            }
        }

        if (transform.position.y <= -10f) 
        {
            Kill(); ;
        }


        //temporary
        if (Input.GetKey(KeyCode.C) && !_hasGravityConnection && _distanceToPlayer < 8) 
        {
            _hasGravityConnection = true;
            GameObject temp = Instantiate(_gravityVisualizer, transform);


            _activeGV = temp.GetComponent<GravityArrow>();
            _activeGV.AttachToPowerup();          

        }
        if (Input.GetKeyUp(KeyCode.C) || _distanceToPlayer >=8) 
        {
            if (_activeGV != null) 
            {
                _hasGravityConnection = false;
                Destroy(_activeGV.gameObject);
            }
        }
        
        //
    }

    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player") 
        {
            if (_audioSource != null) 
            {
                AudioSource.PlayClipAtPoint(_powerupSound,new Vector3(0,0,-10)); //set to the camera position
            }
            Player player = other.gameObject.GetComponent<Player>();

            switch (_powerupID) 
            {
                case -1:
                    player.SetShieldInactive();
                    break;
                case 0:
                    player.setTripleShot_Enabled();
                    
                    break;
                case 1:
                    player.AddSpeedCoolDown(5f);
                    
                    break;
            
                case 2:
                    player.SetShieldActive();
                    
                    break;
                case 3:
                    player.RefillAmmo();
                    break;
                case 4:
                    player.Heal(1);
                    break;
                case 5:
                    player.SetVulcanActive();
                    break;
                case 6:
                    player.AddRockets();
                    break;
                default:
                    Debug.Log("Powerup ID not found");
                    break;
            }
            Kill();
        }
        if (other.tag == "Laser" && _powerupID >= 0) 
        {
            Laser laser = other.gameObject.GetComponent<Laser>();
            if (laser.IsEnemyLaser) 
            {
                Kill();
                PlayDestroySound();
            }
                
        }
        }

    private void Kill() 
    {
        _container.RemovePowerup(gameObject);
        
        Instantiate(_prefabDestroyAnim,transform.position,Quaternion.identity);
        Destroy(gameObject);
    }
    private void PlayDestroySound() 
    {
        if (_audioSource != null)
        {
            AudioSource.PlayClipAtPoint(_destroySound, new Vector3(0, 0, -10)); //set to the camera position
        }
    }

    }

    

