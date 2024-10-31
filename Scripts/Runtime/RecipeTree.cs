using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System;
using UnityEditor.Experimental.GraphView;
using Unity.VisualScripting;
using static Unity.VisualScripting.Metadata;
using UnityEditor.IMGUI.Controls;

namespace Celeste.Tools.RecipeTree
{
    [CreateAssetMenu(menuName = "Tree/RecipeTree/RecipeTree")]
    public class RecipeTree : ScriptableObject
    {
        [SerializeField] private RecipeNode _rootNode;
        [SerializeField] private List<RecipeNode> _nodes = new List<RecipeNode>();

        public List<Image> GetRecipe()
        {
            List <Image> recipe = new List<Image>();
            RecipeRoot root = _rootNode as RecipeRoot;
            
            if (root != null) recipe = root.GetRecipe(); 

            return recipe;
        } 

        public RecipeNode GetRootNode() => _rootNode;
        public RecipeNode SetRootNode(RecipeNode node) => _rootNode = node;
        public List<RecipeNode> GetNodes() => _nodes;

        public RecipeNode CreateNode(System.Type type, Vector2 position)
        {
            if (type == typeof(RecipeRoot))
            {
                if (_rootNode != null) throw new InvalidOperationException("There already exists a Root Node.");
            }

            RecipeNode node = ScriptableObject.CreateInstance(type) as RecipeNode;

            node.name = type.Name;
            node.SetGUID(GUID.Generate().ToString());
            node._position = position;

            Undo.RecordObject(node, "Recipe Tree (CreateNode)");
            _nodes.Add(node);

            if (!Application.isPlaying)
                AssetDatabase.AddObjectToAsset(node, this);
            Undo.RegisterCreatedObjectUndo(node, "Recipe Tree (CreateNode)");
            AssetDatabase.SaveAssets();
            return node;
        }

        public void DeleteNode(RecipeNode node)
        {
            if (node.GetType() == typeof(RecipeRoot))
            {
                throw new InvalidOperationException("Root Node cannot be deleted.");
            }

            Undo.RecordObject(this, "Recipe Tree (DeleteNode)");
            _nodes.Remove(node);

            Undo.DestroyObjectImmediate(node);
            AssetDatabase.SaveAssets();
        }

        public void AddChild(RecipeNode parent, RecipeNode child)
        {
            RecipeRoot rootNode = parent as RecipeRoot;
            if (rootNode != null)
            {
                Undo.RecordObject(rootNode, "Recipe Tree (AddChild)");
                rootNode._children.Add(child);
                EditorUtility.SetDirty(rootNode);
            }

            CuttingNode cutting = parent as CuttingNode;
            if (cutting != null)
            {
                Undo.RecordObject(cutting, "Recipe Tree(AddChild)");
                cutting._child = child;
                EditorUtility.SetDirty(cutting);
            }

            FryingNode frying = parent as FryingNode;
            if (frying != null)
            {
                Undo.RecordObject(frying, "Recipe Tree (AddChild)");
                frying._children.Add(child);
                EditorUtility.SetDirty(frying);
            }

            FurnaceNode furnace = parent as FurnaceNode;
            if (furnace != null)
            {
                Undo.RecordObject(furnace, "Recipe Tree (AddChild)");
                furnace._children.Add(child);
                EditorUtility.SetDirty(furnace);
            }

            MixingNode mixing = parent as MixingNode;
            if (mixing != null)
            {
                Undo.RecordObject(mixing, "Recipe Tree (AddChild)");
                mixing._children.Add(child);
                EditorUtility.SetDirty(mixing);
            }
        }

        public void RemoveChild(RecipeNode parent, RecipeNode child)
        {
            RecipeRoot rootNode = parent as RecipeRoot;
            if (rootNode != null)
            {
                Undo.RecordObject(rootNode, "Recipe Tree (RemoveChild)");
                rootNode._children.Remove(child);
                EditorUtility.SetDirty(rootNode);
            }

            CuttingNode cutting = parent as CuttingNode;
            if (cutting != null)
            {
                Undo.RecordObject(cutting, "Recipe Tree(RemoveChild)");
                cutting._child = null;
                EditorUtility.SetDirty(cutting);
            }

            FryingNode frying = parent as FryingNode;
            if (frying != null)
            {
                Undo.RecordObject(frying, "Recipe Tree (RemoveChild)");
                frying._children.Remove(child);
                EditorUtility.SetDirty(frying);
            }

            FurnaceNode furnace = parent as FurnaceNode;
            if (furnace != null)
            {
                Undo.RecordObject(furnace, "Recipe Tree (RemoveChild)");
                furnace._children.Remove(child);
                EditorUtility.SetDirty(furnace);
            }

            MixingNode mixing = parent as MixingNode;
            if (mixing != null)
            {
                Undo.RecordObject(mixing, "Recipe Tree (RemoveChild)");
                mixing._children.Remove(child);
                EditorUtility.SetDirty(mixing);
            }
        }

        public static List<RecipeNode> GetChildren(RecipeNode parent)
        {
            List<RecipeNode> result = new List<RecipeNode>();

            RecipeRoot rootNode = parent as RecipeRoot;
            if (rootNode != null) result = rootNode._children;

            CuttingNode cutting = parent as CuttingNode;
            if (cutting != null) result.Add(cutting._child);

            FryingNode frying = parent as FryingNode;
            if (frying != null) result = frying._children;

            FurnaceNode furnace = parent as FurnaceNode;
            if (furnace != null) result = furnace._children;

            MixingNode mixing = parent as MixingNode;
            if (mixing != null) result = mixing._children;

            return result;
        }

        public void CreateRoot()
        {
            if (_rootNode == null)
            {
                RecipeRoot root = CreateNode(typeof(RecipeRoot), Vector2.zero) as RecipeRoot;
                _rootNode = root;
            }
        }

        public static List<RecipeNode> GetNodes(RecipeNode node,Type type)
        {
            List<RecipeNode> nodes = new List<RecipeNode>();
            Traverse(node, (n) =>
            {
                if (n.GetType() == type)
                    nodes.Add(n);
            });
            return nodes;
        }

        private static void Traverse(RecipeNode node, Action<RecipeNode> func)
        {
            if (node)
            { 
                List<RecipeNode> children = GetChildren(node);
                children.ForEach((n) => Traverse(n, func));
                func.Invoke(node);
            }
        }

        public RecipeTree Clone() 
        {
            RecipeTree tree = Instantiate(this);
            tree._rootNode = _rootNode.Clone();
            tree._nodes = new List<RecipeNode>();
            Traverse(tree._rootNode,(n) =>
            {
                tree._nodes.Add(n);
            });
            return tree;
        }
    }
}