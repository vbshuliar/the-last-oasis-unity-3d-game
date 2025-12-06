using UnityEngine;
using UnityEngine.AI;

public class RangedEnemyAI : MonoBehaviour
{
    const string IDLE = "Idle";
    const string WALK = "Walk";
    const string ATTACK = "Attack";

    [Header("Combat Settings")]
    [SerializeField] private float detectionRange = 15f;
    [SerializeField] private float attackRange = 10f;
    [SerializeField] private int attackDamage = 1;
    [SerializeField] private float attackSpeed = 1.0f;
    [SerializeField] private float attackDelay = 0.3f;
    [SerializeField] private ParticleSystem hitEffect;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform firePoint;

    [Header("Steering")]
    [SerializeField] private SeekBehaviour seekBehaviour;
    [SerializeField] private FleeBehaviour fleeBehaviour;
    [SerializeField] private float fleeHealthThreshold = 0.3f; // Flee when health is below 30%

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
        if (seekBehaviour == null)
        {
            seekBehaviour = GetComponent<SeekBehaviour>();
            if (seekBehaviour == null)
            {
                seekBehaviour = gameObject.AddComponent<SeekBehaviour>();
            }
        }

        if (fleeBehaviour == null)
        {
            fleeBehaviour = GetComponent<FleeBehaviour>();
            if (fleeBehaviour == null)
            {
                fleeBehaviour = gameObject.AddComponent<FleeBehaviour>();
            }
        }
    }

    void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
            if (seekBehaviour != null)
            {
                seekBehaviour.SetTarget(player);
            }
            if (fleeBehaviour != null)
            {
                fleeBehaviour.SetThreat(player);
            }
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
        float healthPercentage = (float)actor.currentHealth / actor.maxHealth;

        // Determine behaviour based on health and distance
        if (healthPercentage < fleeHealthThreshold && distanceToPlayer < 5f)
        {
            // Flee when low on health
            ApplyFleeSteering();
        }
        else if (distanceToPlayer <= detectionRange)
        {
            FacePlayer();

            if (distanceToPlayer <= attackRange && distanceToPlayer > 3f) // Keep distance for ranged attack
            {
                agent.SetDestination(transform.position);
                TryAttack();
            }
            else if (distanceToPlayer > attackRange)
            {
                // Use seek steering to approach
                ApplySeekSteering();
            }
            else if (distanceToPlayer <= 3f)
            {
                // Too close, back away
                ApplyFleeSteering();
            }
        }

        SetAnimations();
    }

    void ApplySeekSteering()
    {
        if (seekBehaviour != null)
        {
            Vector3 steeringForce = seekBehaviour.CalculateForce();
            Vector3 newPosition = transform.position + steeringForce * Time.deltaTime;
            
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

    void ApplyFleeSteering()
    {
        if (fleeBehaviour != null)
        {
            Vector3 steeringForce = fleeBehaviour.CalculateForce();
            Vector3 newPosition = transform.position + steeringForce * Time.deltaTime;
            
            // Use NavMesh to validate and set destination
            NavMeshHit hit;
            if (NavMesh.SamplePosition(newPosition, out hit, 2f, NavMesh.AllAreas))
            {
                agent.SetDestination(hit.position);
            }
        }
    }

    void FacePlayer()
    {
        Vector3 direction = (player.position - transform.position).normalized;
        direction.y = 0;

        if (direction != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
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

        Invoke(nameof(ShootProjectile), delayToHit);
        Invoke(nameof(ResetAttack), attackDuration);
    }

    void ShootProjectile()
    {
        if (player == null || projectilePrefab == null) return;

        Vector3 firePosition = firePoint != null ? firePoint.position : transform.position + Vector3.up;
        Vector3 direction = (player.position - firePosition).normalized;

        GameObject projectile = Instantiate(projectilePrefab, firePosition, Quaternion.LookRotation(direction));
        
        // You may want to add a Projectile script to handle damage
        // For now, we'll use a simple raycast approach
        RaycastHit hit;
        if (Physics.Raycast(firePosition, direction, out hit, attackRange))
        {
            Actor targetActor = hit.collider.GetComponent<Actor>();
            if (targetActor != null && targetActor.CompareTag("Player"))
            {
                targetActor.TakeDamage(attackDamage);
                if (hitEffect != null)
                {
                    Instantiate(hitEffect, hit.point, Quaternion.identity);
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

