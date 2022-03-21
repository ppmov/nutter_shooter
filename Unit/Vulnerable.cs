using System.Collections;
using UnityEngine;
using UnityEngine.Events;

// unit external interaction class
public class Vulnerable : MonoBehaviour
{
    [SerializeField]
    private float maxHealth;

    private float health;
    private Unit unit;
    private Healthbar healthbar;

    public bool IsPlayer { get => unit.IsPlayer; }
    public float CurrentDamage { get; private set; }
    public float HealthAmount { get => health / maxHealth; }
    public bool CanMove { get; private set; } = true;

    public UnityEvent OnDamage;

    void Start()
    {
        health = maxHealth;
        unit = GetComponent<Unit>();
        healthbar = GetComponentInChildren<Healthbar>();
    }

    public void DealDamage(float damage)
    {
        health -= damage;
        OnDamage.Invoke();
    }

    public void Stun(float time)
    {
        if (unit == null) return;
        unit.SetMoveVector(Vector3.zero);
        StartCoroutine(Stunning());

        IEnumerator Stunning()
        {
            CanMove = false;
            yield return new WaitForSeconds(time);
            CanMove = true;
        }
    }

    public void Highlight(bool state)
    {
        healthbar.Highlight(state);
    }

    public void ScaleAbilitySpeed(float scale, float time)
    {
        unit.AttackAbility.ScaleSpeed(scale, time);
    }
}
