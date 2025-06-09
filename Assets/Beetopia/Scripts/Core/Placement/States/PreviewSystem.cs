using System.Collections.Generic;
using UnityEngine;

public class PreviewSystem : MonoBehaviour {
    private Transform previewObject;
    private List<Transform> previewObjectToPlaceList = new();
    
    public void StartShowingPreview(BasePlaceableSO gridObjectSO) {
        previewObject = Instantiate(gridObjectSO.visual);
        if (gridObjectSO is CropSO cropSo) {
            previewObject.Find("Visual").GetComponent<SpriteRenderer>().sprite = cropSo.cropPhasesList[0].sprite;
        }
    }
    
    public int AddPreviewObject(BasePlaceableSO gridObjectSO, Vector2Int position) {
        var previewObjectToPlace = Instantiate(gridObjectSO.visual);
        if (gridObjectSO is CropSO cropSo) {
            previewObjectToPlace.Find("Visual").GetComponent<SpriteRenderer>().sprite = cropSo.cropPhasesList[0].sprite;
        }
        MovePreview(previewObjectToPlace, position);
        Color c = Color.white;
        c.a = 0.8f;
        previewObjectToPlace.Find("Visual").GetComponent<SpriteRenderer>().color = c;
        previewObjectToPlaceList.Add(previewObjectToPlace);
        
        if (previewObjectToPlaceList.Exists(obj => obj == null)) {
            for (int i = 0; i < previewObjectToPlaceList.Count; i++) {
                if (previewObjectToPlaceList[i] == null) {
                    previewObjectToPlaceList[i] = previewObjectToPlace;
                    return i;
                }
            }
        }
        else {
            previewObjectToPlaceList.Add(previewObjectToPlace);
        } 
        
        return previewObjectToPlaceList.Count - 1;
    }
    
    public void RemovePreviewObject(int index) {
        if(index == -1) Debug.Log($"Obj index: {index}");

        Destroy(previewObjectToPlaceList[index].gameObject);
    }
    
    public void StopShowingPreview() {
        if(previewObject != null)
            Destroy(previewObject.gameObject);
    }
    
    public void UpdatePosition(Vector2Int position, bool validity) {
        if(previewObject != null) {
            MovePreview(previewObject, position);
            ApplyFeedbackToPreview(previewObject, validity);
        }

        /*MoveCursor(position);
        ApplyFeedbackToCursor(validity);*/
    }

    private void MovePreview(Transform previewObject, Vector2Int position) {
        previewObject.transform.position = new Vector2(position.x, position.y)  + new Vector2(0.5f, 0.5f);
    }

    private void ApplyFeedbackToPreview(Transform previewObject, bool validity) {
        Color c = validity ? Color.red : Color.white;
        c.a = 0.8f;
        previewObject.Find("Visual").GetComponent<SpriteRenderer>().color = c;
        previewObject.Find("Visual").GetComponent<SpriteRenderer>().renderingLayerMask = 100;
    }    
}