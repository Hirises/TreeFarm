using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour   //카메라 제어
{
    [SerializeField]
    private GameObject player;  //플레이어 변수
    [SerializeField]
    private float leftEnd;  //카메라 이동한계(좌측)
    [SerializeField]
    private float rightEnd; //카메라 이동한계(우측)

    private void LateUpdate()
    {
        MatchCamera();      //카메라 위치 갱신
    }

    void MatchCamera()  //카메라 위치 갱신
    {
        transform.position = new Vector3(player.transform.position.x, transform.position.y, transform.position.z);          //플레이어 x축 따라 이동
        if(transform.position.x < leftEnd) transform.position = new Vector3(leftEnd, transform.position.y, transform.position.z);       //한계값 검사
        if (transform.position.x > rightEnd) transform.position = new Vector3(rightEnd, transform.position.y, transform.position.z);
    }
}
