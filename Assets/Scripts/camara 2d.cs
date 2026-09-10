using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camara2D : MonoBehaviour
{
    public Transform objetivo;
    public Vector3 offset = new Vector3(0f, 0f, -10f);
    void LateUpdate()
    {
        if (objetivo != null)
        {
            Vector3 posicionDeseada = objetivo.position + offset;
            transform.position = posicionDeseada;
        }
    }
}
