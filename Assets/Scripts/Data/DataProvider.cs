using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DataProvider : MonoBehaviour
{
    private class DataUpdatedEvent : UnityEvent<object> { }

    private class DataValue
    {
        public object Value
        {
            set { OnUpdated.Invoke(value); }
        }
        public DataUpdatedEvent OnUpdated = new DataUpdatedEvent();

        public DataValue(object value)
        {
            Value = value;
        }
    }

    private Dictionary<string, DataValue> values;

    void Start()
    {
        values = new Dictionary<string, DataValue>();
    }

    public void UpdateValue<T>(string key, T value)
    {
        DataValue data;
        if (values.TryGetValue(key, out data))
        {
            data.Value = value;
        }
        else
        {
            values[key] = new DataValue(value);
        }
    }

    public void Subscribe<T>(string key, UnityAction<T> listener)
    {
        DataValue data;
        if (!values.TryGetValue(key, out data))
        {
            data = new DataValue(default(T));
            values[key] = data;
        }
        data.OnUpdated.AddListener(
            obj => listener.Invoke((T)obj));
    }
}