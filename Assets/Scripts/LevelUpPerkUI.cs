using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelUpPerkUI : MonoBehaviour
{
    [System.Serializable]
    public enum PerkType { MaxHP, Heal, FireRate, Damage, MoveSpeed, XPGain }

    [System.Serializable]
    public class PerkDef
    {
        public PerkType type;
        public float value;
        public string title;
        [TextArea] public string desc;
    }

    [Header("Setup")]
    public PlayerRefs player;           // kan lämnas tom → auto-hittas
    public Button btnA, btnB, btnC;     // kan lämnas tom → auto-hittas
    public TMP_Text txtA, txtB, txtC;   // kan lämnas tom → auto-hittas

    [Header("Perkpool (redigera i Inspector)")]
    public List<PerkDef> allPerks = new List<PerkDef>
    {
        new PerkDef { type=PerkType.MaxHP,    value=1,     title="+1 Max HP",       desc="Ökar max HP med 1 och healar 1."},
        new PerkDef { type=PerkType.Heal,     value=1,     title="Heal +1",         desc="Återställ 1 HP."},
        new PerkDef { type=PerkType.FireRate, value=0.10f, title="Fire Rate +10%",  desc="Skjut lite snabbare."},
        new PerkDef { type=PerkType.Damage,   value=0.20f, title="Damage +20%",     desc="Gör mer skada."},
        new PerkDef { type=PerkType.MoveSpeed,value=0.15f, title="Speed +15%",      desc="Rör dig snabbare."},
        new PerkDef { type=PerkType.XPGain,   value=0.20f, title="XP Gain +20%",    desc="Få mer XP framåt."},
    };

    [Header("Behaviour")]
    public bool autoConnect = true;            // koppla automatiskt till PlayerXP2D.onLevelUp
    public Behaviour[] disableWhileOpen;       // dra in t.ex. CharacterMovement2D, PlayerShooting2D, EnemySpawner

    private PerkDef _pickA, _pickB, _pickC;
    private bool _isOpen;
    private PlayerXP2D _xp;                    // sparar kopplad XP-källa för unsubscribe

    void Awake()
    {
        // Panel gömd från start
        gameObject.SetActive(false);

        // --- Auto-find UI om ej satt i Inspector ---
        if (btnA == null) btnA = transform.Find("Content/BtnA")?.GetComponent<Button>();
        if (btnB == null) btnB = transform.Find("Content/BtnB")?.GetComponent<Button>();
        if (btnC == null) btnC = transform.Find("Content/BtnC")?.GetComponent<Button>();

        if (txtA == null && btnA != null) txtA = btnA.GetComponentInChildren<TMP_Text>(true);
        if (txtB == null && btnB != null) txtB = btnB.GetComponentInChildren<TMP_Text>(true);
        if (txtC == null && btnC != null) txtC = btnC.GetComponentInChildren<TMP_Text>(true);

        if (btnA == null || btnB == null || btnC == null || txtA == null || txtB == null || txtC == null)
        {
            Debug.LogWarning("[PerkUI] Saknade UI-referenser. Kontrollera att hierarkin innehåller Content/BtnA(BtnB/BtnC) med TMP_Text under.");
            // fortsätter ändå – Show() avbryter om något saknas
        }

        // Knyt klick (null-säkert)
        if (btnA) btnA.onClick.AddListener(() => Pick(_pickA));
        if (btnB) btnB.onClick.AddListener(() => Pick(_pickB));
        if (btnC) btnC.onClick.AddListener(() => Pick(_pickC));
    }

    void OnEnable()
    {
        if (!autoConnect) return;

        // Hitta PlayerRefs om saknas
        EnsurePlayerRefs();

        // Knyt level-up → Show()
        _xp = player ? player.xp : FindObjectOfType<PlayerXP2D>();
        if (_xp != null) _xp.onLevelUp.AddListener(Show);
        else Debug.LogWarning("[PerkUI] Hittade ingen PlayerXP2D att koppla till (onLevelUp).");
    }

    void OnDisable()
    {
        if (_xp != null) _xp.onLevelUp.RemoveListener(Show);
        _xp = null;
    }

    private void EnsurePlayerRefs()
    {
        if (player != null) return;

        // Först: försök via taggad Player
        var tagged = GameObject.FindGameObjectWithTag("Player");
        if (tagged) player = tagged.GetComponent<PlayerRefs>();

        // Fallback: första som hittas i scenen
        if (player == null) player = FindObjectOfType<PlayerRefs>();

        if (player == null)
            Debug.LogWarning("[PerkUI] Hittar ingen PlayerRefs i scenen. Dra in Player (med PlayerRefs) i fältet 'player'.");
    }

    public void Show()
    {
        if (_isOpen) return;

        // Säkerställ referenser innan vi öppnar
        EnsurePlayerRefs();
        if (player == null || btnA == null || btnB == null || btnC == null || txtA == null || txtB == null || txtC == null)
        {
            Debug.LogError("[PerkUI] Kan inte visas – saknade referenser (player/knappar/texter).");
            return;
        }

        _isOpen = true;

        // Välj tre unika perks
        var pool = new List<PerkDef>(allPerks);
        _pickA = TakeRandom(pool);
        _pickB = TakeRandom(pool);
        _pickC = TakeRandom(pool);

        // Sätt etiketter (titel + ev. kort beskrivning)
        txtA.text = string.IsNullOrEmpty(_pickA.desc) ? _pickA.title : $"{_pickA.title}";
        txtB.text = string.IsNullOrEmpty(_pickB.desc) ? _pickB.title : $"{_pickB.title}";
        txtC.text = string.IsNullOrEmpty(_pickC.desc) ? _pickC.title : $"{_pickC.title}";

        // Pausa spel + stäng av valda komponenter
        if (disableWhileOpen != null)
            foreach (var b in disableWhileOpen) if (b) b.enabled = false;

        Time.timeScale = 0f;
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
        _isOpen = false;

        // Återställ paus
        if (disableWhileOpen != null)
            foreach (var b in disableWhileOpen) if (b) b.enabled = true;

        Time.timeScale = 1f;
    }

    private PerkDef TakeRandom(List<PerkDef> list)
    {
        int i = Random.Range(0, list.Count);
        var p = list[i];
        list.RemoveAt(i);
        return p;
    }

    private void Pick(PerkDef p)
    {
        ApplyPerk(p);
        Hide();
    }

    private void ApplyPerk(PerkDef p)
    {
        if (player == null)
        {
            Debug.LogError("[Perk] PlayerRefs saknas.");
            return;
        }

        // Snabb info-logg
        // Debug.Log($"[Perk] Valde: {p.title} ({p.type}, {p.value})");

        switch (p.type)
        {
            case PerkType.MaxHP:
                if (player.health != null)
                {
                    player.health.maxHP += Mathf.RoundToInt(p.value);
                    player.health.Heal(Mathf.RoundToInt(p.value)); // fyll på lika mycket
                }
                break;

            case PerkType.Heal:
                if (player.health != null)
                {
                    // Om redan full → ge +1 Max HP och fyll 1
                    if (player.health.CurrentHP >= player.health.MaxHP)
                    {
                        player.health.maxHP += 1;
                        player.health.Heal(1);
                    }
                    else
                    {
                        player.health.Heal(Mathf.RoundToInt(p.value));
                    }
                }
                break;

            case PerkType.FireRate:
                if (player.shooting != null)
                {
                    // fireRate = tid mellan skott → mindre = snabbare
                    player.shooting.fireRate = Mathf.Max(0.02f, player.shooting.fireRate * (1f - p.value));
                }
                break;

            case PerkType.Damage:
                if (player.shooting != null)
                {
                    // OBS: nya kulor måste få nuvarande damage vid instansiering (se notis nedan)
                    player.shooting.damage = Mathf.CeilToInt(player.shooting.damage * (1f + p.value));
                }
                break;

            case PerkType.MoveSpeed:
                if (player.move != null)
                {
                    player.move.moveSpeed *= (1f + p.value);
                }
                break;

            case PerkType.XPGain:
                if (player.xp != null)
                {
                    // enkel version: sänk krav lite
                    player.xp.baseXpToNext = Mathf.Max(1, Mathf.RoundToInt(player.xp.baseXpToNext * (1f - p.value * 0.25f)));
                }
                break;
        }

        // Exempel på resultatlogg (bra för snabb felsökning)
        // Debug.Log($"[Perk] RESULT → HP {player.health?.CurrentHP}/{player.health?.MaxHP}, DMG {player.shooting?.damage}, FR {player.shooting?.fireRate}, SPD {player.move?.moveSpeed}");
    }
}
