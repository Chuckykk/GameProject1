using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth2D : MonoBehaviour
{
    [SerializeField] private int maxHP = 3;
    private int _currentHP;

    private void Awake()
    {
        _currentHP = maxHP;
    }

    public void TakeDamage(int dmg)
    {
        _currentHP -= dmg;
        if (_currentHP <= 0)
            Die();
    }

    private void Die()
    {
        Destroy(gameObject); // // För nu: ta bort fienden helt
    }
}
