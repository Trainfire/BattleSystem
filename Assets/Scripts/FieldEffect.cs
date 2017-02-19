using System;

public class FieldEffect
{
    protected int Level { get; private set; }
    protected int Max { get; set; }

    public bool Active { get; private set; }
    public virtual bool CanApply { get { return Level != Max - 1; } }
    public virtual string Message { get { return string.Empty; } }

    public FieldEffect(int max = 1)
    {
        Max = max;
    }

    public void Reset()
    {
        LogEx.Log<FieldEffectSpikes>("Reset effect multiplier");
        Active = false;
        Level = 0;
    }

    public void IncreaseEffect()
    {
        Active = true;

        if (CanApply)
        {
            Level++;
            LogEx.Log<FieldEffectSpikes>("Increased effect multiplier to {0}", Level);
        }
    }

    public virtual void ApplyEffect(Character target)
    {
        if (Level != 0)
            OnApplyEffect(target);
    }

    protected virtual void OnApplyEffect(Character target) { }
}