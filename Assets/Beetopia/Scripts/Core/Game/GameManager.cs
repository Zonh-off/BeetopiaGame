using System;
using System.Collections;
using UnityEngine;

public enum ToolState {
    Hand,
    Building,
    Demolishing
}

public class GameManager : MonoBehaviour, IProviderHandler {
    public event Action OnInteract;
    public event Action OnClick;
    public event Action OnCancel;
    public static event Action OnToolStateChanged;

    private ToolState currentState;

    public IEnumerator Initialize() {
        G.InputManager.OnInteractAction += OnInteractAction;
        G.InputManager.OnCancelAction += OnCancelAction;
        
        SetToolState(ToolState.Hand);

        yield return true;
    }
    
    private void OnDestroy() {
        G.InputManager.OnInteractAction -= OnInteractAction;
        G.InputManager.OnCancelAction -= OnCancelAction;
    }

    private void OnInteractAction(object sender, EventArgs e) {
        OnClick?.Invoke();
        OnInteract?.Invoke();
    }

    private void OnCancelAction(object sender, EventArgs e) {
        OnCancel?.Invoke();
    }
    
    public void SetToolState(ToolState newState) {
        currentState = newState;
        SetCursor(currentState);
        OnToolStateChanged?.Invoke();
    }

    private void SetCursor(ToolState toolState) {
        switch (toolState) {
            case ToolState.Hand:
                Cursor.SetCursor(G.GameAssets.cursorType_Refs.defaultCursor, new Vector2(0, 0), CursorMode.ForceSoftware);
                Cursor.visible = true;
                break;
            
            case ToolState.Building:
                Cursor.SetCursor(G.GameAssets.cursorType_Refs.defaultCursor, new Vector2(0, 0), CursorMode.ForceSoftware);
                Cursor.visible = false;
                break;
            
            case ToolState.Demolishing:
                Cursor.SetCursor(G.GameAssets.cursorType_Refs.defaultCursor, new Vector2(0, 0), CursorMode.ForceSoftware);
                Cursor.visible = true;
                break;
            
            default:
                throw new ArgumentOutOfRangeException(nameof(toolState), toolState, null);
        }
    }

    public ToolState GetToolState() => currentState;
}