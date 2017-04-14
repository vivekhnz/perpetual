using UnityEngine;

public class PlayerHealth : MonoBehaviour {

    public PlayerMovement Parent;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnTriggerEnter2D(Collider2D other)
    {
        if (Parent == null)
            return;
        
        if (other.gameObject.tag == "Enemy")
        {
            Destroy(other.gameObject);
            Destroy(Parent.gameObject);
        }
    }
}
