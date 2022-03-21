using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    const float timeToDestroy = 3f;
    private float timeFromStart;

    private float damage;
    private string owner;
    private GameObject binder;

    public void OnStart(float damage, string owner, GameObject binder = null)
    {
        this.damage = damage;
        this.owner = owner;
        this.binder = binder;
    }

    void Update()
    {
        timeFromStart += Time.deltaTime;

        if (timeFromStart >= timeToDestroy)
            Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!enabled) 
            return;
        
        if (other.gameObject.name == owner) 
            return;

        Vulnerable vul = other.gameObject.GetComponent<Vulnerable>();

        if (vul != null)
            OnCollide(vul);

        Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!enabled) 
            return;

        if (collision.gameObject.name == owner) 
            return;

        Vulnerable vul = collision.gameObject.GetComponent<Vulnerable>();

        if (vul != null)
            OnCollide(vul);
    }

    private void OnCollide(Vulnerable target)
    {
        target.DealDamage(damage);

        if (binder != null)
        {
            GameObject bindr = Instantiate(binder, target.transform);
            Effect effect = bindr.GetComponent<Effect>();

            if (effect != null)
                effect.Enable(target);
        }
    }
}
