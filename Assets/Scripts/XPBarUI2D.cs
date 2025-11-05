using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class XPBarUI2D : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private PlayerXP2D player;         // Dra in Player här (eller lämna tom så autohittar vi)
    [SerializeField] private Slider xpSlider;           // Dra in XP_Slider
    [SerializeField] private TMP_Text percentText;      // Dra in XP_PercentText
    [SerializeField] private TMP_Text levelText;        // Dra in XP_LevelText
    [SerializeField] private TMP_Text numericText;      // Valfritt: Dra in XP_NumericText

    private void Start()
    {
        if (player == null)
        {
            var p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) player = p.GetComponent<PlayerXP2D>();
        }

        if (player != null)
        {
            player.OnXPChanged += HandleXPChanged;
            // Init
            HandleXPChanged(player.CurrentXP, player.XpToNext, player.Level);
        }
        else
        {
            Debug.LogWarning("[XPBarUI2D] Hittar ingen PlayerXP2D. Dra in referensen i inspectorn.");
        }
    }

    private void OnDestroy()
    {
        if (player != null) player.OnXPChanged -= HandleXPChanged;
    }

    private void HandleXPChanged(int currentXP, int xpToNext, int level)
    {
        if (xpSlider) xpSlider.value = xpToNext > 0 ? (float)currentXP / xpToNext : 0f;

        if (percentText)
        {
            int pct = xpToNext > 0 ? Mathf.FloorToInt((currentXP / (float)xpToNext) * 100f) : 0;
            percentText.text = pct + "%";
        }

        if (levelText) levelText.text = $"LVL {level}";

        if (numericText) numericText.text = $"{currentXP} / {xpToNext}";
    }
}
