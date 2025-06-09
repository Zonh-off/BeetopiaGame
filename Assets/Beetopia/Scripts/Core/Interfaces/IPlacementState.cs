public interface IPlacementState {
    void OnAction(UnityEngine.Vector2Int gridPosition);
    void UpdateState(UnityEngine.Vector2Int position);
    void EndState();
}