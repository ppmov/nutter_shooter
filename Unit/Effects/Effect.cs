using System.Collections;
using UnityEngine;

// special ability effect parent class
public class Effect : MonoBehaviour
{
    [SerializeField]
    protected float duration;
    protected Vulnerable target;

    public void Enable(Vulnerable unit)
    {
        target = unit;
        StartCoroutine(Handling());
    }

    private IEnumerator Handling()
    {
        Include();
        yield return new WaitForSeconds(duration);
        Exclude();
        Destroy(gameObject);
    }

    protected virtual void Include()
    {
        return;
    }

    protected virtual void Exclude()
    {
        return;
    }
}
