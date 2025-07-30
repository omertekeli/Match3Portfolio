using System.Collections.Generic;
using System.Linq;
using Match3.Scripts.Enums;
using Match3.Scripts.Systems.Level.Base;
using UnityEngine;

namespace Match3.Scripts.Systems.Level.Data
{
    [CreateAssetMenu(menuName = "Match3/Level Data")]
    public class LevelDataSO : ScriptableObject
    {
        [Header("General")]
        [SerializeField] private int _levelNumber;
        [SerializeField] private int _maxMove;

        [Header("Gem and Piece Information")]
        [SerializeField] private List<GemType> _availablePieceTypes;

        [Header("Board Structure")]
        [Range(3, 20)][SerializeField] private int _width = 8;
        [Range(3, 20)][SerializeField] private int _height = 8;
        [HideInInspector][SerializeField] private TileSetupData[] _tileSetups;

        [Header("Additionals")]
        [SerializeField] private AudioClip _music;

        public int LevelNumber => _levelNumber;
        public int MaxMove => _maxMove;
        public AudioClip Music => _music;
        public int Width => _width;
        public int Height => _height;
        public TileSetupData[] TileSetup => _tileSetups;
        public IReadOnlyList<GemType> AvailablePieceTypes => _availablePieceTypes;

        [SerializeField] private LevelGoalSO[] _levelGoals;
        public IReadOnlyList<LevelGoalSO> LevelGoals => _levelGoals;

        private void OnValidate()
        {
            // Resize when height or width is changed
            if (_tileSetups == null || _tileSetups.Length != _width * _height)
            {
                _tileSetups = new TileSetupData[_width * _height];
                for (int i = 0; i < _tileSetups.Length; i++)
                {
                    // Add new to avoid null 
                    _tileSetups[i] = new TileSetupData();
                }
            }
        }

        public List<LevelGoalBase> CreateRuntimeGoals()
        {
            return _levelGoals.Select(g => g.CreateGoal()).ToList();
        }
    }
}