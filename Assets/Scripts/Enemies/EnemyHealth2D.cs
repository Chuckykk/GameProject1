using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth2D : MonoBehaviour
{
    [SerializeField] private int maxHP = 3;    // // Start-HP
    [SerializeField] private GameObject deathVfx = null; // // (valfritt) partikeleffekt
    private int _hp;                           // // Privat aktuell HP

    private void Awake()
    {
        _hp = maxHP;                           // // Fyll på HP vid start
    }

    public void TakeDamage(int dmg)
    {
        _hp -= dmg;                            // // Minska HP
        if (_hp <= 0) Die();                   // // Död om <= 0
    }

    private void Die()
    {
        if (deathVfx) Instantiate(deathVfx, transform.position, Quaternion.identity); // // Effekt
        Destroy(gameObject);                   // // Ta bort fienden
    }
}
