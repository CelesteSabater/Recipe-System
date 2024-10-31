using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using UnityEditor;

namespace Celeste.Tools.RecipeTree
{
    public class RecipeInspectorView : VisualElement
    {
        public new class UxmlFactory : UxmlFactory<RecipeInspectorView, VisualElement.UxmlTraits> { }

        Editor editor;

        public RecipeInspectorView() { }

        public void UpdateSelection(RecipeNodeView nodeView) 
        {
            Clear();

            UnityEngine.Object.DestroyImmediate(editor);

            editor = Editor.CreateEditor(nodeView.GetNode());
            IMGUIContainer container = new IMGUIContainer(() => { 
                if (editor.target) editor.OnInspectorGUI(); 
            });
            Add(container);
        }
    }
}