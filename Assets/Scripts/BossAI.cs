using UnityEngine;
using UnityEngine.AI;

public class BossAI : MonoBehaviour
{
    const string IDLE = "Idle";
    const string WALK = "Walk";
    const string ATTACK = "Attack";
    const string SPECIAL_ATTACK = "SpecialAttack";

    [Header("Boss Settings")]
    [SerializeField] private float detectionRange = 20f;
    [SerializeField] private float attackRange = 2f;
    [SerializeField] private float specialAttackRange = 8f;
    [SerializeField] private int attackDamage = 3;
    [SerializeField] private int specialAttackDamage = 5;
    [SerializeField] private float attackSpeed = 1.0f;
    [SerializeField] private float attackDelay = 0.3f;
    [SerializeField] private float specialAttackCooldown = 10f;
    [SerializeField] private ParticleSystem hitEffect;
    [SerializeField] private ParticleSystem specialAttackEffect;

    [Header("HFSM")]
    [SerializeField] private HFSM hfsm;

    private Transform player;
    private NavMeshAgent agent;
    private Animator animator;
    private Actor actor;

    private bool isAttacking = false;
    private bool isSpecialAttacking = false;
    private float attackAnimationLength = 0f;
    private float lastAttackTime = 0f;
    private float lastSpecialAttackTime = 0f;

    // HFSM States
    private BossIdleState idleState;
    private BossCombatState combatState;
    private BossDeadState deadState;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        actor = GetComponent<Actor>();

        if (hfsm == null)
        {
            hfsm = gameObject.AddComponent<HFSM>();
        }
    }

    void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
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
            actor.OnDeath += OnBossDeath;
        }

        // Initialize HFSM states
        InitializeHFSM();
    }

    void InitializeHFSM()
    {
        // Create states
        idleState = new BossIdleState(gameObject, hfsm, this);
        combatState = new BossCombatState(gameObject, hfsm, this);
        deadState = new BossDeadState(gameObject, hfsm, this);

        // Create combat sub-states
        BossApproachState approachState = new BossApproachState(gameObject, hfsm, combatState, this);
        BossAttackState attackState = new BossAttackState(gameObject, hfsm, combatState, this);
        BossRetreatState retreatState = new BossRetreatState(gameObject, hfsm, combatState, this);

        // Set sibling state references for easier transitions
        approachState.SetSiblingStates(attackState, retreatState);
        attackState.SetSiblingStates(approachState, retreatState);
        retreatState.SetSiblingStates(approachState);

        // Add sub-states to combat state
        combatState.AddSubState(approachState);
        combatState.AddSubState(attackState);
        combatState.AddSubState(retreatState);

        // Start with idle state
        if (hfsm != null)
        {
            hfsm.ChangeState(idleState);
        }
    }

    void Update()
    {
        if (player == null || actor == null || !actor.IsAlive()) return;

        // HFSM will handle state transitions
        // But we can also check conditions here for immediate responses
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        float healthPercentage = (float)actor.currentHealth / actor.maxHealth;

        // Transition to combat if player is in range
        if (distanceToPlayer <= detectionRange && hfsm.CurrentState == idleState)
        {
            hfsm.ChangeState(combatState);
        }

        // Transition to dead if health is 0
        if (!actor.IsAlive() && hfsm.CurrentState != deadState)
        {
            hfsm.ChangeState(deadState);
        }
    }

    public void TryAttack()
    {
        if (isAttacking || isSpecialAttacking) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        float attackCooldown = attackAnimationLength / attackSpeed;

        // Use special attack if available and player is at medium range
        if (distanceToPlayer <= specialAttackRange && distanceToPlayer > attackRange && 
            Time.time - lastSpecialAttackTime >= specialAttackCooldown)
        {
            TrySpecialAttack();
            return;
        }

        // Use normal attack if in range
        if (distanceToPlayer <= attackRange && Time.time - lastAttackTime >= attackCooldown)
        {
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
    }

    public void TrySpecialAttack()
    {
        if (isAttacking || isSpecialAttacking) return;

        isSpecialAttacking = true;
        lastSpecialAttackTime = Time.time;

        if (animator != null)
        {
            animator.speed = attackSpeed * 0.8f; // Slower for special attack
            animator.Play(SPECIAL_ATTACK, 0, 0f);
        }

        float delayToHit = attackDelay / attackSpeed;
        float attackDuration = attackAnimationLength / attackSpeed * 1.5f; // Longer animation

        Invoke(nameof(DealSpecialDamage), delayToHit);
        Invoke(nameof(ResetSpecialAttack), attackDuration);
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

    void DealSpecialDamage()
    {
        if (player == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        if (distanceToPlayer <= specialAttackRange)
        {
            Actor playerActor = player.GetComponent<Actor>();
            if (playerActor != null)
            {
                playerActor.TakeDamage(specialAttackDamage);
                if (specialAttackEffect != null)
                {
                    Instantiate(specialAttackEffect, player.position + new Vector3(0, 1, 0), Quaternion.identity);
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

    void ResetSpecialAttack()
    {
        isSpecialAttacking = false;
        if (animator != null)
        {
            animator.speed = 1f;
        }
    }

    void OnBossDeath(Actor deadActor)
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.AddKill();
            // Boss kill gives extra points
            GameManager.Instance.AddScore(50);
        }
    }

    void OnDestroy()
    {
        if (actor != null)
        {
            actor.OnDeath -= OnBossDeath;
        }
    }

    // HFSM State Classes
    public class BossIdleState : HFSMState
    {
        private BossAI bossAI;

        public BossIdleState(GameObject gameObject, HFSM fsm, BossAI bossAI) : base(gameObject, fsm, null)
        {
            this.bossAI = bossAI;
        }

        public override void Update()
        {
            base.Update();
            // Idle behavior - look around, patrol, etc.
        }
    }

    public class BossCombatState : HFSMState
    {
        private BossAI bossAI;

        public BossCombatState(GameObject gameObject, HFSM fsm, BossAI bossAI) : base(gameObject, fsm, null)
        {
            this.bossAI = bossAI;
        }

        public override void Enter()
        {
            base.Enter();
            // Start with approach sub-state
            if (GetSubStateCount() > 0)
            {
                ChangeSubState(GetSubState(0)); // Approach state
            }
        }
    }

    public class BossApproachState : HFSMState
    {
        private BossAI bossAI;
        private HFSMState attackState;
        private HFSMState retreatState;

        public BossApproachState(GameObject gameObject, HFSM fsm, HFSMState parent, BossAI bossAI) : base(gameObject, fsm, parent)
        {
            this.bossAI = bossAI;
        }

        public void SetSiblingStates(HFSMState attack, HFSMState retreat)
        {
            attackState = attack;
            retreatState = retreat;
        }

        public override void Update()
        {
            base.Update();
            
            if (bossAI.player == null) return;

            float distance = Vector3.Distance(transform.position, bossAI.player.position);
            
            if (distance <= bossAI.attackRange && parentState != null)
            {
                // Transition to attack state
                if (attackState != null)
                {
                    parentState.ChangeSubState(attackState);
                }
                else if (parentState.GetSubStateCount() > 1)
                {
                    parentState.ChangeSubState(parentState.GetSubState(1));
                }
            }
            else
            {
                // Move towards player
                if (bossAI.agent != null)
                {
                    bossAI.agent.SetDestination(bossAI.player.position);
                }
            }
        }
    }

    public class BossAttackState : HFSMState
    {
        private BossAI bossAI;
        private HFSMState approachState;
        private HFSMState retreatState;

        public BossAttackState(GameObject gameObject, HFSM fsm, HFSMState parent, BossAI bossAI) : base(gameObject, fsm, parent)
        {
            this.bossAI = bossAI;
        }

        public void SetSiblingStates(HFSMState approach, HFSMState retreat)
        {
            approachState = approach;
            retreatState = retreat;
        }

        public override void Update()
        {
            base.Update();
            
            if (bossAI.player == null) return;

            float distance = Vector3.Distance(transform.position, bossAI.player.position);
            
            bossAI.TryAttack();

            if (parentState != null)
            {
                if (distance > bossAI.attackRange * 1.5f)
                {
                    // Too far, go back to approach
                    if (approachState != null)
                    {
                        parentState.ChangeSubState(approachState);
                    }
                    else if (parentState.GetSubStateCount() > 0)
                    {
                        parentState.ChangeSubState(parentState.GetSubState(0));
                    }
                }
                else if (distance < bossAI.attackRange * 0.5f)
                {
                    // Too close, retreat
                    if (retreatState != null)
                    {
                        parentState.ChangeSubState(retreatState);
                    }
                    else if (parentState.GetSubStateCount() > 2)
                    {
                        parentState.ChangeSubState(parentState.GetSubState(2));
                    }
                }
            }
        }
    }

    public class BossRetreatState : HFSMState
    {
        private BossAI bossAI;
        private HFSMState approachState;

        public BossRetreatState(GameObject gameObject, HFSM fsm, HFSMState parent, BossAI bossAI) : base(gameObject, fsm, parent)
        {
            this.bossAI = bossAI;
        }

        public void SetSiblingStates(HFSMState approach)
        {
            approachState = approach;
        }

        public override void Update()
        {
            base.Update();
            
            if (bossAI.player == null) return;

            float distance = Vector3.Distance(transform.position, bossAI.player.position);
            
            // Move away from player
            Vector3 retreatDirection = (transform.position - bossAI.player.position).normalized;
            Vector3 retreatPosition = transform.position + retreatDirection * 3f;
            
            if (bossAI.agent != null)
            {
                NavMeshHit hit;
                if (NavMesh.SamplePosition(retreatPosition, out hit, 2f, NavMesh.AllAreas))
                {
                    bossAI.agent.SetDestination(hit.position);
                }
            }

            if (distance >= bossAI.attackRange && parentState != null)
            {
                // Far enough, go back to approach
                if (approachState != null)
                {
                    parentState.ChangeSubState(approachState);
                }
                else if (parentState.GetSubStateCount() > 0)
                {
                    parentState.ChangeSubState(parentState.GetSubState(0));
                }
            }
        }
    }

    public class BossDeadState : HFSMState
    {
        public BossDeadState(GameObject gameObject, HFSM fsm, BossAI bossAI) : base(gameObject, fsm, null)
        {
        }

        public override void Enter()
        {
            base.Enter();
            // Death animation, cleanup, etc.
        }
    }
}

