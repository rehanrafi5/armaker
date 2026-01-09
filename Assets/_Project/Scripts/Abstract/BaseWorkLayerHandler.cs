using System;
using UnityEngine;

namespace ARMarker
{
    public abstract class BaseWorkLayerHandler : MonoBehaviour
    {

        protected Action onSelect;

        protected bool isSelected;

        public virtual void Select()
        {
            onSelect?.Invoke();   
        }

        public virtual void Deselect()
        {
            //onSelect?.Invoke(false);
        }

        public void RegisterListener(Action listener)
        {
            if (listener == null)
            {
                return;
            }

            onSelect += listener;
        }

    }

}