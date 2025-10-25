using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;   // <- behövs för UnityEvent

public class PlayerHealth2D : MonoBehaviour
{
    [Header("Health")]
    public int maxHP = 5;
    [SerializeField] private int currentHP;

    [Header("Death & UI")]
    public bool pauseOnDeath = true;
    public GameObject roundEndPanel;      // Dra in din RoundEndPanel här
    public UnityEvent onDeath;            // Då får du + i Inspectorn

    [Header("Optional: Disable on death")]
    public Behaviour[] disableOnDeath;    // T.ex. rörelse/skjutscript

    [Header("Optional: Call LevelTimer")]
    public LevelTimer levelTimer;         // Dra in RoundManager (LevelTimer)

    private void Awake()
    {
        currentHP = maxHP;
        if (roundEndPanel != null) roundEndPanel.SetActive(false);
    }

    public void TakeDamage(int amount)
    {
        currentHP = Mathf.Max(0, currentHP - amount);
        if (currentHP <= 0) Die();
    }

    private void Die()
    {
        // Stäng av valda komponenter (rörelse/skjut)
        if (disableOnDeath != null)
        {
            foreach (var b in disableOnDeath) if (b) b.enabled = false;
        }

        // Visa panel
        if (roundEndPanel) roundEndPanel.SetActive(true);

        // Stoppa runda/spawns via LevelTimer (återanvänd samma panelflöde)
        if (levelTimer) levelTimer.EndRound();

        if (pauseOnDeath) Time.timeScale = 0f;

        onDeath?.Invoke();
    }

    // Valfritt
    public void HealToFull() => currentHP = maxHP;
}
