using UnityEngine;

// adapted from http://answers.unity3d.com/questions/219609/auto-destroying-particle-system.html
[RequireComponent(typeof(ParticleSystem))]
public class ParticleSystemAutoDestroy : PooledObject
{
    private ParticleSystem ps;

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
    }

    public void Update()
    {
        if (ps != null && !ps.IsAlive())
            Recycle();
    }
}