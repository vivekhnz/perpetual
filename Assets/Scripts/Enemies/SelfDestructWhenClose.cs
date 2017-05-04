using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfDestructWhenClose : MonoBehaviour {

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

	void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            Debug.Log("Player");
            //rb.AddTorque(50);
        }
    }
}
