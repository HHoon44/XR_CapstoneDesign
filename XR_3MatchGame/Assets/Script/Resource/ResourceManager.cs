using System;
using UnityEngine;
using UnityEngine.U2D;
using XR_3MatchGame.Util;
using XR_3MatchGame_Object;
using XR_3MatchGame_Util;

namespace XR_3MatchGame_Resource
{
    public class ResourceManager : Singleton<ResourceManager>
    {
        public void Initialize()
        {
            LoadAllPrefabs();
            LoadAllAtlas();
        }

        /// <summary>
        /// ���ҽ� ���� �� ����� ��������
        /// �����ͼ� ��ȯ�ϴ� �޼���
        /// </summary>
        /// <param name="path">���ϴ� �������� �����ϴ� ���</param>
        /// <returns></returns>
        public GameObject LoadObject(string path)
        {
            // ���� ���� ���� ���ҽ� ������ �����Ͽ�
            // GameObject ���·� ��ȯ �޴´�
            return Resources.Load<GameObject>(path);
        }

        /// <summary>
        /// ���ҽ� ���� �ȿ� �����ϴ� ��Ʋ�󽺸� �����ͼ�
        /// </summary>
        private void LoadAllAtlas()
        {
            var blockAtlas = Resources.LoadAll<SpriteAtlas>("Atlas/BlockAtlas");
            SpriteLoader.SetAtlas(blockAtlas);

            var iconAtlas = Resources.LoadAll<SpriteAtlas>("Atlas/IconAtlas");
            SpriteLoader.SetAtlas(iconAtlas);
        }

        /// <summary>
        /// ���ҽ� ���� �ȿ� �����ϴ�
        /// ��� �������� �ҷ����� �޼���
        /// </summary>
        private void LoadAllPrefabs()
        {
            LoadPoolableObject<Block>(PoolType.Block, "Prefabs/Block", 80);
        }

        /// <summary>
        /// ������Ʈ Ǯ�� ����� �������� �����ϰ� Ǯ�� ����ϴ� �޼���
        /// </summary>
        /// <typeparam name="T">�������� ������Ʈ</typeparam>
        /// <param name="type">�������� Ÿ��</param>
        /// <param name="path">�������� �����ϴ� ���</param>
        /// <param name="poolCount">Ǯ�� ����</param>
        /// <param name="loadComplete">���� �� ������ �޼���</param>
        public void LoadPoolableObject<T>(PoolType type, string path, int poolCount = 1, Action loadComplete = null)
            where T : MonoBehaviour, IPoolableObject
        {
            var obj = LoadObject(path);

            var tComponent = obj.GetComponent<T>();

            ObjectPoolManager.Instance.RegistPool<T>(type, tComponent, poolCount);

            loadComplete?.Invoke();
        }
    }
}