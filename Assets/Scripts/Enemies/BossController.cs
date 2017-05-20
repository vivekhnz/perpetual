using System;
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

    public Vector3 InitialPosition = new Vector3(-30, 0, 0);

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
            return Controller.DamageableObject.CurrentHealth
                / Controller.DamageableObject.InitialHealth;
        }
    }
    public bool IsBossActive
    {
        get { return currentState == BossState.Active; }
    }

    public event EventHandler Initialized;
    public event EventHandler Defeated;
    public event EventHandler Teleporting;

    private EnemySpawnManager spawnManager;
    private BossState currentState;

    void Start()
    {
        Controller.InstanceReset += (sender, e) => Initialize();

        Initialize();
    }

    void Initialize()
    {
        // retrieve enemy manager
        spawnManager = GameObject.FindObjectOfType<EnemySpawnManager>();
        if (spawnManager == null)
            Debug.LogError("No spawn manager found!");
        spawnManager.StartBossEncounter(
            Controller.DamageableObject.InitialHealth);

        transform.position = InitialPosition;

        if (Initialized != null)
            Initialized(this, EventArgs.Empty);

        PrepareToTeleport();
    }

    public void PrepareToTeleport()
    {
        currentState = BossState.Teleporting;

        if (Teleporting != null)
            Teleporting(this, EventArgs.Empty);
    }

    public void TeleportToPosition(Vector3 destination)
    {
        transform.position = destination;
        currentState = BossState.Appearing;
    }

    public void Activate()
    {
        currentState = BossState.Active;
    }

    public void UpdateBossHealth()
    {
        spawnManager.UpdateBossHealth(
            Controller.DamageableObject.CurrentHealth
            / Controller.DamageableObject.InitialHealth);
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