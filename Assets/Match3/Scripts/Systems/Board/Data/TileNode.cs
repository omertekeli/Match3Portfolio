using System.Collections.Generic;
using Match3.Scripts.Enums;
using Match3.Scripts.Systems.Board.Contents.BoardPower;
using Match3.Scripts.Systems.Board.Contents.Gem;
using UnityEngine;

namespace Match3.Scripts.Systems.Board.Data
{
    public class TileNode
    {
        #region Properties

        public bool IsActive { get; private set; }
        public Vector2Int GridPosition { get; private set; }
        public CellTypes CellType { get; private set; }
        public TileContentTypes ContentType { get; private set; }
        public GemData? Gem { get; private set; }
        public BoardPowerData? Power { get; private set; }
        public Dictionary<Direction, TileNode> Neighbors { get; private set; }

        #endregion

        #region Methods

        #endregion
    }
}