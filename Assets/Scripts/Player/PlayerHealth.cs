using UnityEngine;

public class PlayerHealth : MonoBehaviour {

    public PlayerMovement Parent;

    private HUDController hudController;

	// Use this for initialization
	void Start () {
		hudController = Object.FindObjectOfType<HUDController>();
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

            if (hudController != null)
                hudController.GameOver();
        }
    }
}
