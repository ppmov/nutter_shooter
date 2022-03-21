using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlowEffect : Effect
{
    [SerializeField]
    private float damage;

    protected override void Include()
    {
        gameObject.transform.parent = null;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!enabled) return;
        Vulnerable vul = other.gameObject.GetComponent<Vulnerable>();

        if (vul != null)
            vul.DealDamage(damage);
    }
}
