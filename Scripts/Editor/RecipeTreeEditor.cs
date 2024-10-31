using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.Callbacks;
using System;

namespace Celeste.Tools.RecipeTree
{
    public class RecipeTreeEditor : EditorWindow
    {
        RecipeTreeView _treeView;
        RecipeInspectorView _inspectorView;

        [MenuItem("Tools/Celeste/Recipe Tree Editor")]
        public static void OpenWindow()
        {
            RecipeTreeEditor wnd = GetWindow<RecipeTreeEditor>();
            wnd.titleContent = new GUIContent("RecipeTreeEditor");
        }

        [OnOpenAsset]
        public static bool OnOpenAsset(int instanceId, int line)
        {
            if (Selection.activeObject is RecipeTree)
            {
                OpenWindow();
                return true;
            }
            return false;
        }

        private void OnEnable()
        {
            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        }

        private void OnDisable() 
        {
            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
        }

        private void OnInspectorUpdate()
        {
            if (_treeView != null)
                _treeView?.UpdateNodeStates();
        }

        private void OnPlayModeStateChanged(PlayModeStateChange obj)
        {
            switch (obj)
            {
                case PlayModeStateChange.EnteredEditMode:
                    OnSelectionChange();
                    break;
                case PlayModeStateChange.ExitingEditMode:
                    break;
                case PlayModeStateChange.EnteredPlayMode:
                    OnSelectionChange();
                    break;
                case PlayModeStateChange.ExitingPlayMode:
                    break;
            }
        }

        public void CreateGUI()
        {
            // Each editor window contains a root VisualElement object
            VisualElement root = rootVisualElement;

            // Import UXML
            VisualTreeAsset visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Packages/Celeste/Tools/RecipeTree/UIBuilder/RecipeTreeEditor.uxml");
            visualTree.CloneTree(root);

            // StyleSheet
            StyleSheet styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Packages/Celeste/Tools/RecipeTree/UIBuilder/RecipeTreeEditor.uss");
            root.styleSheets.Add(styleSheet);

            _treeView = root.Q<RecipeTreeView>();
            _inspectorView = root.Q<RecipeInspectorView>();

            _treeView.OnNodeSelected = OnNodeSelectionChanged;
            OnSelectionChange();
        }

        private void OnSelectionChange()
        {
            RecipeTree tree = Selection.activeObject as RecipeTree;

            if (tree && _treeView != null)
            {
                if (Application.isPlaying)
                {
                    if (_treeView != null && tree)
                        _treeView.PopulateView(tree);
                }
                else if (AssetDatabase.CanOpenAssetInEditor(tree.GetInstanceID()))
                    _treeView.PopulateView(tree);
            }
        }

        private void OnNodeSelectionChanged(RecipeNodeView nodeView)
        {
            _inspectorView.UpdateSelection(nodeView);
        }
    }
}