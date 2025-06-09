using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour, IProviderHandler {
    public EventHandler OnInteractAction;
    public EventHandler OnCancelAction;
    
    private PlayerInputActions _playerInputActions;

    public IEnumerator Initialize() {
        _playerInputActions = new PlayerInputActions();
        _playerInputActions.Player.Enable();
        
        _playerInputActions.Player.Interact.performed += Interact_Performed;
        _playerInputActions.Player.Cancel.performed += Cancel_Performed;

        yield return true;
    }

    private void Cancel_Performed(InputAction.CallbackContext obj) {
        OnCancelAction?.Invoke(this, EventArgs.Empty);
    }

    private void Interact_Performed(InputAction.CallbackContext obj) {
        OnInteractAction?.Invoke(this, EventArgs.Empty);
    }
    
    public Vector2 GetMouseScreenPosition() {
        Vector2 position = _playerInputActions.Player.MousePosition.ReadValue<Vector2>();
        
        return position;
    }
    
    private void OnDestroy() {
        _playerInputActions.Player.Interact.performed -= Interact_Performed;
        _playerInputActions.Player.Cancel.performed -= Cancel_Performed;
        
        _playerInputActions.Dispose();
    }
}