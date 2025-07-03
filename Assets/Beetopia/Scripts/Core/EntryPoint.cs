using System.Collections;
using UnityEngine;

public class EntryPoint : MonoBehaviour {
    private void Awake() {
        StartCoroutine(RegisterManager());
    }

    private IEnumerator RegisterManager() {
        G.ClearAll();
        
        // Register managers
        G.Register(FindAnyObjectByType<GameAssets>());
        G.Register(FindAnyObjectByType<DataManager>());
        G.Register(FindAnyObjectByType<InputManager>());
        G.Register(FindAnyObjectByType<CameraManager>());
        G.Register(FindAnyObjectByType<GameManager>());
        G.Register(FindAnyObjectByType<PlacementManager>());
        G.Register(FindAnyObjectByType<WorldExpansionManager>());
        //G.Register(FindAnyObjectByType<QuestManager>());
        G.Register(FindAnyObjectByType<TaskManager>());
        G.Register(FindAnyObjectByType<UnitsManager>());
        
        // Register uis
        G.Register(FindAnyObjectByType<SelectToolTypeUI>());
        G.Register(FindAnyObjectByType<SidePanelUI>());
        
        // Setup beginning of the game
        yield return GameSetup();

        yield return true;
    }
    
    private IEnumerator GameSetup() {
        yield return G.DataManager.InitializeData();
        
        yield return G.DataManager.TryStoreItem(G.GameAssets.itemSO_Refs.honey_chamomile, 15);
        yield return G.DataManager.TryStoreItem(G.GameAssets.itemSO_Refs.nectar_chamomile, 25);
        yield return G.DataManager.TryStoreItem(G.GameAssets.itemSO_Refs.honeyComb_chamomile, 3);
        
        yield return G.InputManager.Initialize();
        yield return G.CameraManager.Initialize();
        yield return G.GameManager.Initialize();
        yield return G.WorldExpansionManager.InitializeChunks();
        yield return G.PlacementManager.Initialize();
        yield return G.UnitsManager.Initialize();
        
        yield return G.UI.SelectToolTypeUI.Initialize();
        yield return G.UI.SidePanelUI.InitializeComponents();
        
        yield return G.UnitsManager.SpawnUnit(G.GameAssets.entity_Refs.beeUnitSpeed, Vector3.zero);
        yield return G.UnitsManager.SpawnUnit(G.GameAssets.entity_Refs.beeUnitSpeed, Vector3.zero);

        yield return true;
    }
    
    private void Update() {
        if (Input.GetKeyDown(KeyCode.C)) {
            G.DataManager.AddCoins(500);
        }
        
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            Time.timeScale = 3;
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            Time.timeScale = 2;
        }
        
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Time.timeScale = 1;
        }
    }
}