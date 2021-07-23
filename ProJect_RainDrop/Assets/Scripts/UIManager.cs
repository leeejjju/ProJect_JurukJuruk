﻿/* UI 컨트롤 스크립트 */

using System;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour {
    public static UIManager instance;

    /*[HideInInspector] */ public Text[] text = new Text[3];

    // 0 = money 1 = local 
    [HideInInspector] public Slider[] slider = new Slider[3];

    // 0 = waterTank or BgmVol 1 = Fx Vol
    [Header("메인 맵 배경")] public Sprite[] localBG = new Sprite[4];

    void Start()
    {
        if (!instance) instance = this;
        else DestroyImmediate(this);
        //TODO scene에 맞춰서 작업할것.


        try
        {
            //--------------------------------------------------------
            //main
            text[0] = GameObject.Find("Canvas/MoneyBack/Money").GetComponent<Text>(); // money
            text[1] = GameObject.Find("Canvas/LocalBack/Local").GetComponent<Text>(); // Local

            slider[0] = GameObject.Find("Canvas/Tank").GetComponent<Slider>(); // waterTank

            if (DataBase.potLevel[DataBase.nowLocal] < 1)
                GameObject.Find("Canvas/BigBox/EmptyExtraBottle").SetActive(false);
            //--------------------------------------------------------
            WaterTankSet();
            WaterTankUpdate();
            MoneySet();
            LocalSet();
            BackGroundSet();
            //--------------------------------------------------------
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
        // clean
        //TODO per click text set

        //--------------------------------------------------------
        // Shop
        text[0] = GameObject.Find("Canvas/MoneyBack/Money").GetComponent<Text>(); // money
        slider[0] = GameObject.Find("Canvas/Tank").GetComponent<Slider>(); // waterTank
        MoneySet();
        WaterTankSet();
        WaterTankUpdate();
    }

    //--------------------------------------------------------
    //main

    #region main

    public void WaterTankUpdate()
    {
        slider[0].value =  DataBase.cleanedWater = Convert.ToInt16(DataBase.perDrop);;
        //TODO : 사막지역에서 사막 물로 변경할것.
        PlayerPrefs.SetInt();
    }

    public void WaterTankSet()
    {
        slider[0].maxValue = DataBase.maxWater;
        slider[0].minValue = 0f;
    }

    public void EmptyWaterTank()
    {
        // 청정구역이라면 
        if (DataBase.nowLocal == 1)
            DataBase.cleanedWater += DataBase.potWater[DataBase.nowLocal];

        // else if (DataBase.nowLocal == 3)
        //     DataBase.cleanedWater += DataBase.potWater[DataBase.nowLocal];
        else
            DataBase.uncleanedWater += DataBase.potWater[DataBase.nowLocal];
        DataBase.potWater[DataBase.nowLocal] = 0;
        //TODo lateTIme Set
    }

    public void MoneySet()
    {
        text[0].text = Convert.ToString(DataBase.money);
    }

    public void LocalSet()
    {
        text[1].text = DataBase.localName[DataBase.nowLocal];
    }


    public void BackGroundSet()
    {
        Image bg = GameObject.Find("Canvas/BackGround").GetComponent<Image>();
        bg.sprite = localBG[DataBase.nowLocal];
    }

    #endregion

    //--------------------------------------------------------
    //map + moveScene

    #region map_moveScene

    public void MoveScene(string val)
    {
        SceneManager.LoadScene(val);
    }

    public void MoveLocal(int val)
    {
        PlayerPrefs.SetInt("NowLocal", val);
        DataBase.nowLocal = val;
        MoveScene("Main");
    }

    #endregion

    //--------------------------------------------------------
    //shop

    #region shop

    public void Sell(int index)
    {
        if (index == 1)
        {
            if (DataBase.uncleanedWater < 1000)
            {
                DataBase.money += Consumer.consumerList[index].perLiter;
                DataBase.uncleanedWater -= 1000;
            }
            else
            {
                return;
                //돈 부족
            }
        }
        else if (index == 3)
        {
            if (DataBase.desertWater < 1000)
            {
                DataBase.money += Consumer.consumerList[index].perLiter;
                DataBase.desertWater -= 1000;
            }
            else
            {
                return;
                //돈 부족
            }
        }
        else
        {
            if (DataBase.cleanedWater < 1000)
            {
                DataBase.money += Consumer.consumerList[index].perLiter;
                DataBase.cleanedWater -= 1000;
            }
            else
            {
                return;
                //돈 부족
            }
        }

        MoneySet();
    }

    #endregion

    //--------------------------------------------------------
    //Setting
    //TODO 1. Slider Setting 2. toggle set

    public bool Drag { get; set; }

    public void ChangeBgmVol()
    {
        if (!Drag)
        {
        }
    }

    public void ChangeFxVol()
    {
        if (!Drag)
        {
        }
    }


    //--------------------------------------------------------
    //Cleaning
    //TODO Cleaning system set, cleaning up system set.
    public void ClickClean()
    {
        if (DataBase.uncleanedWater < DataBase.perclean && DataBase.uncleanedWater > 0)
        {
            if (DataBase.uncleanedWater < 0)
            {
                DataBase.uncleanedWater = 0;
                return;
            }

            DataBase.cleanedWater += DataBase.uncleanedWater;
            DataBase.uncleanedWater = 0;
        }
        else
        {
            if (DataBase.uncleanedWater <= 0)
            {
                DataBase.uncleanedWater = 0;
                return;
            }

            DataBase.uncleanedWater -= DataBase.perclean;
            DataBase.cleanedWater += DataBase.perclean;
        }
    }

    public void UpCleanLevel()
    {
        if (DataBase.money < DataBase.upgradeClean[DataBase.cleanLevel])
        {
            //up
        }
        else
        {
            // 돈부족
        }
    }
    //--------------------------------------------------------

    //Market

    //TODO 1. pot up System set, 2. extra pot up system, 3. water tank system set
}