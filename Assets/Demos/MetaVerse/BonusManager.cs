using UnityEngine;
using System.Collections.Generic;

public class BonusManager : MonoBehaviour
{
    public TCPClient tcpClient;
    public GameObject bonusPrefab;

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
