using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Text;

public class GameManager : MonoBehaviour    //게임실행을 담당하는 클래스
{
    [SerializeField]
    Pool pool;
    [SerializeField]
    private float StartDelay;
    [SerializeField]
    private PlayerController Player;

    [Foldout("Clouds")] //뭉게 구름
    [SerializeField]
    private Sprite[] Clouds_Charged;

    [Foldout("Cumulonimbus")] //적란운
    [SerializeField]
    private Sprite[] Cumulonimbus_Charged;

    [Foldout("Hardwood")]   //활엽수
    [SerializeField]
    private Sprite[] Hardwood_Tree;
    [Foldout("Hardwood")]
    [SerializeField]
    private Sprite[] Hardwood_Fire;

    [Foldout("Pine")]   //침엽수
    [SerializeField]
    private Sprite[] Pine_Tree;
    [Foldout("Pine")]
    [SerializeField]
    private Sprite[] Pine_Fire;

    [SerializeField]
    private UIManger uiManager;
    [SerializeField]
    private PersonController personController;
    [SerializeField]
    private GameObject Pad;
    private Value value;
    public int gameMode;
    private float[] LevelData;
    private string texts;
    [SerializeField]
    [ReadOnly]
    private static int[] S_time = new int[] { -1, -1, -1, -1, -1};
    private int[] S_time_arg;
    private static int[] S_time_add;
    private int[] Fly_Rate;
    private int spawnDelay;
    private int[] B_time = new int[] { -1, -1, -1 };
    private int[] B_time_arg = new int[] { -1, -1, -1};
    [SerializeField]
    [ReadOnly]
    private int[] B_time_base = new int[] { -1, -1, -1 };
    private int[,] B_spawn = new int[3,8];
    [SerializeField]
    [ReadOnly]
    private static int add;

    private void File_IO_set()
    {
        TextAsset Levels = Resources.Load<TextAsset>("Level");
        byte[] bytesforEncoding = Encoding.UTF8.GetBytes(Levels.text);
        string encodedString = System.Convert.ToBase64String(bytesforEncoding);
        byte[] decodedBytes = System.Convert.FromBase64String(encodedString);
        texts = Encoding.UTF8.GetString(decodedBytes);
    }

    private void File_IO(int read_index)
    {   
        string[] Lines = texts.Split('\n');
        string[] Raws;
        int index = 0;
        bool is_read = false;

        LevelData = new float[60];

        foreach (string str in Lines)
        {
            Raws = str.Split('\t');

            if (Raws.Length == 2)
            {
                if(is_read == true)
                {
                    return;
                }
                else if (read_index == int.Parse(Raws[0]))
                {
                    is_read = true;
                }
            }
            else if(Raws.Length == 4 && is_read)
            {
                LevelData[index++] = float.Parse(Raws[3]);
            }
            else if (Raws.Length == 5 && is_read)
            {
                LevelData[index++] = float.Parse(Raws[4]);
            }
        }
    }

    private void Awake()    //정보 동기화
    {
        add = 0;

        File_IO_set();

        value = GameObject.FindGameObjectWithTag("value").GetComponent<Value>();

        switch (value.GameMode) //게임모드 동기화
        {
            case 0:
                File_IO(0);
                value.time_count_down = true;
                break;
            case 1:
                File_IO(1);
                value.time_count_down = false;
                break;
            case 2:
                setMode();
                return;
        }
        uiManager.setTime = (int)LevelData[0];
        uiManager.timeAdd = (int)LevelData[1];

        switch (value.TreeMode) //나무 종류 동기화
        {
            case 0:
                File_IO(2);
                Player.Trees = Hardwood_Tree;
                Player.fire = Hardwood_Fire;
                
                break;
            case 1:
                File_IO(3);
                Player.Trees = Pine_Tree;
                Player.fire = Pine_Fire;
                break;
        }
        Player.LevelHP = new float[] { LevelData[0], LevelData[1], LevelData[2], LevelData[3], LevelData[4], LevelData[5], LevelData[6], LevelData[7] };
        Player.needExp = new float[] { LevelData[8], LevelData[9], LevelData[10], LevelData[11], LevelData[12], LevelData[13], LevelData[14], LevelData[15] };
        Player.HealSelf = LevelData[16];
        Player.DamageRes = LevelData[17];
        Player.fireBase = LevelData[18];
        Player.fireFlame = LevelData[19];
        Player.fireSelfDamage = LevelData[20];
        Player.fireOtherDamage = LevelData[21];

        switch (value.CloudMode)    //구름 종류 동기화
        {
            case 0:
                File_IO(4);
                Player.Clouds = Clouds_Charged;
                break;
            case 1:
                File_IO(5);
                Player.Clouds = Cumulonimbus_Charged;
                break;
        }
        Player.MoveSpeed = LevelData[0];
        Player.waterMax = LevelData[1];
        Player.waterCharged = LevelData[2];
        Player.waterRate = LevelData[3];
        Player.HealTree = LevelData[4];
        Player.HealSelfP = LevelData[5];
        Player.fireFight = LevelData[6];
        Player.RainRange = LevelData[7];
        Player.SetLightningDamage(LevelData[8]);
        Player.SetLightningStun(LevelData[9]);
        Player.lightningMax = LevelData[10];
        uiManager.addMoveSpeed = LevelData[11];
        uiManager.addWaterMax = LevelData[12];
        uiManager.addWaterCharge = LevelData[13];
        uiManager.addWaterRate = LevelData[14];
        uiManager.addWaterHeal = LevelData[15];
        uiManager.addWaterHealP = LevelData[16];
        uiManager.addFirefight = LevelData[17];
        uiManager.addRange = LevelData[18];
        uiManager.addLightningDamage = LevelData[19];
        uiManager.addStun = LevelData[20];
        uiManager.addLightning = LevelData[21];

        //기타 디자인 수치 동기화
        personController.PersonHP = new float[8];
        personController.moveSpeed = new float[8];
        personController.Damage = new float[8];
        personController.DamagePersent = new float[8];
        personController.attcktime = new float[8];
        for(int i = 0; i < 8; i++)
        {
            File_IO(6 + i);
            personController.PersonHP.SetValue(LevelData[0], i);
            personController.moveSpeed.SetValue(LevelData[1], i);
            personController.Damage.SetValue(LevelData[2], i);
            personController.DamagePersent.SetValue(LevelData[3], i);
            personController.attcktime.SetValue(LevelData[4], i);
        }

        //스폰처리 동기화
        switch (value.GameMode)
        {
            case 0:
                File_IO(14 + value.Level);
                break;
            case 1:
                File_IO(14);
                break;
        }
        S_time = new int[] { (int)LevelData[0], (int)LevelData[3], (int)LevelData[6], (int)LevelData[9], (int)LevelData[12] };
        S_time_arg = new int[] { (int)LevelData[1], (int)LevelData[4], (int)LevelData[7], (int)LevelData[10], (int)LevelData[13] };
        S_time_add = new int[] { (int)LevelData[2], (int)LevelData[5], (int)LevelData[8], (int)LevelData[11], (int)LevelData[14] };
        Fly_Rate = new int[] { (int)LevelData[15], (int)LevelData[16], (int)LevelData[17], (int)LevelData[18] };
        spawnDelay = (int)LevelData[19];

        int _index = 0;
        for(int i = 0; i < (int)LevelData[20]; i++)
        {
            B_time_base[0] = (int)LevelData[21 + (_index * 11)];
            B_time[0] = (int)LevelData[22 + (_index * 11)];
            B_time_arg[0] = (int)LevelData[23 + (_index * 11)];
            B_spawn[_index, 0] = (int)LevelData[24 + (_index * 11)];
            B_spawn[_index, 1] = (int)LevelData[25 + (_index * 11)];
            B_spawn[_index, 2] = (int)LevelData[26 + (_index * 11)];
            B_spawn[_index, 3] = (int)LevelData[27 + (_index * 11)];
            B_spawn[_index, 4] = (int)LevelData[28 + (_index * 11)];
            B_spawn[_index, 5] = (int)LevelData[29 + (_index * 11)];
            B_spawn[_index, 6] = (int)LevelData[30 + (_index * 11)];
            B_spawn[_index, 7] = (int)LevelData[31 + (_index * 11)];
            _index++;
        }

        gameMode = value.GameMode;  //게임모드 저장
        LevelData = null;

        //플랫폼 처리
#if UNITY_EDITOR_WIN
        Pad.SetActive(false);   //컴퓨터 실행시 터치패드 제거
#endif
    }

    //모드 - 추후추가예정
    #region
    private void setMode()
    {
        //기본 종료(패배)조건
        switch (Random.Range(0, 1)) 
        {
            case 0: //시간
                value.EndMode = 0;
                value.time_count_down = true;
                break;
            case 1: //수호
                value.EndMode = 1;
                value.time_count_down = true;
                break;
        }

        //기본 종료(승리) 조건
        uiManager.goal = Random.Range(1, 5);
        uiManager.setTime += uiManager.goal * 30;

        //추가 종료(승리) 조건
        switch (Random.Range(0, 1))
        {
            case 0: //나무꾼 30명 처치
                value.WinMode = 0;
                uiManager.setTime += 30;
                break;
            case 1: //없음
                value.WinMode = 1;
                break;
        }

        //추가 규칙
        switch(Random.Range(0, 1))
        {
            case 0: //없음
                value.EctMode = 0;
                break;
            case 1: //나무꾼이 화면밖으로 5번 넘어가면 패배
                value.EndMode = 1;
                break;

        }
        
        //나무 모드
        switch (Random.Range(0,1))
        {
            case 0: //쾌속 성장
                Player.Trees = Hardwood_Tree;
                Player.fire = Hardwood_Fire;
                File_IO(2);
                Player.LevelHP = new float[] { LevelData[0], LevelData[1], LevelData[2], LevelData[3], LevelData[4], LevelData[5], LevelData[6], LevelData[7] };
                Player.needExp = new float[] { LevelData[8], LevelData[9]/2, LevelData[10]/2, LevelData[11]/2, LevelData[12]/2, LevelData[13]/2, LevelData[14]/2, LevelData[15]/2 };
                Player.HealSelf = LevelData[16];
                Player.DamageRes = LevelData[17];
                Player.fireBase = LevelData[18];
                Player.fireFlame = LevelData[19];
                Player.fireSelfDamage = LevelData[20];
                Player.fireOtherDamage = LevelData[21];

                uiManager.setTime -= 60;
                break;
            case 1: //느린 성장
                Player.Trees = Pine_Tree;
                Player.fire = Pine_Fire;
                File_IO(3);
                Player.LevelHP = new float[] { LevelData[0], LevelData[1], LevelData[2], LevelData[3], LevelData[4], LevelData[5], LevelData[6], LevelData[7] };
                Player.needExp = new float[] { LevelData[8], LevelData[9], LevelData[10] + 2, LevelData[11] + 3, LevelData[12] + 4, LevelData[13] + 5, LevelData[14] + 8, LevelData[15] + 10 };
                Player.HealSelf = LevelData[16];
                Player.DamageRes = LevelData[17];
                Player.fireBase = LevelData[18];
                Player.fireFlame = LevelData[19];
                Player.fireSelfDamage = LevelData[20];
                Player.fireOtherDamage = LevelData[21];

                uiManager.setTime += 120;
                break;
            case 2: //없음
                Player.Trees = Hardwood_Tree;
                Player.fire = Hardwood_Fire;
                File_IO(2);
                Player.LevelHP = new float[] { LevelData[0], LevelData[1], LevelData[2], LevelData[3], LevelData[4], LevelData[5], LevelData[6], LevelData[7] };
                Player.needExp = new float[] { LevelData[8], LevelData[9], LevelData[10], LevelData[11], LevelData[12], LevelData[13], LevelData[14], LevelData[15] };
                Player.HealSelf = LevelData[16];
                Player.DamageRes = LevelData[17];
                Player.fireBase = LevelData[18];
                Player.fireFlame = LevelData[19];
                Player.fireSelfDamage = LevelData[20];
                Player.fireOtherDamage = LevelData[21];
                break;
        }

        switch (Random.Range(0, 1)) //나무꾼 규칙
        {
            case 0: //전부 낙하산
                //기타 디자인 수치 동기화
                personController.PersonHP = new float[8];
                personController.moveSpeed = new float[8];
                personController.Damage = new float[8];
                personController.DamagePersent = new float[8];
                personController.attcktime = new float[8];
                for (int i = 0; i < 8; i++)
                {
                    File_IO(6 + i);
                    personController.PersonHP.SetValue(LevelData[0], i);
                    personController.moveSpeed.SetValue(LevelData[1], i);
                    personController.Damage.SetValue(LevelData[2], i);
                    personController.DamagePersent.SetValue(LevelData[3], i);
                    personController.attcktime.SetValue(LevelData[4], i);
                }
                S_time = new int[] { -1, -1, -1, -1, 7 };
                S_time_arg = new int[] { 0, 0, 0, 0, 3 };
                S_time_add = new int[] { 0, 0, 0, 0, 1 };
                Fly_Rate = new int[] { 10, 7, 3, 1 };
                spawnDelay = 20;
                break;
            case 1: //없음
                personController.PersonHP = new float[8];
                personController.moveSpeed = new float[8];
                personController.Damage = new float[8];
                personController.DamagePersent = new float[8];
                personController.attcktime = new float[8];
                for (int i = 0; i < 8; i++)
                {
                    File_IO(6 + i);
                    personController.PersonHP.SetValue(LevelData[0], i);
                    personController.moveSpeed.SetValue(LevelData[1], i);
                    personController.Damage.SetValue(LevelData[2], i);
                    personController.DamagePersent.SetValue(LevelData[3], i);
                    personController.attcktime.SetValue(LevelData[4], i);
                }
                File_IO(14);
                S_time = new int[] { (int)LevelData[0], (int)LevelData[3], (int)LevelData[6], (int)LevelData[9], (int)LevelData[12] };
                S_time_arg = new int[] { (int)LevelData[1], (int)LevelData[4], (int)LevelData[7], (int)LevelData[10], (int)LevelData[13] };
                S_time_add = new int[] { (int)LevelData[2], (int)LevelData[5], (int)LevelData[8], (int)LevelData[11], (int)LevelData[14] };
                Fly_Rate = new int[] { (int)LevelData[15], (int)LevelData[16], (int)LevelData[17], (int)LevelData[18] };
                spawnDelay = (int)LevelData[19];
                break;
        }

        switch (Random.Range(0, 1)) //구름 모드
        {
            case 0: //없음
                File_IO(4);
                Player.Clouds = Clouds_Charged;
                Player.MoveSpeed = LevelData[0];
                Player.waterMax = LevelData[1];
                Player.waterCharged = LevelData[2];
                Player.waterRate = LevelData[3];
                Player.HealTree = LevelData[4];
                Player.HealSelfP = LevelData[5];
                Player.fireFight = LevelData[6];
                Player.RainRange = LevelData[7];
                Player.SetLightningDamage(LevelData[8]);
                Player.SetLightningStun(LevelData[9]);
                Player.lightningMax = LevelData[10];
                uiManager.addMoveSpeed = LevelData[11];
                uiManager.addWaterMax = LevelData[12];
                uiManager.addWaterCharge = LevelData[13];
                uiManager.addWaterRate = LevelData[14];
                uiManager.addWaterHeal = LevelData[15];
                uiManager.addWaterHealP = LevelData[16];
                uiManager.addFirefight = LevelData[17];
                uiManager.addRange = LevelData[18];
                uiManager.addLightningDamage = LevelData[19];
                uiManager.addStun = LevelData[20];
                uiManager.addLightning = LevelData[21];
                break;
            case 1: //쾌속
                File_IO(4);
                Player.Clouds = Clouds_Charged;
                Player.MoveSpeed = LevelData[0] + 5;
                Player.waterMax = LevelData[1];
                Player.waterCharged = LevelData[2];
                Player.waterRate = LevelData[3];
                Player.HealTree = LevelData[4];
                Player.HealSelfP = LevelData[5];
                Player.fireFight = LevelData[6];
                Player.RainRange = LevelData[7];
                Player.SetLightningDamage(LevelData[8]);
                Player.SetLightningStun(LevelData[9]);
                Player.lightningMax = LevelData[10];
                uiManager.addMoveSpeed = LevelData[11];
                uiManager.addWaterMax = LevelData[12];
                uiManager.addWaterCharge = LevelData[13];
                uiManager.addWaterRate = LevelData[14];
                uiManager.addWaterHeal = LevelData[15];
                uiManager.addWaterHealP = LevelData[16];
                uiManager.addFirefight = LevelData[17];
                uiManager.addRange = LevelData[18];
                uiManager.addLightningDamage = LevelData[19];
                uiManager.addStun = LevelData[20];
                uiManager.addLightning = LevelData[21];
                break;
        }

        if(value.EndMode == 0)
        {
            uiManager.setTime += 120;
        }
        else
        {
            uiManager.setTime = 120 - uiManager.setTime;
        }

        if(uiManager.setTime < 120 && value.time_count_down)
        {
            uiManager.setTime = 120;
        }
        LevelData = null;
    }
    #endregion  //모드 - 추후추가 예정  //모드 - 추후추가예정

    private int getType()
    {
        int i = Random.Range(0, Fly_Rate[0] + Fly_Rate[1] + Fly_Rate[2] + Fly_Rate[3]);
        if (i < Fly_Rate[0])
        {
            return 4;
        }else if (i < Fly_Rate[0] + Fly_Rate[1])
        {
            return 5;
        }
        else if (i < Fly_Rate[0] + Fly_Rate[1] + Fly_Rate[2])
        {
            return 6;
        }
        else
        {
            return 7;
        }
    }

    public void EndGame()   //게임 종료
    {
        SceneManager.LoadScene("ScoreScene");
    }

    private void Start()    //게임 시작시 적 스폰
    {
        if(S_time[0] > -1) StartCoroutine("Spawn0");
        if (S_time[1] > -1) StartCoroutine("Spawn1");
        if (S_time[2] > -1) StartCoroutine("Spawn2");
        if (S_time[4] > -1) StartCoroutine("Spawn3");
        if (S_time[3] > -1) StartCoroutine("Spawn4");
        if (B_time_base[0] > -1) StartCoroutine("SpawnBurn1");
        if (B_time_base[1] > -1) StartCoroutine("SpawnBurn2");
        if (B_time_base[2] > -1) StartCoroutine("SpawnBurn3");
    }

    public static void LevelSet()
    {
        if (add >= 5)
        {
            return;
        }
        add++;
        for (int i = 0; i < S_time.Length; i++)
        {
            S_time[i] -= S_time_add[i];
        }
    }

    public void spawnEntity(int level)
    {
        PersonController o = pool.getPool()?.GetComponent<PersonController>();
        if (o != null)
        {
            o.setLevel(level);
            o.gameObject.SetActive(true);
        }
    }

    IEnumerator Spawn0()    //도끼
    {
        yield return new WaitForSeconds(StartDelay);
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(S_time[0], S_time[0] + S_time_arg[0]));
            spawnEntity(0);
        }
    }
    IEnumerator Spawn1()    //전기톱
    {
        yield return new WaitForSeconds(StartDelay);
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(S_time[1], S_time[1] + S_time_arg[1]));
            spawnEntity(1);
        }
    }
    IEnumerator Spawn2()    //화염방사기
    {
        yield return new WaitForSeconds(StartDelay);
        yield return new WaitWhile( () => { return uiManager.goal == 5 && value.GameMode == 1; });
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(S_time[2], S_time[2] + S_time_arg[2]));
            spawnEntity(2);
        }
    }
    IEnumerator Spawn3()    //낙하산
    {
        yield return new WaitForSeconds(StartDelay);
        yield return new WaitWhile(() => { return uiManager.goal >= 4 && value.GameMode == 1; });
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(S_time[4], S_time[4] + S_time_arg[4]));
            spawnEntity(getType());
        }
    }
    IEnumerator Spawn4()    //양복
    {
        yield return new WaitForSeconds(StartDelay);
        yield return new WaitWhile(() => { return uiManager.goal >= 3 && value.GameMode == 1; });
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(S_time[3], S_time[3] + S_time_arg[3]));
            spawnEntity(3);
        }
    }
    IEnumerator SpawnBurn1()    //번1
    {
        yield return new WaitForSeconds(StartDelay);
        yield return new WaitWhile( () => { return B_time_base[0] < add; });
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(B_time[0], B_time[0] + B_time_arg[0]));
            for (int j = 0; j < 8; j++)
            {
                for (int i = 0; i < B_spawn[0, j]; i++)
                {
                    spawnEntity(j);
                    yield return new WaitForSeconds(0.1f);
                }
            }
        }
    }
    IEnumerator SpawnBurn2()    //번2
    {
        yield return new WaitForSeconds(StartDelay);
        yield return new WaitWhile(() => { return B_time_base[1] < add; });
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(B_time[1], B_time[1] + B_time_arg[1]));
            for (int j = 0; j < 8; j++)
            {
                for (int i = 0; i < B_spawn[1, j]; i++)
                {
                    spawnEntity(j);
                    yield return new WaitForSeconds(0.1f);
                }
            }
        }
    }
    IEnumerator SpawnBurn3()    //번3
    {
        yield return new WaitForSeconds(StartDelay);
        yield return new WaitWhile(() => { return B_time_base[2] < add; });
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(B_time[2], B_time[2] + B_time_arg[2]));
            for (int j = 0; j < 8; j++)
            {
                for (int i = 0; i < B_spawn[2, j]; i++)
                {
                    spawnEntity(j);
                    yield return new WaitForSeconds(0.1f);
                }
            }
        }
    }
}
