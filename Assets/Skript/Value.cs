using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Value : MonoBehaviour  //게임씬으로 넘길 변수들
{
    [SerializeField]
    public int TreeMode;    //나무 종류 0:활엽수
    [SerializeField]
    public int CloudMode;   //구름 종류 0:뭉게구름
    [SerializeField]
    public int GameMode;    //게임 모드 0:첼린지모드 1:무한모드
    [SerializeField]
    public int Level;      //첼린지 단계
    [SerializeField]
    public bool time_count_down;
    [SerializeField]
    public int EndMode;
    [SerializeField]
    public int WinMode;
    [SerializeField]
    public int EctMode;
    [SerializeField]
    private AudioSource back;
    [SerializeField]
    public int Sound;
    static public Value instance;

    private void Awake()    //씬 이동시에 보존 설정
    {
        if (instance == null)
        {
            Screen.SetResolution(1920, 1080, false);
            DontDestroyOnLoad(this);
            instance = this;
            back.Play();
            Sound = 3;
        }else
        {
            Destroy(this);
        }
    }

    public void ResetSound()
    {
        switch (Sound)
        {
            case 0:
                back.mute = true;
                back.Stop();
                break;
            case 1:
                back.volume = 1f / 3f;
                break;
            case 2:
                back.volume = 2f / 3f;
                break;
            case 3:
                back.mute = false;
                back.volume = 1;
                back.Play();
                break;
        }
    }
}
