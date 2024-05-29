using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndOfLevel : MonoBehaviour
{
    [SerializeField]
    private string sceneToLoad;

    public static EndOfLevel instance;

    public IDictionary<string, string> levelName;
    public IDictionary<string, float> levelInfo;
    public IDictionary<string, float> playerTimeInfo;
    public IDictionary<string, float> causeOfDeath;

    private bool dataExported = false; // Flag pour s'assurer que les donn�es sont export�es une seule fois

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("More than one instance of EndOfLevel found!");
            return;
        }
        instance = this;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !dataExported)
        {
            DataToStore.instance.ProcessEndOfLevelData();   //On calcul nos taux de bonus / kill
            FindDicoDatas();                                //On envoie nos donn�es � EndOfLevel.cs
            DataToStore.instance.ResetData(sceneToLoad);    //On reset nos dictionnaires pour le prochain niveau

            SceneManager.LoadScene(sceneToLoad);
            dataExported = true; // mettre le flag � true pour �viter les exports dupliqu�s
        }
    }

    public void FindDicoDatas()
    {
        // R�cup�rer les donn�es de DataToStore
        levelName = DataToStore.instance.levelNameDic;          // Le nom du niveau (sp�cifiquement pour le moment)
        levelInfo = DataToStore.instance.levelInfo;             // Celles du niveau
        playerTimeInfo = DataToStore.instance.playerTimeInfo;   // Celles du joueur et des Timer
        causeOfDeath = DataToStore.instance.causeOfDeath;       // Celles des morts
        // Exporter les donn�es au format CSV
        string filePath = Path.Combine(Application.dataPath, "GameData.csv"); // Chemin du fichier de sortie qui sera cr�� dans le dossier de l'application (Assets)
        ExportDataToCSV(filePath);
    }

    private void ExportDataToCSV(string filePath)
    {
        // R�cup�rer toutes les cl�s de tous les dictionnaires
        var allKeys = new List<string>();
        allKeys.AddRange(levelName.Keys);
        allKeys.AddRange(levelInfo.Keys);
        allKeys.AddRange(playerTimeInfo.Keys);
        allKeys.AddRange(causeOfDeath.Keys);

        // R�cup�rer les cl�s uniques car on ne veut pas de doublons
        var uniqueKeys = allKeys.Distinct().ToList();
        // Cr�er un dictionnaire pour stocker les valeurs
        var values = new Dictionary<string, string>();

        foreach (var key in uniqueKeys)
        {
            if (levelName.ContainsKey(key))
            {
                values[key] = levelName[key];
            }
            else

            if (levelInfo.ContainsKey(key))
            {
                //remplacer , par . pour pas tout casser
                values[key] = levelInfo[key].ToString().Replace(",", ".");
            }
            else if (playerTimeInfo.ContainsKey(key))
            {
                values[key] = playerTimeInfo[key].ToString().Replace(",", ".");
            }
            else if (causeOfDeath.ContainsKey(key))
            {
                values[key] = causeOfDeath[key].ToString().Replace(",", ".");
            }
            else
            {
                values[key] = "";
            }
        }

        // V�rifier si le fichier existe d�j�
        bool fileExists = File.Exists(filePath);
        // �crire les donn�es dans le fichier
        using (StreamWriter writer = new StreamWriter(filePath, true))
        {
            // Ecrire les cl�s si le fichier n'existe pas
            if (!fileExists)
            {
                writer.WriteLine(string.Join(",", uniqueKeys));
            }

            // Ecrire les valeurs
            var valueList = uniqueKeys.Select(key => values[key]).ToList();
            writer.WriteLine(string.Join(",", valueList));
        }

        Debug.Log($"Data exported to {filePath}"); // Juste du debug on peut le retirer
    }
}
