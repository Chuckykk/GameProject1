using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    public int damage = 1;
    public float lifeTime = 3f;

    void Start()
    {
        Destroy(gameObject, lifeTime); // Försvinn efter några sekunder
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerHealth2D playerHealth = other.GetComponent<PlayerHealth2D>();
            if (playerHealth != null)
                playerHealth.TakeDamage(damage);

            Destroy(gameObject);
        }

        // Försvinn om du träffar något annat (vägg t.ex.)
        if (other.CompareTag("Wall"))
        {
            Destroy(gameObject);
        }
    }
}
