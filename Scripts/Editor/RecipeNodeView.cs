using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using UnityEditor;

namespace Celeste.Tools.RecipeTree
{
    public class RecipeNodeView : UnityEditor.Experimental.GraphView.Node
    {
        public Action<RecipeNodeView> OnNodeSelected;
        public RecipeNode _node;
        [SerializeField] private Port _input, _output;

        public RecipeNodeView(RecipeNode node) : base("Assets/Packages/Celeste/Tools/RecipeTree/UIBuilder/RecipeNodeView.uxml")
        {
            _node = node;
            title = node.name;
            viewDataKey = node.GetGUID();

            Vector2 v = node.GetPosition();
            style.left = v.x;
            style.top = v.y;

            CreateInputPorts();
            CreateOutputPorts();
            SetupClasses();
            UpdateLabel();
        }

        public void UpdateLabel()
        {
            Label descriptionLabel = this.Q<Label>("description");
            descriptionLabel.text = _node.GetFoodName();
        }

        private void SetupClasses()
        {
            switch (_node)
            {
                case RecipeRoot _node:
                    AddToClassList("root");
                    break;
                case CuttingNode _node:
                    AddToClassList("cutting");
                    break;
                case FryingNode _node:
                    AddToClassList("frying");
                    break;
                case FurnaceNode _node:
                    AddToClassList("furnace");
                    break;
                case MixingNode _node:
                    AddToClassList("mixing");
                    break;
                case IngredientNode _node:
                    AddToClassList("ingredient");
                    break;
            }
        }

        private void CreateInputPorts()
        {
            switch (_node)
            {
                case RecipeRoot:
                    break;
                case CuttingNode: 
                case FryingNode:
                case FurnaceNode:
                case MixingNode:
                case IngredientNode:
                    _input = InstantiatePort(Orientation.Vertical, Direction.Input, Port.Capacity.Single, typeof(bool));
                    break;
            }

            if (_input != null)
            {
                _input.portName = "";
                _input.style.flexDirection = FlexDirection.Column;
                inputContainer.Add(_input);
            }
        }

        private void CreateOutputPorts()
        {
            switch (_node)
            {
                case IngredientNode:
                    break;
                case CuttingNode:
                    _output = InstantiatePort(Orientation.Vertical, Direction.Output, Port.Capacity.Single, typeof(bool));
                    break;
                case RecipeRoot:
                case FryingNode:
                case FurnaceNode:
                case MixingNode:
                    _output = InstantiatePort(Orientation.Vertical, Direction.Output, Port.Capacity.Multi, typeof(bool));
                    break;
            }

            if (_output != null)
            {
                _output.portName = "";
                _output.style.flexDirection = FlexDirection.ColumnReverse;
                outputContainer.Add(_output);
            }
        }

        public RecipeNode GetNode() => _node;

        public override void SetPosition(Rect newPos)
        {
            base.SetPosition(newPos);
            _node.SetPosition(new Vector2(newPos.xMin, newPos.yMin));
        }

        internal Port GetOutput() => _output;
        internal Port GetInput() => _input;

        public override void OnSelected()
        {
            base.OnSelected();
            UpdateLabel();
            if (OnNodeSelected != null) OnNodeSelected.Invoke(this);
        }
    }
}