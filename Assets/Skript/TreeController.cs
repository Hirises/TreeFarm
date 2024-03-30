using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class TreeController : MonoBehaviour //나무 클래스
{
    private float EXP = 0;  //경험치 변수
    private float HP;
    private int level; //레벨
    public Sprite[] Trees;     //레벨에 따른 스프라이트
    [SerializeField]
    private SpriteRenderer spriteRenderer;      //렌더러 변수
    private PlayerController player;    //플레이어 변수
    [SerializeField]
    [ReadOnly]
    private float[] needExp;  //레벨업시 필요한 경험치
    [SerializeField]
    [ReadOnly]
    private float[] LevelHP;
    [SerializeField]
    private BoxCollider2D boxCollider1;
    public Sprite[] fire;
    [SerializeField]
    private TreeController left;
    [SerializeField]
    private TreeController right;
    [SerializeField]
    private GameObject HPInd; //0.2 1.8
    [SerializeField]
    private GameObject EXPInd; //0.2 1.8
    [SerializeField]
    private GameObject Firefill; //0.2 1.8
    [SerializeField]
    private GameObject pop;
    [SerializeField]
    private GameObject EXPfill;
    [SerializeField]
    private GameObject[] Apple;
    public float RainRange;
    private float fireBase;  //기본 시작 불 변수
    private float fireFlame; //불 번짐 변수
    private float HealSelf;
    private float fireSelfDamage;
    private float fireOtherDamage;
    private float DamageRes;
    public int X;   //자신의 순서 변수
    private UIManger uiManger;
    public bool is_wet = false;
    public byte state = 0;
    private float fireLevel = 0;

    public bool getActive() //현재 활성화(자라있는)상태인지 반환
    {
        return level > 1;
    }

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        uiManger = GameObject.FindGameObjectWithTag("uimanager").GetComponent<UIManger>();
        RainRange = player.RainRange;
        fireBase = player.fireBase;
        fireFlame = player.fireFlame;
        HealSelf = player.HealSelf;
        needExp = player.needExp;
        LevelHP = player.LevelHP;
        Trees = player.Trees;
        fire = player.fire;
        DamageRes = player.DamageRes;
        fireSelfDamage = player.fireSelfDamage;
        fireOtherDamage = player.fireOtherDamage;
        Reset();    //시작시 리셋
    }

    private void Update()
    {
        Watered();  //물 확인
        LevelCheck();   //레벨업 확인
        Fire(); //불 확인
        Pop();  //팝업 확인
    }

    void Pop()  //팝업 확인
    {
        if (Mathf.Abs(player.transform.position.x - transform.position.x) < RainRange + 2 && level != 1)    //4범위 내에 있을시
        {
            if (pop.activeSelf == false) pop.SetActive(true);   //팝업 활성화
            //HP표기 동기화
            HPInd.transform.localPosition = new Vector3(((level <= 1 ? 1 : (HP / LevelHP[level])) * 1.6f) - 1.8f, HPInd.transform.localPosition.y, HPInd.transform.localPosition.z);
            if(state == 0 && level != 0)    //일반상태 - HP/경험치
            {
                if (EXPfill.activeSelf == false) EXPfill.SetActive(true);
                if (Firefill.activeSelf == true) Firefill.SetActive(false);
                EXPInd.transform.localPosition = 
                    new Vector3(((level == 7 ? 1 : EXP / needExp[level]) * 1.6f) - 1.8f, EXPInd.transform.localPosition.y, EXPInd.transform.localPosition.z);
            }
            else if(state == 1 && level != 0)   //화염상태 - HP/화염
            {
                if (EXPfill.activeSelf == true) EXPfill.SetActive(false);
                if (Firefill.activeSelf == false) Firefill.SetActive(true);
                EXPInd.transform.localPosition = new Vector3(((fireLevel / fireFlame) * 1.6f) - 1.8f, EXPInd.transform.localPosition.y, EXPInd.transform.localPosition.z);
            }else if(level == 0)    //재 상태 - HP(고정)/재생(화염)
            {
                if (EXPfill.activeSelf == true) EXPfill.SetActive(false);
                if (Firefill.activeSelf == false) Firefill.SetActive(true);
                EXPInd.transform.localPosition = new Vector3((((needExp[level] - EXP) / needExp[level]) * 1.6f) - 1.8f, EXPInd.transform.localPosition.y, EXPInd.transform.localPosition.z);
            }

        }
        else //팝업 비활성화
        {
            if(pop.activeSelf == true) pop.SetActive(false);
        }
    }

    void Fire() //불 갱신
    {
        if(state == 1)  //화염 상태면
        {
            Demaged(0, Time.deltaTime * fireSelfDamage); //자신 데미지 (초당 체력의 20%)
            left.Demaged(0, Time.deltaTime * fireOtherDamage); //주변 데미지 (초당 체력의 10%)
            right.Demaged(0, Time.deltaTime * fireOtherDamage);
            if (fireLevel < fireFlame) fireLevel += Time.deltaTime; //화염상태 증가
            if(fireLevel >= fireFlame)  //화염 전파
            {
                left?.Fired(true);
                right?.Fired(true);
            }else if(level == 0)    //재 상태였으면
            {
                left?.Fired(true);  //즉시 화염전파
                right?.Fired(true);
                state = 0;
                Reset();
            }
        }
    }

    public void Fired(bool is_fire) //불 태우기/끄기
    {
        if (is_fire && state == 0 && (Mathf.Abs(player.transform.position.x - transform.position.x) > RainRange || player.state == 0) && level != 1)    //불태우기 (젖어있지 않을시)
        {
            state = 1;
            fireLevel = fireBase;
            spriteRenderer.sprite = fire[level];
            uiManger.addNotice(1, X);   //현위치에 화염 경고 추가
        }else if (!is_fire && state == 1)   //불끄기
        {
            state = 0;
            fireLevel = 0;
            spriteRenderer.sprite = Trees[level];
            uiManger.removeNotice(1, X);    //현위치의 화염 경고 제거
        }
    }

    void Watered()  //물 주기
    {
        if(Mathf.Abs(player.transform.position.x - transform.position.x) < RainRange && player.state == 1)
        {
            if(state == 0 && level < 7) EXP += Time.deltaTime * player.waterRate;   //최대레벨이 아니고 불타고있지 않을시 경험치 증가
            if(HP < LevelHP[level]) HP += Time.deltaTime * player.HealTree; //체력이 최대가 아니면 체력회복
            if (HP < LevelHP[level]) HP += Time.deltaTime * player.HealSelfP * LevelHP[level]; //체력이 최대가 아니면 체력회복
            if (HP > LevelHP[level]) HP = LevelHP[level];
            if(fireLevel > 0 && state == 1) fireLevel -= Time.deltaTime * (player.fireFight + 1); //불타고 있을시 소화 
            if(fireLevel <= 0 && state == 1) Fired(false);  //불끄기
            is_wet = true;  //젖음
        }else {
            is_wet = false; //마름
            if(level == 1 && EXP > 4)   //맨땅 상태일시 경험치 초기화 (맨땅작 방지용)
            {
                EXP -= Time.deltaTime;
            }
        }
        if (HP < LevelHP[level]) HP += Time.deltaTime * HealSelf * LevelHP[level]; //자연회복
    }

    void LevelCheck()   //레벨업 체크
    {
        if(EXP >= needExp[level] && level < 7)   //경험치 체크
        {
            LevelUP();  //레벨업
        }
    }

    void LevelUP()  //레벨업
    {
        level++;    //레벨 증가
        EXP = 0;
        if (state == 0)
        {
            spriteRenderer.sprite = Trees[level];   //모양 변경
        }
        else
        {
            spriteRenderer.sprite = fire[level];   //모양 변경
        }
        if(level == 2)  //활성화시 콜라이더 켜기
        {
            boxCollider1.enabled = true;
        }
        HP += LevelHP[level] - LevelHP[level - 1];  //체력 증가
        if(level == 7)  //최대레벨 도달시 열매 생성
        {
            Apple[Value.instance.TreeMode].SetActive(true);
            GameManager.LevelSet();
        }
    }

    private void OnMouseDown()  //클릭시
    {
        Apples();
    }

    public void Apples()
    {
        if (level == 7 && Apple[Value.instance.TreeMode].activeSelf == true)  //열매 수확
        {
            SoundManager.instance.ShootSound(3);
            Apple[Value.instance.TreeMode].SetActive(false);
            uiManger.LevelUP();
        }
    }

    public void Demaged(float damage, float damageP)    //데미지 받기 (고정딜, 체력 퍼센트딜)
    {
        HP -= damage;
        HP -= LevelHP[level] * damageP * (1 - DamageRes);
        if (HP <= 0)    //사망처리
        {
            Reset();
        }
    }

    void Reset()    //리셋
    {
        if (state == 1)
        {
            state = 0;
            level = 0; //레벨 리셋
        }
        else
        {
            state = 0;
            level = 1; //레벨 리셋
        }
        EXP = 0;    //경험치 리셋
        uiManger.removeNotice(1, X);    //화염 경고 제거
        Apple[Value.instance.TreeMode].SetActive(false); //열매 제거
        fireLevel = 0;;
        HP = LevelHP[level];
        spriteRenderer.sprite = Trees[level];   //모양 리셋
        boxCollider1.enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag.Equals("bomb"))
        {
            Fired(true);
        }
    }
}
