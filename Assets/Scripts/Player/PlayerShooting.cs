using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    public ProjectileController Projectile;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // calculate world position of mouse cursor
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        var plane = new Plane(Vector3.forward, Vector3.zero);
        float distance = 0.0f;
        if (plane.Raycast(ray, out distance))
        {
            Vector3 mousePos = ray.GetPoint(distance);

            // calculate rotation based on direction to mouse cursor
            Vector3 dirToMouse = mousePos - transform.position;
            float rotation = Mathf.Atan2(dirToMouse.y, dirToMouse.x)
                * Mathf.Rad2Deg;

            // set the player rotation
            transform.rotation = Quaternion.Euler(0.0f, 0.0f, rotation);
        }

        // fire weapon
        if (Input.GetButton("Fire"))
            Fire();
    }

    void Fire()
    {
        if (Projectile == null)
            return;
		
		// calculate position of new projectile
		float zRot = transform.rotation.eulerAngles.z * Mathf.Deg2Rad;
		Vector3 offset = new Vector3(
			Mathf.Cos(zRot), Mathf.Sin(zRot), 0)
			* (transform.localScale.x * 0.6f);
        
		// spawn projectile
		GameObject.Instantiate(Projectile,
            transform.position + offset, transform.rotation);
    }
}
