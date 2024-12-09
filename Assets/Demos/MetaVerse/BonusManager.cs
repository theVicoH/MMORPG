using UnityEngine;
using System.Collections.Generic;

public class BonusManager : MonoBehaviour
{
    public TCPClient tcpClient;
    public GameObject bonusPrefab;

    // TODO Exemple List dans unity directement au lieu d'écrire manuellement
    // public List<Transform> bonusPos = new List<Transform>();

    // public List<Vector3> bonusSpawns = new List<Vector3>
    // {
    //     new Vector3(248.54f, 0.2789344f, 255.5f),
    //     new Vector3(224.85f, 0.2789344f, 240.29f),
    //     new Vector3(247.38f, 0.2789344f, 256.31f),
    //     new Vector3(230.08f, 0.2789344f, 240.29f),
    //     new Vector3(248.02f, 0.2789344f, 240.29f),
    //     new Vector3(253.58f, 0.2789344f, 240.29f),
    //     new Vector3(212.2f, 0.2789344f, 221.7f),
    //     new Vector3(216.4f, 0.2789344f, 205.5f),
    // };

    // private List<BonusData> bonus = new List<BonusData>
    // {
    //     new BonusData
    //     {
    //         ID = System.Guid.NewGuid().ToString(),
    //         position = new Vector3(248.54f, 0.2789344f, 255.5f),
    //         isActive = true
    //     },
    //     new BonusData
    //     {
    //         ID = System.Guid.NewGuid().ToString(),
    //         position = new Vector3(224.85f, 0.2789344f, 240.29f),
    //         isActive = true
    //     },
    //     new BonusData
    //     {
    //         ID = System.Guid.NewGuid().ToString(),
    //         position = new Vector3(247.38f, 0.2789344f, 256.31f),
    //         isActive = true
    //     },
    //     new BonusData
    //     {
    //         ID = System.Guid.NewGuid().ToString(),
    //         position = new Vector3(230.08f, 0.2789344f, 240.29f),
    //         isActive = true
    //     },
    //     new BonusData
    //     {
    //         ID = System.Guid.NewGuid().ToString(),
    //         position = new Vector3(253.58f, 0.2789344f, 240.29f),
    //         isActive = true
    //     },
    //     new BonusData
    //     {
    //         ID = System.Guid.NewGuid().ToString(),
    //         position = new Vector3(216.4f, 0.2789344f, 205.5f),
    //         isActive = true
    //     }
    // };

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ApplyBonusUpdate(List<BonusData> bonuses) {
        foreach(BonusData bonus in bonuses) {
            if (bonus.isActive) {
                Instantiate(bonusPrefab, bonus.position, Quaternion.identity);
            }
        }
    }
}
