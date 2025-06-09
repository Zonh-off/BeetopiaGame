using System.Collections;
using System.Collections.Generic;
using CodeMonkey.Utils;
using DG.Tweening;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using Vector2 = UnityEngine.Vector2;

[System.Serializable]
public struct ChunkData {
    public uint price;
    public RuleTile groundTile;
    public int2 pos;
    public List<GridObject> objectsList;
    
    [System.Serializable]
    public struct GridObject {
        public BasePlaceableSO basePlaceableSo;
        public Vector2Int position;
    }
}

public class WorldExpansionManager : MonoBehaviour, IProviderHandler {
    public Tilemap baseTileMap;

    [SerializeField] private Grid grid;
    [SerializeField] private int chunkSize;
    [SerializeField] private List<ChunkData> chunks;
    
    private HashSet<ChunkData> _unlockedChunksDatabase = new();
    private Dictionary<int2, ChunkData> _chunksDatabase = new();

    private ObjectPlaceSystem _objectPlaceSystem;

    public Dictionary<int2, ChunkData> GetChunks() => _chunksDatabase;
    
    public IEnumerator InitializeChunks() {
        _objectPlaceSystem = FindObjectOfType<ObjectPlaceSystem>();
        if (_objectPlaceSystem == null) {
            Debug.LogError("ObjectPlaceSystem not found in the scene!");
        }
        
        foreach (var obj in chunks) {
            _chunksDatabase[obj.pos] = obj;
        }
        
        FillChunk(_chunksDatabase[new int2(0, 0)]);

        yield return true;
    }
    
    public void FillChunk(ChunkData chunk) {
        for (int x = 0; x < chunkSize; x++) {
            for (int y = 0; y < chunkSize; y++) {
                Vector3Int tilePos = new Vector3Int(x + chunk.pos.x * chunkSize,
                    y + chunk.pos.y * chunkSize, 0);
                    
                baseTileMap.SetTile(tilePos, chunk.groundTile);
            }
        }
        
        _unlockedChunksDatabase.Add(chunk);
        _chunksDatabase.Remove(chunk.pos);
        
        PlaceObjectsInChunk(chunk);
    }
    
    private void PlaceObjectsInChunk(ChunkData chunk) {
        foreach (var gridObject in chunk.objectsList) {
            Vector2Int gridObjPos = new Vector2Int(gridObject.position.x + chunk.pos.x * chunkSize,
                gridObject.position.y + chunk.pos.y * chunkSize);
            
            if (_objectPlaceSystem != null) {
                int index = _objectPlaceSystem.PlaceObject(gridObject.basePlaceableSo, gridObjPos);
                if (G.GameAssets != null && G.DataManager.GridDatabase != null) {
                    G.DataManager.GridDatabase.AddGridObject(gridObjPos, gridObject.basePlaceableSo, index);
                }
                _objectPlaceSystem.TryGetStoredGameObjectByIndex(index).GetComponent<BasePlacedObject>().Setup(gridObject.basePlaceableSo);
            }
        }
    }
    
    public void ShowAvailableChunks() {
        Transform chunkContainer = transform.Find("ChunksContainer");
        Transform chunkTemplate = chunkContainer.Find("Template");
        chunkTemplate.Find("Canvas").GetComponent<RectTransform>().sizeDelta = new Vector2(chunkSize, chunkSize);
        chunkTemplate.GetComponent<BoxCollider2D>().size = new Vector2(chunkSize, chunkSize);
        chunkTemplate.gameObject.SetActive(false);

        foreach (Transform transform in chunkContainer) {
            if (transform != chunkTemplate) {
                Destroy(transform.gameObject);
            }
        }

        var chunks = GetChunks();
        
        foreach (var chunk in chunks) {
            // Set center pos
            int2 pos = chunk.Value.pos;
            float2 centerPos = new float2((float)chunkSize / 2, (float)chunkSize / 2);
            Vector2 normilizedCenterPos = pos * chunkSize + centerPos;
            
            // Instantiate
            Transform chunkTransform = Instantiate(chunkTemplate, chunkContainer);
            chunkTransform.gameObject.SetActive(true);
            chunkTransform.position = normilizedCenterPos;
            chunkTransform.Find("Canvas").Find("Price").GetComponent<TextMeshProUGUI>().text = $"<color=green>{chunk.Value.price}C</color>";
            
            // Outline update
            if (G.DataManager.CanAfford(chunk.Value.price)) {
                Color outlineColor = Color.green;
                outlineColor.a = 0.5f;
                chunkTransform.Find("Canvas").Find("Outline").GetComponent<Image>().color = outlineColor;
            }
            else {
                Color outlineColor = Color.red;
                outlineColor.a = 0.5f;
                chunkTransform.Find("Canvas").Find("Outline").GetComponent<Image>().color = outlineColor;
            }
            
            // Actions
            chunkTransform.GetComponent<Button_Sprite>().ClickFunc = () => {
                if (G.DataManager.CanAfford(chunk.Value.price)) {
                    FillChunk(chunk.Value);
                    G.DataManager.RemoveCoins(chunk.Value.price);
                }
                else {
                    UtilsClass.CreateWorldTextPopup("Not enough coins!", new Vector3(0, 0,0));
                }
            };            
            chunkTransform.GetComponent<Button_Sprite>().MouseOverOnceFunc = () => {
                chunkTransform.Find("Canvas").Find("Outline").DORewind();
                chunkTransform.Find("Canvas").Find("Outline")
                   .DOPunchScale(new Vector3(.1f, .1f, .1f), .25f)
                   .SetEase(Ease.InOutSine);
            };
        }
    }
    
    public void DestroyAvailableChunks() {
        Transform chunkContainer = transform.Find("ChunksContainer");
        Transform chunkTemplate = chunkContainer.Find("Template");
        
        foreach (Transform transform in chunkContainer) {
            if (transform != chunkTemplate) {
                Destroy(transform.gameObject);
            }
            else {
                chunkTemplate.gameObject.SetActive(false);
            }
        }
    }
}