using UnityEngine;
using UnityEngine.Serialization;

public class GameAssets : MonoBehaviour, IProviderHandler {

    [System.Serializable]
    public class CursorType_Refs {
        public Texture2D defaultCursor;
        public Texture2D smokerCursor;
        public Texture2D buildingCursor;
    }

    public CursorType_Refs cursorType_Refs;
    
    [System.Serializable]
    public class PlacedObjectTypeSO_Refs {

        public BasePlaceableSO storage;
        public BasePlaceableSO waterwell;
        public BasePlaceableSO nectarProcessorComb;
        public BasePlaceableSO nectarProcessorFramedComb;
        public BasePlaceableSO combProcessor;
        public BasePlaceableSO framedCombProcessor;
    }

    public PlacedObjectTypeSO_Refs placedObjectTypeSO_Refs;
    
    [System.Serializable]
    public class CropObjectTypeSO_Refs {

        public CropSO chamomile;
        public CropSO pinkCat;
        public CropSO purpleConeflower;
        public CropSO rose;
        public CropSO shadedViolet;
        public CropSO springRose;
        public CropSO sweetJasmine;
    }

    public CropObjectTypeSO_Refs cropObjectTypeSO_Refs;

    
    [System.Serializable]
    public class ItemSO_Refs {

        public ItemSO nectar_chamomile;
        public ItemSO honeyComb_chamomile;
        public ItemSO honeyCombFramed_chamomile;
        public ItemSO honey_chamomile;

        public ItemSO any;
        public ItemSO none;
    }
    
    public ItemSO_Refs itemSO_Refs;
    public Transform pfWorldItem;

    [System.Serializable]
    public class HintSprite_Refs {

        public Sprite water;
        public Sprite nectar;
    }
    
    public HintSprite_Refs hintSprite_Refs;


    [System.Serializable]
    public class Entity_Refs {

        [FormerlySerializedAs("beeDefault")] public BeeUnitSO beeUnitDefault;
        [FormerlySerializedAs("beeSpeed")] public BeeUnitSO beeUnitSpeed;
        [FormerlySerializedAs("beeLoader")] public BeeUnitSO beeUnitLoader;

        public BeeUnitSO any;
        public BeeUnitSO none;
    }
    
    public Entity_Refs entity_Refs;

    
    public string ToJson()
    {
        return JsonUtility.ToJson(this, true);
    }

    public void LoadJson(string jsonFilepath)
    {
        JsonUtility.FromJsonOverwrite(jsonFilepath, this);
    }
}