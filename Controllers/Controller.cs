using UnityEngine;
using UnityEngine.UI;

public class Controller : MonoBehaviour
{
    [SerializeField]
    private Text leftHandMagazine;
    [SerializeField]
    private Text rightHandMagazine;

    [SerializeField]
    private Joystick moveStick;
    [SerializeField]
    private Joystick attackStick;
    [SerializeField]
    private GameObject buffButton;

    private GameManager manager;

    public Unit PlayerUnit { get => manager.Player; }
    private Vulnerable Highlighted { get; set; }

    private bool IsMoving { get => moveStick != null && moveStick.DragDirection != Vector2.zero; }
    private bool IsAiming { get => attackStick != null && attackStick.DragDirection != Vector2.zero; }

    private void Start()
    {
        manager = GetComponent<GameManager>();

        leftHandMagazine.gameObject.SetActive(PlayerUnit.AttackAbility != null);
        rightHandMagazine.gameObject.SetActive(PlayerUnit.BuffAbility != null);
        attackStick.gameObject.SetActive(PlayerUnit.AttackAbility != null);
        buffButton.gameObject.SetActive(PlayerUnit.BuffAbility != null);
        PlayerUnit.AttackAbility.OnLaunchEvent.AddListener(OnAttackLaunch);
    }

    // shoot ability button
    public void OnLeftClick()
    {
        if (PlayerUnit == null)
            return;

        // rotate unit if there are no targeted enemy
        if (Highlighted != null)
            PlayerUnit.SetLookVector(Highlighted.transform.position - PlayerUnit.transform.position);

        PlayerUnit.TryAttack();
    }

    // buff ability button
    public void OnRightClick()
    {
        if (PlayerUnit == null)
            return;

        PlayerUnit.TryBuff();
    }

    private void OnGUI()
    {
        // abilities current state
        if (PlayerUnit == null)
            return;

        if (PlayerUnit.AttackAbility != null)
        {
            leftHandMagazine.text = PlayerUnit.AttackAbility.IsReloading ? "..." : PlayerUnit.AttackAbility.Magazine;
            leftHandMagazine.color = PlayerUnit.AttackAbility.IsEmpty && !PlayerUnit.AttackAbility.IsReloading ? Color.red : Color.white;
        }

        if (PlayerUnit.BuffAbility != null)
        {
            rightHandMagazine.text = PlayerUnit.BuffAbility.IsReloading ? "..." : PlayerUnit.BuffAbility.Magazine;
            rightHandMagazine.color = PlayerUnit.BuffAbility.IsEmpty && !PlayerUnit.BuffAbility.IsReloading ? Color.red : Color.white;
        }
    }

    private void Update()
    { 
        if (PlayerUnit == null) 
            return;

        if (IsMoving)
            PlayerUnit.SetMoveVector(new Vector3(moveStick.DragDirection.x, 0f, moveStick.DragDirection.y));

        if (IsAiming)
        {
            // if player is trying to aim, disable auto-rotate
            PlayerUnit.CanNavRotate = false;
            PlayerUnit.SetLookVector(new Vector3(attackStick.DragDirection.x, 0f, attackStick.DragDirection.y));
            // highlight enemy in field of view
            Highlight(UnitsGetter.FindNearestTarget(PlayerUnit.transform, 30f));
        }
    }

    private void OnAttackLaunch()
    {
        // refresh aiming after shoot
        PlayerUnit.CanNavRotate = true;
        Highlight();
    }

    private void Highlight(Vulnerable target = null)
    {
        // disable current highlight
        if (Highlighted != null)
            Highlighted.Highlight(false);

        if (target == null)
            return;

        // highlight new target
        target.Highlight(true);
        Highlighted = target;
    }
}
