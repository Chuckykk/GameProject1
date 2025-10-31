using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class LevelTimer : MonoBehaviour
{
    [Header("Timer Settings")]
    public float durationSeconds = 30f;
    public bool autoStart = true;

    [Header("Round / Level")]
    public int currentLevel = 1;

    [Header("References")]
    public TMP_Text timerLabel;
    public MonoBehaviour spawnerToStop;

    [Header("WIN Panel (RoundEndPanel)")]
    [Tooltip("Panel som visas när rundan KLARAS.")]
    public GameObject winPanel;                          // <- NYTT namn
    public TMP_Text winHeader;                           // t.ex. "Round Complete"
    public TMP_Text winInfoText;                         // valfritt
    public GameObject winNextButton;                     // "Next Round"-knapp

    [Header("DEATH Panel (RoundDeathPanel)")]
    [Tooltip("Panel som visas när spelaren DÖR.")]
    public GameObject deathPanel;                        // <- separat panel
    public TMP_Text deathHeader;                         // t.ex. "You Died"
    public TMP_Text deathInfoText;                       // "Killed by X"
    public Image deathKillerIcon;                        // valfritt
    public GameObject deathRetryButton;                  // "Retry"-knapp

    [Header("Player Reset (rek)")]
    public PlayerHealth2D playerHealth;
    public string enemyTag = "Enemy";

    [Header("Events (valfria)")]
    public UnityEvent onRoundStart;
    public UnityEvent onRoundEnd;

    private float timeLeft;
    private bool isRunning;
    private string lastKillerName = null;
    private Sprite lastKillerSprite = null;

    private void Start()
    {
        SafeSetActive(winPanel, false);
        SafeSetActive(deathPanel, false);

        if (autoStart) StartRound(durationSeconds);
        else { isRunning = false; timeLeft = durationSeconds; UpdateTimerLabel(); }
    }

    private void Update()
    {
        if (!isRunning) return;

        float dt = (Time.timeScale > 0f) ? Time.deltaTime : 0f;
        timeLeft -= dt;

        UpdateTimerLabel();

        if (timeLeft <= 0f)
        {
            // Vinst (timer tog slut)
            lastKillerName = null;
            lastKillerSprite = null;
            EndRound(); // visar WIN-panel
        }
    }

    // ========= API =========

    public void StartRound(float seconds)
    {
        // Stäng paneler
        SafeSetActive(winPanel, false);
        SafeSetActive(deathPanel, false);

        // Städ & reset
        Time.timeScale = 1f;
        ClearLiveEnemies();
        if (playerHealth) playerHealth.ResetHP();

        // Timer
        durationSeconds = Mathf.Max(1f, seconds);
        timeLeft = durationSeconds;
        isRunning = true;
        UpdateTimerLabel();

        // Spawner
        if (spawnerToStop) spawnerToStop.enabled = true;

        onRoundStart?.Invoke();
    }

    /// Dödsvariant – anropas från PlayerHealth2D när spelaren dör
    public void EndRoundByDeath(string killerName, Sprite killerSprite = null)
    {
        lastKillerName = killerName;
        lastKillerSprite = killerSprite;

        isRunning = false;
        if (spawnerToStop) spawnerToStop.enabled = false;

        // Fyll DEATH-panel
        if (deathHeader) deathHeader.text = "YOU DIED";
        if (deathInfoText)
        {
            string secLeft = Mathf.Max(0f, timeLeft).ToString("F1");
            string who = string.IsNullOrEmpty(killerName) ? "an enemy" : killerName;
            deathInfoText.text = $"Killed by {who}\nLevel {currentLevel} • {secLeft}s left";
        }
        if (deathKillerIcon)
        {
            if (lastKillerSprite != null)
            {
                deathKillerIcon.sprite = lastKillerSprite;
                deathKillerIcon.enabled = true;
            }
            else
            {
                deathKillerIcon.enabled = false;
                deathKillerIcon.sprite = null;
            }
        }

        // Visa DEATH-panel (eller fallback till winPanel om deathPanel saknas)
        if (deathPanel) SafeSetActive(deathPanel, true);
        else if (winPanel) SafeSetActive(winPanel, true);

        // Döp knapparna rätt: Retry synlig, NextRound dold
        SafeSetActive(deathRetryButton, true);
        SafeSetActive(winNextButton, false);

        Time.timeScale = 0f;
        onRoundEnd?.Invoke();
    }

    /// Vinstvariant – kallas när timer tar slut
    public void EndRound()
    {
        isRunning = false;
        if (spawnerToStop) spawnerToStop.enabled = false;

        // Fyll WIN-panel
        if (winHeader) winHeader.text = "ROUND COMPLETE";
        if (winInfoText) winInfoText.text = $"Level {currentLevel} complete!";

        // Visa WIN-panel (fallback till deathPanel om winPanel saknas)
        if (winPanel) SafeSetActive(winPanel, true);
        else if (deathPanel) SafeSetActive(deathPanel, true);

        // Knappar: NextRound synlig, Retry dold
        SafeSetActive(winNextButton, true);
        SafeSetActive(deathRetryButton, false);

        Time.timeScale = 0f;
        onRoundEnd?.Invoke();
    }

    public void NextRound()
    {
        Time.timeScale = 1f;
        currentLevel = Mathf.Max(1, currentLevel + 1);
        StartRound(durationSeconds);
    }

    public void Retry()
    {
        Time.timeScale = 1f;
        var sc = SceneManager.GetActiveScene();
        SceneManager.LoadScene(sc.buildIndex);
    }

    // ========= Helpers =========

    private void UpdateTimerLabel()
    {
        if (!timerLabel) return;
        int t = Mathf.CeilToInt(Mathf.Max(0f, timeLeft));
        timerLabel.text = t.ToString();
    }

    private void SafeSetActive(Object obj, bool state)
    {
        if (!obj) return;
        if (obj is GameObject go) go.SetActive(state);
        else if (obj is Behaviour b) b.gameObject.SetActive(state);
    }

    private void ClearLiveEnemies()
    {
        if (string.IsNullOrEmpty(enemyTag)) return;
        var enemies = GameObject.FindGameObjectsWithTag(enemyTag);
        foreach (var e in enemies) Destroy(e);
    }
}
