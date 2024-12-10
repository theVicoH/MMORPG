using UnityEngine;
using System.Collections.Generic;

public class BonusManager : MonoBehaviour
{
    public TCPClient tcpClient;
    public GameObject bonusPrefab;

    Dictionary<string, GameObject> localBonusDict = new Dictionary<string, GameObject>();

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
                GameObject newBonus = Instantiate(bonusPrefab, bonus.position, Quaternion.identity);
                newBonus.GetComponent<Bonus>().id = bonus.ID;
                localBonusDict[bonus.ID] = newBonus;
            }
        }
    }

    public void UpdateBonusIsActive(string bonusId) {
        GameObject obj = localBonusDict[bonusId];
        if (obj == null) return;

        Destroy(obj);
        localBonusDict.Remove(bonusId);
    }
}
