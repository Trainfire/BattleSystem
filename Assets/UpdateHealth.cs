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

    protected override void OnExecute(BattleSystem battleSystem)
    {
        StartCoroutine(Animate());
    }

    IEnumerator Animate()
    {
        while (_healthChange.Health.Current != _healthChange.NewValue)
        {
            //if (_healthChange.NewValue > _healthChange.OldValue)
            //{
            //    _healthChange.Health.ChangeHealth(1);
            //}
            //else
            //{
            //    _healthChange.Health.ChangeHealth(-1);
            //}

            //yield return new WaitForSeconds(0.1f);

            _healthChange.Health.Set(_healthChange.NewValue);

            yield return 0;
        }

        TriggerCompletion();
    }

    public static UpdateHealth Create(HealthChangeEvent e)
    {
        var updateHealth = new GameObject("UpdateHealth").AddComponent<UpdateHealth>();
        updateHealth.tag = "DontDestroyOnTrigger";
        updateHealth.Initialize(e);
        return updateHealth;
    }
}
