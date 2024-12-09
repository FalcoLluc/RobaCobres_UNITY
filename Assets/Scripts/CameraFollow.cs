using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target; // El objetivo que la cámara seguirá (en este caso, el Player)
    public float smoothSpeed = 0.125f; // Velocidad de suavizado de la cámara
    public Vector3 offset; // Desplazamiento de la cámara respecto al jugador

    private void LateUpdate()
    {
        if (target != null) // Verificamos si hay un objetivo asignado
        {
            // La posición deseada de la cámara es la del Player más el desplazamiento
            Vector3 desiredPosition = target.position + offset;

            // Suavizamos el movimiento de la cámara para que no sea brusco
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);

            // Actualizamos la posición de la cámara
            transform.position = smoothedPosition;
        }
    }
}

