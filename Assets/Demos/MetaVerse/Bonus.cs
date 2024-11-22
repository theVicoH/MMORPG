using UnityEngine;

public class Bonus : MonoBehaviour
{
    public LayerMask CollisionLayers;
    public int Points = 1;

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
      // TODO : Mettre à jour le score du joueur dans le serveur
      // TODO : Mettre la logique pour changer l'état du bonus
      // TODO : Incrémenter le score
      if (cScore != null) {
        cScore.AddScore(Points);
      }

      Destroy(gameObject);
    }
}
