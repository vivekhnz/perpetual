﻿using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(AudioSource))]
public class ScorePopupController : MonoBehaviour
{
    public float Speed = 0.1f;
    private Vector2 velocity;

    public void Initialize(string text, Vector3 position, Vector2 direction,
        float animationSpeed, bool playSound)
    {
        GetComponent<Text>().text = text;
        GetComponent<Animator>().SetFloat("Speed", animationSpeed);
        if (playSound)
            GetComponent<AudioSource>().Play();

        transform.position = position;
        transform.localScale = new Vector3(1, 1, 1);
        this.velocity = direction * Speed;
    }

    void FixedUpdate()
    {
        transform.Translate(this.velocity);
        this.velocity *= 0.9f;
    }

    public void SelfDestruct()
    {
        Destroy(gameObject);
    }
}
