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
    [Header("Flee Behaviour")]
    [SerializeField, Range(0.05f, 0.95f)] float fleeHealthThreshold = 0.4f;
    [SerializeField] float fleeDistance = 12f;
    [SerializeField, Range(0.1f, 1f)] float fleeSpeedMultiplier = 0.5f;

    Transform player;
    NavMeshAgent agent;
    Animator animator;
    Actor actor;

    bool isAttacking = false;
    bool isFleeing = false;
    float attackAnimationLength = 0f;
    float lastAttackTime = 0f;
    float baseAgentSpeed = 0f;

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

        if (agent != null)
        {
            baseAgentSpeed = agent.speed;
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

        bool shouldFlee = ShouldFlee();
        if (shouldFlee)
        {
            HandleFleeing();
            SetAnimations();
            return;
        }
        else if (isFleeing)
        {
            ExitFleeState();
        }

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

    bool ShouldFlee()
    {
        if (actor == null || actor.maxHealth <= 0)
        {
            return false;
        }

        float healthPercent = (float)actor.currentHealth / actor.maxHealth;
        return healthPercent <= fleeHealthThreshold;
    }

    void HandleFleeing()
    {
        EnterFleeState();

        if (agent == null || player == null)
        {
            return;
        }

        Vector3 fleeDirection = (transform.position - player.position).normalized;
        if (fleeDirection.sqrMagnitude < 0.01f)
        {
            fleeDirection = -player.forward;
        }

        Vector3 targetPosition = transform.position + fleeDirection * fleeDistance;
        if (UnityEngine.AI.NavMesh.SamplePosition(targetPosition, out UnityEngine.AI.NavMeshHit hit, fleeDistance, UnityEngine.AI.NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
        }
        else
        {
            agent.SetDestination(transform.position + fleeDirection * Mathf.Max(2f, fleeDistance * 0.25f));
        }

        FaceDirection(fleeDirection);
        isAttacking = false;
    }

    void EnterFleeState()
    {
        if (agent == null)
        {
            return;
        }

        isFleeing = true;
        float targetSpeed = baseAgentSpeed > 0f ? baseAgentSpeed * fleeSpeedMultiplier : agent.speed * fleeSpeedMultiplier;
        agent.speed = targetSpeed;
    }

    void ExitFleeState()
    {
        isFleeing = false;
        if (agent != null && baseAgentSpeed > 0f)
        {
            agent.speed = baseAgentSpeed;
        }
    }

    void FacePlayer()
    {
        Vector3 direction = (player.position - transform.position).normalized;
        // keep rotation on horizontal plane only (ignore y axis)
        direction.y = 0;

        FaceDirection(direction);
    }

    void FaceDirection(Vector3 direction)
    {
        if (direction == Vector3.zero)
        {
            return;
        }

        Quaternion lookRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
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

                if (AudioManager.Instance != null)
                {
                    AudioManager.Instance.PlayPunchSound(player.position);
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
