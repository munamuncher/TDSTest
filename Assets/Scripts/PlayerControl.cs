using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Interfaces;


public enum PlayerState
{
    idle,
    Attack,
}

public class PlayerControl : MonoBehaviour , IDamageable
{
    public List<GameObject> allObjects = new List<GameObject>();
    public GameObject nearTarget;
    private float nearestDistance = float.MaxValue;
    public GameObject target { get; set; }
    [SerializeField]
    private GameObject invisWall;
    [SerializeField]
    private GameObject gun;
    [SerializeField]
    private GameObject muzzle;
    [SerializeField]
    private GameObject bullet;
    [SerializeField]
    private int health;
    private PlayerState pState;


    public void Update()
    {
        if (nearTarget != null && nearTarget.activeInHierarchy)
        {
            Vector3 direction = (nearTarget.transform.position - transform.position).normalized;
            Quaternion rotation = Quaternion.LookRotation(Vector3.forward, direction);
            rotation *= Quaternion.Euler(0, 0, 60f);
            gun.transform.rotation = rotation;
        }
        if(pState == PlayerState.idle)
        {
           if(target != null)
            {
                PlayerStateController(PlayerState.Attack);
            }
           else
            {
                FindClosestTarget();
            }
        }
    }
    
    private void PlayerStateController(PlayerState playerstate)
    {
        pState = playerstate;
        switch(playerstate)
        {
            case PlayerState.idle:
                break;
            case PlayerState.Attack:
                StartCoroutine(AttackTarget());
                break;
        }
    }


    IEnumerator AttackTarget() //if time make bullet object pool maybe???
    {
        while (pState == PlayerState.Attack)
        {
            if (target != null && target.activeInHierarchy)
            {
                GameObject obj = Instantiate(bullet, muzzle.transform.position,muzzle.transform.rotation);
                yield return new WaitForSeconds(1f);
            }
            else
            {
                PlayerStateController(PlayerState.idle);
                yield break;
            }
        }
    }

    public void UpdateEnemyList()
    {
        allObjects.Clear();
        allObjects.AddRange(GameObject.FindGameObjectsWithTag("Monster"));
    }
    public void FindClosestTarget()
    {
        UpdateEnemyList();
        nearestDistance = float.MaxValue;
        foreach (GameObject obj in allObjects)
        {
            if (obj != null && obj.activeInHierarchy)
            {
                float distance = Vector3.Distance(this.transform.position, obj.transform.position);
                if (distance < nearestDistance)
                {
                    nearTarget = obj;
                    nearestDistance = distance;

                }
            }
        }
        target = nearTarget;
    }

    public void Damage(int amount)
    {
        health -= amount;
        if (health < 0)
        {
            Destroy(invisWall);
            Destroy(gameObject);
        }
    }
}
