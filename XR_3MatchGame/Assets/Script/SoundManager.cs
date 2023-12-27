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
    /// 씬에 존재하는 오디오 소스와 씬에서 사용할 오디오 클립들을
    /// 세팅하는 메서드
    /// </summary>
    public void Initialize(SceneType type)
    {
        // 한번 초기화
        audioSources.Clear();

        // BGM, EGM 찾기
        audioSources.Add(GameObject.Find("BGM").GetComponent<AudioSource>());
        audioSources.Add(GameObject.Find("EGM").GetComponent<AudioSource>());

        // 처음 실행은 배경음악 설정
        SetBGM(type.ToString());
    }

    /// <summary>
    /// 배경 음악 세팅 메서드
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
    /// 효과음 세팅 메서드
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
    /// 모든 오디오 정지 메서드
    /// </summary>
    public void AllStop()
    {
        for (int i = 0; i < audioSources.Count; i++)
        {
            audioSources[i].Stop();
        }
    }

}