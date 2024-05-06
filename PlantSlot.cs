using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlantSlot : MonoBehaviour
{
    public Sprite plantSprite;
    public GameObject plantObject;
    public Image icon;
    public TextMeshProUGUI priceText;
    public int price;
    public float rechargeTime;
    private GameManager gms;

    private void Awake()
    {
        gms = GameObject.Find("GameManager").GetComponent<GameManager>();
        GetComponent<Button>().onClick.AddListener(BuyPlant);
    }

    private void BuyPlant()
    {
        if (gms.totalSuns >= gms.Price && gms.currentRechargedTime <= 0)
            gms.BuyPlant(plantObject, plantSprite, this);
    }

    private void OnValidate()
    {
        if (plantSprite)
        {
            icon.enabled = true;
            icon.sprite = plantSprite;
            priceText.text = price.ToString();
        } else
        {
            icon.enabled = false;
        }
    }
}
