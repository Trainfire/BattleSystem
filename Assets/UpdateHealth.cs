using UnityEngine;
using System.Collections;
using System;

public class UpdateHealth : BaseAction
{
    private HealthChangeEvent _healthChange;

    public void Initialize(HealthChangeEvent healthChange)
    {
        _healthChange = healthChange;
    }

    protected override void OnExecute()
    {
        StartCoroutine(Animate());
    }

    IEnumerator Animate()
    {
        while (_healthChange.Health.Current != _healthChange.NewValue)
        {
            if (_healthChange.NewValue > _healthChange.OldValue)
            {
                _healthChange.Health.ChangeHealth(1);
            }
            else
            {
                _healthChange.Health.ChangeHealth(-1);
            }

            yield return new WaitForSeconds(0.1f);
        }

        TriggerCompletion();
    }

    public static UpdateHealth Create(HealthChangeEvent e)
    {
        var updateHealth = new GameObject("UpdateHealth").AddComponent<UpdateHealth>();
        updateHealth.Initialize(e);
        return updateHealth;
    }
}
