using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static Interfaces;

public enum AIState
{
   Move,
   Attack,
   Die
}

[RequireComponent(typeof(CapsuleCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class MonsterMovementAI : MonoBehaviour , IDamageable
{
    private CapsuleCollider2D ccd;
    private Rigidbody2D rb;
    private Animator anim;
    [SerializeField]
    private GameObject target;
    private int spawnLayer;

    private IDamageable dmg;

    [Header("MonsterMovement")]
    [SerializeField]
    private int monsterMoveSpeed;
    [SerializeField]
    private float attackRange;

    [Header("MonsterClimb")]
    [SerializeField]
    private float vertical;
    [SerializeField]
    private bool isClimbing = false;
    [SerializeField]
    private bool isLadder = false;
    [SerializeField]
    private bool isAttacking = false;
    [SerializeField]
    private GameObject ladder;

    [Header("MonsterStatus")]
    [SerializeField]
    private int health = 100;



    private AIState currentState;

    void Start()
    {
        if (TryGetComponent<CapsuleCollider2D>(out ccd))
        {
            ccd.offset = new Vector2(-0.15f, 0.60f);
            ccd.size = new Vector2(0.3f, 1.0f);
        }
        if (TryGetComponent<Rigidbody2D>(out rb))
        {
            rb.simulated = true;
            rb.gravityScale = 1;
        }
        if(!TryGetComponent<Animator>(out anim))
        {
            Debug.LogWarning("animatior 참조 실패 -  monstermovementAI.cs - Awake()");
        }
        MonsterStateController(AIState.Move);
    }

    private void Update()
    {
        RayCastTarget();
        CheckFloor();
        if (isLadder)
        {
            isClimbing = true;
            ladder.SetActive(false);
        }
        else
        {
            isClimbing = false;
            ladder.SetActive(true);
        }
    }

    private void FixedUpdate()
    {

        if (isClimbing)
        {
            //     Debug.Log("its climbing");
            rb.velocity = new Vector2(0, vertical * 1.5f);
        }
        else if (currentState == AIState.Move)
        {
            MoveLeft();
            Debug.Log("monster is Moving");
        }
        else
        {
          //  Debug.Log("monster is attacking");
        }

    }
    public void ReceiveLayer(int layer)
    {
        spawnLayer = layer;
        transform.gameObject.layer = layer;
        foreach (Transform child in transform.GetComponentsInChildren<Transform>())
        {
            child.gameObject.layer = layer;
        }
    }
    private void MonsterStateController(AIState aistate)
    {
        currentState = aistate;
        Debug.Log($"currentState {currentState}");
        switch (aistate)
        {
            case AIState.Move:
                anim.SetBool("IsDead", false);
                anim.SetBool("IsAttacking", false);
                target = null;
                break;
            case AIState.Attack:
                anim.SetBool("IsAttacking", true);
                break;
            case AIState.Die:
                anim.SetBool("IsAttacking", false);
                anim.SetBool("IsDead", true);
                break;
        }
    }
    
    private void RayCastTarget()
    {
        int mask = (1 << 6) | (1 << spawnLayer);
        Vector2 startPoint = new Vector2(transform.position.x - 0.5f, transform.position.y + 0.3f);
        RaycastHit2D hit = Physics2D.Raycast(startPoint, Vector2.left, 0.1f , mask);
       // Debug.DrawLine(startPoint, startPoint + Vector2.left * 0.1f, Color.red, 0.5f);

        if (hit.collider != null && hit.collider.CompareTag("Ladder"))
        {
            //   Debug.Log("Monster detected in front, attempting to jump");
            isLadder = true;
            MonsterStateController(AIState.Move);
        }
        else if (hit.collider != null && hit.collider.CompareTag("Player"))
        {
            Debug.Log("Monster is attacking the player");
            if (!isAttacking)
            {
                Debug.Log("monster is attacking");
                target = hit.collider.gameObject;
                MonsterStateController(AIState.Attack);
                isAttacking = true;
            }
        }
        else
        {
            if (isAttacking && target == null)
            {
                isAttacking = false;
                MonsterStateController(AIState.Move);
            }
            else if (!isAttacking)
            {
                target = null;
            }
            isLadder = false;
        }
    }
     
    private void CheckFloor()
    {
        RaycastHit2D Floorhit = Physics2D.Raycast(transform.position, Vector2.down, 0.5f);
      //  Debug.DrawLine(transform.position, transform.position + Vector3.down * 0.5f, Color.green, 0.5f);
        if (Floorhit.collider != null && Floorhit.collider.CompareTag("Monster"))
        {
            //Debug.Log("currently Stakced");
            rb.gravityScale = 1f;
        }
        else if(Floorhit.collider != null && Floorhit.collider.CompareTag("Floor"))
        {
           //Debug.Log("currently on the Floor");
            rb.gravityScale = 0;
        }
        else if(Floorhit.collider == null)
        {
           //Debug.Log("currently in the air");
            rb.gravityScale = 1f;
        }
        
    }


    private void OnAttack()
    {
        if (target != null && target.activeInHierarchy)
        {
            target.TryGetComponent<IDamageable>(out dmg);
            if (dmg != null)
            {
                dmg.Damage(20);
            }
        }
    }

    private void MoveLeft()
    {
        if (currentState != AIState.Attack)
        {
            Vector2 targetPosition = new Vector2(transform.position.x - monsterMoveSpeed * Time.deltaTime, transform.position.y);
            rb.velocity = new Vector2(-monsterMoveSpeed, rb.velocity.y);
        }
        else
        {
            rb.velocity = Vector2.zero;
        }
    }

    public void Damage(int amount)
    {
        health -= amount;
        if (health < 0)
        {
            MonsterStateController(AIState.Die);
            Destroy(gameObject);
        }
    }
}
