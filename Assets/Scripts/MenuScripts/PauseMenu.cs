using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//A noter que le menu pause a le m�me behavior que le main menu sauf pour son bouton resume donc on h�rite de MainMenu
public class PauseMenu : MainMenu
{
    public static bool GameIsPaused = false; // Stocke l'�tat du jeu
    public GameObject pauseMenuUI;

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Tab))
        {
            if(GameIsPaused)
            {
                Resume();
            }
            else
            {
                Paused();
            }
        }
    }

    private void Paused()
    {
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().enabled = false;
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f; // arreter le temps
        GameIsPaused = true; //changer l'�tat du jeu
    }  
    
    public void Resume()
    {
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().enabled = true;
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1.0f;
        GameIsPaused = false;
    }
}
