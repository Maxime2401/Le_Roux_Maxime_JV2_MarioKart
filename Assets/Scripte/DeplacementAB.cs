using UnityEngine;

public class DeplacementAB : MonoBehaviour
{
    public Transform pointA; // Point A
    public Transform pointB; // Point B
    public float speed = 2f; // Vitesse de déplacement

    private Vector3 previousPosition;
    private Rigidbody rb;
    private bool wasAtPointA = false;
    private bool wasAtPointB = false;

    void Start()
    {
        previousPosition = transform.position;
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        float t = Mathf.PingPong(Time.time * speed, 1f);
        Vector3 newPosition = Vector3.Lerp(pointA.position, pointB.position, t);

        // Vérifier si on arrive au point A
        if (t < 0.01f && !wasAtPointA)
        {
            FlipScaleY();
            wasAtPointA = true;
            wasAtPointB = false;
        }
        // Vérifier si on arrive au point B
        else if (t > 0.99f && !wasAtPointB)
        {
            FlipScaleY();
            wasAtPointB = true;
            wasAtPointA = false;
        }

        rb.MovePosition(newPosition);
        previousPosition = newPosition;
    }

    void FlipScaleY()
    {
        Vector3 newScale = transform.localScale;
        newScale.z *= -1f;
        transform.localScale = newScale;
    }
}