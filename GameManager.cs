using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    // General
    public bool bIsRunningGame { get; private set; }

    // Camera
    public Camera MainCamera;
    private Vector3 startPosition;
    private Vector3 targetPosition;
    public float MainCameraDuration;
    private float startTime;
    private bool bIsMovingForward = true;

    // UI - User Interface + Audio
    public Canvas LoopUI;
    public Canvas StartingTextUI;
    public Image[] StartingText;
    public AudioClip[] StartingTextAudios;
    public GameObject plants;

    // Suns
    public int totalSuns;
    public LayerMask sunMask;

    // Tiles and Manage of Plants
    public GameObject CurrentPlant { get; private set; }
    public Sprite CurrentPlantSprite { get; private set; }
    public LayerMask tileLayer;
    public GameObject gridTile;

    public int Price { get; private set; }
    public PlantSlot SlotPlant { get; private set; }

    public float currentRechargedTime;

    private void Awake()
    {
        LoopUI.enabled = false;
        StartingTextUI.enabled = false;

        for (int i = 0; i < StartingText.Length; i++)
        {
            StartingText[i].enabled = false;
        }

        startPosition = MainCamera.transform.position;
        targetPosition = new Vector3(5.75f, 0, -10);

        startTime = Time.time;
        StartCoroutine(MovementOfCamera());

        foreach (Transform row in gridTile.transform.GetComponentInChildren<Transform>())
        {
            foreach (Transform tile in row.transform.GetComponentInChildren<Transform>())
            {
                tile.AddComponent<Tile>();
            }
        }
    }

    private void Update()
    {
        foreach (Transform row in gridTile.transform.GetComponentInChildren<Transform>())
        {
            foreach (Transform tile in row.transform.GetComponentInChildren<Transform>())
            {
                tile.GetComponent<SpriteRenderer>().enabled = false;
            }
        }

        foreach (PlantSlot plant in plants.GetComponentsInChildren<PlantSlot>())
        {
            Price = plant.price;
        }

        Debug.Log(currentRechargedTime);

        if (totalSuns >= Price && currentRechargedTime <= 0)
        {
            // Check if our mouse position is on the tile
            RaycastHit2D tileHit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, Mathf.Infinity, tileLayer);

            if (CurrentPlant && tileHit.collider && !tileHit.collider.GetComponent<Tile>().bHasPlant)
            {
                tileHit.collider.GetComponent<SpriteRenderer>().sprite = CurrentPlantSprite;
                tileHit.collider.GetComponent<SpriteRenderer>().enabled = true;

                if (Input.GetMouseButtonDown(0))
                {
                    SlotPlant.GetComponent<PlantSlot>().GetComponentInChildren<RechargedProgress>().currentReachargedTime = SlotPlant.GetComponent<PlantSlot>().GetComponentInChildren<RechargedProgress>().maxRechargedTime;
                    Instantiate(CurrentPlant, tileHit.collider.transform.position, Quaternion.identity);
                    tileHit.collider.GetComponent<Tile>().bHasPlant = true;
                    totalSuns -= Price;
                    tileHit.transform.GetComponent<SpriteRenderer>().enabled = false;
                    CurrentPlant = null;
                    CurrentPlantSprite = null;
                }
            }
        }

        if (Input.GetMouseButtonDown(0))
        {
            // Check if our mouse position is on the sun
            RaycastHit2D sunHit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, Mathf.Infinity, sunMask);
            if (sunHit.collider)
            {
                totalSuns += 50;
                Destroy(sunHit.collider.gameObject);
            }
        }
    }

    IEnumerator MovementOfCamera()
    {
        int i = 0;

        while (i < 2)
        {
            float timeSinceStart = Time.time - startTime;
            float t = Mathf.Clamp01(timeSinceStart / MainCameraDuration);

            if (bIsMovingForward)
            {
                MainCamera.transform.position = Vector3.Lerp(startPosition, targetPosition, t);
            } else
            {
                MainCamera.transform.position = Vector3.Lerp(targetPosition, startPosition, t);
            }

            yield return null;

            if (t >= 1)
            {
                bIsMovingForward = !bIsMovingForward;
                startTime = Time.time;
                i++;
            }
        }

        StartCoroutine(ShowStartingText());
    }

    IEnumerator ShowStartingText()
    {
        StartingTextUI.enabled = true;
        for (int i = 0; i < StartingText.Length; i++)
        {
            yield return new WaitForSeconds(1);
            StartingText[i].enabled = true;
            if (i > 0) StartingText[i - 1].enabled = false;
            SoundManager.instance.PlaySound(StartingTextAudios[i]);
        }

        yield return new WaitForSeconds(1);
        StartingText[StartingText.Length - 1].enabled = false;
        LoopUI.enabled = true;
        bIsRunningGame = true;
    }

    public void BuyPlant(GameObject plant, Sprite sprite, PlantSlot slot)
    {
        CurrentPlant = plant;
        CurrentPlantSprite = sprite;
        SlotPlant = slot;
    }
}
