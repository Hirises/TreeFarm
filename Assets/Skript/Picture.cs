using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Picture : MonoBehaviour    //메뉴의 미리보기 사진
{
    [SerializeField]
    private ManuManager manuManager;

    private void OnMouseUp()  //클릭시 선택
    {
        SoundManager.instance.ShootSound(0);
        manuManager.Select();
    }
}
