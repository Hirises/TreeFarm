using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeftArrow : MonoBehaviour  //메뉴의 왼쪽 화살표
{
    [SerializeField]
    private ManuManager manuManager;

    private void OnMouseDown()  //클릭시 이전
    {
        SoundManager.instance.ShootSound(0);
        manuManager.Previous();
    }
}
