using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeekerEnemyMovement : MonoBehaviour {

    public float MovementSpeed;
    public Transform Player;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        Vector2 direction = Player.position - transform.position;
        direction.Normalize();
        Vector2 movement = direction * MovementSpeed * Time.deltaTime;
        transform.Translate(movement.x, movement.y, 0);
	}
}
