using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SpawnVentController : MonoBehaviour
{

    public GameObject point;

    public GameObject boxPrefab;

    public float timeBetweenSpawns = 30f;

    public float numberOfItemsToPack = 0f;

    public float currentTime;


    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        currentTime = currentTime - Time.deltaTime;

        if (currentTime <= 0) {
            currentTime = timeBetweenSpawns;

            GameManager.Instance.RequestPlayNewBoxSoundSound();

            var gameObject = Instantiate(boxPrefab, point.transform.position, Quaternion.identity);
            var numberOfItems = Mathf.Max(Mathf.Min(numberOfItemsToPack, 18), 0);

            
            var allItems = new List<ItemType> { ItemType.baseball, ItemType.basketball, ItemType.coins, ItemType.fish, ItemType.meat, ItemType.pens, ItemType.portraits, ItemType.truck, ItemType.videoGame };
            var items = new List<ItemType>();
            for (var i = 0; i < numberOfItems; i ++) {
                var randItemId = Random.Range(0, 1000) % allItems.Count;
                items.Add(allItems[randItemId]);
            }


            var allSectorColors = new List<ConveyorSectorColor> { ConveyorSectorColor.blue, ConveyorSectorColor.green, ConveyorSectorColor.red };
            var randColorId = Random.Range(0, 1000) % allSectorColors.Count;
            var sectorColor = allSectorColors[randColorId];
            
            var box = gameObject.GetComponent<Box>();
            var label = box.boxLabel.GetComponent<BoxLabel>();
            label.task.requiresItems = items;
            label.task.sectorColor = sectorColor;
            label.UpdateLabel();

            if (numberOfItems <= 1) {
                box.size = new Vector3(0.5f, 0.6f, 0.4f);
            } else if (numberOfItems <= 3) {
                box.size = new Vector3(0.6f, 0.6f, 0.6f);
            } else if (numberOfItems <= 5) {
                box.size = new Vector3(0.8f, 0.6f, 0.4f);
            } else if (numberOfItems <= 7) {
                box.size = new Vector3(0.8f, 0.6f, 0.6f);
            }  else if (numberOfItems <= 9) {
                box.size = new Vector3(0.8f, 0.8f, 0.8f);
            }  else if (numberOfItems <= 11) {
                box.size = new Vector3(1f, 0.8f, 0.8f);
            }  else if (numberOfItems <= 12) {
                box.size = new Vector3(1f, 1f, 0.8f);
            }  else {
                box.size = new Vector3(1f, 1f, 1f);
            }
        }

    }
}
