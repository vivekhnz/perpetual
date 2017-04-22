using UnityEngine;
using System;
using UnityEngine.SceneManagement;
using UnityEditor;

// object pooling implementation based on http://catlikecoding.com/unity/tutorials/object-pools/
public class PooledObject : MonoBehaviour
{
    public ObjectPool Pool { get; set; }
    
    public virtual void ResetInstance() { }
    public virtual void CleanupInstance() { }

    public event EventHandler InstanceReset;
    public event EventHandler InstanceRecycled;

	public PooledObject() {
		SceneManager.sceneLoaded += OnSceneLoaded;
	}

	~PooledObject() {
		SceneManager.sceneLoaded -= OnSceneLoaded;
	}

    public void Recycle()
    {
        // ensure the object is not a prefab
        if (PrefabUtility.GetPrefabType(gameObject) != PrefabType.None)
            return;
        
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
        CleanupInstance();
    }

    public T Fetch<T>() where T : PooledObject
    {
		Pool = Pool ?? ObjectPool.GetPool(this);
        var obj = (T)Pool.Fetch();
		if (obj.InstanceReset != null)
			obj.InstanceReset(obj, EventArgs.Empty);
        obj.ResetInstance();
        return obj;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Recycle();
    }
}