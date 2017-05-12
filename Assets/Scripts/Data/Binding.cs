using System;
using UnityEngine.Events;

public class Binding<T>
{
    public DataProvider Source;
    public string Key = "MyValueName";
    public T DefaultValue;

    public void Subscribe(UnityAction<T> listener)
    {
        if (Source != null)
            Source.Subscribe(Key, listener);

        listener.Invoke(DefaultValue);
    }
}

[Serializable] public class FloatBinding : Binding<float> { }
[Serializable] public class BooleanBinding : Binding<bool> { }