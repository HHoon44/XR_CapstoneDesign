using System.Collections.Generic;
using UnityEngine;
using XR_3MatchGame.Util;

namespace XR_3MatchGame_Util
{
    public class ObjectPoolManager : Singleton<ObjectPoolManager>
    {
        /// <summary>
        /// Ǯ�� ��Ƴ��� ��ųʸ�
        /// </summary>
        private Dictionary<PoolType, object> poolDic = new Dictionary<PoolType, object>();

        /// <summary>
        /// Ǯ�� ������Ʈ�� ����ϴ� �޼���
        /// </summary>
        /// <typeparam name="T">���ѵ� Ÿ��</typeparam>
        /// <param name="type">��û Ǯ Ÿ��</param>
        /// <param name="obj">����� ������Ʈ</param>
        /// <param name="poolCount">���� ����</param>
        public void RegistPool<T>(PoolType type, T obj, int poolCount = 1)
            where T : MonoBehaviour, IPoolableObject
        {
            ObjectPool<T> pool = null;

            if (poolDic.ContainsKey(type))
            {
                pool = poolDic[type] as ObjectPool<T>;
            }
            else
            {
                pool = new ObjectPool<T>();
                poolDic.Add(type, pool);
            }

            if (pool.holder == null)
            {
                pool.holder = new GameObject($"{type.ToString()}Holder").transform;
                pool.holder.parent = transform;
                pool.holder.position = Vector3.zero;
            }

            for (int i = 0; i < poolCount; i++)
            {
                var poolableObj = Instantiate(obj);
                poolableObj.name = obj.name;
                poolableObj.transform.SetParent(pool.holder);
                poolableObj.gameObject.SetActive(false);

                pool.RegistPoolableObject(poolableObj);
            }
        }

        /// <summary>
        /// Ǯ ����ҿ��� Ǯ�� ��ȯ���ִ� �޼���
        /// </summary>
        /// <typeparam name="T">���ѵ� Ÿ��</typeparam>
        /// <param name="type">��û Ǯ Ÿ��</param>
        /// <returns></returns>
        public ObjectPool<T> GetPool<T>(PoolType type)
            where T : MonoBehaviour, IPoolableObject
        {
            if (!poolDic.ContainsKey(type))
            {
                return null;
            }

            return poolDic[type] as ObjectPool<T>;
        }

        /// <summary>
        /// Ǯ ���� �޼���
        /// </summary>
        /// <typeparam name="T">���ѵ� Ÿ��</typeparam>
        /// <param name="type">��û Ǯ Ÿ��</param>
        public void ClearPool<T>(PoolType type)
            where T : MonoBehaviour, IPoolableObject
        {
            var pool = GetPool<T>(type)?.Pool;

            if (pool == null)
            {
                return;
            }

            for (int i = 0; i < pool.Count; i++)
            {
                if (pool[i] != null)
                {
                    Destroy(pool[i].gameObject);
                }
            }

            pool.Clear();
        }
    }
}