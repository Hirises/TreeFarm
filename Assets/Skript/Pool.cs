using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pool : MonoBehaviour   //오브젝트 풀링
{
    private int index = 0;
    [SerializeField]
    private bool is_reIns = false;
    [SerializeField]
    private int reIns_num = 0;
    [SerializeField]
    private GameObject pool;

    private void Awake()
    {
        if (is_reIns)
        {
            GameObject o;
            for(int i = 0; i < reIns_num; i++)
            {
                o = Instantiate(pool);
                o.SetActive(false);
                o.transform.parent = transform;
            }
        }
    }

    public GameObject getPool() //풀 요소 가져오기
    {
        byte i = 0;
        while (transform.GetChild(index).gameObject.activeSelf == true) //비활성화된 요소를 서치
        {
            index++;
            i++;
            if (index >= transform.childCount)  //인덱스가 넘어가면 초기화
            {
                index = 0;
            }
            if (i > transform.childCount)   //모든 요소가 사용중이면 null을 반환
            {
                return null;
            }
        }
        return transform.GetChild(index).gameObject;    //찾은 요소를 반환
    }
}
