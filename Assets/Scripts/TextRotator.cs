using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextRotator : MonoBehaviour
{
    private Transform _cam;

    #region MONO
    private void Awake()
    {
        _cam = Camera.main.transform;
    }
    #endregion

    private void Update()
    {
        transform.LookAt(_cam, Vector3.up);
    }
}
