using UnityEngine;

public class BattleWeather : MonoBehaviour
{
    [SerializeField] private Weather _initialWeather;

    public GameObject Sandstorm;

    public BaseAction Current { get; private set; }

    private BattleSystem _battleSystem;

    public void Initialize(BattleSystem battleSystem)
    {
        _battleSystem = battleSystem;

        Set(_initialWeather);
    }

    public void Set(Weather weather)
    {
        LogEx.Log<BattleWeather>("Weather is now: " + weather.ToString());

        switch (weather)
        {
            case Weather.None: Clear(); break;
            case Weather.Sandstorm: Create(Sandstorm); break;
        }

        if (Current != null)
        {
            _battleSystem.Queue.RegisterWeather(Current);
        }
        else
        {
            _battleSystem.Queue.ClearWeather();
        }
    }

    public void Clear()
    {
        if (Current != null)
            Destroy(Current.gameObject);
    }

    void Create(GameObject prototype)
    {
        Clear();
        Current = Instantiate(prototype).GetComponent<BaseAction>();
    }
}
