using UnityEngine;

public class Bonus : MonoBehaviour
{
    public LayerMask CollisionLayers;
    public int Points = 1;

    public GameObject SpawnPoint;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private bool ShouldHandleObject(Collider other) {
       return (CollisionLayers.value & (1 << other.gameObject.layer)) > 0;
    }

    void OnTriggerEnter(Collider other) {
      if (!ShouldHandleObject(other)) { return; }

      CharacterScore cScore = other.gameObject.GetComponentInChildren<CharacterScore>();
      if (cScore != null) {
        cScore.AddScore(Points);
      }

      SpawnBonus spawner = GameObject.FindObjectOfType<SpawnBonus>();
     
          spawner.BonusTaken(SpawnPoint);
      
      // spawner.BonusTaken();

      Destroy(gameObject);
    }
}
