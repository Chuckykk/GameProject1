using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoundStartUI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Button playButton;

    [Header("Hold These Until Start")]
    [Tooltip("Lägg in alla scripts som INTE ska köra förrän du trycker PLAY (t ex spawner, LevelTimer, CharacterMovement2D).")]
    [SerializeField] private MonoBehaviour[] behavioursToHold;

    [Header("Optional Movement Lock")]
    [Tooltip("Om du har ditt CharacterMovement2D, dra in det här så låser vi rörelsen snyggt istället för bara Time.timeScale.")]
    [SerializeField] private CharacterMovement2D playerMovement;

    private bool _started;

    private void Awake()
    {
        // 1) Stoppa allt som inte ska rulla än
        foreach (var b in behavioursToHold)
        {
            if (b != null) b.enabled = false;
        }

        // 2) Lås spelarkontroller (om du använder ditt egna API)
        if (playerMovement != null)
            playerMovement.SetMovementEnabled(false);

        // 3) Pausa världens tid om du vill frysa fysik/animationer
        Time.timeScale = 0f;

        // 4) Hooka knappen
        if (playButton != null)
            playButton.onClick.AddListener(OnPlayClicked);

        // Visa panelen
        gameObject.SetActive(true);
    }

    private void OnPlayClicked()
    {
        if (_started) return;
        _started = true;

        // Släpp loss allt
        foreach (var b in behavioursToHold)
        {
            if (b != null) b.enabled = true;
        }

        if (playerMovement != null)
            playerMovement.SetMovementEnabled(true);

        Time.timeScale = 1f;           // av-paus
        gameObject.SetActive(false);   // göm panelen
    }
}
