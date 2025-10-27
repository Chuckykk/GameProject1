using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth2D : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private int maxHP = 3;              
    private int _hp;                                     

    [Header("Death Effect")]
    [SerializeField] private GameObject explosionPrefab; 

    private void Awake()
    {
        _hp = maxHP; // Fyll HP vid start
    }

    // Tar skada
    public void TakeDamage(int dmg)
    {
        _hp -= dmg;

        // Hitta ev. "flash" effekt på träff
        var flash = GetComponentInChildren<EnemyFlashOnHit>();
        if (flash != null)
            flash.Flash();

        // Om HP slut -> dö
        if (_hp <= 0)
            Die();
    }

    private void Die()
    {
        
        if (explosionPrefab != null)
        {
            Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        }

        
        Destroy(gameObject);
    }
}
