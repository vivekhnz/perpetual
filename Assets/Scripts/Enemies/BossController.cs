using System;
using UnityEngine;

[RequireComponent(typeof(EnemyController))]
public class BossController : MonoBehaviour
{
    public Vector3 InitialPosition = new Vector3(-30, 0, 0);

    public PlayerHealth Player
    {
        get { return controller.Player; }
    }
    public DamageableObject DamageableObject
    {
        get { return controller.DamageableObject; }
    }

    public event EventHandler Initialized;
    public event EventHandler Defeated;

    private EnemyController controller;
    private EnemySpawnManager spawnManager;

    void Start()
    {
        controller = GetComponent<EnemyController>();
        controller.InstanceReset += (sender, e) => Initialize();

        Initialize();
    }

    void Initialize()
    {
        // retrieve enemy manager
        spawnManager = GameObject.FindObjectOfType<EnemySpawnManager>();
        if (spawnManager == null)
            Debug.LogError("No spawn manager found!");
        spawnManager.StartBossEncounter(
            controller.DamageableObject.InitialHealth);

        transform.position = InitialPosition;

        if (Initialized != null)
            Initialized(this, EventArgs.Empty);
    }

    public void UpdateBossHealth()
    {
        spawnManager.UpdateBossHealth(
            controller.DamageableObject.CurrentHealth
            / controller.DamageableObject.InitialHealth);
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