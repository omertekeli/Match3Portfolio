using System.Collections.Generic;
using Match3.Scripts.Configs;
using Match3.Scripts.Enums;
using UnityEngine;

namespace Match3.Scripts.Core
{
    public class GemSpriteProvider : IService
    {
        private readonly Dictionary<GemType, Sprite> _spriteDict;

        public GemSpriteProvider(GemSpriteLibrarySO spriteLibrary)
        {
            _spriteDict = new Dictionary<GemType, Sprite>();
            foreach (var pair in spriteLibrary.GemSprites)
            {
                if (pair.Sprite != null && !_spriteDict.ContainsKey(pair.Type))
                {
                    _spriteDict.Add(pair.Type, pair.Sprite);
                }
            }
        }

        public Sprite GetSprite(GemType type)
        {
            return _spriteDict.TryGetValue(type, out var sprite) ? sprite : null;
        }
    }
}