using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using NaughtyAttributes;
using UnityEngine.SceneManagement;

public class ManuManager : MonoBehaviour    //메뉴 씬 메니저
{
    [SerializeField]
    private GameObject loading;
    [SerializeField]
    private Text title; //설명 - 타이틀
    [SerializeField]
    private Text text;  //설명 - 내용
    [SerializeField]
    private GameObject[] ModePicture;   //미리보기 이미지
    [SerializeField]
    private GameObject[] TreePicture;   //미리보기 이미지
    [SerializeField]
    private GameObject[] CloudsPicture;   //미리보기 이미지
    [SerializeField]
    private GameObject[] LevelsPicture;   //미리보기 이미지
    private Value value;    //게임씬으로 넘길 오브젝트
    [SerializeField]
    private string[] ModeTitle; //타이틀 리스트
    [ResizableTextArea]
    [SerializeField]
    private string[] ModeExplain;   //내용 리스트
    [SerializeField]
    private string[] TreeTitle; //타이틀 리스트
    [ResizableTextArea]
    [SerializeField]
    private string[] TreeExplain;   //내용 리스트
    [SerializeField]
    private string[] LevelTitle; //타이틀 리스트
    [ResizableTextArea]
    [SerializeField]
    private string[] LevelExplain;   //내용 리스트
    [SerializeField]
    private string[] CloudTitle; //타이틀 리스트
    [ResizableTextArea]
    [SerializeField]
    private string[] CloudExplain;   //내용 리스트
    private int step = 0;   //현재 선택 단계 0:모드 1:지역(나무) 2:구름
    private int index = 0;  //현재 선택된 내용

    private void Awake()    //씬 시작시 초기화
    {
        loading.SetActive(false);

        value = GameObject.FindGameObjectWithTag("value").GetComponent<Value>();

        step = 0;
        index = 0;
        ModePicture[index].SetActive(true);
        setScene();
    }

    public void Previous()  //이전으로
    {
        switch (step)
        {
            case 0:     //모드 선택시
                ModePicture[index--].SetActive(false);      //이전 미리보기 비활성화
                if (index < 0) index = ModePicture.Length - 1;
                ModePicture[index].SetActive(true);     //미리보기 활성화
                break;
            case 1:     //나무 선택시
                TreePicture[index--].SetActive(false);      //이전 미리보기 비활성화
                if (index < 0) index = TreePicture.Length - 1;
                TreePicture[index].SetActive(true);     //미리보기 활성화
                break;
            case 2:     //구름 선택시
                CloudsPicture[index--].SetActive(false);      //이전 미리보기 비활성화
                if (index < 0) index = CloudsPicture.Length - 1;
                CloudsPicture[index].SetActive(true);     //미리보기 활성화
                break;
            case 3:     //레벨 선택시
                LevelsPicture[index--].SetActive(false);      //이전 미리보기 비활성화
                if (index < 0) index = LevelsPicture.Length - 1;
                LevelsPicture[index].SetActive(true);     //미리보기 활성화
                break;
        }
        setScene(); //설명 반영
    }

    public void Next()  //다음으로
    {
        switch (step)
        {
            case 0: //모드 선택시
                ModePicture[index++].SetActive(false);  //이전 미리보기 비활성화
                if (index > ModePicture.Length - 1) index = 0;
                ModePicture[index].SetActive(true); //미리보기 활성화
                break;
            case 1:     //나무 선택시
                TreePicture[index++].SetActive(false);      //이전 미리보기 비활성화
                if (index > TreePicture.Length - 1) index = 0;
                TreePicture[index].SetActive(true);     //미리보기 활성화
                break;
            case 2:     //나무 선택시
                CloudsPicture[index++].SetActive(false);      //이전 미리보기 비활성화
                if (index > CloudsPicture.Length - 1) index = 0;
                CloudsPicture[index].SetActive(true);     //미리보기 활성화
                break;
            case 3:     // 선택시
                LevelsPicture[index++].SetActive(false);      //이전 미리보기 비활성화
                if (index > LevelsPicture.Length - 1) index = 0;
                LevelsPicture[index].SetActive(true);     //미리보기 활성화
                break;
        }
        setScene(); //설명 반영
    }

    public void Select()    //선택
    {
        switch (step)
        {
            case 0:
                ModePicture[index].SetActive(false);    //미리보기 비활성화
                value.GameMode = index;
                break;
            case 1:
                TreePicture[index].SetActive(false);    //미리보기 비활성화
                value.TreeMode = index;
                break;
            case 2:
                CloudsPicture[index].SetActive(false);    //미리보기 비활성화
                value.CloudMode = index;
                break;
            case 3:
                LevelsPicture[index].SetActive(false);    //미리보기 비활성화
                value.Level = index + 1;
                break;
        }
        step++; //단계 증가
        index = 0;  //인덱스 초기화
        switch (step)   //초기화
        {
            case 0:     //모드 선택시
                    ModePicture[index].SetActive(true);
                break;
            case 1:     //나무 선택시
                if (value.GameMode == 2)
                {
                    loading.SetActive(true);
                    SceneManager.LoadScene("GameScene");
                }
                else
                {
                    TreePicture[index].SetActive(true);
                }
                break;
            case 2:     //구름 선택시
                CloudsPicture[index].SetActive(true);
                break;
            case 3:     //게임시작
                if(value.GameMode != 0)
                {
                    loading.SetActive(true);
                    SceneManager.LoadScene("GameScene");
                }
                else
                {
                    LevelsPicture[index].SetActive(true);
                }
                break;
            case 4:     //게임시작
                loading.SetActive(true);
                SceneManager.LoadScene("GameScene");
                break;
        }
        setScene(); //설명 반영
    }

    private void setScene() //설명 반영
    {
        switch (step)
        {
            case 0: //모드 선택시
                title.text = ModeTitle[index];
                text.text = ModeExplain[index];
                break;
            case 1: //나무 선택시
                title.text = TreeTitle[index];
                text.text = TreeExplain[index];
                break;
            case 2: //구름 선택시
                title.text = CloudTitle[index];
                text.text = CloudExplain[index];
                break;
            case 3: // 선택시
                title.text = LevelTitle[index];
                text.text = LevelExplain[index];
                break;
        }
    }
}
