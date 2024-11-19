using UnityEngine;

public class Movements : MonoBehaviour
{
    public float moveSpeed = 5f;
    public UDPSender Sender;

    private Vector3 lastPosition;
    private float updateInterval = 0.1f;
    private float timer = 0f;

    void Start()
    {
        lastPosition = transform.position;
        Debug.Log($"[Movements] Started at position: {lastPosition}");
        
        // Vérification que UDPSender est bien assigné
        if (Sender == null)
        {
            Debug.LogError("[Movements] UDPSender n'est pas assigné! Veuillez assigner un UDPSender dans l'inspecteur.");
            enabled = false; // Désactive le script si pas de Sender
            return;
        }
    }

    void Update()
    {
        // Vérification de sécurité
        if (Sender == null) return;

        Vector3 movement = Vector3.zero;

        if (Input.GetKey(KeyCode.RightArrow))
        {
            movement.x = 1;
            Debug.Log("[Movements] Touche droite pressée");
        }
        else if (Input.GetKey(KeyCode.LeftArrow))
        {
            movement.x = -1;
            Debug.Log("[Movements] Touche gauche pressée");
        }

        if (Input.GetKey(KeyCode.UpArrow))
        {
            movement.z = 1;
            Debug.Log("[Movements] Touche haut pressée");
        }
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            movement.z = -1;
            Debug.Log("[Movements] Touche bas pressée");
        }

        if (movement != Vector3.zero)
        {
            transform.Translate(movement * moveSpeed * Time.deltaTime);
            Debug.Log($"[Movements] Nouvelle position: {transform.position}");
        }

        timer += Time.deltaTime;
        if (timer >= updateInterval && Sender != null)
        {
            if (transform.position != lastPosition)
            {
                string positionMessage = string.Format("POS:{0:F2},{1:F2},{2:F2}", 
                    transform.position.x,
                    transform.position.y,
                    transform.position.z
                );
                Debug.Log($"[Movements] Envoi UDP: {positionMessage}");
                Sender.SendUDPMessage(positionMessage);
                lastPosition = transform.position;
            }
            timer = 0f;
        }
    }
}
