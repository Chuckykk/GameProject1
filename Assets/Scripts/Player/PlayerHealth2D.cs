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
    public LevelTimer levelTimer;         // Dra in LevelTimer (RoundManager)

    private bool isDead = false;          // NEW – håller koll på om spelaren är död
    private string pendingKillerName;
private Sprite pendingKillerSprite;
public void MarkKiller(string name, Sprite icon = null) { pendingKillerName = name; pendingKillerSprite = icon; }

    private void Awake()
    {
        currentHP = maxHP;
        if (roundEndPanel != null) 
            roundEndPanel.SetActive(false);
    }

    public void TakeDamage(int amount)
    {
        if (isDead) return;               // NEW – förhindra dubbel-död

        currentHP = Mathf.Max(0, currentHP - amount);
        if (currentHP <= 0)
            Die();
    }

private void Die()
{
    isDead = true;

    if (disableOnDeath != null)
        foreach (var b in disableOnDeath) if (b) b.enabled = false;

    // INTE: if (roundEndPanel) roundEndPanel.SetActive(true);

    // ✅ VIKTIGT – kalla LevelTimer på RoundManager:
    if (levelTimer)
        levelTimer.EndRoundByDeath("an enemy", null); // eller din riktiga fiendenamn/ikon

    if (pauseOnDeath) Time.timeScale = 0f;

    onDeath?.Invoke();
}


    // === NYTT ===
    /// <summary>Återställ spelarens HP och återaktivera komponenter mellan rundor.</summary>
    public void ResetHP()
    {
        currentHP = maxHP;
        isDead = false;

        // Återaktivera komponenter som stängdes vid död
        if (disableOnDeath != null)
        {
            foreach (var b in disableOnDeath)
                if (b) b.enabled = true;
        }

        // Dölj eventuell RoundEnd-panel om den råkar vara kvar
        if (roundEndPanel)
            roundEndPanel.SetActive(false);

        // Se till att spelet fortsätter
        Time.timeScale = 1f;
    }

    // Valfritt kortkommando om du vill heala utan full reset
    public void HealToFull() => currentHP = maxHP;
}
