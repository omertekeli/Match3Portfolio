using System.Collections.Generic;
using Match3.Scripts.Systems.Board.Contents.Gem;
using Match3.Scripts.Systems.Board.Data;

namespace Match3.Scripts.Systems.Board.Systems
{
    public class MatchFindingSystem
    {
        #region Fields
        private readonly Board _board;
        #endregion

        public MatchFindingSystem(Board board)
        {
            _board = board;
        }

        public List<TileNode> FindAllMatches()
        {
            var matchedNodes = new HashSet<TileNode>();

            for (int y = 0; y < _board.Height; y++)
            {
                for (int x = 0; x < _board.Width; x++)
                {
                    var node = _board[x, y];
                    if (node == null || !node.IsPlayable || !node.IsMatchable || node.IsEmpty)
                        continue;

                    var gem = node.Content as Gem;
                    if (gem == null)
                        continue;

                    var horizontalMatch = new List<TileNode> { node };
                    for (int i = x + 1; i < _board.Width; i++)
                    {
                        var nextNode = _board[i, y];
                        if (nextNode != null && nextNode.IsMatchable && nextNode.Content is Gem nextGem && nextGem.Type == gem.Type)
                        {
                            horizontalMatch.Add(nextNode);
                        }
                        else break;
                    }
                    if (horizontalMatch.Count >= 3)
                    {
                        foreach (var matchedNode in horizontalMatch)
                            matchedNodes.Add(matchedNode);
                    }

                    var verticalMatch = new List<TileNode> { node };
                    for (int i = y + 1; i < _board.Height; i++)
                    {
                        var nextNode = _board[x, i];
                        if (nextNode != null && nextNode.IsMatchable && nextNode.Content is Gem nextGem && nextGem.Type == gem.Type)
                        {
                            verticalMatch.Add(nextNode);
                        }
                        else break;
                    }
                    if (verticalMatch.Count >= 3)
                    {
                        foreach (var matchedNode in verticalMatch)
                            matchedNodes.Add(matchedNode);
                    }
                }
            }
            return new List<TileNode>(matchedNodes);
        }

        #region Methods

        #endregion
    }
}