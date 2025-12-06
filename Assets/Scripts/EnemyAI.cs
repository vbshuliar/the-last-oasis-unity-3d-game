using UnityEngine;
using UnityEngine.AI;

// basic enemy ai that chases and attacks the player using navmesh pathfinding
public class EnemyAI : MonoBehaviour
{
    const string IDLE = "Idle";
    const string WALK = "Walk";
    const string ATTACK = "Attack";

    [Header("Combat Settings")]
    [SerializeField] float detectionRange = 10f;
    [SerializeField] float attackRange = 1.5f;
    [SerializeField] int attackDamage = 1;
    [SerializeField] float attackSpeed = 1.0f;
    [SerializeField] float attackDelay = 0.3f;
    [SerializeField] ParticleSystem hitEffect;

    [Header("Movement")]
    [SerializeField] float rotationSpeed = 5f;

    Transform player;
    NavMeshAgent agent;
    Animator animator;
    Actor actor;

    bool isAttacking = false;
    float attackAnimationLength = 0f;
    float lastAttackTime = 0f;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        actor = GetComponent<Actor>();
    }

    void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }

        // find attack animation length by searching through all animation clips
        if (animator != null)
        {
            RuntimeAnimatorController ac = animator.runtimeAnimatorController;
            foreach (AnimationClip clip in ac.animationClips)
            {
                if (clip.name == ATTACK)
                {
                    attackAnimationLength = clip.length;
                    break;
                }
            }

            if (attackAnimationLength == 0f)
            {
                attackAnimationLength = 1f;
            }
        }

        // subscribe to death event to notify gamemanager when enemy dies
        if (actor != null)
        {
            actor.OnDeath += OnEnemyDeath;
        }
    }

    void OnEnemyDeath(Actor deadActor)
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.AddKill();
        }
    }

    void Update()
    {
        if (player == null || actor == null || !actor.IsAlive()) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= detectionRange)
        {
            FacePlayer();

            if (distanceToPlayer <= attackRange)
            {
                agent.SetDestination(transform.position);
                TryAttack();
            }
            else
            {
                agent.SetDestination(player.position);
            }
        }

        SetAnimations();
    }

    void FacePlayer()
    {
        Vector3 direction = (player.position - transform.position).normalized;
        // keep rotation on horizontal plane only (ignore y axis)
        direction.y = 0;

        if (direction != Vector3.zero)
        {
            // smoothly rotate to face player using quaternion slerp
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
        }
    }

    void TryAttack()
    {
        if (isAttacking) return;

        float attackCooldown = attackAnimationLength / attackSpeed;
        if (Time.time - lastAttackTime < attackCooldown) return;

        isAttacking = true;
        lastAttackTime = Time.time;

        if (animator != null)
        {
            animator.speed = attackSpeed;
            animator.Play(ATTACK, 0, 0f);
        }

        float delayToHit = attackDelay / attackSpeed;
        float attackDuration = attackAnimationLength / attackSpeed;

        // invoke calls methods after a delay, damage happens partway through animation
        Invoke(nameof(DealDamage), delayToHit);
        Invoke(nameof(ResetAttack), attackDuration);
    }

    void DealDamage()
    {
        if (player == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        if (distanceToPlayer <= attackRange)
        {
            Actor playerActor = player.GetComponent<Actor>();

            if (playerActor != null)
            {
                playerActor.TakeDamage(attackDamage);

                if (hitEffect != null)
                {
                    Instantiate(hitEffect, player.position + new Vector3(0, 1, 0), Quaternion.identity);
                }
            }
        }
    }

    void ResetAttack()
    {
        isAttacking = false;
        if (animator != null)
        {
            animator.speed = 1f;
        }
    }

    void SetAnimations()
    {
        if (animator == null) return;
        if (isAttacking) return;

        if (agent.velocity.magnitude > 0.1f)
        {
            animator.Play(WALK);
        }
        else
        {
            animator.Play(IDLE);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }

    void OnDestroy()
    {
        if (actor != null)
        {
            actor.OnDeath -= OnEnemyDeath;
        }
    }
}
