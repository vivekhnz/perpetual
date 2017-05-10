using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BossTeleporter : MonoBehaviour {

    public float timer = 5;

    private List<Vector3> teleportLocations;
    private float teleportTime;
    
	void Start () {

        // initialise timeholder
        teleportTime = Time.time + 10;
        
        // find teleport points
        teleportLocations = GameObject.FindGameObjectsWithTag("TeleportPoint")
            .Select(obj => obj.transform.position).ToList();
        if (teleportLocations.Count == 0)
            Debug.LogError("No teleport points defined!");

    }
	
	void FixedUpdate () {
		if ((Time.time - teleportTime) > timer)
        {
            transform.position = teleportLocations[Random.Range(0, teleportLocations.Count)];
            teleportTime = Time.time;
        }
	}
}
