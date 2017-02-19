using System;

[Serializable]
public class FieldEffectSpikesParameters
{
    public int[] PercentageDamagePerLevel;
}

public class FieldEffectSpikes : FieldEffect
{
    private FieldEffectSpikesParameters _parameters;

    public override string Message { get { return "{TARGET} was hurt by spikes!"; } }

    public FieldEffectSpikes(FieldEffectSpikesParameters parameters) : base()
    {
        _parameters = parameters;

        Max = _parameters.PercentageDamagePerLevel.Length;
    }

    protected override void OnApplyEffect(Character target)
    {
        LogEx.Log<FieldEffectSpikes>("Applying spikes. Multiplier {0}", Level);

        var damage = _parameters.PercentageDamagePerLevel[Level];

        target.Health.ChangeHealth(null, -damage);
    }
}