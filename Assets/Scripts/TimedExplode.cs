using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimedExplode : MonoBehaviour
{
    private Material _objectMat;
    [SerializeField]
    private float _timeInSeconds = 5f;
    private Timer _timer = new Timer();
    [SerializeField]
    private GameObject _prefabExplosion;
    private bool _hasExploded = false;
    [SerializeField]
    private AudioClip _tickSound;
    private AudioSource _audioSource;
    // Start is called before the first frame update
    void Start()
    {
        _objectMat = GetComponent<SpriteRenderer>().material;
        _timer.StartTimer();
        _audioSource = GetComponent<AudioSource>();
        StartCoroutine(Blink(_timeInSeconds));
    }

    // Update is called once per frame
    void Update()
    {
        _timer.incTimer();
        Debug.Log(_timer.FTimer);
        if (_timer.FTimer >= _timeInSeconds && !_hasExploded) 
        {
            Explode();
        }
    }

    IEnumerator Blink(float timeUntilExplode)
    {
        timeUntilExplode /= 2;
        Color32 originalColor = _objectMat.color;
        yield return new WaitForSeconds(.2f);
        while (true) 
        {
            
            _objectMat.color = Color.red;
            if (_tickSound != null) 
            {
                _audioSource.PlayOneShot(_tickSound,.2f);
            }
            yield return new WaitForSeconds(.05f);
            _objectMat.color = originalColor;
            yield return new WaitForSeconds(timeUntilExplode);
            timeUntilExplode /= 3;
        }
        
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player") 
        {
            Player player = other.gameObject.GetComponent<Player>();
            Explode();
            player.Damage(3);
        }
        if (other.tag == "Laser") 
        {
            Laser laser = other.gameObject.GetComponent<Laser>();
            if (!laser.IsEnemyLaser) 
            {
                Destroy(other.gameObject);
                Explode();
            }
        }
    }
    private void Explode() 
    {
        _hasExploded = true;
        Instantiate(_prefabExplosion,transform.position,Quaternion.identity);
        Destroy(gameObject);
    }
}
