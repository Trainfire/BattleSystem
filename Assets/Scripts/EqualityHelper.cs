using System;

public static class EqualityHelper
{
    public static bool IsEqual(Equality equality, float value, float comparison)
    {
        switch (equality)
        {
            case Equality.LessThan: return value < comparison;
            case Equality.LessThanEqualTo: return value <= comparison;
            case Equality.Equal: return value == comparison;
            case Equality.GreaterThan: return value > comparison;
            case Equality.GreaterThanEqualTo: return value >= comparison;
        }

        return false;
    }
}
