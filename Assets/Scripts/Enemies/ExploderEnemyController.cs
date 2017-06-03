using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(ChasePlayer))]
[RequireComponent(typeof(EnemyController))]
[RequireComponent(typeof(AudioSource))]
public class ExploderEnemyController : MonoBehaviour
{
    const float MIN_PROXIMITY = 1.0f;
    const float MAX_PROXIMITY = 6.0f;
    const float DETONATION_DISTANCE = 1.5f;

    public ShockwaveController SelfDestructShockwave;
    public ShockwaveController KilledByPlayerShockwave;

    private Animator animator;
    private ChasePlayer chaser;
    private EnemyController controller;
    private AudioSource blip;
    private bool detonated;

    void Start()
    {
        if (SelfDestructShockwave == null)
            Debug.LogError("No self-destruct shockwave prefab specified.");

        if (KilledByPlayerShockwave == null)
            Debug.LogError("No killed by player shockwave prefab specified.");

        animator = GetComponent<Animator>();
        chaser = GetComponent<ChasePlayer>();
        controller = GetComponent<EnemyController>();
        blip = GetComponent<AudioSource>();

        detonated = false;
        controller.InstanceReset += (sender, e) =>
        {
            detonated = false;
        };
    }

    void FixedUpdate()
    {
        // speed up explosive flash animation has enemy gets closer to player
        var proximity = Mathf.Clamp(
            (MAX_PROXIMITY + DETONATION_DISTANCE) - chaser.GetDistanceToPlayer(),
            MIN_PROXIMITY, MAX_PROXIMITY);
        animator.SetFloat("Proximity", proximity);
    }

    public void Detonate()
    {
        // self-destruct
        detonated = true;
        controller.DamageableObject.SelfDestruct();

        // spawn a shockwave that damages the player
        var shockwave = SelfDestructShockwave.Fetch<ShockwaveController>();
        shockwave.transform.position = transform.position;
    }

    public void CreateShockwave()
    {
        // if we self-destructed, don't spawn a second shockwave
        if (detonated)
            return;

        // spawn a shockwave that does not damage the player
        var shockwave = KilledByPlayerShockwave.Fetch<ShockwaveController>();
        shockwave.transform.position = transform.position;
    }

    public void Blip()
    {
        float distance = chaser.GetDistanceToPlayer();
        if (distance > MAX_PROXIMITY)
            return;

        float proximity = 1.0f - ((distance - DETONATION_DISTANCE)
            / (MAX_PROXIMITY - DETONATION_DISTANCE));
        blip.volume = proximity * 0.7f;
        blip.Play();
    }
}
