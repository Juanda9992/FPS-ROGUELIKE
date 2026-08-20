using System;
using UnityEngine;

[Serializable]
public class EnemyStatsData
{
    [Header("Health Settings")]
    [SerializeField] private int _health = 100;

    [Header("Movement Settings")]
    [SerializeField] private float _speed = 3f;

    [Header("Combat Settings")]
    [SerializeField] private int _damage = 10;

    public int Health
    {
        get => _health;
    }

    public float Speed
    {
        get => _speed;
    }

    public int Damage
    {
        get => _damage;
    }
}
