using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XPOrb2D : MonoBehaviour
{
    [Min(1)] public int xpValue = 1;
    [Header("Magnet (valfritt)")]
    public float magnetRange = 3f;
    public float magnetSpeed = 6f;

    private Transform _player;

    void Start()
    {
        var player = GameObject.FindGameObjectWithTag("Player");
        if (player) _player = player.transform;
    }

    void Update()
    {
        if (_player == null) return;

        float d = Vector2.Distance(transform.position, _player.position);
        if (d <= magnetRange)
        {
            transform.position = Vector2.MoveTowards(
                transform.position,
                _player.position,
                magnetSpeed * Time.deltaTime
            );
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        var xp = other.GetComponent<PlayerXP2D>();
        if (xp != null)
            xp.AddXP(xpValue);

        Destroy(gameObject);
    }
}