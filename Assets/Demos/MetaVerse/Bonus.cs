using UnityEngine;

public class Bonus : MonoBehaviour
{
    public LayerMask CollisionLayers;
    public int Points = 1;
    public string id = "";
    private TCPClient tcpClient;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
      tcpClient = GameObject.FindFirstObjectByType<TCPClient>();
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

      string message = "updateBonus " + id + " false";
      tcpClient.SendTCPMessage(message);

      Destroy(gameObject);
    }
}
