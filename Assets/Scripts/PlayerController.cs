using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.AI;

// handles player movement attacking and item pickup using click to move controls
public class PlayerController : MonoBehaviour
{
    const string IDLE = "Idle";
    const string WALK = "Walk";
    const string ATTACK = "Attack";
    const string PICKUP = "Pickup";

    CustomActions input;

    NavMeshAgent agent;
    Animator animator;
    Actor actor;

    [Header("Movement")]
    [SerializeField] ParticleSystem clickEffect;
    [SerializeField] LayerMask clickableLayers;

    float lookRotationSpeed = 8f;
    Vector3 lastMoveDirection;

    float clickInterval = 0.2f;
    float lastClickTime = 0f;

    [Header("Attack")]
    [SerializeField] float attackSpeed = 1.5f;
    [SerializeField] float attackDelay = 0.3f;
    [SerializeField] float attackDistance = 1.5f;
    [SerializeField] int attackDamage = 1;
    [SerializeField] ParticleSystem hitEffect;

    bool playerBusy = false;
    Interactable target;
    float attackAnimationLength = 0f;

    bool isPoweredUp = false;
    Vector3 originalScale;
    float originalSpeed;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        actor = GetComponent<Actor>();

        input = new CustomActions();
    }

    void Start()
    {
        // loop through all animations to find attack animation length
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

        originalScale = transform.localScale;
        originalSpeed = agent.speed;

        // subscribe to death event so we know when player dies
        if (actor != null)
        {
            actor.OnDeath += OnPlayerDeath;
        }

        if (GameManager.Instance != null)
        {
            GameManager.Instance.StartGame();
        }
    }

    void OnPlayerDeath(Actor deadActor)
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.GameOver();
        }
    }

    void ClickToMove()
    {
        // raycast shoots invisible line from camera through mouse position to see what was clicked
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100, clickableLayers))
        {
            if (hit.transform.CompareTag("Interactable"))
            {
                target = hit.transform.GetComponent<Interactable>();
                if (clickEffect != null)
                { Instantiate(clickEffect, hit.transform.position + new Vector3(0, 0.1f, 0), clickEffect.transform.rotation); }
            }
            else
            {
                target = null;
                // navmesh agent automatically finds path to destination
                agent.destination = hit.point;
                if (clickEffect != null)
                { Instantiate(clickEffect, hit.point + new Vector3(0, 0.1f, 0), clickEffect.transform.rotation); }
            }
        }
    }

    void OnEnable()
    { input.Enable(); }

    void OnDisable()
    { input.Disable(); }

    void Update()
    {
        if (actor != null && !actor.IsAlive())
        {
            agent.SetDestination(transform.position);
            return;
        }

        HandleMouseInput();
        FollowTarget();
        FaceTarget();
        SetAnimations();
    }

    void HandleMouseInput()
    {
        if (Mouse.current == null) return;

        if (Mouse.current.rightButton.wasPressedThisFrame)
        {
            ClickToMove();
            lastClickTime = Time.time;
        }
        else if (Mouse.current.rightButton.isPressed)
        {
            float timeSinceLastClick = Time.time - lastClickTime;
            if (timeSinceLastClick >= clickInterval)
            {
                ClickToMove();
                lastClickTime = Time.time;
            }
        }
    }

    void FollowTarget()
    {
        if (target == null) return;

        if (Vector3.Distance(target.transform.position, transform.position) <= attackDistance)
        { ReachDistance(); }
        else
        { agent.SetDestination(target.transform.position); }
    }

    void FaceTarget()
    {
        Vector3 facing = Vector3.zero;

        if (target != null)
        {
            facing = target.transform.position;
        }
        else
        {
            if (agent.velocity.sqrMagnitude > 0.1f)
            {
                lastMoveDirection = agent.velocity.normalized;
            }

            if (lastMoveDirection != Vector3.zero)
            {
                facing = transform.position + lastMoveDirection;
            }
            else
            {
                return;
            }
        }

        // smoothly rotate to face target using quaternion slerp (spherical interpolation)
        Vector3 direction = (facing - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * lookRotationSpeed);
    }

    void ReachDistance()
    {
        agent.SetDestination(transform.position);

        if (playerBusy) return;

        playerBusy = true;

        switch (target.interactionType)
        {
            case InteractableType.Enemy:
                animator.speed = attackSpeed;
                animator.Play(ATTACK, 0, 0f);

                float attackDuration = attackAnimationLength / attackSpeed;
                float delayToHit = attackDelay / attackSpeed;

                // invoke calls a method after a delay, damage happens partway through animation
                Invoke(nameof(SendAttack), delayToHit);
                Invoke(nameof(ResetBusyState), attackDuration);
                break;
            case InteractableType.Item:
                target.InteractWithItem(this);
                target = null;

                Invoke(nameof(ResetBusyState), 0.5f);
                break;
        }
    }

    void SendAttack()
    {
        if (target == null) return;

        if (actor != null && !actor.IsAlive()) return;

        if (target.myActor != null && !target.myActor.IsAlive())
        { target = null; return; }

        Instantiate(hitEffect, target.transform.position + new Vector3(0, 1, 0), Quaternion.identity);
        target.GetComponent<Actor>().TakeDamage(attackDamage);
    }

    void ResetBusyState()
    {
        playerBusy = false;
        animator.speed = 1f;
        SetAnimations();
    }

    void SetAnimations()
    {
        if (playerBusy) return;

        if (agent.velocity.magnitude > 0.1f)
        { animator.Play(WALK); }
        else
        { animator.Play(IDLE); }
    }

    public void ApplyGreenPotionEffect(float sizeMultiplier, float speedMultiplier, float duration)
    {
        if (isPoweredUp)
        {
            StopCoroutine("PowerUpCoroutine");
        }

        StartCoroutine(PowerUpCoroutine(sizeMultiplier, speedMultiplier, duration));
    }

    // coroutine runs over multiple frames and can wait for time to pass
    IEnumerator PowerUpCoroutine(float sizeMultiplier, float speedMultiplier, float duration)
    {
        isPoweredUp = true;

        transform.localScale = originalScale * sizeMultiplier;
        agent.speed = originalSpeed * speedMultiplier;

        // wait for duration seconds
        yield return new WaitForSeconds(duration);

        transform.localScale = originalScale;
        agent.speed = originalSpeed;

        isPoweredUp = false;
    }

    void OnDestroy()
    {
        if (actor != null)
        {
            actor.OnDeath -= OnPlayerDeath;
        }
    }
}
