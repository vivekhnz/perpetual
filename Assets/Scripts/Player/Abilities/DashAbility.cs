﻿using System;
using UnityEngine;

public class DashAbility : PlayerAbility<DashAbilityUpgrade>
{
    // force of the player's dash
    public float Speed = 0.25f;
    // how long the player cannot dash for in seconds
    public float Cooldown = 3;
    // multiplier to slow down a player's dash (must be < 1)
    public float Friction = 0.9f;

    private Vector2 velocity;
    private float dashTime;
    private AudioSource DashSound;

    public override void ExtractAbilityInfo(DashAbilityUpgrade upgrade)
    {
        DashSound = upgrade.DashSound;
    }

    void Update()
    {
        // can I dash?
        if (Input.GetButton("Ability") && Time.time - dashTime > Cooldown)
            Dash();
    }

    void FixedUpdate()
    {
        if (velocity.magnitude < 0.01f)
        {
            // stop moving once we get really slow
            velocity = Vector2.zero;
        }
        else
        {
            // slow down over time
            velocity *= Friction;
        }

        transform.Translate(
            velocity.x, velocity.y, 0, Space.World);
    }

    private void Dash()
    {
        // apply dash velocity
        var dashDirection = new Vector2(
            Input.GetAxis("Horizontal"),
            Input.GetAxis("Vertical")).normalized;

        // are we actually moving?
        if (dashDirection.magnitude < 0.001f)
            return;

        velocity += dashDirection * Speed;

        // play sound
        var sound = Instantiate(DashSound);
        Destroy(sound.gameObject, sound.clip.length);

        // reset cooldown
        dashTime = Time.time;
    }

    public override float GetCharge()
    {
        return Mathf.Clamp(
            (Time.time - dashTime) / Cooldown, 0, 1);
    }
}