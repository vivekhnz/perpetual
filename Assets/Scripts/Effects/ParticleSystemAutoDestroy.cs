using UnityEngine;

// adapted from http://answers.unity3d.com/questions/219609/auto-destroying-particle-system.html
public class ParticleSystemAutoDestroy : MonoBehaviour
{
    private ParticleSystem ps;

    public void Start()
    {
        ps = GetComponent<ParticleSystem>();
    }

    public void Update()
    {
        if (ps != null && !ps.IsAlive())
        {
            Destroy(gameObject);
        }
    }
}