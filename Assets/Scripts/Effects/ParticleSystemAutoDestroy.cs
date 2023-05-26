using UnityEngine;

// adapted from http://answers.unity3d.com/questions/219609/auto-destroying-particle-system.html
[RequireComponent(typeof(ParticleSystem))]
public class ParticleSystemAutoDestroy : PooledObject
{
    private ParticleSystem ps;
    private bool hasCreatedParticles;
    private float createdAtTime;

    public ParticleSystem ParticleSystem
    {
        get { return ps; }
    }

    public void Start()
    {
        ps = GetComponent<ParticleSystem>();
    }

    public override void ResetInstance()
    {
        ps = GetComponent<ParticleSystem>();
        ps.Clear();
        ps.Play();

        hasCreatedParticles = false;
        createdAtTime = Time.time;
    }

    public void Update()
    {
        if (ps == null) return;

        // don't recycle the particle system until at least one particle has spawned
        if (hasCreatedParticles)
        {
            if (!ps.IsAlive())
            {
                Recycle();
            }
        }
        else
        {
            if (ps.particleCount > 0)
            {
                // we've seen a particle, so this particle can be destroyed once no more particles exist
                hasCreatedParticles = true;
            }
            else if (Time.time - createdAtTime >= ps.main.duration * 10)
            {
                // escape hatch in case we never see a particle for some reason
                // allow the particle system to be deleted if it's been at least 10x the system duration since it was spawned
                Debug.LogWarning($"Leftover particle system '{name}' marked for deletion.");
                hasCreatedParticles = true;
            }
        }
    }
}