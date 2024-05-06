using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RechargedProgress : MonoBehaviour
{
    private GameManager gms;
    public float maxRechargedTime;
    public float currentReachargedTime;
    public Color ColorAlpha;
    public Color ColorDarken;

    private void Awake()
    {
        gms = GameObject.Find("GameManager").GetComponent<GameManager>();
        currentReachargedTime = maxRechargedTime;
        GetComponentInChildren<Image>().color = ColorDarken;
    }

    private void Update()
    {
        if (gms.bIsRunningGame && currentReachargedTime > 0 && gms.SlotPlant != null)
        {
            currentReachargedTime -= Time.deltaTime;
            gms.currentRechargedTime = gms.SlotPlant.GetComponent<PlantSlot>().GetComponentInChildren<RechargedProgress>().currentReachargedTime;
            GetComponent<Slider>().value = currentReachargedTime / maxRechargedTime;
            GetComponentInChildren<Image>().color = ColorDarken;
        }
        else
        {
            GetComponentInChildren<Image>().color = ColorAlpha;
        }
    }
}
