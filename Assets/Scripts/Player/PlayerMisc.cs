using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

//Classe qui peut aussi etre deplace dans le GameManager dependant du pdv
public class PlayerMisc : MonoBehaviour
{
    private GameObject playerSpawn;
    private GameObject LastPositionRegistered;         // R�f�rence au point de spawn du joueur
    private GameObject player;                         // R�f�rence au joueur

    //Appel� en premier dans l'�x�cution du prgramme
    private void Awake()
    {
        playerSpawn = GameObject.FindGameObjectWithTag("PlayerSpawn");
        SceneManager.sceneLoaded += OnSceneLoad;
        LastPositionRegistered = GameObject.FindGameObjectWithTag("LastPositionRegistered");
    }
    
    //Appel� lorsqu'une sc�ne est charg�e
    void OnSceneLoad(Scene scene, LoadSceneMode mode)
    {
        player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
        {
            playerSpawn = GameObject.FindGameObjectWithTag("PlayerSpawn");
            LastPositionRegistered = GameObject.FindGameObjectWithTag("LastPositionRegistered");
            GameObject.FindGameObjectWithTag("Player").transform.position = playerSpawn.transform.position;
        }
    }

    void UpdatePlayerSpawn()
    {
        LastPositionRegistered.transform.position = new Vector3(Mathf.Round(gameObject.transform.position.x), Mathf.Round(gameObject.transform.position.y), 0);
    }

    //Appel� � chaque frame
    private void Update()
    {
        UpdatePlayerSpawn();
    }
}
