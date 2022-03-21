using System.Collections.Generic;
using UnityEngine;

public static class UnitsGetter
{
    public static List<Vulnerable> Container = new List<Vulnerable>();

    public static bool IsTargetFree(Vector3 relative, Vector3 target)
    {
        if (target == null)
            return false;

        if (Physics.Raycast(relative, target - relative, out RaycastHit hit))
            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Unit"))
                return true;

        return false;
    }

    public static Vulnerable FindFirstFreeTarget(Vector3 relative)
    {
        foreach (Vulnerable vul in Container)
            if (vul != null)
                if (IsTargetFree(relative, vul.transform.position))
                    return vul;

        return null;
    }

    public static Vulnerable FindNearestTarget(Transform relative, float maxAngle = 180f, float maxMagnitude = Mathf.Infinity)
    {
        Vulnerable nearest = null;

        foreach (Vulnerable vul in Container)
        {
            if (vul == null)
                continue;

            Vector3 toTarget = vul.transform.position - relative.position;

            if (!IsTargetFree(relative.position, vul.transform.position))
                continue;

            if (Vector3.Angle(relative.forward, toTarget) > maxAngle)
                continue;

            if (toTarget.magnitude > maxMagnitude)
                continue;

            if (nearest == null || toTarget.magnitude < (nearest.transform.position - relative.position).magnitude)
                nearest = vul;
        }

        return nearest;
    }
}
