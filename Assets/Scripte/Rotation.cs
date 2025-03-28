using UnityEngine;
using System.Collections;
public class Rotation : MonoBehaviour
{
    // Vitesse de rotation en degrés par frame
    public float rotationSpeed = 1f;

    // Axe de rotation (X, Y ou Z)
    public Vector3 rotationAxis = Vector3.up;

    void Start()
    {
        StartCoroutine(RotateCoroutine());
    }

    IEnumerator RotateCoroutine()
    {
        while (true)
        {
            // Faire tourner l'objet autour de l'axe choisi
            transform.Rotate(rotationAxis * rotationSpeed);
            yield return null; // Attendre le prochain frame
        }
    }
}
