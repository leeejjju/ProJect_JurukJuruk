﻿using UnityEngine;
using UnityEngine.UI;

public class UI_Market : MonoBehaviour {
    //pail
    private Text pailInfo;
    private Text pailCost;

    //tank
    private Text tankInfo;
    private Text tankCost;

    //pot
    private Text[] potInfo = new Text[DataBase.potLevel.Length];
    private Text[] potCost = new Text[DataBase.potLevel.Length];

    // Texts
    private Text btnTextYN; // 가격 Text

    private Text btnTextOK; // OK popup 설명Text
    private Button yesBtn; // 구매 버튼


    private void Awake()
    {
        // Set UI
        yesBtn = UI_MultiScene.instance.popUpYN.GetComponentsInChildren<Button>()[1];
        btnTextYN = yesBtn.gameObject.GetComponentInChildren<Text>();
        btnTextOK = UI_MultiScene.instance.popUpOK.gameObject.GetComponentsInChildren<Text>()[1];

        pailInfo = GameObject.Find("Canvas/BackGround/Goods/Pail_BG/Info").GetComponent<Text>();
        pailCost = GameObject.Find("Canvas/BackGround/Goods/Pail_BG/PailUp/Text").GetComponent<Text>();

        tankInfo = GameObject.Find("Canvas/BackGround/Goods/Tank_BG/Info").GetComponent<Text>();
        tankCost = GameObject.Find("Canvas/BackGround/Goods/Tank_BG/TankUp/Text").GetComponent<Text>();
        for (int i = 0; i < DataBase.potLevel.Length; i++)
        {
            potInfo[i] = GameObject.Find("Canvas/BackGround/Goods/ListView/Viewport/Content/Pot_" + i + "/Info")
                .GetComponent<Text>();
            potCost[i] = GameObject.Find("Canvas/BackGround/Goods/ListView/Viewport/Content/Pot_" + i + "/Button/Text")
                .GetComponent<Text>();
        }
    }

    void Start()
    {
        DataBase.getMoney();
        DataBase.getWaterData();
        DataBase.getLevels();

        // Set Lockers
        setPotLockers();

        //set Text
        setMarketText();
        UI_MultiScene.instance.setMoney();
        return;
    }

    //set locker 
    public void setPotLockers()
    {
        for (int i = 0; i < DataBase.potLevel.Length; i++)
            if (DataBase.potLevel[i] > 0)
                GameObject.Find("Canvas/BackGround/Goods/ListView/Viewport/Content/Pot_" + i + "/Lock").gameObject
                    .SetActive(false);
    }

    //set market text 
    public void setMarketText()
    {
        // get data 
        DataBase.getWaterData();
        DataBase.getLevels();

        // pail 업그레이드 가능시
        if (DataBase.pailLevel != DataBase.valuePerDrop.Length - 1)
        {
            pailInfo.text =
                "업그레이드시\n한 물방울 = " + DataBase.valuePerDrop[DataBase.pailLevel] + "ml >> " +
                DataBase.valuePerDrop[DataBase.pailLevel + 1] + "ml";
            pailCost.text = DataBase.upgradePail[DataBase.pailLevel + 1] + " $";
        }
        //  pail 최대 레벨일때
        else
        {
            pailInfo.text = "한계에 도달했습니다.\n" + "한 물방울 =" + DataBase.valuePerDrop[DataBase.pailLevel] + "ml";
            pailCost.text = "Max";
        }

        // tank 업그레이드 가능시
        if (DataBase.tankLevel != DataBase.valueMaxWater.Length - 1)
        {
            tankInfo.text = "업그레이드시\n" + DataBase.valueMaxWater[DataBase.tankLevel] + "ml >> " +
                            DataBase.valueMaxWater[DataBase.tankLevel + 1] + "ml";
            tankCost.text = DataBase.upgradeTank[DataBase.tankLevel + 1] + " $";
        }
        // tank 최대 레벨일때
        else
        {
            tankInfo.text = "한계에 도달했습니다.\n" + DataBase.valueMaxWater[DataBase.tankLevel] + "ml";
            tankCost.text = "Max";
        }

        // set pot text
        for (int i = 0; i < DataBase.potWater.Length; i++)
        {
            // pot 업그레이드 가능시
            if (DataBase.potLevel[i] != DataBase.valuePotMax.Length - 1)
                potCost[i].text = DataBase.unLockPot[i] + DataBase.upgradePot[DataBase.potLevel[i]] + "$";
            // pot 최대 레벨일때
            else
                potCost[i].text = "Max";
        }

        // set pot info
        for (int i = 0; i < DataBase.potLevel.Length; i++)
            if (DataBase.potLevel[i] > 0)
                potInfo[i].text = (DataBase.locals[i].potCycle < 60)
                    ? DataBase.locals[i].potCycle + "초당 " + DataBase.perSecond[DataBase.potLevel[i]] + "ml"
                    : "1분당 " + DataBase.perSecond[DataBase.potLevel[i]] + "ml";
    }

    // up pail level
    public void upPailLevel()
    {
        // get data
        DataBase.getLevels();
        DataBase.getMoney();

        if (DataBase.pailLevel >= DataBase.upgradePail.Length - 1) // Lv Max
        {
            // 이전버전 버그 대응
            DataBase.pailLevel = DataBase.upgradePail.Length - 1;
            DataBase.setLevels();
            return;
        }

        // 업그레이드할 돈이 있다면 
        if (DataBase.money >= DataBase.upgradePail[DataBase.pailLevel + 1])
        {
            DataBase.money -= DataBase.upgradePail[++DataBase.pailLevel];

            //set data
            DataBase.setLevels();
            DataBase.setMoney();

            //set Texts
            setMarketText();
            UI_MultiScene.instance.setMoney();
            
            // sound (FX)
            SoundManager.instance.playFx(1);
        }
        //돈이 부족하다면
        else
        {
            //popup활성화
            UI_MultiScene.instance.setPopupOK("보유 금액이 부족합니다.");
        }
    }


    // up tank level
    public void upTankLevel()
    {
        // get data 
        DataBase.getLevels();
        DataBase.getMoney();
        if (DataBase.tankLevel >= DataBase.upgradeTank.Length - 1) // Lv Max
        {
            // 이전버전 버그 대응
            DataBase.tankLevel = DataBase.upgradeTank.Length - 1;
            DataBase.setLevels();
            return;
        }


        // 업그레이드 할 돈이 있다면
        if (DataBase.money >= DataBase.upgradeTank[DataBase.tankLevel + 1])
        {
            DataBase.money -= DataBase.upgradeTank[++DataBase.tankLevel];

            // set data
            DataBase.setLevels();
            DataBase.setMoney();

            // set text
            setMarketText();
            UI_MultiScene.instance.setMoney();

            //sound
            SoundManager.instance.playFx(1);
        }
        // 돈 부족
        else
        {
            //popup 활성화
            //돈부족 
            UI_MultiScene.instance.setPopupOK("보유 금액이 부족합니다.");
        }
    }

    // up pot level (local)
    public void upPotLevel(int val)
    {
        //Get Datas
        DataBase.getLevels();
        DataBase.getMoney();
        DataBase.getLocalData(val);

        //팝업 비활성화
        UI_MultiScene.instance.popupIsOn = false;
        UI_MultiScene.instance.popUpBG.SetActive(false);
        UI_MultiScene.instance.popUpYN.SetActive(false);

        // 해금 | 해금 가격과 업글가격을 합쳐서 계산하는 방식이기 때문에, 시스템 분리
        if (DataBase.potLevel[val] == 0)
        {
            if (DataBase.money >= DataBase.unLockPot[val])
            {
                DataBase.money -= DataBase.unLockPot[val];
                DataBase.potLevel[val]++;

                DataBase.potWater[val] = 0;
                DataBase.setWaterData();
                
                // set data
                DataBase.setLevels();
                DataBase.setMoney();

                // set text 
                setMarketText();
                UI_MultiScene.instance.setMoney();
                
                //set Pot Lockers
                setPotLockers();
                
                //Sound (FX)
                SoundManager.instance.playFx(1);
                return;
                
            }
        }
        // 일반 업글
        else
        {
            // 최대 레벨이면 함수 종료
            if (DataBase.potLevel[val] >= DataBase.upgradePot.Length - 1)
            {
                // 이전 버전 버그 대응
                DataBase.potLevel[val] = DataBase.upgradePot.Length - 1;
                DataBase.setLevels();
                return;
            }


            // 업그레이드 가능한가?
            if (DataBase.money >= (DataBase.unLockPot[val] + DataBase.upgradePot[DataBase.potLevel[val]]))
            {
                DataBase.money -= (DataBase.unLockPot[val] + DataBase.upgradePot[DataBase.potLevel[val]++]);

                //set data
                DataBase.setLevels();
                DataBase.setMoney();

                //set texts
                setMarketText();
                UI_MultiScene.instance.setMoney();

                //set pot lockers
                setPotLockers();
                
                //Sound (FX)
                SoundManager.instance.playFx(1);
                return;
            }
        }

        // 돈부족
        // 최적화를 위해(중복성 최소화) 다른 경우엔 함수 종료
        // 팝업 활성화
        UI_MultiScene.instance.setPopupOK("보유 금액이 부족합니다.");
    }


    // pot btn
    public void upPotBtn(int val)
    {
        // get data 
        DataBase.getLocalData(val);
        DataBase.getMoney();

        //지역 해금 안됌
        if (DataBase.locals[val].isLock)
        {
            // 팝업 활성화
            UI_MultiScene.instance.setPopupOK("지역이 해금되지 않았습니다.");
        }

        // 지역이 해금되어 있다면
        else
        {
            // 양동이 해금
            if (DataBase.potLevel[val] <= 0)
            {
                UI_MultiScene.instance.popupIsOn = true;
                UI_MultiScene.instance.popUpBG.SetActive(true);
                UI_MultiScene.instance.popUpYN.SetActive(true);
                btnTextOK.text = "해금하시겠습니까?";

                // 버튼 text 설정
                btnTextYN.text = DataBase.unLockPot[val] + " $";

                // 버튼 함수 설정 
                UI_MultiScene.instance.setBtnFunc(yesBtn, upPotLevel, val);
            }
            // 일반 업그레이드
            else
            {
                upPotLevel(val);
            }
        }
    }
}