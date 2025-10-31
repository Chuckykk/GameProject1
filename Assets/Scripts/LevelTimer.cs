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
    public int currentLevel = 1;

    [Header("References")]
    [Tooltip("TextMeshPro-text som visar nedräkningen.")]
    public TMP_Text timerLabel;

    [Tooltip("Din EnemySpawner (eller valfritt spawner script) som ska stoppas vid timeout/död.")]
    public MonoBehaviour spawnerToStop;

    [Tooltip("Panel som visar rundslut (både vinst/död).")]
    public GameObject roundEndPanel;

    [Header("Round End UI (innehåll i panelen)")]
    [Tooltip("Rubrik – t.ex. 'Round Complete' eller 'You Died' (valfritt).")]
    public TMP_Text roundEndHeader;            // NEW
    [Tooltip("Textfält där vi skriver detaljer – t.ex. kvarvarande tid, level, killer, osv.")]
    public TMP_Text deathInfoText;
    [Tooltip("Ikon för fienden som dödade dig (valfritt).")]
    public Image killerIcon;

    [Header("Round End Buttons (visa/dölj)")]
    public GameObject nextRoundButton;         // NEW (visas vid vinst)
    public GameObject retryButton;             // NEW (visas vid död)

    [Header("Player Reset (valfritt men rekommenderat)")]
    [Tooltip("Dra in din PlayerHealth2D så vi kan resetta HP mellan rundor.")]
    public PlayerHealth2D playerHealth;        // NEW
    [Tooltip("Taggen som alla fiender har i scenen.")]
    public string enemyTag = "Enemy";          // NEW

    [Header("Events (valfria)")]
    public UnityEvent onRoundStart;
    public UnityEvent onRoundEnd;

    // Internt tillstånd
    private float timeLeft;
    private bool isRunning;

    // Dödsinfo (för UI)
    private string lastKillerName = null;
    private Sprite lastKillerSprite = null;

    private bool EndedByDeath => !string.IsNullOrEmpty(lastKillerName); // NEW

    void Start()
    {
        if (roundEndPanel != null) roundEndPanel.SetActive(false);

        if (autoStart)
            StartRound(durationSeconds);
        else
            PrepareIdleTimer();
    }

    void Update()
    {
        if (!isRunning) return;

        // Om spelet är pausat ska timern inte tappa tid
        float dt = (Time.timeScale > 0f) ? Time.deltaTime : 0f;  // NEW
        timeLeft -= dt;

        if (timerLabel != null)
        {
            float clamped = Mathf.Max(0f, timeLeft);
            timerLabel.text = Mathf.Ceil(clamped).ToString("0");
        }

        if (timeLeft <= 0f)
        {
            // Tiden tog slut (vinst; ingen killer)
            lastKillerName = null;
            lastKillerSprite = null;
            EndRound();
        }
    }

    private void PrepareIdleTimer() // NEW
    {
        isRunning = false;
        timeLeft = durationSeconds;
        UpdateTimerLabel();
    }

    private void UpdateTimerLabel() // NEW
    {
        if (timerLabel == null) return;
        int t = Mathf.CeilToInt(Mathf.Max(0f, timeLeft));
        timerLabel.text = t.ToString();
    }

    /// <summary>Startar en ny runda och nollställer timer, UI och spawner.</summary>
    public void StartRound(float seconds)
    {
        // UI state
        if (roundEndPanel != null) roundEndPanel.SetActive(false);
        ShowButton(nextRoundButton, false);  // NEW
        ShowButton(retryButton, false);      // NEW

        // Nollställ dödsinfo varje runda
        lastKillerName = null;
        lastKillerSprite = null;
        if (roundEndHeader != null) roundEndHeader.text = ""; // NEW
        if (deathInfoText != null) deathInfoText.text = "";
        if (killerIcon != null)
        {
            killerIcon.enabled = false;
            killerIcon.sprite = null;
        }

        // Spel-state
        Time.timeScale = 1f;
        ClearLiveEnemies();                  // NEW – städa ev. kvarvarande fiender
        if (playerHealth != null) playerHealth.ResetHP(); // NEW – full HP mellan rundor

        // Timer
        durationSeconds = Mathf.Max(1f, seconds);
        timeLeft = durationSeconds;
        isRunning = true;
        UpdateTimerLabel();

        // Spawner
        if (spawnerToStop != null) spawnerToStop.enabled = true;

        onRoundStart?.Invoke();
    }

    /// <summary>
    /// Anropa denna när spelaren dör (skickar in vem som dödade + ev. sprite).
    /// </summary>
    public void EndRoundByDeath(string killerName, Sprite killerSprite = null)
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

        // Sätt texter
        string secLeft = Mathf.Max(0f, timeLeft).ToString("F1");
        if (roundEndHeader != null)
            roundEndHeader.text = EndedByDeath ? "YOU DIED" : "ROUND COMPLETE"; // NEW

        if (deathInfoText != null)
        {
            if (EndedByDeath)
                deathInfoText.text = $"You were killed by {lastKillerName}\nLevel {currentLevel} • {secLeft}s left on the clock";
            else
                deathInfoText.text = $"Time's up!\nLevel {currentLevel}";
        }

        // Sätt ikon om vi har en
        if (killerIcon != null)
        {
            if (EndedByDeath && lastKillerSprite != null)
            {
                killerIcon.sprite = lastKillerSprite;
                killerIcon.enabled = true;
                // killerIcon.SetNativeSize(); // låt UI styra storlek, kommentera in om du vill auto-sizea
            }
            else
            {
                killerIcon.enabled = false;
                killerIcon.sprite = null;
            }
        }

        // Visa rätt knappar
        ShowButton(nextRoundButton, !EndedByDeath); // vinst → NextRound
        ShowButton(retryButton, EndedByDeath);      // död → Retry

        if (roundEndPanel != null)
            roundEndPanel.SetActive(true);

        Time.timeScale = 0f;

        onRoundEnd?.Invoke();
    }

    /// <summary>Hämtar kvarvarande tid (aldrig under 0).</summary>
    public float GetTimeLeft() => Mathf.Max(0f, timeLeft);

    // ==== UI-knappar ====

    public void Retry()
    {
        Time.timeScale = 1f;
        Scene current = SceneManager.GetActiveScene();
        SceneManager.LoadScene(current.buildIndex);
    }

    public void NextRound()
    {
        // Endast meningsfullt vid vinst (knappen är dold vid död)
        Time.timeScale = 1f;
        currentLevel = Mathf.Max(1, currentLevel + 1); // NEW – öka level
        float nextSeconds = durationSeconds;           // ev. skala svårighetsgrad här
        StartRound(nextSeconds);
    }

    public void SetLevel(int level)
    {
        currentLevel = Mathf.Max(1, level);
    }

    // ==== Hjälpmetoder ====

    private void ShowButton(GameObject go, bool state) // NEW
    {
        if (go != null) go.SetActive(state);
    }

    private void ClearLiveEnemies() // NEW
    {
        if (string.IsNullOrEmpty(enemyTag)) return;
        var enemies = GameObject.FindGameObjectsWithTag(enemyTag);
        foreach (var e in enemies) Destroy(e);
    }
}
