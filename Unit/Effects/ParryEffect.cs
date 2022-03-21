using UnityEngine;

public class ParryEffect : Effect
{
    private bool state;

    protected override void Include()
    {
        state = true;
    }

    private void Update()
    {
        if (!state) return;
        if (target == null) return;
        Vulnerable enemy = UnitsGetter.FindFirstFreeTarget(target.transform.position);
        Vector3 look = enemy == null ? -target.transform.forward : (enemy.transform.position - target.transform.position);

        transform.rotation = Quaternion.LookRotation(look) * Quaternion.Euler(-90f, 0f, 0f);
        transform.position = target.transform.position + look.normalized + Vector3.up;
    }
}
