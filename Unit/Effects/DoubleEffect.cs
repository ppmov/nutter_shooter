using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoubleEffect : Effect
{
    [SerializeField]
    private float launchPing;

    private Ability attackAbility;

    protected override void Include()
    {
        attackAbility = target.GetComponent<Unit>().AttackAbility;

        if (attackAbility != null && attackAbility.enabled)
            attackAbility.OnLaunchEvent.AddListener(LateLaunch);
    }

    protected override void Exclude()
    {
        if (attackAbility != null)
            attackAbility.OnLaunchEvent.RemoveListener(LateLaunch);
    }

    private void LateLaunch()
    {
        StartCoroutine(WaitAndDo());

        IEnumerator WaitAndDo()
        {
            yield return new WaitForSeconds(launchPing);
            attackAbility.OnLaunchEvent.RemoveListener(LateLaunch);
            attackAbility.OnLaunchEvent.Invoke();
            attackAbility.OnLaunchEvent.AddListener(LateLaunch);
        }
    }
}
