using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using NaughtyAttributes;

public class UIManger : MonoBehaviour   //UI 및 이펙트 표기 클래스
{
    private float time = 0; //시간
    private string timeStr; //시간 표기 변환용 변수
    public int goal = 5;   //목표(열매)
    [SerializeField]
    private GameManager gameManager;
    [SerializeField]
    private Text Score;
    [SerializeField]
    private PlayerController player;
    [SerializeField]
    private GameObject Goal;    //목표 UI
    [SerializeField]
    private Image target1;  //목표 1(열매)
    [SerializeField]
    private Image target2;  //목표 2(열매)
    [SerializeField]
    private Image target3;  //목표 3(열매)
    [SerializeField]
    private Image target4;  //목표 4(열매)
    [SerializeField]
    private Image target5;  //목표 5(열매)
    [SerializeField]
    private GameObject NoticeLeft0; //일반 경고 좌측
    [SerializeField]
    private GameObject NoticeLeft1; //화염 경고 좌측
    [SerializeField]
    private GameObject NoticeRight0;    //일반경고 우측
    [SerializeField]
    private GameObject NoticeRight1;    //화염 경고 우측
    private List<float> Notice0 = new List<float>();  //일반 경고 위치저장 리스트
    private bool[] Notice1 = new bool[29];  //화염 경고 위치저장 리스트
    [ReadOnly]
    public int timeAdd; //열매 수확시 시간 증가량(자동 대입)
    [ReadOnly]
    public int setTime; //시작 시간(자동 대입)
    [ReadOnly]
    public float addLightning;  //열매 수확시 번개충전시간 감소량 (자동 대입)
    [ReadOnly]
    public float addLightningDamage;  //열매수확시 대미지 증가량 (자동 대입)
    [ReadOnly]
    public float addWaterMax;   //열매 수확시 비 유지시간 증가량 (자동 대입)
    [ReadOnly]
    public float addWaterRate;  //열매 수확시 비 효율 증가량 (자동 대입)
    [ReadOnly]
    public float addWaterCharge;    //열매 수확시 비 재충전 시간 감소량 (자동 대입)
    [ReadOnly]
    public float addWaterHeal;  //열매 수확시 비 회복량 증가량 (자동 대입)
    [ReadOnly]
    public float addWaterHealP;  //열매 수확시 비 회복량 증가량 (자동 대입)
    [ReadOnly]
    public float addMoveSpeed;  //열매 수확시 이동속도 증가량 (자동 대입)
    [ReadOnly]
    public float addFirefight;  //열매 수확시 소화 속도 증가량 (자동 대입)
    [ReadOnly]
    public float addStun;   //열매 수확시 공격 스턴시간 증가량 (자동 대입)
    [ReadOnly]
    public float addRange;   //열매 수확시 비범위 증가량 (자동 대입)

    private void Start()
    {
        time = setTime; //시간 설정
        if (gameManager.gameMode == 1)  //무한모드일시 목표 UI제거
        {
            Goal.SetActive(false);
            Goal = null;
        }else if (gameManager.gameMode == 0)
        {
            switch (Value.instance.Level)
            {
                case 1:
                    goal = 1;
                    break;
                case 2:
                    goal = 3;
                    break;
            }
        }
        target1.gameObject.SetActive(goal > 0 ? true : false);  //목표 UI 갱신
        target2.gameObject.SetActive(goal > 1 ? true : false);
        target3.gameObject.SetActive(goal > 2 ? true : false);
        target4.gameObject.SetActive(goal > 3 ? true : false);
        target5.gameObject.SetActive(goal > 4 ? true : false);
    }

    private void Update()
    {
        Timeset();
        NoticeSet();
    }

    void NoticeSet()    //경고 갱신
    {
        bool j = false; //일반 경고 갱신
        bool k = false;
        foreach(float x in Notice0)
        {
            if(x - player.transform.position.x > 10)
            {
                j = true;
                break;
            }else if(x - player.transform.position.x < -10)
            {
                k = true;
                break;
            }
        }
        NoticeRight0.SetActive(j);
        NoticeLeft0.SetActive(k);

        j = false;  //화염 경고 갱신
        k = false;
        for (int i = 0; i < 29; i++)
        {
            if (Notice1[i] && (i - 14) * 2 - player.transform.position.x > 10)
            {
                j = true;
                break;
            }
            else if (Notice1[i] && (i - 14) * 2 - player.transform.position.x < -10)
            {
                k = true;
                break;
            }
        }
        NoticeRight1.SetActive(j);
        NoticeLeft1.SetActive(k);
    }

    public void addNotice(int state, float x) //경고 추가
    {
        switch (state)
        {
            case 0: //일반 경고
                Notice0.Add(x);
                break;
            case 1: //화염 경고
                Notice1[(int)Mathf.Round(x)] = true;
                break;
        }
    }

    public void removeNotice(int state, float x)  //경고 제거
    {
        switch (state)
        {
            case 0: //일반 경고
                Notice0.Remove(x);
                break;
            case 1: //화염 경고
                Notice1[(int)Mathf.Round(x)] = false;
                break;
        }
    }

    public void setPopUp(int key, bool active)  //UI제거 (플래이어와 겹치지 않게 하기 위해서)
    {
        switch(key)
        {
            case 0: //점수 UI제거
                Score?.gameObject.SetActive(active);
                break;
            case 1: //목표 UI제거
                Goal?.gameObject.SetActive(active);
                break;
        }
    }

    public void LevelUP()   //열매수확
    {
        if (goal >= 1)
        {
            player.lightningMax -= addLightning;    //스텟 상승
            if (player.lightningMax < 0.5f) player.lightningMax = 0.5f;
            player.SetLightningDamage(player.GetLightningDamage() + addLightningDamage);
            player.MoveSpeed += addMoveSpeed;
            player.waterRate += addWaterRate;
            player.waterMax += addWaterMax;
            player.waterCharged -= addWaterCharge;
            if (player.waterCharged < 0.5f) player.waterCharged = 0.5f;
            player.HealTree += addWaterHeal;
            player.fireFight += addFirefight;
            player.SetLightningStun(player.GetLightningStun() + addStun);
            player.HealSelfP += addWaterHealP;
            player.RainRange += addRange;
        }
        goal -= 1;  //목표 증가
        time += timeAdd;    //시간(점수) 증가
        target1.gameObject.SetActive(goal > 0 ? true : false);  //목표 UI 갱신
        target2.gameObject.SetActive(goal > 1 ? true : false);
        target3.gameObject.SetActive(goal > 2 ? true : false);
        target4.gameObject.SetActive(goal > 3 ? true : false);
        target5.gameObject.SetActive(goal > 4 ? true : false);

        if (goal < 1 && Value.instance.GameMode == 0) gameManager.EndGame();    //게임 종료 검사
    }

    void Timeset()  //시간 표기 갱신
    {
        if(Value.instance.time_count_down) time -= Time.deltaTime;   //시간증가
        else time += Time.deltaTime;    //시간감소
        if (time < 0) gameManager.EndGame();    //시간이 0이면 게임 종료
        timeStr = "000000" + Mathf.Floor(time); //000,000형식으로 변환
        Score.text = timeStr.Substring(timeStr.Length - 6, 3) + "," + timeStr.Substring(timeStr.Length - 3, 3);
    }
}
