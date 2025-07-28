using System.Collections.Generic;
using Match3.Scripts.Keys;
using UnityEngine;

namespace Match3.Scripts.Configs
{
    [CreateAssetMenu(fileName = "GemSpriteLibrary", menuName = "Match3/Gem Sprite Library")]
    public class GemSpriteLibrarySO : ScriptableObject
    {
        [SerializeField] private List<GemTypeSpritePair> _gemSprites;
        public IReadOnlyList<GemTypeSpritePair> GemSprites => _gemSprites;
    }
}