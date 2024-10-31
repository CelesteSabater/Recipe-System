using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Celeste.Tools.RecipeTree
{
    public class RecipeRoot : RecipeNode
    {
        [HideInInspector] public List<RecipeNode> _children = new List<RecipeNode>();
        [SerializeField] private List<Image> _recipe = new List<Image>();
        public List<Image> GetRecipe() => _recipe;
        
        public override RecipeNode Clone()
        {
            RecipeRoot node = Instantiate(this);
            node._children = _children.ConvertAll(child => child.Clone());  
            return node;
        }

    }
}
