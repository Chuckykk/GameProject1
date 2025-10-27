using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAfterAnimation : MonoBehaviour
{
    public float lifetime = 1f; 
    void Start()
    {
        Destroy(gameObject, lifetime);
    }
}