using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;

[RequireComponent(typeof(SpriteOutline))]
public abstract class BasePlacedObject : MonoBehaviour {
    public static Action<BasePlacedObject> OnPlacedObjectSelected;

    public float clickCooldownTime = 1f;
    public bool isOnCooldown = false;

    public virtual void Setup(BasePlaceableSO basePlaceableSO) {
        PlayPunchAnim();
    }

    public void PlayPunchAnim(Vector3 punchScale = default) {
        if (punchScale == default) punchScale = new Vector3(.35f, .35f, .35f);
        
        transform.DORewind();
        transform
            .DOPunchScale(punchScale, .25f)
            .SetEase(Ease.InOutSine);
    }

    protected IEnumerator StartClickCooldown() {
        isOnCooldown = true;
        yield return new WaitForSeconds(clickCooldownTime);
        isOnCooldown = false;
    }

    public Vector2Int GetGridPosition() => G.PlacementManager.GetWorldToCellPosition(transform.position);
}
