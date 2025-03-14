﻿using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_CleanScene : MonoBehaviour {
    private Button clickZone; // CleanBtn

    private Text upText; // 업그레이드 가격 Text

    private Text explain; // 현재, 다음 업그레이드 설명 Text 

    [Header("클린 스프라이트")] public Sprite[] cleanFx = new Sprite[4]; // click FX

    [Header("투명 스프라이트")] public Sprite nullImage;

    private void Awake()
    {
        clickZone = GameObject.Find("Canvas/ClickZone").GetComponent<Button>();
        explain = GameObject.Find("Canvas/Explain_Memo/Recent").GetComponent<Text>();
        upText = GameObject.Find("Canvas/Up/Text").GetComponent<Text>();
    }

    void Start()
    {
        // 버튼 이벤트 트리거 생성.
        EventTrigger trgY = clickZone.gameObject.AddComponent<EventTrigger>();
        // hold, up
        EventTrigger.Entry enBH = new EventTrigger.Entry();
        EventTrigger.Entry enBU = new EventTrigger.Entry();

        // 각 행동 등록 
        // PointerDown : 클릭시 | PointerUp : 클릭 종료시
        enBH.eventID = EventTriggerType.PointerDown;
        enBU.eventID = EventTriggerType.PointerUp;

        // 각 entry에 함수 등록
        enBH.callback.AddListener(delegate { setLevelImage(); });
        enBU.callback.AddListener(delegate
        {
            GameObject.Find("Canvas/ClickZone/onClick").GetComponent<Image>().sprite = nullImage;
        });
        trgY.triggers.Add(enBH);
        trgY.triggers.Add(enBU);

        // ClickZone 기본 sprite 적용 (투명 이미지)
        GameObject.Find("Canvas/ClickZone/onClick").GetComponent<Image>().sprite = nullImage;
        //get datas 
        DataBase.getMoney();
        DataBase.getWaterData();
        DataBase.getLevels();

        // set slider
        UI_MultiScene.instance.setWaterTank();
        UI_MultiScene.instance.updateWaterTank();

        //Set Text
        UI_MultiScene.instance.setWaterCounter();
        UI_MultiScene.instance.setMoney();
        setCleanText();
    }

    public void setLevelImage()
    {
        GameObject.Find("Canvas/ClickZone/onClick").GetComponent<Image>().sprite = cleanFx[DataBase.cleanLevel / 2];
    }

    // 정화 버튼 클릭
    public void clickClean()
    {
        //get Datas
        DataBase.getLevels();
        DataBase.getWaterData();

        // 만약 각 터치당 정화량 보다 보유중인 물의 양이 적을 때
        if (DataBase.water[0] < DataBase.valueCleanWater[DataBase.cleanLevel])
        {
            DataBase.water[1] += DataBase.water[0];
            DataBase.water[0] = 0;
        }
        // 일반 경우의 정화
        else
        {
            //만약 정화할 물이 없다면 (아마 접근 불가)
            if (DataBase.water[0] == 0)
            {
                return;
            }

            // 정화
            DataBase.water[0] -= DataBase.valueCleanWater[DataBase.cleanLevel];
            DataBase.water[1] += DataBase.valueCleanWater[DataBase.cleanLevel];
        }

        //Set data
        DataBase.setWaterData();

        //Set Text
        UI_MultiScene.instance.setWaterCounter();
    }

    //현 정보 표시 Texts 세팅
    public void setCleanText()
    {
        //get Datas
        DataBase.getLevels();
        DataBase.getWaterData();

        // 최고 레벨보다 낮다면
        if (DataBase.cleanLevel != DataBase.valueCleanWater.Length - 1)
        {
            explain.text = "현재 :  1터치 = " + DataBase.valueCleanWater[DataBase.cleanLevel] + "ml\n" +
                           "업그레이드 :  1터치 = " +
                           DataBase.valueCleanWater[DataBase.cleanLevel + 1] + "ml";
            upText.text = DataBase.upgradeClean[DataBase.cleanLevel + 1] + "$";
        }
        // 최고 레벨이라면
        else
        {
            explain.text =
                "현재 :  1터치 = " + DataBase.valueCleanWater[DataBase.cleanLevel] + "ml\n" + "최고 레벨입니다.";
            upText.text = "MAX";
        }
    }

    //정화소 업그레이드
    public void upCleanLevel()
    {
        //Get data
        DataBase.getMoney();
        DataBase.getLevels();

        // 업그레이드 비용이 충분하고, 레벨이 최대가 아니라면
        if (DataBase.cleanLevel >= DataBase.upgradeClean.Length - 1)
        {
            // 이전버전 버그 대응
            DataBase.cleanLevel = DataBase.upgradeClean.Length - 1;
            DataBase.setLevels();
            return;
        }
 
        if ((DataBase.money >= DataBase.upgradeClean[DataBase.cleanLevel + 1]) &&
            (DataBase.cleanLevel < DataBase.valueCleanWater.Length))
        {
            DataBase.money -= DataBase.upgradeClean[++DataBase.cleanLevel];

            //Set Data
            DataBase.setLevels();
            DataBase.setMoney();

            //Set Texts
            setCleanText();
            UI_MultiScene.instance.setMoney();

            //sound
            SoundManager.instance.playFx(1);
        }
        // 돈부족
        else
        {
            // 팝업 활성화
            UI_MultiScene.instance.setPopupOK("보유 금액이 부족합니다.");
        }
    }
}