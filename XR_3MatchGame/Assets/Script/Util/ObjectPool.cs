using System;
using System.Collections.Generic;
using UnityEngine;

namespace XR_3MatchGame_Util
{
    public class ObjectPool<T> where T : MonoBehaviour, IPoolableObject
    {
        public Transform holder;

        public List<T> Pool { get; private set; } = new List<T>();

        public bool canRecycle => Pool.Find(obj => obj.CanRecycle) != null;

        /// <summary>
        /// ������Ʈ�� Ǯ�� ����ϴ� �޼���
        /// </summary>
        /// <param name="obj">����� ������Ʈ</param>
        public void RegistPoolableObject(T obj)
        {
            Pool.Add(obj);
        }

        /// <summary>
        /// ������Ʈ�� Ǯ�� ��ȯ�ϴ� �޼���
        /// </summary>
        /// <param name="obj"></param>
        public void ReturnPoolableObject(T obj)
        {
            obj.transform.SetParent(holder);
            obj.gameObject.SetActive(false);
            obj.CanRecycle = true;
        }

        public T GetPoolableObject(Func<T, bool> pred = null)
        {
            if (!canRecycle)
            {
                var protoObj = Pool.Count > 0 ? Pool[0] : null;

                if (protoObj != null)
                {
                    var newObj = GameObject.Instantiate(protoObj.gameObject, holder);
                    newObj.name = protoObj.name;
                    newObj.SetActive(false);

                    RegistPoolableObject(newObj.GetComponent<T>());
                }
                else
                {
                    return null;
                }
            }

            T recycleObj = (pred == null) ? (Pool.Count > 0 ? Pool[0] : null) :
                (Pool.Find(obj => pred(obj) && obj.CanRecycle));

            if (recycleObj == null)
            {
                return null;
            }

            recycleObj.CanRecycle = false;

            return recycleObj;
        }
    }
}