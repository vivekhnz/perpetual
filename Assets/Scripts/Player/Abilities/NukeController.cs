using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NukeController : MonoBehaviour {

    public ShockwaveController NukeShockwave;

    public void CreateShockwave()
    {
        var shockwave = NukeShockwave.Fetch<ShockwaveController>();
        shockwave.transform.position = transform.position;
    }
}
