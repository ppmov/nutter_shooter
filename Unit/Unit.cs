using UnityEngine;
using UnityEngine.AI;
using static UnitsGetter;
using static UnityEngine.Mathf;

public class Unit : MonoBehaviour
{
    private Ability AbilityByClipName(string clip)
    {
        if (clip.StartsWith("aa"))
            return AttackAbility;
        else
        if (clip.StartsWith("ab"))
            return BuffAbility;

        return null;
    }

    public bool IsPlayer { get; set; }
    public bool CanNavRotate { set => navAgent.updateRotation = value; }

    public Ability AttackAbility { get; private set; }
    public Ability BuffAbility { get; private set; }

    private bool CanMove { get => vulner == null ? true : vulner.CanMove; }

    private Vulnerable vulner;
    private Animator animator;
    private NavMeshAgent navAgent;
    private float defaultMoveSpeed;
    private float lastMoveAiTime;
    private Vector3 lastDirection;

    public bool TryAttack()
    {
        if (AttackAbility != null && AttackAbility.TryUse())
        {
            animator.SetTrigger("attack");
            navAgent.ResetPath();
            return true;
        }

        return false;
    }

    public bool TryBuff()
    {
        if (BuffAbility != null && BuffAbility.TryUse())
        {
            animator.SetTrigger("buff");
            navAgent.ResetPath();
            return true;
        }

        return false;
    }

    // ability could called from animation
    public void OnLaunch(string clip)
    {
        AbilityByClipName(clip).OnLaunchEvent.Invoke();
    }

    // rotation
    public void SetLookVector(Vector3 direction)
    {
        transform.rotation = Quaternion.LookRotation(direction);
    }

    public Vulnerable RotateToFirstTarget()
    {
        Vulnerable target = FindFirstFreeTarget(transform.position);

        if (target != null)
            SetLookVector(target.transform.position - transform.position);

        return target;
    }

    public Vulnerable RotateToNearestTarget()
    {
        Vulnerable target = FindNearestTarget(transform);

        if (target != null)
            SetLookVector(target.transform.position - transform.position);

        return target;
    }

    // moving
    public void SetMoveVector(Vector3 direction)
    {
        if (navAgent == null) 
            return;

        if (direction == Vector3.zero)
        {
            navAgent.ResetPath();
            return;
        }

        if (AttackAbility != null && AttackAbility.IsHandling) 
            return;

        if (BuffAbility != null && BuffAbility.IsHandling) 
            return;

        if (!CanMove) 
            return;

        navAgent.SetDestination(transform.position + direction);
    }

    private void Awake()
    {
        Ability[] abilities = GetComponents<Ability>();
        AttackAbility = abilities?[0].enabled ?? false ? abilities[0] : null;
        BuffAbility = abilities?[1].enabled ?? false ? abilities[1] : null;

        animator = GetComponent<Animator>();
        navAgent = GetComponent<NavMeshAgent>();
        vulner = GetComponent<Vulnerable>();
        vulner.OnDamage.AddListener(OnDealDamage);
        defaultMoveSpeed = navAgent.speed;
    }

    private void Update()
    {
        UpdateNavMeshAgent();
        UpdateAnimations();
    }

    private void UpdateNavMeshAgent()
    {
        if (navAgent == null) 
            return;

        float angleRad = Vector3.Angle(navAgent.destination - transform.position, transform.forward) * Deg2Rad;
        navAgent.speed = defaultMoveSpeed - defaultMoveSpeed * angleRad / 2f / PI;
    }

    private void UpdateAnimations()
    {
        if (animator == null) 
            return;

        // animator states
        animator.SetBool("move", navAgent.hasPath);
        animator.SetFloat("moveSpeedKf", navAgent.speed / defaultMoveSpeed);

        if (AttackAbility != null)
            animator.SetFloat("attackSpeedKf", AttackAbility.Speed);

        // add animation events if needed
        foreach (AnimationClip clip in animator.runtimeAnimatorController.animationClips)
            TryPrepareAnimEvents(clip);
    }

    private void TryPrepareAnimEvents(AnimationClip clip)
    {
        Ability ability = AbilityByClipName(clip.name);

        if (ability == null)
            return;

        foreach (AnimationEvent evnt in clip.events)
            if (evnt.functionName == nameof(OnLaunch))
                return;

        AnimationEvent onLaunch = new AnimationEvent();
        onLaunch.time = ability.LaunchFromAnimationTime;
        onLaunch.functionName = nameof(OnLaunch);
        onLaunch.stringParameter = clip.name;
        clip.AddEvent(onLaunch);
    }

    private void FixedUpdate()
    {
        if (!IsPlayer)
        {
            // AI logic
            if (RotateToFirstTarget() == null)
            {
                if (!TryBuff())
                    MoveAnywhere();
            }
            else
            {
                if (!TryAttack())
                    if (!TryBuff())
                        MoveAnywhere();

                if (vulner != null && vulner.HealthAmount <= 0.3f)
                    MoveAnywhere();
            }
        }
    }

    // random moving for AI
    private void MoveAnywhere()
    {
        if (navAgent == null)
            return;

        if (navAgent.hasPath)
            return;

        Vector3 direction;

        if (Time.time - lastMoveAiTime >= 1f)
        {
            while (true)
            {
                direction = new Vector3(Random.Range(-1, 2), 0, Random.Range(-1, 2));
                direction.Scale(new Vector3(Random.Range(5, 7), 0, Random.Range(5, 7)));
                Vector3 ground = transform.position + direction;
                ground.y = 0;

                if (Physics.Raycast(ground + Vector3.up, -Vector3.up, 2f, 1 << 6))
                    break;
            }

            lastMoveAiTime = Time.time;
            lastDirection = direction;
        }
        else
            direction = lastDirection;

        SetMoveVector(direction);
    }

    private void OnDealDamage()
    {
        if (vulner.HealthAmount <= 0f)
            Death();
    }

    private void Death()
    {
        animator.SetTrigger("death");

        foreach (AnimationClip clip in animator.runtimeAnimatorController.animationClips)
            if (AbilityByClipName(clip.name) != null)
                clip.events = null;

        Destroy(AttackAbility);
        Destroy(BuffAbility);
        Destroy(navAgent);
        Destroy(GetComponent<Collider>());
        Destroy(GetComponent<Vulnerable>());
        Destroy(this);
    }
}
