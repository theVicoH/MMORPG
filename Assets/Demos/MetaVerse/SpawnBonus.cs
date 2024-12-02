using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpawnBonus : MonoBehaviour
{
    private float _timer = 0;
    // private float spawnCounter = 0;
    private List<GameObject> LastPoints = new List <GameObject>();
    public GameObject[] spawnPoints;
    public GameObject BonusPrefab;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        _timer += Time.deltaTime;
        if (_timer > 5)
        {
            GameObject spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
            if (!LastPoints.Contains(spawnPoint)){
                LastPoints.Add(spawnPoint);
                GameObject BonusSpawned = Instantiate(BonusPrefab, spawnPoint.transform.position, Quaternion.identity);

                Bonus bonusScript = BonusSpawned.GetComponent<Bonus>();
                if (bonusScript != null)
                {
                    bonusScript.SpawnPoint = spawnPoint;
                }
            }
            
            _timer = 0;
        }
    }   

    public void BonusTaken(GameObject spawnPoint) {
        // supprimer element du rableau LastPoints
        Debug.Log("BONUS TAKEN");
        Debug.Log(spawnPoint);

        // Remove the spawn point from the LastPoints list
        if (LastPoints.Contains(spawnPoint))
        {
            LastPoints.Remove(spawnPoint);
            Debug.Log("Bonus taken at: " + spawnPoint.name);
        }

    }


}
