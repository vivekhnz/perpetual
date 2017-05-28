using UnityEngine;
using System;
using UnityEngine.SceneManagement;

// object pooling implementation based on http://catlikecoding.com/unity/tutorials/object-pools/
public class PooledObject : MonoBehaviour
{
    public ObjectPool Pool { get; set; }

    public virtual void ResetInstance() { }
    public virtual void CleanupInstance() { }

    public event EventHandler InstanceReset;
    public event EventHandler InstanceRecycled;

    public void Recycle()
    {
        CleanupInstance();
        if (Pool == null)
        {
            Destroy(gameObject);
        }
        else
        {
            if (InstanceRecycled != null)
                InstanceRecycled(this, EventArgs.Empty);
            Pool.Recycle(this);
        }
    }

    public T Fetch<T>() where T : PooledObject
    {
        Pool = Pool ?? ObjectPool.GetPool(this);
        var obj = (T)Pool.Fetch();
        obj.ResetInstance();
        if (obj.InstanceReset != null)
            obj.InstanceReset(obj, EventArgs.Empty);
        return obj;
    }
}