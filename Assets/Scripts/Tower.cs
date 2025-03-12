using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Interfaces;

public class Tower : MonoBehaviour , IDamageable
{

    [SerializeField]
    private int health = 1000;
    [SerializeField]
    private GameObject tower;

    public void Damage(int amount)
    {
        health -= amount;
        Debug.Log($"taking Damage{health} - {amount} current hp = {health}");
        if (health <= 0)
        {
            TowerManager.TmInstance.StartFall();
            Destroy(gameObject);
        }
    }
}
