using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class Ability : MonoBehaviour
{
    public enum Target { Enemy, Myself }
    const float bulletSpeed = 10f;

    [SerializeField]
    private Target targetType;
    [SerializeField]
    private GameObject projectile;
    [SerializeField]
    private GameObject binder;
    [SerializeField]
    private float damage;

    [SerializeField]
    private int magazine;
    [SerializeField]
    private float cooldown;
    [SerializeField]
    [Range(0f, 0.3f)]
    private float launchFromAnimationTime = 0.01f;

    public bool IsEmpty { get => launchCount >= magazine; }
    public bool IsReloading { get => wastedTime < cooldown; }
    public bool IsFull { get => launchCount == 0; }
    public bool IsHandling { get; private set; }

    public string Magazine { get => (magazine - launchCount).ToString() + " / " + magazine.ToString(); }
    public float LaunchFromAnimationTime { get => launchFromAnimationTime; }
    public float Speed { get; private set; } = 1f;
    public UnityEvent OnLaunchEvent;

    private int launchCount;
    private float wastedTime;

    private void Start()
    {
        wastedTime = cooldown;
        launchCount = 0;
        launchFromAnimationTime /= 0.3f; // conversion from msec animation to decimal time
        OnLaunchEvent.AddListener(Launch);
    }

    public bool TryUse()
    {
        if (IsEmpty || IsReloading)
        {
            TryReload();
            return false;
        }

        IsHandling = true;

        if (LaunchFromAnimationTime == 0f)
            Launch();

        return true;
    }

    private bool TryReload()
    {
        if (IsHandling) 
            return false;

        if (IsReloading) 
            return false;

        if (IsFull) 
            return false;

        wastedTime = 0f;
        StartCoroutine(Reload());
        return true;
    }

    IEnumerator Reload()
    {
        while (wastedTime < cooldown)
        {
            yield return new WaitForEndOfFrame();
            wastedTime += Time.deltaTime;
        }

        launchCount = 0;
        IsHandling = false;
    }

    // called from animation
    private void Launch()
    {
        IsHandling = false;

        if (IsEmpty) 
            return;

        if (IsReloading) 
            return;

        launchCount++;

        GameObject bullet = Instantiate(projectile, projectile.transform.position, projectile.transform.rotation);
        bullet.SetActive(true);

        Projectile proj = bullet.AddComponent<Projectile>();
        proj.OnStart(damage, targetType == Target.Enemy ? gameObject.name : string.Empty, binder);

        Rigidbody rigid = bullet.GetComponent<Rigidbody>();
        rigid.AddForce(transform.forward * bulletSpeed);

        if (IsEmpty)
            TryReload();
    }

    public void ScaleSpeed(float scale, float time)
    {
        if (Speed != 1f) 
            return;

        Speed = scale;
        StartCoroutine(Scaler());

        IEnumerator Scaler()
        {
            yield return new WaitForSeconds(time);
            Speed = 1f;
        }
    }
}
