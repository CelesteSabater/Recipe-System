using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using static Unity.VisualScripting.Metadata;

namespace Celeste.Tools.RecipeTree
{
    public class CuttingNode : RecipeNode
    {
        [HideInInspector] public RecipeNode _child;

        public override RecipeNode Clone()
        {
            CuttingNode node = Instantiate(this);
            node._child = _child.Clone();
            return node;
        }
    }
}