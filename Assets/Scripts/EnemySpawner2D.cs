using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events; // // För valfritt event på runda-slut

public class EnemySpawner2D : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform player;                 // // Vem vi spawna runt
    [SerializeField] private GameObject enemyMeleePrefab;      // // Din vanliga närstridsfiende (Enemy1)
    [SerializeField] private GameObject enemyRangedPrefab;     // // Din svävande distansfiende (Enemy2)

    [Header("Spawn Settings")]
    [SerializeField] private float spawnEvery = 0.8f;          // // Intervall mellan spawns
    [SerializeField] private int burstCount = 1;               // // Hur många per spawn-tick
    [SerializeField] private float minRadius = 6f;             // // Min avstånd från spelaren
    [SerializeField] private float maxRadius = 10f;            // // Max avstånd från spelaren

    [Header("Round Settings")]
    [SerializeField] private bool autoStart = true;            // // Starta runda automatiskt vid Start()
    [SerializeField] private float roundDuration = 30f;        // // Längd på runda (sek)
    [SerializeField] private UnityEvent onRoundEnd;            // // (Valfritt) Hooka UI här

    [Header("Ranged (Enemy2) Progression")]
    [Tooltip("Andel ranged i början av rundan (0..1). 0.2 = 20%.")]
    [SerializeField, Range(0f, 1f)] private float rangedChanceAtStart = 0.2f;

    [Tooltip("Andel ranged i slutet av rundan (0..1). 0.6 = 60%.")]
    [SerializeField, Range(0f, 1f)] private float rangedChanceAtEnd = 0.6f;

    // // Interna timers/states
    private float _nextSpawn;   // // Tidpunkt för nästa spawn
    private float _roundEnd;    // // När rundan tar slut
    private bool _running;      // // Om spawnern är igång

    private void Start()
    {
        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p) player = p.transform;
        }

        if (autoStart)
            StartRound(); // // Starta direkt för Level 1
    }

    /// <summary>
    /// Starta en ny runda med nuvarande roundDuration.
    /// </summary>
    public void StartRound()
    {
        if (player == null)
        {
            Debug.LogWarning("[EnemySpawner2D] Ingen Player referens hittad.");
            return;
        }

        _running = true;
        _roundEnd = Time.time + roundDuration;
        _nextSpawn = Time.time + 0.5f; // // Låt första spawnen dröja lite
    }

    /// <summary>
    /// (Valfritt) Starta runda med specifik längd.
    /// </summary>
    public void StartRound(float durationSeconds)
    {
        roundDuration = Mathf.Max(1f, durationSeconds);
        StartRound();
    }

    /// <summary>
    /// Stoppa rundan (t.ex. vid Game Over).
    /// </summary>
    public void StopRound()
    {
        _running = false;
    }

    private void Update()
    {
        if (!_running || player == null) return;

        // // Avsluta runda
        if (Time.time >= _roundEnd)
        {
            _running = false;
            Debug.Log("ROUND OVER");
            onRoundEnd?.Invoke(); // // UI-panel etc.
            return;
        }

        // // Spawna vid intervall
        if (Time.time >= _nextSpawn)
        {
            for (int i = 0; i < burstCount; i++)
                SpawnOne();

            _nextSpawn = Time.time + spawnEvery;
        }
    }

    private void SpawnOne()
    {
        // // Slumpa position i en ring runt spelaren
        float angle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
        float radius = Random.Range(minRadius, maxRadius);
        Vector2 offset = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * radius;
        Vector2 pos = (Vector2)player.position + offset;

        // // Hur långt in i rundan är vi? 0 = start, 1 = slut
        float t = 1f - Mathf.Clamp01((_roundEnd - Time.time) / Mathf.Max(0.0001f, roundDuration));
        float rangedChanceNow = Mathf.Lerp(rangedChanceAtStart, rangedChanceAtEnd, t);

        // // Välj typ baserat på current chance
        bool spawnRanged = Random.value < rangedChanceNow;

        GameObject prefabToSpawn = spawnRanged ? enemyRangedPrefab : enemyMeleePrefab;

        if (prefabToSpawn == null)
        {
            Debug.LogWarning("[EnemySpawner2D] Prefab saknas. Kolla 'enemyMeleePrefab' & 'enemyRangedPrefab'.");
            return;
        }

        Instantiate(prefabToSpawn, pos, Quaternion.identity);
    }

    // // Bara för att lätt se ringen i editorn
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        if (player != null)
        {
            // // Rita min/max cirklar runt spelaren
            DrawCircle(player.position, minRadius);
            DrawCircle(player.position, maxRadius);
        }
        else
        {
            // // Rita runt spawnern om player ej finns i editorn
            DrawCircle(transform.position, minRadius);
            DrawCircle(transform.position, maxRadius);
        }
    }

    private void DrawCircle(Vector3 center, float r, int segments = 40)
    {
        Vector3 prev = center + new Vector3(r, 0f, 0f);
        for (int i = 1; i <= segments; i++)
        {
            float angle = (i / (float)segments) * Mathf.PI * 2f;
            Vector3 next = center + new Vector3(Mathf.Cos(angle) * r, Mathf.Sin(angle) * r, 0f);
            Gizmos.DrawLine(prev, next);
            prev = next;
        }
    }
}
