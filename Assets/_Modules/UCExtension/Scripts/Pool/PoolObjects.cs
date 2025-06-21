using System;
using System.Collections;
using System.Collections.Generic;
using UCExtension;
using UnityEngine;

namespace UCExtension
{
    public class PoolObjects : AutoInstantiateSingletonUC<PoolObjects>
    {
        Dictionary<string, Queue<RecyclableObject>> instantiatedObjects = new Dictionary<string, Queue<RecyclableObject>>();

        public T Spawn <T>(T prefab) where T : RecyclableObject
        {
            return GetNewObject(prefab) as T;
        }

        public T Spawn<T>(T prefab, Transform parent) where T : RecyclableObject
        {
            var obj = GetNewObject(prefab);
            if (parent) obj.transform.SetParent(parent);
            obj.transform.localPosition = Vector3.zero;
            obj.transform.localEulerAngles = Vector3.zero;
            return obj as T;
        }


        public T Spawn<T>(T prefab, Vector3 position) where T : RecyclableObject
        {
            var obj = GetNewObject(prefab);
            obj.transform.position = position;
            return obj as T;
        }


        public T Spawn<T>(T prefab, Vector3 position, Quaternion rotation) where T : RecyclableObject
        {
            var obj = GetNewObject(prefab);
            obj.transform.position = position;
            obj.transform.rotation = rotation;
            return obj as T;
        }

        RecyclableObject GetNewObject(RecyclableObject prefab)
        {
            RecyclableObject newObj = Dequeue(prefab.PoolName);
            if (!newObj)
            {
                newObj = Instantiate(prefab);
                newObj.SetPoolName(prefab.PoolName);
            }
            newObj.gameObject.SetActive(true);
            newObj.IsInPool = false;
            newObj.OnSpawn();
            return newObj;
        }

        public void Destroy(RecyclableObject obj)
        {
            if (obj.IsInPool) return;
            obj.IsInPool = true;
            obj.gameObject.SetActive(false);
            obj.transform.SetParent(transform);
            Enqueue(obj);
        }

        RecyclableObject Dequeue(string poolName)
        {
            if (instantiatedObjects.ContainsKey(poolName) && instantiatedObjects[poolName].Count > 0)
            {
                return instantiatedObjects[poolName].Dequeue();
            }
            return null;
        }

        void Enqueue(RecyclableObject obj)
        {
            var id = obj.PoolName;
            if (!instantiatedObjects.ContainsKey(id))
            {
                instantiatedObjects[id] = new Queue<RecyclableObject>();
            }
            instantiatedObjects[id].Enqueue(obj);
        }
    }
}

public static class PoolExtension
{
    public static void Recyle<T>(this List<T> objects) where T : RecyclableObject
    {
        foreach (var item in objects)
        {
            item.Recycle();
        }
        objects.Clear();
    }
}