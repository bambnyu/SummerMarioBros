using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform player;
    public float timeOffset;

    private Vector3 velocity;

    void Update()
    {
        //Enregistrer la position de d�part de la cam�ra
        Vector3 posOffset = new Vector3(0, transform.position.y, transform.position.z);

        //Enregistrer la position de la cam�ra
        Vector3 cameraPositionX = new Vector3(transform.position.x, transform.position.y, transform.position.z);

        //Enregistrer la position du joueur et ajouter l'offset de la cam�ra sur les axes Y et Z
        Vector3 playerPosition = new Vector3(player.position.x, 0, 0) + posOffset;


        //Faite en sorte que la cam�ra ne puisse pas suivre le joueur en dehors de la bordure du monde
        float minBound = DataToStore.instance.LevelCompoCol2D.bounds.min.x;
        float maxBound = DataToStore.instance.LevelCompoCol2D.bounds.max.x;

        //cas min && max
        if (playerPosition.x > minBound + (gameObject.GetComponent<Camera>().orthographicSize * 2 - 1)
            && playerPosition.x < maxBound - (gameObject.GetComponent<Camera>().orthographicSize * 2 - 1)) 
        {
            // Bouger la cam�ra de mani�re fluide de sa position actuelle � la position du joueur (Uniquement sur l'axe X)
            transform.position = Vector3.SmoothDamp(cameraPositionX, playerPosition, ref velocity, timeOffset);
        }
    }
}
