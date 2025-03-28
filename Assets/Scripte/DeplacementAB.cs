using UnityEngine;

public class DeplacementAB : MonoBehaviour
{
    public Transform pointA; // Point A
    public Transform pointB; // Point B
    public float speed = 2f; // Vitesse de d�placement

    private Vector3 previousPosition; // Position pr�c�dente de la plateforme
    private Rigidbody rb; // Rigidbody pour la gravit�
    private Transform playerTransform; // Transform du joueur

    void Start()
    {
        previousPosition = transform.position;
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        // Calculer la position interpol�e entre A et B
        float t = Mathf.PingPong(Time.time * speed, 1f);
        Vector3 newPosition = Vector3.Lerp(pointA.position, pointB.position, t);

        // Calculer le d�placement de la plateforme
        Vector3 deltaPosition = newPosition - previousPosition;

        // D�placer l'objet vers la nouvelle position
        rb.MovePosition(newPosition);

        // Si le joueur est sur la plateforme, d�placer le joueur avec la plateforme
        if (playerTransform != null)
        {
            playerTransform.position += deltaPosition;
        }

        // Mettre � jour la position pr�c�dente de la plateforme
        previousPosition = newPosition;
    }
}