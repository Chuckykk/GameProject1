using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth2D : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private int maxHP = 5;     // // Spelarens maxliv
    private int _currentHP;

    private void Awake()
    {
        _currentHP = maxHP;                     // // Fyll HP vid start
    }

    public void TakeDamage(int amount)
    {
        _currentHP -= amount;                   // // Minska HP
        Debug.Log($"Player HP: {_currentHP}/{maxHP}");

        if (_currentHP <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("PLAYER DIED!");
        // // Här kan du lägga: spela dödsanimation, pausa spelet, visa Game Over UI, osv.
        gameObject.SetActive(false);            // // Enkel lösning – stäng av spelaren
    }

    // // (Valfritt) För att heala senare
    public void Heal(int amount)
    {
        _currentHP = Mathf.Min(_currentHP + amount, maxHP);
    }

    // // För UI som behöver veta nuvarande HP
    public int CurrentHP => _currentHP;
    public int MaxHP => maxHP;
}
