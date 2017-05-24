using UnityEngine;

public class NukeAbility : PlayerAbility
{
    public float Cooldown = 12;

    private float nukeTime;
    private NukeController NukeShockwave;

    public override void ExtractAbilityInfo(AbilityInfo info)
    {
        base.ExtractAbilityInfo(info);

        NukeShockwave = info.GetComponent<NukeController>("NukeShockwave");
    }

    void FixedUpdate()
    {
        if (Input.GetButton("Ability") && Time.time - nukeTime > Cooldown)
            Nuke();
    }

    private void Nuke()
    {
        var shockwave = NukeShockwave.Fetch<NukeController>();
        shockwave.transform.position = transform.position;
        nukeTime = Time.time;
    }

    public override float GetCharge()
    {
        return Mathf.Clamp(
            (Time.time - nukeTime) / Cooldown, 0, 1);
    }
}
