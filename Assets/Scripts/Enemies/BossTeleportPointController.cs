using UnityEngine;

[RequireComponent(typeof(Animator))]
public class BossTeleportPointController : MonoBehaviour
{
    // percentage of boss health that this teleport point becomes active at
    public float HealthThreshold = 1.0f;

    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
        if (animator == null)
            Debug.LogError("No animator found!");
    }

    public void Activate()
    {
        animator.SetBool("IsActive", true);
    }

    public void Deactivate()
    {
        animator.SetBool("IsActive", false);
    }
}
