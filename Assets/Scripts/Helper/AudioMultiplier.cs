using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioMultiplier : MonoBehaviour
{

    private AudioSource _audioSource;
    [SerializeField]
    private float volumeMultiplier = 2.0f;
    // Start is called before the first frame update
    void Start()
    {
        if (_audioSource == null) 
        {
            _audioSource = GetComponent<AudioSource>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        _audioSource.volume *= volumeMultiplier;
    }
}
