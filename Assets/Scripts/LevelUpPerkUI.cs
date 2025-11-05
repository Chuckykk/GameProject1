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
    public PlayerRefs player;         // Dra in Player (med PlayerRefs)
    public Button btnA, btnB, btnC;   // Dra in
    public TMP_Text txtA, txtB, txtC; // Dra in

    [Header("Perkpool (kan redigeras i Inspector)")]
    public List<PerkDef> allPerks = new List<PerkDef>
    {
        new PerkDef { type=PerkType.MaxHP,    value=1,    title="+1 Max HP",      desc="Ökar max HP med 1 och healar 1."},
        new PerkDef { type=PerkType.Heal,     value=1,    title="Heal +1",        desc="Återställ 1 HP."},
        new PerkDef { type=PerkType.FireRate, value=0.10f,title="Fire Rate +10%", desc="Skjut lite snabbare."},
        new PerkDef { type=PerkType.Damage,   value=0.20f,title="Damage +20%",    desc="Gör mer skada."},
        new PerkDef { type=PerkType.MoveSpeed,value=0.15f,title="Speed +15%",     desc="Rör dig snabbare."},
        new PerkDef { type=PerkType.XPGain,   value=0.20f,title="XP Gain +20%",   desc="Få mer XP framåt."},
    };

    private PerkDef _pickA, _pickB, _pickC;
    private bool _isOpen;

    void Awake()
    {
        gameObject.SetActive(false);
        btnA.onClick.AddListener(() => Pick(_pickA));
        btnB.onClick.AddListener(() => Pick(_pickB));
        btnC.onClick.AddListener(() => Pick(_pickC));
    }

    public void Show()
    {
        if (_isOpen) return;
        _isOpen = true;

        var pool = new List<PerkDef>(allPerks);
        _pickA = TakeRandom(pool);
        _pickB = TakeRandom(pool);
        _pickC = TakeRandom(pool);

        txtA.text = _pickA.title;
        txtB.text = _pickB.title;
        txtC.text = _pickC.title;

        Time.timeScale = 0f;   // Pausa spelet medan man väljer
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
        _isOpen = false;
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
        if (player == null) return;

        switch (p.type)
        {
            case PerkType.MaxHP:
                if (player.health != null)
                {
                    player.health.maxHP += Mathf.RoundToInt(p.value);
                    player.health.Heal(Mathf.RoundToInt(p.value)); // kräver Heal(int)
                }
                break;

            case PerkType.Heal:
                if (player.health != null)
                    player.health.Heal(Mathf.RoundToInt(p.value));
                break;

            case PerkType.FireRate:
                if (player.shooting != null)
                    player.shooting.fireRate = Mathf.Max(0.02f, player.shooting.fireRate * (1f - p.value));
                break;

            case PerkType.Damage:
                if (player.shooting != null)
                    player.shooting.damage = Mathf.CeilToInt(player.shooting.damage * (1f + p.value));
                break;

            case PerkType.MoveSpeed:
                if (player.move != null)
                    player.move.moveSpeed *= (1f + p.value);
                break;

            case PerkType.XPGain:
                if (player.xp != null)
                    player.xp.baseXpToNext = Mathf.Max(1, Mathf.RoundToInt(player.xp.baseXpToNext * (1f - p.value * 0.25f)));
                break;
        }
    }
}
