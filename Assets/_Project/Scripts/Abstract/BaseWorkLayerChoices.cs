using System.Collections.Generic;
using UnityEngine;

namespace ARMarker
{

    public abstract class BaseWorkLayerChoices<T> 
        : ScriptableObject 
        where T : class
    {

        [SerializeField]
        private new string name;

        [SerializeField]
        private List<T> choices = new();

        public string Name => name;
        public List<T> Choices => choices;

    }

}