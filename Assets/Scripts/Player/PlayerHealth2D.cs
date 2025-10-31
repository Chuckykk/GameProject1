using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerHealth2D : MonoBehaviour
{
    [Header("Health")]
    public int maxHP = 5;
    [SerializeField] private int currentHP;

    [Header("Death & UI")]
    [Tooltip("Låt LevelTimer sköta paus/paneler. Lämna denna false.")]
    public bool pauseOnDeath = false;            // <- Låt LevelTimer pausa
    [Tooltip("OBS: Används inte längre. Panel hanteras av LevelTimer.")]
    public GameObject roundEndPanel;             // <- Lämnas orörd/ignoreras
    public UnityEvent onDeath;                   // + i Inspectorn

    [Header("Optional: Disable on death")]
    public Behaviour[] disableOnDeath;           // T.ex. rörelse/skjutscript

    [Header("Optional: Call LevelTimer")]
    public LevelTimer levelTimer;                // Dra in LevelTimer (RoundManager). Hittas auto om tomt.

    // Internt state
    private bool isDead = false;

    // (Valfritt) Om en angripare vill skicka med ett ikon-sprite
    private string pendingKillerName;
    private Sprite pendingKillerSprite;
    public void MarkKiller(string name, Sprite icon = null)
    {
        pendingKillerName = name;
        pendingKillerSprite = icon;
    }

    private void Awake()
    {
        currentHP = maxHP;

        // Dölj ev. gammal panel om den råkat ligga kvar
        if (roundEndPanel != null)
            roundEndPanel.SetActive(false);

        // Auto-fallback för LevelTimer om inte satt i Inspector
        if (levelTimer == null)
            levelTimer = FindObjectOfType<LevelTimer>();
    }

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

        // Dölj ev. panel (LevelTimer visar/döljer egentligen)
        if (roundEndPanel)
            roundEndPanel.SetActive(false);

        // Låt LevelTimer styra Time.timeScale, men detta skadar inte mellan rundor
        Time.timeScale = 1f;

        // Nollställ ev. pending killer för nästa runda
        pendingKillerName = null;
        pendingKillerSprite = null;
    }

    public void HealToFull() => currentHP = maxHP;

    public void TakeDamage(int amount)
    {
        if (isDead) return;

        currentHP = Mathf.Max(0, currentHP - Mathf.Max(0, amount));
        if (currentHP <= 0)
            Die();
    }

    private void Die()
    {
        if (isDead) return;
        isDead = true;

        // Stäng av kontroller/systems om du vill
        if (disableOnDeath != null)
        {
            foreach (var b in disableOnDeath)
                if (b) b.enabled = false;
        }

        // Ta fram rätt killer-namn:
        // 1) Om någon kallat MarkKiller() (med ev. sprite) – använd det.
        // 2) Annars använd DeathContext.KillerName (sätts av projektil/melee).
        string killer =
            !string.IsNullOrEmpty(pendingKillerName) ? pendingKillerName :
            (!string.IsNullOrEmpty(DeathContext.KillerName) ? DeathContext.KillerName : "an enemy");

        // Låt LevelTimer hantera panel, text och paus
        if (levelTimer != null)
        {
            levelTimer.EndRoundByDeath(killer, pendingKillerSprite);
        }
        else
        {
            Debug.LogWarning("[PlayerHealth2D] LevelTimer saknas – kan inte visa Death-panel.");
            if (pauseOnDeath) Time.timeScale = 0f; // fallback om du verkligen vill pausa här
        }

        onDeath?.Invoke();
    }
}
