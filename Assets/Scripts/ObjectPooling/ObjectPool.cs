using UnityEngine;
using System.Collections.Generic;

// object pooling implementation based on http://catlikecoding.com/unity/tutorials/object-pools/
public class ObjectPool : MonoBehaviour
{
    PooledObject prefab;
    List<PooledObject> availableObjects;

    public PooledObject Fetch()
    {
        if (availableObjects == null)
        {
            availableObjects = new List<PooledObject>();
        }

        PooledObject obj = null;

        // are any objects available?
        int lastAvailableIndex = availableObjects.Count - 1;
        if (lastAvailableIndex >= 0)
        {
            // retrieve the last available object
            obj = availableObjects[lastAvailableIndex];
            // remove the object from the list of available objects
            availableObjects.RemoveAt(lastAvailableIndex);
            // re-activate the object
            obj.gameObject.SetActive(true);
        }
        else
        {
            // create a new instance of the prefab
            obj = Instantiate(prefab);
            // associate the new instance with this object pool
            obj.transform.SetParent(transform, false);
            obj.Pool = this;
        }

        return obj;
    }

    public void Recycle(PooledObject obj)
    {
        // deactivate the game object associated with the component
        obj.gameObject.SetActive(false);
        // add it to the list of available instances
        availableObjects.Add(obj);
    }

    public static ObjectPool GetPool(PooledObject prefab)
    {
        GameObject obj = null;
        ObjectPool pool = null;

        if (Application.isEditor)
        {
            // if we are running in the editor, look for an existing instance
            // of the pool before creating a new one
            obj = GameObject.Find(prefab.name + " Pool");
            if (obj != null)
            {
                pool = obj.GetComponent<ObjectPool>();
                if (pool != null)
                    return pool;
            }
        }

        // create a new object pool for the given prefab
        obj = new GameObject(string.Format("{0} Pool", prefab.name));
        DontDestroyOnLoad(obj);
        pool = obj.AddComponent<ObjectPool>();
        pool.prefab = prefab;
        return pool;
    }
}