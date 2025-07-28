using System.Collections.Generic;
using System.Linq;
using Match3.Scripts.Systems.Level.Base;
using UnityEngine;

namespace Match3.Scripts.Systems.Level.Data
{   
    [CreateAssetMenu(menuName = "Match3/Level Data")]
    public class LevelDataSO: ScriptableObject
    {
        [SerializeField] private int _levelNumber;
        [SerializeField] private int _maxMove;
        [SerializeField] private int _lowMoveTrigger = 10;
        [SerializeField] private float _borderMargin = 0.3f; // Margin around the board for camera framing.
        [SerializeField] private Sprite _backgroundSprite;
        [SerializeField] private AudioClip _music;
        
        public int LevelNumber => _levelNumber;
        public int MaxMove => _maxMove;
        public int LowMoveTrigger => _lowMoveTrigger;
        public float BorderMargin => _borderMargin;
        public Sprite BackgroundSprite => _backgroundSprite;
        public AudioClip Music => _music;

        [SerializeField] private LevelGoalSO[] _levelGoals;
        public IReadOnlyList<LevelGoalSO> LevelGoals => _levelGoals;

        public List<LevelGoalBase> CreateRuntimeGoals()
        {
            return _levelGoals.Select(g => g.CreateGoal()).ToList();
        }
    }
}