using System;
using UnityEngine;
using UnityEngine.Events;

[DisallowMultipleComponent]
public class PlayerXP2D : MonoBehaviour
{
    [Header("XP & Level (enkelt)")]
    [SerializeField] private int level = 1;
    [SerializeField] private int currentXP = 0;

    [Tooltip("Bas-krav för level 1 → 2 (ändras gärna via perks).")]
    [SerializeField] public int baseXpToNext = 20;

    [Tooltip("Hur mycket kravet skalar per level (t.ex. 1.25 = +25%).")]
    [SerializeField] private float growth = 1.25f;

    [Tooltip("Global XP-multiplikator (kan buffas via perks).")]
    [SerializeField] public float xpMultiplier = 1f;

    [Header("Event")]
    public UnityEvent onLevelUp;  // Koppla t.ex. LevelUpPerkUI.Show()

    // Beräknat krav för aktuell level
    [SerializeField] private int xpToNext;

    /// <summary>
    /// UI-hook: (currentXP, xpToNext, level)
    /// Prenumerera i UI-bar eller liknande.
    /// </summary>
    public event Action<int, int, int> OnXPChanged;

    // Offentliga read-only properties för UI/andra system
    public int Level        => level;
    public int CurrentXP    => currentXP;
    public int XpToNext     => xpToNext;
    public float Progress01 => xpToNext > 0 ? (float)currentXP / xpToNext : 0f;

    private void Awake()
    {
        // Initiera kravet för nuvarande level
        xpToNext = GetXPNeededForLevel(level);
        // Skicka initial status till UI
        OnXPChanged?.Invoke(currentXP, xpToNext, level);
    }

    /// <summary>
    /// Kallas av XP-orber när spelaren plockar XP.
    /// </summary>
    public void AddXP(int rawAmount)
    {
        if (rawAmount <= 0) return;

        // Tillämpa multiplikator (avrunda uppåt/lämpligt)
        int gain = Mathf.Max(0, Mathf.RoundToInt(rawAmount * xpMultiplier));
        if (gain == 0) return;

        currentXP += gain;

        // Hantera flera level-ups om det behövs
        while (xpToNext > 0 && currentXP >= xpToNext)
        {
            currentXP -= xpToNext;
            LevelUpOnce(); // ropar onLevelUp och räknar fram nytt krav
        }

        // Skicka UI-uppdatering
        OnXPChanged?.Invoke(currentXP, xpToNext, level);
    }

    /// <summary>
    /// En (1) level-up: öka level, räkna fram nytt krav och trigga event.
    /// </summary>
   private void LevelUpOnce()
{
    Debug.Log($"[XP] LEVEL UP → Ny nivå: {level + 1}");
    level = Mathf.Max(1, level + 1);
    xpToNext = GetXPNeededForLevel(level);
    OnXPChanged?.Invoke(currentXP, xpToNext, level);
    onLevelUp?.Invoke();
}

    /// <summary>
    /// Enkel exponentiell kurva: base * growth^(level-1).
    /// Ex: base=20, growth=1.25 → L1=20, L2=25, L3=31, L4=39, ...
    /// </summary>
    private int GetXPNeededForLevel(int lvl)
    {
        if (lvl <= 1) return Mathf.Max(1, baseXpToNext);
        float req = baseXpToNext * Mathf.Pow(Mathf.Max(1.0f, growth), lvl - 1);
        return Mathf.Max(1, Mathf.RoundToInt(req));
    }

    /// <summary>
    /// Nollställ XP (praktiskt vid test). currentXP=0, behåller level (eller sätter ny).
    /// </summary>
    public void ResetXP(int newLevel = 1)
    {
        level = Mathf.Max(1, newLevel);
        currentXP = 0;
        xpToNext = GetXPNeededForLevel(level);
        OnXPChanged?.Invoke(currentXP, xpToNext, level);
    }

    /// <summary>
    /// Sätt multiplikator direkt (t.ex. 1.2f = +20% XP).
    /// </summary>
    public void SetXPMultiplier(float multiplier)
    {
        xpMultiplier = Mathf.Max(0f, multiplier);
    }

    /// <summary>
    /// Lägg till pågående multiplikator (t.ex. +0.2f = +20%).
    /// </summary>
    public void AddXPMultiplier(float delta)
    {
        xpMultiplier = Mathf.Max(0f, xpMultiplier + delta);
    }

    /// <summary>
    /// Forcera en level-up (debug/cheat/test). Ger inte XP, triggar bara nästa level.
    /// </summary>
    public void ForceLevelUp()
    {
        LevelUpOnce();
        OnXPChanged?.Invoke(currentXP, xpToNext, level);
    }
}
