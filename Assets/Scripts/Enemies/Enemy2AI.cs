using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy2AI : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 2f;
    public float stopDistance = 3f;  // hur nära den får komma spelaren

    [Header("Shooting")]
    public GameObject projectilePrefab;
    public float fireRate = 2f;      // sekunder mellan skott
    public float projectileSpeed = 6f;

    private Transform player;
    private float fireTimer;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    void Update()
    {
        if (player == null) return;

        // --- RÖRELSE ---
        float distance = Vector2.Distance(transform.position, player.position);

        if (distance > stopDistance)
        {
            // Följ spelaren
            Vector2 direction = (player.position - transform.position).normalized;
            transform.position += (Vector3)(direction * moveSpeed * Time.deltaTime);
        }

        // --- ANFALL ---
        fireTimer += Time.deltaTime;
        if (fireTimer >= fireRate)
        {
            ShootAtPlayer();
            fireTimer = 0f;
        }
    }

    void ShootAtPlayer()
    {
        if (projectilePrefab == null) return;

        // Skapa projektil
        GameObject bullet = Instantiate(projectilePrefab, transform.position, Quaternion.identity);

        // Rikta den mot spelaren
        Vector2 direction = (player.position - transform.position).normalized;
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        rb.velocity = direction * projectileSpeed;
    }
}
