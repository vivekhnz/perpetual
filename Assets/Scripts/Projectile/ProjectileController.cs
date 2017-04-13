using UnityEngine;

public class ProjectileController : MonoBehaviour {

	public float MovementSpeed = 4.0f;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		transform.Translate(
			Vector3.right * Time.deltaTime * MovementSpeed);
	}

    void OnTriggerEnter2D(Collider2D collider)
    {
		Destroy(gameObject);
    }
}
