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
    [Tooltip("Hur lång rundan är i sekunder.")]
    public float durationSeconds = 30f;

    [Tooltip("Starta timern automatiskt när scenen startar.")]
    public bool autoStart = true;

    [Header("Round / Level")]
    [Tooltip("Vilken level som spelas just nu.")]
    public int currentLevel = 1; // NEW

    [Header("References")]
    [Tooltip("TextMeshPro-text som visar nedräkningen.")]
    public TMP_Text timerLabel;

    [Tooltip("Din EnemySpawner (eller valfritt spawner script) som ska stoppas vid timeout/död.")]
    public MonoBehaviour spawnerToStop;

    [Tooltip("Panel som visas när rundan är klar (Round End UI).")]
    public GameObject roundEndPanel;

    [Header("Round End UI (extra info)")]
    [Tooltip("Textfält där vi skriver 'You died from X' och kvarvarande tid.")]
    public TMP_Text deathInfoText; // NEW
    [Tooltip("Ikon för fienden som dödade dig (valfritt).")]
    public Image killerIcon;       // NEW

    [Header("Events (valfria)")]
    public UnityEvent onRoundStart;
    public UnityEvent onRoundEnd;

    // Internt tillstånd
    private float timeLeft;
    private bool isRunning;

    // Dödsinfo (för UI)
    private string lastKillerName = null;  // NEW
    private Sprite lastKillerSprite = null; // NEW

    void Start()
    {
        if (roundEndPanel != null)
            roundEndPanel.SetActive(false);

        if (autoStart)
            StartRound(durationSeconds);
    }

    void Update()
    {
        if (!isRunning) return;

        timeLeft -= Time.deltaTime;

        if (timerLabel != null)
        {
            float clamped = Mathf.Max(0f, timeLeft);
            timerLabel.text = Mathf.Ceil(clamped).ToString("0");
        }

        if (timeLeft <= 0f)
        {
            // Tiden tog slut (ingen killer)
            lastKillerName = null;       // NEW
            lastKillerSprite = null;     // NEW
            EndRound();                  // visar "time's up"-text
        }
    }

    /// <summary>Startar en ny runda och nollställer timer, UI och spawner.</summary>
    public void StartRound(float seconds)
    {
        timeLeft = seconds;
        isRunning = true;

        Time.timeScale = 1f;

        if (roundEndPanel != null)
            roundEndPanel.SetActive(false);

        // Nollställ dödsinfo varje runda
        lastKillerName = null;    // NEW
        lastKillerSprite = null;  // NEW
        if (deathInfoText != null) deathInfoText.text = ""; // NEW
        if (killerIcon != null) killerIcon.sprite = null;   // NEW
        if (killerIcon != null) killerIcon.enabled = false; // NEW

        if (spawnerToStop != null)
            spawnerToStop.enabled = true;

        onRoundStart?.Invoke();
    }

    /// <summary>
    /// Anropa denna när spelaren dör (skickar in vem som dödade + ev. sprite).
    /// </summary>
    public void EndRoundByDeath(string killerName, Sprite killerSprite = null) // NEW
    {
        lastKillerName = killerName;
        lastKillerSprite = killerSprite;
        EndRound();
    }

    /// <summary>Avslutar rundan: stoppar spawner, visar UI, pausar spel – skriver orsak.</summary>
    public void EndRound()
    {
        isRunning = false;

        if (spawnerToStop != null)
            spawnerToStop.enabled = false;

        // Sätt texten innan vi visar panelen
        if (deathInfoText != null) // NEW
        {
            string secLeft = Mathf.Max(0f, timeLeft).ToString("F1");
            if (!string.IsNullOrEmpty(lastKillerName))
            {
                // Dödad av fiende
                deathInfoText.text = $"You died from {lastKillerName}\nIt was {secLeft} sec left of level {currentLevel}";
            }
            else
            {
                // Tiden tog slut
                deathInfoText.text = $"Time's up!\nIt was {secLeft} sec left shown when round ended – Level {currentLevel}";
            }
        }

        // Sätt ikon om vi har en
        if (killerIcon != null) // NEW
        {
            if (lastKillerSprite != null)
            {
                killerIcon.sprite = lastKillerSprite;
                killerIcon.enabled = true;
                killerIcon.SetNativeSize(); // valfritt: ta bort om du vill styra storlek i UI
            }
            else
            {
                killerIcon.enabled = false;
                killerIcon.sprite = null;
            }
        }

        if (roundEndPanel != null)
            roundEndPanel.SetActive(true);

        Time.timeScale = 0f;

        onRoundEnd?.Invoke();
    }

    /// <summary>Hämtar kvarvarande tid (aldrig under 0).</summary>
    public float GetTimeLeft()
    {
        return Mathf.Max(0f, timeLeft);
    }

    // ==== Valfria UI-knappar ====

    public void Retry()
    {
        Time.timeScale = 1f;
        Scene current = SceneManager.GetActiveScene();
        SceneManager.LoadScene(current.buildIndex);
    }

    public void NextRound()
    {
        float nextSeconds = durationSeconds;
        StartRound(nextSeconds);
    }

    // Hjälpmetod om du vill byta level externt
    public void SetLevel(int level) // NEW
    {
        currentLevel = Mathf.Max(1, level);
    }
}
