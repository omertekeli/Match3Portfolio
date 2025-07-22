using System;
using System.ComponentModel.Design;
using UnityEngine;

namespace Match3.Scripts.UI.Base
{
    public abstract class UIView : MonoBehaviour, IUIView
    {
        [SerializeField] protected GameObject _root;

        public virtual void SetVisible(bool shouldVisible)
        {
            if (!_root)
            {
                _root.SetActive(shouldVisible);
            }
            else
            {
                gameObject.SetActive(shouldVisible);
            }
        }
    }
}