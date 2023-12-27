using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using XR_3MatchGame.Util;

public class SoundManager : Singleton<SoundManager>
{
    public List<AudioClip> BGMClips = new List<AudioClip>();
    public List<AudioClip> EGMClips = new List<AudioClip>();

    private List<AudioSource> audioSources = new List<AudioSource>();

    /// <summary>
    /// ���� �����ϴ� ����� �ҽ��� ������ ����� ����� Ŭ������
    /// �����ϴ� �޼���
    /// </summary>
    public void Initialize(SceneType type)
    {
        // �ѹ� �ʱ�ȭ
        audioSources.Clear();

        // BGM, EGM ã��
        audioSources.Add(GameObject.Find("BGM").GetComponent<AudioSource>());
        audioSources.Add(GameObject.Find("EGM").GetComponent<AudioSource>());

        // ó�� ������ ������� ����
        SetBGM(type.ToString());
    }

    /// <summary>
    /// ��� ���� ���� �޼���
    /// </summary>
    /// <param name="name"></param>
    public void SetBGM(string name)
    {
        for (int i = 0; i < BGMClips.Count; i++)
        {
            if (BGMClips[i].name.Split('_')[0] == name)
            {
                audioSources[0].clip = BGMClips[i];
                audioSources[0].Play();
            }
        }
    }

    /// <summary>
    /// ȿ���� ���� �޼���
    /// </summary>
    /// <param name="name"></param>
    public void SetEGM(string name)
    {
        for (int i = 0; i < EGMClips.Count; i++)
        {
            if (EGMClips[i].name.Split('_')[0] == name)
            {
                audioSources[1].clip = EGMClips[i];
            }
        }
    }

    /// <summary>
    /// ��� ����� ���� �޼���
    /// </summary>
    public void AllStop()
    {
        for (int i = 0; i < audioSources.Count; i++)
        {
            audioSources[i].Stop();
        }
    }

}