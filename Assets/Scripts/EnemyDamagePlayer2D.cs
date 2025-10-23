using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDamagePlayer2D : MonoBehaviour
{
    [SerializeField] private int contactDamage = 1;   // // Hur mycket skada spelaren tar
    [SerializeField] private float hitCooldown = 0.4f; // // Cooldown så man inte får skada 10 ggr på en sekund

    private float _nextHitTime = 0f;                  // // Tid för nästa tillåtna träff

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (Time.time < _nextHitTime) return;         // // Om cooldown inte gått ut → gör inget

        if (collision.collider.CompareTag("Player"))  // // Kolla om vi träffar spelaren
        {
            // // Försök hämta spelarens HP-script
            if (collision.collider.TryGetComponent<PlayerHealth2D>(out var hp))
            {
                hp.TakeDamage(contactDamage);          // // Spelaren tar skada
            }

            // // Fienden dör direkt efter träffen
            EnemyHealth2D self = GetComponent<EnemyHealth2D>();
            if (self != null)
            {
                self.TakeDamage(9999);                 // // Överdrivet värde för att garantera död
            }
            else
            {
                Destroy(gameObject);                   // // Om ingen EnemyHealth finns, förstör direkt
            }

            _nextHitTime = Time.time + hitCooldown;    // // Starta cooldown för säkerhets skull
        }
    }
}
