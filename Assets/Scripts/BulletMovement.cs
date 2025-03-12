using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Interfaces;

public class BulletMovement : MonoBehaviour
{
    private int bulletspeed = 15;
    private IDamageable dmg;
    void Update()
    {
        transform.Translate(Vector3.up * bulletspeed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Monster"))
        {
            if (collision.gameObject.TryGetComponent<IDamageable>(out dmg))
            {
                dmg.Damage(30);
                Destroy(gameObject);
            }
        }
    }
}
