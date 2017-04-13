using UnityEngine;

public class PlayerMovement : MonoBehaviour {

	public float MovementSpeed = 1.0f;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		Vector2 movement = new Vector2(
			Input.GetAxis("Horizontal"),
			Input.GetAxis("Vertical"))
			* Time.deltaTime * MovementSpeed;
		transform.Translate(
			movement.x, movement.y, 0);
	}
}
