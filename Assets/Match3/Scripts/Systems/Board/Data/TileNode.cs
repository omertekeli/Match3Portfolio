using System.Collections.Generic;
using System.Linq;
using Match3.Scripts.Enums;
using Match3.Scripts.Systems.Board.Contents;
using Match3.Scripts.Systems.Board.Contents.Gem;
using UnityEngine;

namespace Match3.Scripts.Systems.Board.Data
{
    public class TileNode
    {
        #region Base properties

        public Vector2Int GridPosition { get; private set; }
        public CellBehaviorType Behavior { get; private set; }
        public Dictionary<Direction, TileNode> Neighbors { get; private set; }

        #endregion

        #region Layer Properties

        public IBoardContent Content { get; private set; }
        public List<IOverlay> Overlays { get; private set; }

        #endregion

        #region Helper Properties
        public bool IsPlayable => Behavior != CellBehaviorType.Hole;
        public bool IsEmpty => Content == null;
        public bool HasGem => Content is Gem;
        public bool IsMatchable
        {
            get
            {
                if (!HasGem) return false;
                if (Overlays.Any(overlay => overlay.IsBlockingMatch())) return false;
                return true;
            }
        }
        public bool IsSwappable
        {
            get
            {
                if (!IsPlayable || IsEmpty) return false;
                if (Overlays.Any(overlay => overlay.IsBlockingSwap())) return false;
                if (!Content.CanBeSwapped()) return false;
                return true;
            }
        }


        #endregion

        public TileNode(int x, int y, CellBehaviorType behavior)
        {
            GridPosition = new Vector2Int(x, y);
            Behavior = behavior;
            Neighbors = new Dictionary<Direction, TileNode>();
            Overlays = new List<IOverlay>();
        }

        #region Methods



        #endregion
    }
}