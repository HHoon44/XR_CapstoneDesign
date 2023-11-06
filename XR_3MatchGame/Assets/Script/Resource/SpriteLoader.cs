using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using XR_3MatchGame.Util;

namespace XR_3MatchGame_Resource
{
    public static class SpriteLoader
    {
        /// <summary>
        /// ��� ��Ʋ�󽺸� ��� ���� �����
        /// </summary>
        private static Dictionary<AtlasType, SpriteAtlas> atlasDic =
            new Dictionary<AtlasType, SpriteAtlas>();

        /// <summary>
        /// ������ �����ϴ� ��Ʋ�󽺸� ���� �����ͼ�
        /// ����ҿ� �����ϴ� �޼���
        /// </summary>
        /// <param name="atlases">������ �����ϴ� ��Ʋ��</param>
        public static void SetAtlas(SpriteAtlas[] atlases)
        {
            for (int i = 0; i < atlases.Length; i++)
            {
                var key = (AtlasType)Enum.Parse(typeof(AtlasType), atlases[i].name);

                atlasDic.Add(key, atlases[i]);
            }
        }

        /// <summary>
        /// ��û�� ��Ʋ�󿡼� ���ϴ� ��������Ʈ��
        /// ã�� ��ȯ�ϴ� �޼���
        /// </summary>
        /// <param name="type">���ϴ� ��Ʋ��</param>
        /// <param name="spriteName">���ϴ� ��������Ʈ �̸�</param>
        /// <returns></returns>
        public static Sprite GetSprite(AtlasType type, string spriteName)
        {
            if (!atlasDic.ContainsKey(type))
            {
                // ���ϴ� ��������Ʈ�� ���ٸ� �����Ѵ�
                return null;
            }

            // ���ϴ� ��������Ʈ�� �������� ���ؼ�
            // ����ҿ� �����Ͽ� ��Ʋ�󽺸� ������
            // ��Ʋ�󽺰� ���� GetSprite��� �Լ��� �̿��ؼ�
            // ��������Ʈ�� �����´�
            return atlasDic[type].GetSprite(spriteName);
        }

    }
}