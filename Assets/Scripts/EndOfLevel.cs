using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndOfLevel : MonoBehaviour
{
<<<<<<< HEAD
    [SerializeField]
    private string sceneToLoad;
=======
    public IDictionary<string, float> levelInfo;
    public IDictionary<string, float> playerTimeInfo;
    public IDictionary<string, string> causeOfDeath;

    private bool dataExported = false; // Flag pour s'assurer que les donn�es sont export�es une seule fois
>>>>>>> aed09f11cc138e760ec2a20f3573d8ab019e99ae

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !dataExported)
        {
<<<<<<< HEAD
            SceneManager.LoadScene(sceneToLoad);
=======
            Debug.Log("End of level reached!");
            dataExported = true; // mettre le flag � true pour �viter les exports dupliqu�s
            FindDicoDatas();
>>>>>>> aed09f11cc138e760ec2a20f3573d8ab019e99ae
        }
    }

    void FindDicoDatas()
    {
        // R�cup�rer les donn�es de DataToStore
        levelInfo = DataToStore.instance.levelInfo; // Celles du niveau
        playerTimeInfo = DataToStore.instance.playerTimeInfo; // Celles du joueur et des Timer
        causeOfDeath = DataToStore.instance.causeOfDeath; // Celles des morts
        // Exporter les donn�es au format CSV
        string filePath = Path.Combine(Application.dataPath, "GameData.csv"); // Chemin du fichier de sortie qui sera cr�� dans le dossier de l'application (Assets)
        ExportDataToCSV(filePath);
    }

    void ExportDataToCSV(string filePath)
    {
        // R�cup�rer toutes les cl�s de tous les dictionnaires
        var allKeys = new List<string>();
        allKeys.AddRange(levelInfo.Keys); 
        allKeys.AddRange(playerTimeInfo.Keys);
        allKeys.AddRange(causeOfDeath.Keys);

        // R�cup�rer les cl�s uniques car on ne veut pas de doublons
        var uniqueKeys = allKeys.Distinct().ToList();
        // Cr�er un dictionnaire pour stocker les valeurs
        var values = new Dictionary<string, string>(); 

        foreach (var key in uniqueKeys)
        {
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
                values[key] = causeOfDeath[key]; 
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
