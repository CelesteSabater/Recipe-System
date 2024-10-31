using System.Collections.Generic;
using UnityEngine.UIElements;
using UnityEditor.Experimental.GraphView;
using UnityEditor;
using System;
using System.Linq;
using System.Numerics;
using UnityEngine;

namespace Celeste.Tools.RecipeTree
{
    public class RecipeTreeView : GraphView
    {
        public Action<RecipeNodeView> OnNodeSelected;
        public new class UxmlFactory : UxmlFactory<RecipeTreeView, GraphView.UxmlTraits> { }

        private UnityEngine.Vector2 worldMousePosition = UnityEngine.Vector2.zero;

        RecipeTree _tree;
        public RecipeTreeView() 
        {
            Insert(0, new GridBackground());

            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new ContentZoomer());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());

            StyleSheet styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Packages/Celeste/Tools/RecipeTree/UIBuilder/RecipeTreeEditor.uss");
            styleSheets.Add(styleSheet);

            focusable = true;
            Undo.undoRedoPerformed += OnUndoRedo;
        }

        public void UpdateNodeStates()
        {
            if (_tree != null) 
                _tree.GetNodes().ForEach(n => 
                {
                    RecipeNodeView node = FindNodeView(n);
                    node.UpdateLabel();
                });
        }

        private void OnUndoRedo()
        {
            PopulateView(_tree);
            AssetDatabase.SaveAssets();
        }

        private RecipeNodeView FindNodeView(RecipeNode node)
        {
            return GetNodeByGuid(node.GetGUID()) as RecipeNodeView;
        }

        internal void PopulateView(RecipeTree tree)
        {
            _tree = tree;

            if (_tree != null)
            {
                graphViewChanged -= OnGraphViewChanged;
                DeleteElements(graphElements);
                graphViewChanged += OnGraphViewChanged;

                if (_tree.GetRootNode() == null)
                {
                    _tree.CreateRoot();
                    EditorUtility.SetDirty(_tree);
                    AssetDatabase.SaveAssets();
                }

                foreach (RecipeNode node in _tree.GetNodes()) 
                    CreateNodeView(node);

                foreach (RecipeNode parent in _tree.GetNodes())
                {
                    List<RecipeNode> children = RecipeTree.GetChildren(parent);
                    foreach (RecipeNode child in children)
                    {
                        if (child != null)
                        {
                            RecipeNodeView parentView = FindNodeView(parent);
                            RecipeNodeView childView = FindNodeView(child);

                            Edge edge = parentView.GetOutput().ConnectTo(childView.GetInput());
                            AddElement(edge);
                        }
                    }
                }
            }
        }

        private GraphViewChange OnGraphViewChanged(GraphViewChange graphViewChange)
        {
            if (graphViewChange.elementsToRemove != null)
            {
                graphViewChange.elementsToRemove.ForEach(elem =>
                {
                    RecipeNodeView nodeView = elem as RecipeNodeView;
                    if (nodeView != null) _tree.DeleteNode(nodeView.GetNode());

                    Edge edge = elem as Edge;
                    if (edge != null)
                    {
                        RecipeNodeView parentView = edge.output.node as RecipeNodeView;
                        RecipeNodeView childrenView = edge.input.node as RecipeNodeView;
                        _tree.RemoveChild(parentView.GetNode(), childrenView.GetNode());
                    }
                });
            }

            if (graphViewChange.edgesToCreate != null)
            {
                graphViewChange.edgesToCreate.ForEach(edge =>
                {
                    RecipeNodeView parentView = edge.output.node as RecipeNodeView;
                    RecipeNodeView childrenView = edge.input.node as RecipeNodeView;
                    _tree.AddChild(parentView.GetNode(), childrenView.GetNode());
                });
            }
            
            return graphViewChange;
        }

        private void CreateNodeView(RecipeNode node)
        {
            RecipeNodeView nodeView = new RecipeNodeView(node);
            nodeView.OnNodeSelected = OnNodeSelected;
            AddElement(nodeView);
        }

        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            return ports.ToList().Where(endPort => 
                endPort.direction != startPort.direction && 
                endPort.node != startPort.node
            ).ToList();
        }

        private void CreateNode(Type type)
        {
            RecipeNode node = _tree.CreateNode(type, worldMousePosition);
            CreateNodeView(node);
        }

        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            //base.BuildContextualMenu(evt);

            VisualElement contentViewContainer = ElementAt(1);
            UnityEngine.Vector3 screenMousePosition = evt.localMousePosition;
            worldMousePosition = screenMousePosition - contentViewContainer.transform.position;
            worldMousePosition *= 1 / contentViewContainer.transform.scale.x;
            
            var types = TypeCache.GetTypesDerivedFrom<RecipeNode>();
            foreach (var type in types) evt.menu.AppendAction($"{type.Name}", (a) => CreateNode(type));
        }
    }
}