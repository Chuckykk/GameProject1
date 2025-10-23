using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet2D : MonoBehaviour
{
    [Header("Bullet Settings")]
    [SerializeField] private float lifetime = 2f;          // // Hur länge kulan lever
    [SerializeField] private int damage = 1;               // // Skadan kulan gör

    private void Start()
    {
        Destroy(gameObject, lifetime);                     // // Ta bort kulan efter X sekunder
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // // Kolla om vi träffade något med tag "Enemy"
        if (collision.CompareTag("Enemy"))
        {
            // // Hämta fiendens script (om det finns)
            EnemyHealth2D enemy = collision.GetComponent<EnemyHealth2D>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);                  // // Gör skada
            }

            Destroy(gameObject);                           // // Förstör kulan när den träffar
        }
    }
}
