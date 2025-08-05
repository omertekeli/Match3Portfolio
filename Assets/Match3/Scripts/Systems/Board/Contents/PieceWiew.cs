using UnityEngine;

namespace Match3.Scripts.Systems.Board.Contents
{
    public abstract class PieceView : MonoBehaviour
    {
        public object Model { get; private set; }
        public virtual void Initialize(object model)
        {
            this.Model = model;
        }
    }
}