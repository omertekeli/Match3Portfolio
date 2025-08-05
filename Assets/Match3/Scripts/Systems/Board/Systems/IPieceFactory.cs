using UnityEngine;
using Match3.Scripts.Systems.Board.Data;
using Match3.Scripts.Systems.Level.Data;

public interface IPieceFactory
{
    void CreateContentDataForNode(TileNode node, TileSetupData setupData);
    GameObject CreateVisualForNode(TileNode node, Vector3 startPosition);
}