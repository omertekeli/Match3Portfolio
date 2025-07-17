using System.Collections.Generic;
using Match3.Scripts.Enums;
using Match3.Scripts.Keys;
using UnityEngine;

namespace Match3.Scripts.Core
{
    public class GemSpriteProvider : MonoBehaviour, IService
    {
        [SerializeField] private List<GemTypeSpritePair> _gemSprites;
        
        private Dictionary<GemType, Sprite> _spriteDict;

        private void Awake()
        {
            SetDictionary();
            DontDestroyOnLoad(gameObject);
        }

        private void SetDictionary()
        {
            _spriteDict = new Dictionary<GemType, Sprite>();
            foreach (var pair in _gemSprites)
            {
                _spriteDict[pair.Type] = pair.Sprite;
            }
        }

        public Sprite GetSprite(GemType type)
        {
            return _spriteDict.TryGetValue(type, out var sprite) ? sprite : null;
        }
    }
}