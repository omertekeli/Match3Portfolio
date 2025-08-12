using UnityEngine;
using Match3.Scripts.Enums;

public class CellView : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private BoxCollider2D _collider;

    public void Initialize(CellBehaviorType behaviorType)
    {
        switch (behaviorType)
        {
            case CellBehaviorType.Hole:
                _spriteRenderer.enabled = false;
                _collider.enabled = true;
                break;
            case CellBehaviorType.Normal:
            case CellBehaviorType.Generator:
            default:
                _spriteRenderer.enabled = true;
                _collider.enabled = false;
                break;
        }
    }
}