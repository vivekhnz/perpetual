using UnityEngine;

public class ProjectileController : PooledObject {

	public float MovementSpeed = 4.0f;
    public float Damage = 10.0f;
	
	public void Initialize(Vector3 position, Quaternion rotation) {
		transform.position = position;
		transform.rotation = rotation;
	}
	
	// Update is called once per frame
	void Update () {
		transform.Translate(
			Vector3.right * Time.deltaTime * MovementSpeed);
	}

    void OnTriggerEnter2D(Collider2D collider)
    {
		if (collider.gameObject.CompareTag("Solid"))
			Recycle();
    }
}
