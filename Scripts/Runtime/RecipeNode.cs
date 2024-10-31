using UnityEditor;
using UnityEngine;

namespace Celeste.Tools.RecipeTree
{
    public abstract class RecipeNode : ScriptableObject
    {
        [HideInInspector] public string _guid;
        [HideInInspector] public Vector2 _position;
        [SerializeField] private FoodData _data;

        public void SetGUID(string guid) => _guid = guid;
        public string GetGUID() => _guid;

        public FoodData GetFoodData() => _data;
        public string GetFoodName()
        {
            string name = "";
            if (_data != null) name = _data.GetFoodName();
            return name;
        }

        public GameObject GetPrefab() => _data.GetPrefab();

        public GameObject GetFailedFood() => _data.GetFailedFood();

        public void SetPosition(Vector2 position)
        {
            Undo.RecordObject(this, "Behaviour Tree (Set Position)");
            _position = position;
            EditorUtility.SetDirty(this);
        }
        public Vector2 GetPosition() => _position;

        public virtual RecipeNode Clone()
        { 
            return Instantiate(this);
        }


    }
}