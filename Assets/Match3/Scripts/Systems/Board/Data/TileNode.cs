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
        public IOverlay Overlay { get; private set; }

        #endregion

        #region Helper Properties
        public bool IsPlayable => Behavior != CellBehaviorType.Hole;
        public bool IsEmpty => Content == null;
        public bool HasGem => Content is Gem;
        public bool HasOverlay => Overlay != null;
        public bool IsMatchable
        {
            get
            {
                if (!HasGem)
                    return false;
                if (HasOverlay && Overlay.IsBlockingMatch())
                    return false;
                return true;
            }
        }
        public bool IsSwappable
        {
            get
            {
                if (!IsPlayable || IsEmpty)
                    return false;
                if (HasOverlay && Overlay.IsBlockingSwap())
                    return false;
                if (!Content.CanBeSwapped())
                    return false;
                return true;
            }
        }

        #endregion

        public TileNode(int x, int y, CellBehaviorType behavior)
        {
            Behavior = behavior;
            GridPosition = new Vector2Int(x, y);
            Neighbors = new Dictionary<Direction, TileNode>();
        }

        #region Methods

        /// <summary>
        /// Sets the main content of this tile. Should only be used during board creation or when content is cleared.
        /// </summary>
        public void SetContent(IBoardContent newContent)
        {
            if (Content != null && newContent != null)
            {
                Debug.LogWarning($"Tile at {GridPosition} already has content! Overwriting.");
            }
            Content = newContent;
        }

        /// <summary>
        /// Adds an overlay to this tile.
        /// </summary>
        public void SetOverlay(IOverlay newOverlay)
        {
            if (Overlay != null && newOverlay != null)
            {
                Debug.LogWarning($"Tile at {GridPosition} already has overlay! Overwriting.");
            }
            Overlay = newOverlay;
        }

        public void UpdateContent(IBoardContent newContent)
        {
            Content = newContent;
            if (Content != null)
            {
                Content.Node = this;
            }
        }

        public void ClearOverlay()
        {
            Overlay = null;
        }

        #endregion
    }
}