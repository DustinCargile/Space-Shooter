using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateZ : MonoBehaviour
{
    [SerializeField]
    private float _rotateSpeed = 5.0f;
    // Update is called once per frame
    void Update()
    {
        transform.Rotate(0, 0, _rotateSpeed * Time.deltaTime);
    }
}
