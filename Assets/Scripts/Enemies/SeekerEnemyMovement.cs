using UnityEngine;

public class SeekerEnemyMovement : MonoBehaviour {

    public float MovementSpeed;
    public Transform Player;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        // rotate to face player
        Vector2 direction = Player.position - transform.position;
        direction.Normalize();
        transform.rotation = Quaternion.Euler(0, 0,
            Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg);
        
        // move forward
        transform.Translate(
            Vector3.right * MovementSpeed * Time.deltaTime);
	}
}
