using UnityEngine;

public class ExploderEnemyController : MonoBehaviour
{
    public ShockwaveController Shockwave;

    void Start()
    {
        if (Shockwave == null)
            Debug.LogError("No shockwave prefab specified.");
    }

    public void CreateShockwave()
    {
        var shockwave = Instantiate(Shockwave);
        shockwave.transform.position = transform.position;
    }
}
