using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BossTeleporter : MonoBehaviour {

    public float TimeToTeleport = 5;
    public float TimeToHide = 3;

    private List<Vector3> teleportLocations;
    private float teleportTime;
    private Vector3 selectedTeleport;
    private Vector3 hidingSpot;
    private bool hiding;
    private ChasePlayer movement;
    
	void Start () {

        // get movement
        movement = GetComponent<ChasePlayer>();

        // initialise timeholder and bool
        hiding = false;
        teleportTime = Time.time;
        
        // find teleport points
        teleportLocations = GameObject.FindGameObjectsWithTag("TeleportPoint")
            .Select(obj => obj.transform.position).ToList();
        if (teleportLocations.Count == 0)
            Debug.LogError("No teleport points defined!");

        // hiding spot (dont know how to temporarily disable)
        hidingSpot = new Vector3(-30, 0, 0);
    }
	
	void FixedUpdate () {

        // if time to teleport, teleport boss
		if (!hiding && (Time.time - teleportTime) > TimeToTeleport)
        {
            hiding = true;
            transform.position = hidingSpot;
            selectedTeleport = teleportLocations[Random.Range(0, teleportLocations.Count)];
            movement.MovementSpeed = 0f;
            teleportTime = Time.time;
        } else
        {
            // if time to re appear, teleport boss to teleport location
            if (hiding && (Time.time - teleportTime) > TimeToHide)
            {
                hiding = false;
                transform.position = selectedTeleport;
                movement.MovementSpeed = 2.0f;
                teleportTime = Time.time;
            }
        }
    }
}
