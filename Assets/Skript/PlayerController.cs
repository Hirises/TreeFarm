using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour   //플레이어 클래스
{
    public float MoveSpeed;    //이동 속도
    [SerializeField]
    private float leftEnd;  //이동 한계값(좌측)
    [SerializeField]
    private float rightEnd; //이동 한계값(우측)
    public float waterRate;  //현재 물 계수
    public float HealTree;  //비 치유량
    [SerializeField]
    private Pool rainPool;
    [SerializeField]
    private RectTransform lightMask;   //-0.46  -1.55
    public float lightningMax;
    public GameObject lightBlot;
    [SerializeField]
    private RectTransform waterMask;   //-0.46  -1.55
    [SerializeField]
    private GameObject M;   //산 배경
    [SerializeField]
    private GameObject C1;  //구름 배경1
    [SerializeField]
    private GameObject C2;  //구름 배경2
    [SerializeField]
    private UIManger uiManger;
    public Sprite[] Clouds;
    [SerializeField]
    private SpriteRenderer spriteRenderer;
    public float waterMax;
    public float waterCharged;
    public float fireFight;
    private float water;
    private float lightning = 0;
    public int state = 1;   //현재 상태 0: 비충전 1: 비내림
    private float t1 = 0;   //비 이펙트 소환용 타이머
    public float RainRange;
    public float HealSelfP;

    public Sprite[] fire;
    public Sprite[] Trees;     //레벨에 따른 스프라이트
    public float fireBase;  //기본 시작 불 변수
    public float fireFlame; //불 번짐 변수
    public float HealSelf;
    public float[] needExp;  //레벨업시 필요한 경험치
    public float[] LevelHP;
    public float fireSelfDamage;
    public float fireOtherDamage;
    public float DamageRes;

    private void Start()    //시작시 초기화
    {
        water = waterMax;
        spriteRenderer.sprite = Clouds[0];
    }

    void Update()
    {
        Move();     //이동
        MoveBackGround();   //배경 이동
        BarrierCheck();     //한계값 체크
        SpawnRain();    //비 이펙트 소환
        chargedLightning(); //번개 충전
    }

    void MoveBackGround()   //배경 이동
    {
        M.transform.localPosition = new Vector3(transform.position.x / 56f * -1.5f, M.transform.localPosition.y, M.transform.localPosition.z);
        C1.transform.localPosition = new Vector3(transform.position.x / 56f * -3f - 6, C1.transform.localPosition.y, C1.transform.localPosition.z);
        C2.transform.localPosition = new Vector3(transform.position.x / 56f * -3f + 5, C2.transform.localPosition.y, C2.transform.localPosition.z);
    }

    public void SetLightningDamage(float damage)    //번개 데미지 설정
    {
        lightBlot.GetComponent<Light>().Damage = damage;
    }

    public float GetLightningDamage()   //번개 데미지 읽기
    {
        return lightBlot.GetComponent<Light>().Damage;
    }

    public void SetLightningStun(float damage)  //스턴 시간 설정
    {
        lightBlot.GetComponent<Light>().Stun = damage;
    }

    public float GetLightningStun() //스턴 시간 읽기
    {
        return lightBlot.GetComponent<Light>().Stun;
    }

    void Move()     //이동
    {
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow) || (TouchPad_Left.Touch_Left && !TouchPad_Right.Touch_Right))
        {
            transform.Translate(Vector3.left * Time.deltaTime * MoveSpeed);
        }
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow) || (!TouchPad_Left.Touch_Left && TouchPad_Right.Touch_Right))
        {
            transform.Translate(Vector3.right * Time.deltaTime * MoveSpeed);
        }
    }

    void BarrierCheck()     //한계값 체크
    {
        if (transform.position.x < leftEnd) transform.position = new Vector3(leftEnd, transform.position.y, transform.position.z);
        if (transform.position.x > rightEnd) transform.position = new Vector3(rightEnd, transform.position.y, transform.position.z);
        if (transform.position.x < -24.5f) uiManger.setPopUp(0, false);
        else uiManger.setPopUp(0, true);
        if (transform.position.x > 21f) uiManger.setPopUp(1, false);
        else uiManger.setPopUp(1, true);
    }

    void SpawnRain()
    {
        if (state == 1) //비 내리는 상태면
        {
            if(t1 > 0.1)    //0.1초마다 이펙트 생성
            {
                GameObject o = rainPool.getPool();
                if (o != null)
                {
                    o.transform.position = transform.position;
                    o.transform.Translate(Vector3.right * Random.Range(-RainRange, RainRange)); //위치 랜덤
                    o.SetActive(true);
                }
                t1 = 0;
            }else
            {
                t1 += Time.deltaTime;
            }
            water -= Time.deltaTime;
            waterMask.position = new Vector3(waterMask.position.x, ((water / waterMax) * 1.09f) + 2.95f, waterMask.position.z); //물 표기 갱신
            if (water <= 0) state = 0;
        }
        if(state == 0)  //물 충전 상태면
        {
            water += waterMax / waterCharged * Time.deltaTime;
            waterMask.position = new Vector3(waterMask.position.x, ((water / waterMax) * 1.09f) + 2.95f, waterMask.position.z); //물 표기 갱신
            if (water >= waterMax) state = 1;

        }
    }

    void chargedLightning() //번개 충전
    {
        if (lightning < lightningMax && lightning >= 0)
        {
            lightning += Time.deltaTime;
            lightMask.position = new Vector3(lightMask.position.x, ((lightning / lightningMax) * 1.09f) + 2.95f, lightMask.position.z); //번개 표시 갱신
        }else if(lightning > lightningMax)
        {
            spriteRenderer.sprite = Clouds[1];  //충전완료시 이미지 변경
        }
        if ((Input.GetKeyDown(KeyCode.Space) || (TouchPad_Left.Touch_Left && TouchPad_Right.Touch_Right)) && lightning >= lightningMax)   //번개 사용
        {
            SoundManager.instance.ShootSound(1);
            StartCoroutine("Light");
            spriteRenderer.sprite = Clouds[0];  //번개 사용시 이미지 변경
        }
        if(lightning <= 0)
        {
            lightning += Time.deltaTime;
            lightMask.position = new Vector3(lightMask.position.x, ((lightning / -0.2f) * 1.09f) + 2.95f, lightMask.position.z);    //번개 표시 갱신
        }
    }

    IEnumerator Light() //번개 생성 코루틴
    {
        lightning = -1;
        lightBlot.transform.position = new Vector3(transform.position.x, lightBlot.transform.position.y, lightBlot.transform.position.z);
        lightBlot.SetActive(true);
        yield return new WaitForSeconds(0.2f);
        lightBlot.SetActive(false);
        lightning = 0;
        lightMask.position = new Vector3(lightMask.position.x, -1.55f, lightMask.position.z);
    }
}
