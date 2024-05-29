using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class DataToStore : MonoBehaviour
{
    public static DataToStore instance; //Singleton pattern

    public float playerMovement;            // Stocke la valeur de l'axe horizontal du joueur
    public bool firstMovementCheck = false; // Stocke l'�tat du premier mouvement du joueur

    public Text coinsCountText; // Texte affichant le nombre de pi�ces

    public IDictionary<string, string> levelNameDic;    // Stocke le nom du niveau
    public IDictionary<string, float> levelInfo;        // Stocke des informations sur le niveau
    public IDictionary<string, float> playerTimeInfo;   // Stocke diverse informations importantes concernant le joueur
    public IDictionary<string, float> causeOfDeath;    // Stocke des informations sur la cause de la mort du joueur

    public bool isGrounded = true;                      // Stocke l'�tat du joueur par rapport au sol
    public bool levelFinished = false;                  // Stocke l'�tat du niveau
    public string levelName = "Level01";                // Nom du niveau
    public CompositeCollider2D LevelCompoCol2D;

    //Singleton pattern
    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("More than one instance of DataToStore found!");
            return;
        }
        instance = this;

        levelName = SceneManager.GetActiveScene().name;
        levelNameDic = new Dictionary<string, string>();
        playerTimeInfo = new Dictionary<string, float>();
        levelInfo = new Dictionary<string, float>();
        causeOfDeath = new Dictionary<string, float>();

        InitDataToStoreField();
        StartCoroutine(StartTimer(levelName));

        LevelCompoCol2D = GameObject.FindGameObjectWithTag("Ground").GetComponent<CompositeCollider2D>();
    }

    //Initialise les champs importants pour le stockage des donn�es du joueur)
    private void InitDataToStoreField()
    {
        //Nom du niveau
        levelNameDic.Add("levelName", levelName);

        //Temps que le joueur a pass� dans tel niveau
        playerTimeInfo.Add("Timer", 0.0f);

        //Temps auquel le joueur fait son premier deplacement
        playerTimeInfo.Add("FirstDeplacementTimer", 0.0f);

        //Temps auquel le joueur s'arrete de bouger (utilise pour calculer la plus grande pause)
        playerTimeInfo.Add("StartPause", 0.0f);
        //Temps de la plus grande pause sans se deplacer du joueur
        playerTimeInfo.Add("MaxPause", 0.0f);

        //Temps que le joueur a passe sans bouger
        playerTimeInfo.Add("PauseTime", 0.0f);
        //Temps que le joueur a passe a aller a gauche
        playerTimeInfo.Add("LeftDeplacementTimer", 0.0f);
        //Temps que le joueur a passe a aller a droite
        playerTimeInfo.Add("RightDeplacementTimer", 0.0f);

        //Nombre de saut que le joueur a r�alis�s
        playerTimeInfo.Add("JumpCount", 0.0f);

        //Si le joueur saute, alors on stock le temps de d�part (o� le saut commence) pour pouvoir le soustraire quand il atterira
        playerTimeInfo.Add("JumpAirTimeStart", 0.0f);
        //Idem mais mais pour le temps passe en l'air
        playerTimeInfo.Add("AirTimeStart", 0.0f);

        //Temps passe en l'air
        playerTimeInfo.Add("AirTime", 0.0f);
        //Temps passe en l'air apres un saut
        playerTimeInfo.Add("JumpAirTime", 0.0f);

        //Toutes les informations relatives � la mort du joueur
        for (int i = 0; i < 3; i++) {
            causeOfDeath.Add("CauseOfDeath" + i, -1.0f);
            causeOfDeath.Add("XDeath" + i, -1.0f);
            causeOfDeath.Add("YDeath" + i, -1.0f);
        }

        EnemyList();
        CoinsList();
    }

    //Fonction pour venir calculer les derni�res informations � transmettre au csv avant la fin du niveau
    // Cette fonction est appel�e dans PlayerHealth.GameOver() et dans EndOfLevel.OnTriggerEnter2D()
    public void ProcessEndOfLevelData()
    {
        //Calcul du ratio de pieces collect�es
        float coinsRatio = levelInfo["CoinsCollected"] / levelInfo["Coins"];
        levelInfo["CoinsCollected"] = coinsRatio;

        //Calcul du ratio d'ennemis tu�s
        float enemiesRatio = levelInfo["EnemiesKilled"] / levelInfo["Enemies"];
        levelInfo["EnemiesKilled"] = enemiesRatio;
    }


    public void ResetData(string newSceneName)
    {
        //Supprimer les dictionnaires de donn�es
        levelNameDic.Clear();
        playerTimeInfo.Clear();
        levelInfo.Clear();
        causeOfDeath.Clear();

        //Changement de niveau
        StopAllCoroutines();    //Je n'ai pas reussi a faire fonctionner le StopCoroutine simple donc a ameliorer dans le futur
        levelName = newSceneName;
        InitDataToStoreField();
        StartCoroutine(StartTimer(levelName));
    }

    public void SetCauseOfDeath(int PVLeft, float cause, float x, float y)
    {
        //PVLeft sert � tenir compte de la vie restante du joueur apr�s s'�tre pris le coup.
        // cette valeur va de 2 � 0 avec 0 �tant la raison de la mort du joueur
        causeOfDeath["CauseOfDeath" + PVLeft] = cause;
        causeOfDeath["XDeath" + PVLeft] = x;
        causeOfDeath["YDeath" + PVLeft] = y;
    }

    private void EnemyList()
    {
        int numberOfEnemies = 0;
        GameObject GroupOfEnemy = GameObject.FindGameObjectWithTag("Enemy");
        foreach (Transform enemy in GroupOfEnemy.GetComponentInChildren<Transform>())
        {
            numberOfEnemies++;
        }
        levelInfo.Add("Enemies", numberOfEnemies);
        levelInfo.Add("EnemiesKilled", 0.0f);
    }

    private void CoinsList()
    {
        int numberOfCoins = 0;
        GameObject GroupOfCoins = GameObject.FindGameObjectWithTag("Coins");
        foreach (Transform coin in GroupOfCoins.GetComponentInChildren<Transform>())
        {
            numberOfCoins++;
        }
        levelInfo.Add("Coins", numberOfCoins);
        levelInfo.Add("CoinsCollected", 0.0f);
    }

    public void AddEnemyKilledCount()
    {
        levelInfo["EnemiesKilled"] += 1.0f;
    }

    public void AddCoins(int count)
    {
        levelInfo["CoinsCollected"] += (float)count;
        coinsCountText.text = levelInfo["CoinsCollected"].ToString();
    }

    public void RemoveCoins(int count)
    {
        levelInfo["CoinsCollected"] -= (float)count;
        coinsCountText.text = levelInfo["CoinsCollected"].ToString();
    }

    //Une fonction set afin de formaliser la modification de la valeur de playerMovement dans DataToStore
    public void UpdatePlayerMovement(float Movement)
    {
        playerMovement = Movement;
    }

    //Une autre fonction de set pour renseigner les informations de saut
    public void jumpingData()
    {
        playerTimeInfo["JumpAirTimeStart"] = playerTimeInfo["Timer"];
    }

    public void UpdateGroundCheckData(bool playerIsGrounded)
    {
        if (playerIsGrounded == isGrounded) return; //verification des etats de transition de isGrounded pour eviter les problemes de calcul
        //(Le joueur met en moyenne 4 frames a quitter le sol, donc on ne veut pas enregistrer le saut 4 fois en 1 frame)
        isGrounded = playerIsGrounded;
        isGroundedCheckTimer();
    }

    private IEnumerator StartTimer(string levelName)
    {
        float clockFreq = 0.25f;
        while (!levelFinished)
        {
            yield return new WaitForSeconds(clockFreq);
            playerTimeInfo["Timer"] += clockFreq;

            DirectionCheckTimer(clockFreq);
        }
    }

    private void isGroundedCheckTimer()
    {
        //Le joueur est en l'air
        if (!isGrounded)
        {
            //Le joueur n'a pas saute et qu'il vient de quitter le sol alors :
            if (playerTimeInfo["JumpAirTimeStart"] == 0 && playerTimeInfo["AirTimeStart"] == 0)
            {
                playerTimeInfo["AirTimeStart"] = playerTimeInfo["Timer"];
            }
        }
        //Quand le joueur re touche le sol
        else
        {
            if (playerTimeInfo["JumpAirTimeStart"] != 0.0f)
            {
                playerTimeInfo["JumpAirTime"] += playerTimeInfo["Timer"] - playerTimeInfo["JumpAirTimeStart"];
                playerTimeInfo["JumpAirTimeStart"] = 0.0f;
                playerTimeInfo["JumpCount"]++;
            }
            else if (playerTimeInfo["AirTimeStart"] != 0.0f)
            {
                playerTimeInfo["AirTime"] += playerTimeInfo["Timer"] - playerTimeInfo["AirTimeStart"];
                playerTimeInfo["AirTimeStart"] = 0.0f;
            }
        }
    }

    //Fonction un peu foure tout pour les timers reli� � la direction du joueur / mouvements du joueur
    private void DirectionCheckTimer(float clockFreq)
    {
        if (playerMovement != 0)
        {
            //Temps passe sans bouger avant le premier mouvement
            if (firstMovementCheck == false)
            {
                firstMovementCheck = true;
                playerTimeInfo["FirstDeplacementTimer"] = playerTimeInfo["Timer"];
            }

            //Si le joueur va � droite / gauche
            if (playerMovement < 0)
            {
                playerTimeInfo["LeftDeplacementTimer"] += clockFreq;
            } else
            {
                playerTimeInfo["RightDeplacementTimer"] += clockFreq;
            }

            //Si le joueur bouge et que le timer de pause est diff�rent de 0, on stocke le temps que le joueur a pass� sans bouger
            //pr�cision, on pourrait certainement ne pas avoir � r�aliser ce if mais pour des raisons de clart�, on pr�f�rera le laisser
            if (playerTimeInfo["StartPause"] != 0)
            {
                float timePaused = playerTimeInfo["Timer"] - playerTimeInfo["StartPause"];
                if (timePaused > playerTimeInfo["MaxPause"])
                {
                    playerTimeInfo["MaxPause"] = timePaused;
                }
                playerTimeInfo["StartPause"] = 0.0f;
            }
        }
        else
        {
            //Si le joueur s'arr�te de bouger, on stocke le temps o� le joueur commence � arreter de bouger
            if (playerTimeInfo["StartPause"] == 0.0f)
            {
                playerTimeInfo["StartPause"] = playerTimeInfo["Timer"];
            }
            //On incremente aussi le temps que le joueur passe sans bouger
            playerTimeInfo["PauseTime"] += clockFreq;
        }
    }
}