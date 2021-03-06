﻿using System;
using UnityEngine;

[RequireComponent(typeof(EnemyController))]
public class BossController : MonoBehaviour
{
    public enum BossState
    {
        Appearing = 0,
        Active = 1,
        Teleporting = 2
    }

    public class TeleportingEventArgs : EventArgs
    {
        public Vector3 Destination;

        public TeleportingEventArgs(Vector3 destination)
        {
            Destination = destination;
        }
    }

    public Vector3 OffScreenPosition = new Vector3(-30, 0, 0);
    public Vector3 InitialPosition = Vector3.zero;
    public ShockwaveController Shockwave;

    private EnemyController controller;
    private EnemyController Controller
    {
        get
        {
            if (controller == null)
                controller = GetComponent<EnemyController>();
            return controller;
        }
    }

    public PlayerHealth Player
    {
        get { return Controller.Player; }
    }
    public float HealthPercentage
    {
        get
        {
            return Controller.DamageableObject.CurrentHealth / initialHealth;
        }
    }
    public bool IsBossActive
    {
        get { return currentState == BossState.Active; }
    }

    public event EventHandler Initialized;
    public event EventHandler Defeated;
    public event EventHandler<TeleportingEventArgs> Teleporting;
    public event EventHandler Teleported;
    public event EventHandler Activated;

    public EnemySpawnManager spawnManager;
    private BossState currentState;
    private Vector3 teleportDestination;
    private float initialHealth;

    void Start()
    {
        Controller.InstanceReset += (sender, e) => Initialize();

        if (Shockwave == null)
            Debug.LogError("Shockwave object not found!");

        Initialize();
    }

    void Initialize()
    {
        // retrieve enemy manager
        spawnManager = GameObject.FindObjectOfType<EnemySpawnManager>();
        if (spawnManager == null)
            Debug.LogError("No spawn manager found!");

        // add more health
        float a = 0.889f;
        float b = 1.1f;
        float c = 0.01676f;
        float multiplier = (a * Mathf.Pow(b, spawnManager.GetRound())) + c;
        initialHealth = (int)(
            Controller.DamageableObject.InitialHealth * multiplier);

        spawnManager.StartBossEncounter(initialHealth);
        Controller.DamageableObject.ResetHealth(initialHealth);

        transform.position = OffScreenPosition;

        if (Initialized != null)
            Initialized(this, EventArgs.Empty);
        BeginTeleport();
    }

    public void BeginTeleport()
    {
        currentState = BossState.Teleporting;

        TeleportingEventArgs args = new TeleportingEventArgs(InitialPosition);
        if (Teleporting != null)
            Teleporting(this, args);

        teleportDestination = args.Destination;
    }

    public void FinishTeleport()
    {
        transform.position = teleportDestination;
        currentState = BossState.Appearing;

        if (Teleported != null)
            Teleported(this, EventArgs.Empty);
    }

    public void Activate()
    {
        currentState = BossState.Active;

        // spawn shockwave
        var shockwave = Shockwave.Fetch<ShockwaveController>();
        shockwave.transform.position = transform.position;

        if (Activated != null)
            Activated(this, EventArgs.Empty);
    }

    public void UpdateBossHealth()
    {
        spawnManager.UpdateBossHealth(HealthPercentage);
    }

    public void OnDefeated()
    {
        // kill projectiles
        var projectiles = GameObject.FindObjectsOfType<BossProjectileController>();
        foreach (var projectile in projectiles)
            projectile.Recycle();

        // notify encounter completion
        spawnManager.FinishBossEncounter();

        if (Defeated != null)
            Defeated(this, EventArgs.Empty);
    }
}