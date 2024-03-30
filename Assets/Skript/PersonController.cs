using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersonController : MonoBehaviour   //나무꾼 클래스
{
    private sbyte dir = 1;   //오른쪽일때 1
    private bool is_hit = false;
    private int level = 0;
    private float HP;
    private TreeController tree = null;
    public float[] moveSpeed;
    public float[] Damage; //고정딜
    public float[] DamagePersent;  //체력 퍼센트 딜
    public float[] attcktime;  //공속
    [SerializeField]
    private Sprite[] Person;
    public float[] PersonHP;
    [SerializeField]
    private SpriteRenderer spriteRenderer;
    [SerializeField]
    private float leftEnd;  //이동 한계값(좌측)
    [SerializeField]
    private float rightEnd; //이동 한계값(우측)
    [SerializeField]
    private ParticleSystem FirePart;    //화염방사기 이펙트
    [SerializeField]
    private ParticleSystem DustPart;    //전기톱 이펙트
    [SerializeField]
    private GameObject Particle;    //스턴 이펙트
    [SerializeField]
    private GameObject moneyEffect; //강화 이펙트
    [SerializeField]
    private GameObject effectArea;  //강화 효과 범위
    private Pool stuffPool;
    private UIManger uiManger;
    private float stun;
    private bool is_rain;   //강화 상태
    private bool is_place;
    private PlayerController player;
    private float t2;

    public void setLevel(int level) //레벨 설정 (활성화 하기전 미리 호출)
    {
        this.level = level;
        spriteRenderer.sprite = Person[level];
        HP = PersonHP[level];
    }

    private void OnEnable() //활성화시 초기화
    {
        if (uiManger == null) uiManger = GameObject.FindGameObjectWithTag("uimanager").GetComponent<UIManger>();

        t2 = 0;
        stuffPool = GameObject.FindGameObjectWithTag("stuff").GetComponent<Pool>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();

        is_place = false;
        is_rain = false;
        Particle.SetActive(false);
        is_hit = false;

        if (level < 4) {    //일반 유닛이면
            if (Random.Range(0, 2) == 0)    //좌,우로 랜덤 배치
            {
                transform.position = new Vector3(-33, -4.185f, 0);
                dir = 1;
                transform.localScale = new Vector3(1, transform.localScale.y, transform.localScale.z);
                moneyEffect.GetComponent<SpriteRenderer>().flipX = false;
            } else
            {
                transform.position = new Vector3(33, -4.185f, 0);
                dir = -1;
                transform.localScale = new Vector3(-1, transform.localScale.y, transform.localScale.z);
                moneyEffect.GetComponent<SpriteRenderer>().flipX = true;
            }
        }else if(level < 8) //낙하산 유닛이면
        {
            if (Random.Range(0, 2) == 0)    //위로 배치
            {
                transform.position = new Vector3(Random.Range(0f, -20f), 6, 0);
                dir = 1;
                transform.localScale = new Vector3(1, transform.localScale.y, transform.localScale.z);
                moneyEffect.GetComponent<SpriteRenderer>().flipX = false;
            }
            else
            {
                transform.position = new Vector3(Random.Range(0f, 20f), 6, 0);
                dir = -1;
                transform.localScale = new Vector3(-1, transform.localScale.y, transform.localScale.z);
                moneyEffect.GetComponent<SpriteRenderer>().flipX = true;
            }
        }
        if(level == 3)
        {
            effectArea.SetActive(true);
        }
    }

    private void Update()
    {
        if(!is_hit && level < 4) transform.Translate(Vector3.right * dir * Time.deltaTime * moveSpeed[level]);  //일반 유닛 - 이동
        if (!is_hit && level < 4 && is_rain) transform.Translate(Vector3.right * dir * Time.deltaTime * moveSpeed[level] * 0.5f);  //일반 유닛 - 이동 - 강화
        if (level > 3) transform.Translate(Vector3.down * Time.deltaTime * moveSpeed[level]);    //낙하산 유닛 - 낙하
        if (level > 3 && transform.position.y < -4.18f) Falldown(); //낙하산 유닛 - 낙하산 해제
        if (transform.position.x < leftEnd) gameObject.SetActive(false);    //화면 밖으로 나가면 삭제
        if (transform.position.x > rightEnd) gameObject.SetActive(false);
    }

    void Falldown() //낙하산 해제
    {
        setLevel(level - 4);    //일반유닛으로 변경
        transform.position = new Vector3(transform.position.x, -4.185f, 0); //위치 재조정
    }

    private void OnTriggerEnter2D(Collider2D collision) //콜라이더 활성화
    {
        if (collision.tag.Equals("tree") && tree == null && level < 4)  //나무에 닿았을시
        {
            is_hit = true;  //닿음 설정
            tree = collision.gameObject.GetComponent<TreeController>(); //나무 클래스 저장
            if(level != 3) StartCoroutine("Hit");  //데미지
            if (level == 2) StartCoroutine("Hit1"); //화염방사기 - 특수공격
            if (level == 3 && !is_place)
            {
                Stuff o = stuffPool.getPool()?.GetComponent<Stuff>();
                if (o != null)
                {
                    o.transform.position = this.transform.position;
                    o.gameObject.SetActive(true);
                }
                is_place = true;
            }
            if (level == 2)
            {
                FirePart.Play();    //화염방사기 이펙트 실행
            }
            else if (level == 1)
            {
                DustPart.Play();    //전기톱 이펙트 실행
            }
        }else if (collision.tag.Equals("light"))    //번개
        {
            Damaged(collision.gameObject.GetComponent<Light>().Damage); //번개 데미지 받기
            stun = collision.gameObject.GetComponent<Light>().Stun; //스턴 시간 저장
            if (HP > 0) //스턴 실행
            {
                StopCoroutine("Hit");
                if(level == 2) StopCoroutine("Hit1");
                StartCoroutine("Lightning");
            }
        }else if (collision.tag.Equals("money"))
        {
            if(level < 3)
            {
                moneyEffect.SetActive(true);
                is_rain = true;
            }
        }
    }

    IEnumerator Lightning() //스턴
    {
        is_hit = true;
        Particle.SetActive(true);   //스턴 이펙트 활성화
        yield return new WaitForSeconds(stun);
        Particle.SetActive(false);  //스턴 이펙트 비활성화
        if(tree != null && tree.getActive())    //캐고 있었다면 다시 캐기 시작
        {
            if (level != 3) StartCoroutine("Hit");
            if (level == 2) StartCoroutine("Hit1");
            is_hit = true;
        }else //안키고 있었다면 다시 이동
        {
            is_hit = false;
            tree = null;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)  //콜라이더 비활성화
    {
        if (collision.tag == "tree" && collision.gameObject.GetComponent<TreeController>().Equals(tree) && level < 4)   //나무면
        {
            is_hit = false;
            if (level != 3) StopCoroutine("Hit");   //캐기중단
            if (level == 2) {
                StopCoroutine("Hit1");
            }  //화염방사기 - 특수공격 중단
            if (level == 2)
            {
                FirePart.Stop();    //화염방사기 이펙트 중단
            }else if(level == 1)
            {
                DustPart.Stop();    //전기톱 이펙트 중단
            }
            tree = null;
        }else if (collision.tag.Equals("money"))
        {
            if (level < 3)
            {
                moneyEffect.SetActive(false);
                is_rain = false;
            }
        }
    }

    private void Damaged(float damage)  //데미지
    {
        HP -= damage;
        if(HP <= 0) //사망판정
        {
            if (tree != null) uiManger.removeNotice(0, tree.X);
            if (level == 3)
            {
                effectArea.SetActive(false);
            }
            StopCoroutine("Hit");
            StopCoroutine("Hit1");
            gameObject.SetActive(false);
        }
    }

    IEnumerator Hit()   //데미지
    {
        while (true)
        {
            yield return new WaitForSeconds(attcktime[level]);
            if(level == 0 && Mathf.Abs(player.transform.position.x - transform.position.x) < 10) SoundManager.instance.ShootSound(2);
            tree.Demaged(Damage[level] + (is_rain ? Damage[level] : 0), DamagePersent[level]);
        }
    }

    IEnumerator Hit1()  //화염방사기 - 특수공격
    {
        while (true)
        {
            yield return new WaitWhile(() => { return tree.GetComponent<TreeController>().is_wet; });
            yield return new WaitForSeconds(1.5f);  //발화 대기 시간
            tree?.Fired(true);
            yield return new WaitWhile(() => { return tree.GetComponent<TreeController>().state == 1; });
        }
    }
}
