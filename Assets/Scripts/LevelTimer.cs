using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;     // // För Retry/NextRound exempel (ladda scen)
using TMPro;                           // // För TextMeshPro-timertext

public class LevelTimer : MonoBehaviour
{
    [Header("Timer Settings")]
    [Tooltip("Hur lång rundan är i sekunder.")]
    public float durationSeconds = 30f;        // // Standard: 30 sek för Level 1

    [Tooltip("Starta timern automatiskt när scenen startar.")]
    public bool autoStart = true;              // // Praktiskt i Level 1

    [Header("References")]
    [Tooltip("TextMeshPro-text som visar nedräkningen.")]
    public TMP_Text timerLabel;                // // Dra in din TMP-Text här

    [Tooltip("Din EnemySpawner (eller valfritt spawner script) som ska stoppas vid timeout.")]
    public MonoBehaviour spawnerToStop;        // // Exempel: EnemySpawner script-komponenten

    [Tooltip("Panel som visas när rundan är klar (Round End UI).")]
    public GameObject roundEndPanel;           // // Valfri – visas vid slut

    [Header("Events (valfria)")]
    public UnityEvent onRoundStart;            // // Körs när rundan startas
    public UnityEvent onRoundEnd;              // // Körs när rundan avslutas

    // // Internt tillstånd
    private float timeLeft;                    // // Nedräkning i sekunder
    private bool isRunning;                    // // Om timern körs just nu

    void Start()
    {
        // // Dölj RoundEnd-panel i början (om den finns)
        if (roundEndPanel != null)
            roundEndPanel.SetActive(false);

        // // Starta automatiskt om flaggan är satt
        if (autoStart)
            StartRound(durationSeconds);
    }

    void Update()
    {
        // // Räkna bara ner om timern är igång
        if (!isRunning) return;

        // // Ticka ned med verklig speltid
        timeLeft -= Time.deltaTime;

        // // Uppdatera UI-etikett (ex: "29", "28", ...)
        if (timerLabel != null)
        {
            // // Visa alltid minst 0
            float clamped = Mathf.Max(0f, timeLeft);
            timerLabel.text = Mathf.Ceil(clamped).ToString("0");
        }

        // // När tiden är slut -> avsluta rundan
        if (timeLeft <= 0f)
        {
            EndRound();
        }
    }

    /// <summary>
    /// Startar en ny runda och nollställer timer, UI och spawner.
    /// </summary>
    public void StartRound(float seconds)
    {
        // // Återställ tid och status
        timeLeft = seconds;
        isRunning = true;

        // // Säkerställ att spelet rullar (om vi pausade tidigare)
        Time.timeScale = 1f;

        // // Dölj RoundEnd-panel
        if (roundEndPanel != null)
            roundEndPanel.SetActive(false);

        // // Starta spawner (om referens finns)
        if (spawnerToStop != null)
            spawnerToStop.enabled = true;

        // // Event-hook
        onRoundStart?.Invoke();
    }

    /// <summary>
    /// Avslutar rundan: stoppar spawner, visar UI och pausar spelet.
    /// </summary>
    public void EndRound()
    {
        // // Stäng av timer
        isRunning = false;

        // // Stoppa spawnern så inga fler fiender kommer
        if (spawnerToStop != null)
            spawnerToStop.enabled = false;

        // // Visa RoundEnd-panel
        if (roundEndPanel != null)
            roundEndPanel.SetActive(true);

        // // Pausa spelet när rundan är klar (kan ändras om du vill)
        Time.timeScale = 0f;

        // // Event-hook
        onRoundEnd?.Invoke();
    }

    /// <summary>
    /// Hämtar kvarvarande tid (aldrig under 0).
    /// </summary>
    public float GetTimeLeft()
    {
        return Mathf.Max(0f, timeLeft);
    }

    // ======================================================================
    // // Nedan två metoder är HELT VALFRIA – praktiska knappar för UI.
    // ======================================================================

    /// <summary>
    /// Ladda om nuvarande scen (börja om rundan).
    /// </summary>
    public void Retry()
    {
        // // Återställ paus och ladda om aktuell scen
        Time.timeScale = 1f;
        Scene current = SceneManager.GetActiveScene();
        SceneManager.LoadScene(current.buildIndex);
    }

    /// <summary>
    /// Enkel "Next Round" – startar en ny runda direkt i samma scen.
    /// </summary>
    public void NextRound()
    {
        // // Exempel: öka svårighet genom att förkorta tiden lite, eller låt samma 30s
        float nextSeconds = durationSeconds; // // eller t.ex. durationSeconds - 2f;
        StartRound(nextSeconds);
    }
}
