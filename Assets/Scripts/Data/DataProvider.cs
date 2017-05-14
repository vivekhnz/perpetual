using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DataProvider : MonoBehaviour
{
    private class DataUpdatedEvent : UnityEvent<object> { }

    private class DataValue
    {
        private object data;
        public object Value
        {
            get { return data; }
            set
            {
                data = value;
                OnUpdated.Invoke(value);
            }
        }

        public DataUpdatedEvent OnUpdated = new DataUpdatedEvent();

        public DataValue(object value)
        {
            data = value;
        }
    }

    private Dictionary<string, DataValue> values
        = new Dictionary<string, DataValue>();

    void Awake()
    {
        values.Clear();
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

    public void Subscribe<T>(string key, UnityAction<T> listener,
        T defaultValue)
    {
        DataValue data;
        if (!values.TryGetValue(key, out data))
        {
            data = new DataValue(defaultValue);
            values[key] = data;
        }

        UnityAction<object> action = obj => listener.Invoke((T)obj);
        data.OnUpdated.AddListener(action);
        action.Invoke(data.Value);
    }
}