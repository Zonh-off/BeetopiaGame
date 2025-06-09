using UnityEngine;

public class SpriteOutline : MonoBehaviour {
    private void OnEnable() {
        BasePlacedObject.OnPlacedObjectSelected += OnObjectOutlineUpdate;
    }

    private void OnDisable() {
        BasePlacedObject.OnPlacedObjectSelected -= OnObjectOutlineUpdate;
    }
    
    private void OnObjectOutlineUpdate(BasePlacedObject obj) {
        if (obj ==  transform.GetComponent<BasePlacedObject>()) {
            var visual = transform.Find("Visual");
            
            visual.GetComponent<SpriteRenderer>().material.SetFloat("_Radius", 1);
        }
        else {
            var visual = transform.Find("Visual");
            
            visual.GetComponent<SpriteRenderer>().material.SetFloat("_Radius", 0);
        }
    }
}