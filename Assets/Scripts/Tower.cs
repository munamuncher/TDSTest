using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;
using static Interfaces;

public class Tower : MonoBehaviour , IDamageable
{
    [SerializeField]
    private int health = 1000;
    private SpriteRenderer sr;
    private void Awake()
    {
        if (!TryGetComponent<SpriteRenderer>(out sr))
        {
            Debug.LogWarning("spriteRenderer 참조 실패 - Awake() - Tower.cs");
        }
    }

    public void Damage(int amount)
    {
        health -= amount;
        StartCoroutine(takeDamge());
        Debug.Log($"taking Damage{health} - {amount} current hp = {health}");
        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }
    private IEnumerator takeDamge()
    {
        if (sr != null)
        {
            sr.color = Color.red;
            yield return new WaitForSeconds(0.1f);
            sr.color = Color.white;
            yield break;
        }
    }

}
