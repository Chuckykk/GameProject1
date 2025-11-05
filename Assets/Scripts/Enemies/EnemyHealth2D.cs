using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth2D : MonoBehaviour
{
    [SerializeField] GameObject xpOrbPrefab;
    [SerializeField] int minOrbs = 1;
    [SerializeField] int maxOrbs = 2;


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
        Instantiate(explosionPrefab, transform.position, Quaternion.identity);

    if (xpOrbPrefab == null)
    {
        Debug.LogWarning($"[{name}] xpOrbPrefab saknas på EnemyHealth2D!", this);
    }

    int lo = Mathf.Max(0, minOrbs);
    int hi = Mathf.Max(lo, maxOrbs);
    int count = Random.Range(lo, hi + 1);

    Debug.Log($"[{name}] Die() → droppar {count} XP-orbs", this);

    for (int i = 0; i < count; i++)
    {
        if (xpOrbPrefab == null) break;
        Vector2 offset = Random.insideUnitCircle * 0.35f;
        var orb = Instantiate(xpOrbPrefab, (Vector2)transform.position + offset, Quaternion.identity);
        // valfritt: orb.name = $"XPOrb_{i}";
    }

    Destroy(gameObject);
}
}
