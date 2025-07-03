using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraManager : MonoBehaviour, IProviderHandler {
    public float moveSpeed = 10f;
    public float screenEdgeThreshold = 5f;
    public float zoomSpeed = 5f;
    public float minZoom = 3f, maxZoom = 10f;

    private Camera cam;

    public IEnumerator Initialize() {
        cam = Camera.main;

        yield return true;
    }

    private void Update()
    {
        MoveCamera();
        ZoomCamera();
    }

    private void MoveCamera()
    {
        Vector3 move = Vector3.zero;
        
        if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed) move.y += 1;
        if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed) move.y -= 1;
        if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed) move.x -= 1;
        if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed) move.x += 1;

#if !UNITY_EDITOR
        /*Vector2 mousePos = Mouse.current.position.ReadValue();
        if (mousePos.x <= screenEdgeThreshold) move.x -= 0.1f;
        if (mousePos.x >= Screen.width - screenEdgeThreshold) move.x += 0.1f;
        if (mousePos.y <= screenEdgeThreshold) move.y -= 0.1f;
        if (mousePos.y >= Screen.height - screenEdgeThreshold) move.y += 0.1f;*/
#endif
        
        Camera.main.transform.position += move.normalized * moveSpeed * Time.deltaTime;
    }

    private void ZoomCamera()
    {
        float scroll = Mouse.current.scroll.ReadValue().y;
        if (scroll != 0)
        {
            cam.orthographicSize -= scroll * zoomSpeed * Time.deltaTime;
            cam.orthographicSize = Mathf.Clamp(cam.orthographicSize, minZoom, maxZoom);
        }
    }
}