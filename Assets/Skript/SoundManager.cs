using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [SerializeField]
    private AudioSource audioSource;
    [SerializeField]
    private AudioClip[] clips;  //0:클릭

    public static SoundManager instance;

    private void Awake()    //씬 이동시에 보존 설정
    {
        if (instance == null)
        {
            DontDestroyOnLoad(this);
            instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    public void SetVolum()
    {
        switch (Value.instance.Sound)
        {
            case 0:
                audioSource.mute = true;
                break;
            case 1:
                audioSource.volume = 1f / 3f;
                break;
            case 2:
                audioSource.volume = 2f / 3f;
                break;
            case 3:
                audioSource.mute = false;
                audioSource.volume = 1;
                break;
        }
    }

    public void ShootSound(int _index)
    {
        audioSource.clip = clips[_index];
        audioSource.pitch = Random.Range(0.8f, 1.2f);
        audioSource.Play();
    }
}
