using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace Celeste.Tools.RecipeTree
{
    public class FryingNode : RecipeNode
    {
        [HideInInspector] public List<RecipeNode> _children = new List<RecipeNode>();

        public override RecipeNode Clone()
        {
            FryingNode node = Instantiate(this);
            node._children = _children.ConvertAll(child => child.Clone());  
            return node;
        }
    }
}