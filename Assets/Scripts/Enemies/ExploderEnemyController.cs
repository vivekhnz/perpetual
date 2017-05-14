using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(ChasePlayer))]
public class ExploderEnemyController : MonoBehaviour
{
    const float MIN_PROXIMITY = 1.0f;
    const float MAX_PROXIMITY = 6.0f;
    const float DETONATION_DISTANCE = 1.5f;

    public ShockwaveController Shockwave;

    private Animator animator;
    private ChasePlayer chaser;

    void Start()
    {
        if (Shockwave == null)
            Debug.LogError("No shockwave prefab specified.");

        animator = GetComponent<Animator>();
        chaser = GetComponent<ChasePlayer>();
    }

    void FixedUpdate()
    {
        // speed up explosive flash animation has enemy gets closer to player
        var proximity = Mathf.Clamp(
            (MAX_PROXIMITY + DETONATION_DISTANCE) - chaser.GetDistanceToPlayer(),
            MIN_PROXIMITY, MAX_PROXIMITY);
        animator.SetFloat("Proximity", proximity);
    }

    public void CreateShockwave()
    {
        var shockwave = Shockwave.Fetch<ShockwaveController>();
        shockwave.transform.position = transform.position;
    }
}
