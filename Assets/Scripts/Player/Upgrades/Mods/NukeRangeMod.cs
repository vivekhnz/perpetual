using System;

public class NukeRangeMod : AbilityModUpgrade<NukeAbility>
{
    public float Range = 2.0f;

    protected override void ApplyMod(NukeAbility ability)
    {
        ability.Range = Range;
    }
}