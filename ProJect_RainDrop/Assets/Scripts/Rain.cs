﻿using System;
using UnityEngine;
using UnityEngine.UI;
using Random = System.Random;

public class Rain : MonoBehaviour {
    public Sprite[] type = new Sprite[4];
    Random random = new Random();

    void Start()
    {
        gameObject.GetComponent<Image>().sprite = type[random.Next(0, 3)];
        gameObject.GetComponent<Rigidbody2D>().AddForce(new Vector2(0, -200), ForceMode2D.Impulse);
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.collider.gameObject.tag.Equals("Ground")) Destroy(gameObject);
        else if (other.collider.gameObject.tag.Equals("Pail"))
        {
            Destroy(this.gameObject);

            if (DataBase.maxWater > DataBase.uncleanedWater + DataBase.cleanedWater)
            {
                if (DataBase.nowLocal == 1) DataBase.cleanedWater = Convert.ToInt16(DataBase.perDrop);
                else if (DataBase.nowLocal == 3)DataBase.desertWater += Convert.ToInt16(DataBase.perDrop);
                else  DataBase.uncleanedWater = Convert.ToInt16(DataBase.perDrop);
            }
        }

        //UIManager.instance.MoneySet();
        UIManager.instance.WaterTankUpdate();
    }


}