using UnityEngine;

public class DeplacementAB : MonoBehaviour
{
    public Transform pointA; // Point A
    public Transform pointB; // Point B
    public float speed = 2f; // Vitesse de déplacement

    private Vector3 previousPosition; // Position précédente de la plateforme
    private Rigidbody rb; // Rigidbody pour la gravité
    private Transform playerTransform; // Transform du joueur

    void Start()
    {
        previousPosition = transform.position;
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        // Calculer la position interpolée entre A et B
        float t = Mathf.PingPong(Time.time * speed, 1f);
        Vector3 newPosition = Vector3.Lerp(pointA.position, pointB.position, t);

        // Calculer le déplacement de la plateforme
        Vector3 deltaPosition = newPosition - previousPosition;

        // Déplacer l'objet vers la nouvelle position
        rb.MovePosition(newPosition);

        // Si le joueur est sur la plateforme, déplacer le joueur avec la plateforme
        if (playerTransform != null)
        {
            playerTransform.position += deltaPosition;
        }

        // Mettre à jour la position précédente de la plateforme
        previousPosition = newPosition;
    }
}