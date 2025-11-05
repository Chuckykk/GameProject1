using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HPBarUI2D : MonoBehaviour
{
    [Header("UI-Referenser")]
    public Slider slider;          // Dra in din Slider här
    public TMP_Text hpLabel;       // (valfritt) "3/5"-text

    [Header("Auto-bind")]
    [Tooltip("Försök hitta PlayerHealth2D automatiskt när scenen startar.")]
    public bool autoBindPlayer = true;

    private PlayerHealth2D _hp;

    void Start()
    {
        if (autoBindPlayer)
        {
            var player = FindObjectOfType<PlayerHealth2D>();
            if (player) Bind(player);
        }
    }

    public void Bind(PlayerHealth2D hp)
    {
        if (_hp == hp) return;

        // Koppla bort ev. gammal lyssnare
        if (_hp != null) _hp.OnHPChanged.RemoveListener(UpdateUI);

        _hp = hp;

        if (_hp != null)
        {
            _hp.OnHPChanged.AddListener(UpdateUI);
            // Init direkt
            UpdateUI(_hp.CurrentHP, _hp.MaxHP);
        }
    }

    private void OnDestroy()
    {
        if (_hp != null) _hp.OnHPChanged.RemoveListener(UpdateUI);
    }

    // Får värden från PlayerHealth2D
    private void UpdateUI(int current, int max)
    {
        if (slider)
        {
            slider.maxValue = max;
            slider.value = current;
        }
        if (hpLabel) hpLabel.text = $"{current}/{max}";
    }
}
