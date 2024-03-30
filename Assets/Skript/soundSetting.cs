using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class soundSetting : MonoBehaviour
{
    [SerializeField]
    private Sprite[] SoundImages;
    [SerializeField]
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer.sprite = SoundImages[Value.instance.Sound];
    }

    private void OnMouseDown()
    {
        SoundManager.instance.ShootSound(0);
        if (--Value.instance.Sound < 0)
        {
            Value.instance.Sound = 3;
        }
        Value.instance.ResetSound();
        SoundManager.instance.SetVolum();
        spriteRenderer.sprite = SoundImages[Value.instance.Sound];
    }
}
