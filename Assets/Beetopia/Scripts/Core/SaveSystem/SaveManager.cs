using System;
using UnityEngine;

[RequireComponent(typeof(DataManager))]
public class SaveManager : MonoBehaviour, IProviderHandler {
    public static event Action GameDataLoaded;

    [Tooltip("Filename to save game and settings data")]
    [SerializeField] string m_SaveFilename = "savegame.dat";
    [Tooltip("Show Debug messages.")]
    [SerializeField] bool m_Debug;

    DataManager _mDataManager;
    
    public void Awake() {
        _mDataManager = GetComponent<DataManager>();
    }
    
    void OnApplicationQuit()
    {
        //SaveGame();
    }
    
    public GameData NewGame()
    {
        return new GameData();
    }

    public void LoadGame()
    {
        // load saved data from FileDataHandler

        if (_mDataManager.GameData == null)
        {
            if (m_Debug)
            {
                Debug.Log("GAME DATA MANAGER LoadGame: Initializing game data.");
            }

            _mDataManager.GameData = NewGame();
        }
        else if (FileManager.LoadFromFile(m_SaveFilename, out var jsonString))
        {
            _mDataManager.GameData.LoadJson(jsonString);

            if (m_Debug)
            {
                    Debug.Log("SaveManager.LoadGame: " + m_SaveFilename + " json string: " + jsonString);
            }
        }

        // notify other game objects 
        if (_mDataManager.GameData != null)
        {
            GameDataLoaded?.Invoke();
        }
    }

    public void SaveGame() {
        string jsonFile = _mDataManager.GameData.ToJson();

        // save to disk with FileDataHandler
        if (FileManager.WriteToFile(m_SaveFilename, jsonFile) && m_Debug)
        {
            Debug.Log("SaveManager.SaveGame: " + m_SaveFilename + " json string: " + jsonFile);
        }
    }
}