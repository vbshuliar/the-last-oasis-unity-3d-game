using UnityEngine;
using UnityEngine.AI;

public class FastEnemyAI : MonoBehaviour
{
    const string IDLE = "Idle";
    const string WALK = "Walk";
    const string ATTACK = "Attack";

    [Header("Combat Settings")]
    [SerializeField] private float detectionRange = 12f;
    [SerializeField] private float attackRange = 1.5f;
    [SerializeField] private int attackDamage = 1;
    [SerializeField] private float attackSpeed = 1.2f;
    [SerializeField] private float attackDelay = 0.2f;
    [SerializeField] private ParticleSystem hitEffect;

    [Header("Steering")]
    [SerializeField] private PursuitBehaviour pursuitBehaviour;
    [SerializeField] private ObstacleAvoidanceBehaviour obstacleAvoidanceBehaviour;

    private Transform player;
    private NavMeshAgent agent;
    private Animator animator;
    private Actor actor;
    private Rigidbody rb;

    private bool isAttacking = false;
    private float attackAnimationLength = 0f;
    private float lastAttackTime = 0f;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        actor = GetComponent<Actor>();
        rb = GetComponent<Rigidbody>();

        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
            rb.isKinematic = true;
        }

        // Get or add steering behaviours
        if (pursuitBehaviour == null)
        {
            pursuitBehaviour = GetComponent<PursuitBehaviour>();
            if (pursuitBehaviour == null)
            {
                pursuitBehaviour = gameObject.AddComponent<PursuitBehaviour>();
            }
        }

        if (obstacleAvoidanceBehaviour == null)
        {
            obstacleAvoidanceBehaviour = GetComponent<ObstacleAvoidanceBehaviour>();
            if (obstacleAvoidanceBehaviour == null)
            {
                obstacleAvoidanceBehaviour = gameObject.AddComponent<ObstacleAvoidanceBehaviour>();
            }
        }
    }

    void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
            if (pursuitBehaviour != null)
            {
                pursuitBehaviour.SetTarget(player);
            }
        }

        // Fast enemy has higher speed
        if (agent != null)
        {
            agent.speed *= 1.5f;
        }

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

        if (actor != null)
        {
            actor.OnDeath += OnEnemyDeath;
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
                // Use pursuit steering to chase player
                ApplyPursuitSteering();
            }
        }

        SetAnimations();
    }

    void ApplyPursuitSteering()
    {
        if (pursuitBehaviour != null)
        {
            Vector3 pursuitForce = pursuitBehaviour.CalculateForce();
            
            // Combine with obstacle avoidance
            Vector3 avoidanceForce = Vector3.zero;
            if (obstacleAvoidanceBehaviour != null)
            {
                avoidanceForce = obstacleAvoidanceBehaviour.CalculateForce();
            }

            Vector3 combinedForce = pursuitForce + avoidanceForce;
            Vector3 newPosition = transform.position + combinedForce * Time.deltaTime;
            
            // Use NavMesh to validate and set destination
            NavMeshHit hit;
            if (NavMesh.SamplePosition(newPosition, out hit, 2f, NavMesh.AllAreas))
            {
                agent.SetDestination(hit.position);
            }
        }
        else
        {
            agent.SetDestination(player.position);
        }
    }

    void FacePlayer()
    {
        Vector3 direction = (player.position - transform.position).normalized;
        direction.y = 0;

        if (direction != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 8f);
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

    void OnEnemyDeath(Actor deadActor)
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.AddKill();
        }
    }

    void OnDestroy()
    {
        if (actor != null)
        {
            actor.OnDeath -= OnEnemyDeath;
        }
    }
}

