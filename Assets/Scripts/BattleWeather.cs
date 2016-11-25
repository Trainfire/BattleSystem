using UnityEngine;
using System;

public class BattleWeather : MonoBehaviour
{
    public event Action<BattleWeather> Changed;

    public GameObject Sandstorm;

    [SerializeField] private Weather _initialWeather;

    public BaseAction Current { get; private set; }

    void Start()
    {
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

        if (Changed != null)
            Changed.Invoke(this);
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
