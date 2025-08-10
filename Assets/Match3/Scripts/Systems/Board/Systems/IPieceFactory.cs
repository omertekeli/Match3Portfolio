using UnityEngine;
using Match3.Scripts.Systems.Board.Data;
using Match3.Scripts.Systems.Level.Data;
using Match3.Scripts.Systems.Board;

public interface IPieceFactory
{
    void CreateContentDataForNode(TileNode node, TileSetupData setupData);
    GameObject CreateVisualForNode(Board board, TileNode node, Vector3 startPosition);
}