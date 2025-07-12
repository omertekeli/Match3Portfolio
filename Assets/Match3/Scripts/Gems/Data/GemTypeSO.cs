using Match3.Scripts.Enums;
using UnityEngine;

namespace Match3.Scripts.Gems.Data
{
    [CreateAssetMenu(fileName = "GemTypeSO", menuName = "Match3/Gem Type")]
    public class GemTypeSO: ScriptableObject
    {
        [Header("Basic Info")]
        [SerializeField] private GemType _type;
        
        public GemType Type => _type;
    }
}