using System;
using UnityEngine.Events;

public class Binding<T>
{
    public DataProvider Source;
    public string Key = "MyValueName";

    public void Subscribe(UnityAction<T> listener)
    {
        if (Source != null)
            Source.Subscribe(Key, listener);
    }
}

[Serializable] public class FloatBinding : Binding<float> { }