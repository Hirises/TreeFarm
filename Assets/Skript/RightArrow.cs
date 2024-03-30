using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RightArrow : MonoBehaviour     //메뉴의 오른쪽 화살표
{
    [SerializeField]
    private ManuManager manuManager;

    private void OnMouseDown()  //클릭시 다음으로
    {
        SoundManager.instance.ShootSound(0);
        manuManager.Next();
    }
}
