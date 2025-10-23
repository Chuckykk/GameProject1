using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyChase2D : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 3f;   // // Gåfart – justera per fiendetyp
    [SerializeField] private float stopDistance = 0.1f; // // Hur nära spelaren vi slutar försöka röra oss

    private Rigidbody2D _rb;                         // // Cache för fysik
    private Transform _target;                       // // Spelaren (tag: Player)

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();          // // Hämta RB2D
        GameObject p = GameObject.FindGameObjectWithTag("Player"); // // Leta Player
        if (p != null) _target = p.transform;       // // Spara transform
    }

    private void FixedUpdate()
    {
        if (_target == null) return;                // // Finns ingen spelare → gör inget

        Vector2 from = _rb.position;               // // Fiendens position
        Vector2 to = _target.position;             // // Spelarens position
        Vector2 dir = to - from;                   // // Riktning mot spelaren

        if (dir.sqrMagnitude <= stopDistance * stopDistance) return; // // För nära → stå still

        dir.Normalize();                            // // Normalisera så hastighet blir jämn
        Vector2 step = dir * moveSpeed * Time.fixedDeltaTime; // // Steglängd denna frame
        _rb.MovePosition(from + step);              // // Flytta snärtigt utan velocity-tröghet
    }
}
