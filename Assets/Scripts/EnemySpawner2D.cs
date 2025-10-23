using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner2D : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform player;          // // Vem vi spawna runt
    [SerializeField] private GameObject enemyPrefab;    // // Prefaben ovan

    [Header("Spawn Settings")]
    [SerializeField] private float spawnEvery = 0.8f;   // // Intervall mellan spawns
    [SerializeField] private int burstCount = 1;        // // Hur många per spawn-tick
    [SerializeField] private float minRadius = 6f;      // // Min avstånd från spelaren
    [SerializeField] private float maxRadius = 10f;     // // Max avstånd från spelaren

    [Header("Round Settings")]
    [SerializeField] private float roundDuration = 30f; // // Level 1 längd (sek)
    private float _nextSpawn;                           // // Tidpunkt för nästa spawn
    private float _roundEnd;                            // // När rundan tar slut
    private bool _running;                              // // Om spawnern är igång

    private void Start()
    {
        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p) player = p.transform;
        }
        StartRound();                                   // // Starta direkt för Level 1
    }

    public void StartRound()
    {
        _running = true;                                // // Spawnern aktiv
        _roundEnd = Time.time + roundDuration;          // // Sluttid
        _nextSpawn = Time.time + 0.5f;                  // // Låt första spawnen dröja lite
    }

    private void Update()
    {
        if (!_running || player == null) return;        // // Inget att göra

        // // Avsluta runda
        if (Time.time >= _roundEnd)
        {
            _running = false;                           // // Stäng av spawns
            Debug.Log("ROUND OVER");                    // // Hooka UI senare
            return;
        }

        // // Spawna vid intervall
        if (Time.time >= _nextSpawn)
        {
            for (int i = 0; i < burstCount; i++)
                SpawnOne();
            _nextSpawn = Time.time + spawnEvery;        // // Boka nästa spawn
        }
    }

    private void SpawnOne()
    {
        // // Slumpa vinkel och radie i en ring runt spelaren
        float angle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
        float radius = Random.Range(minRadius, maxRadius);
        Vector2 offset = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * radius;
        Vector2 pos = (Vector2)player.position + offset;

        Instantiate(enemyPrefab, pos, Quaternion.identity); // // Skapa fiende
    }
}
