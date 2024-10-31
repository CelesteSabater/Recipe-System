using System;
using UnityEditor;
using UnityEngine;

namespace Celeste.Tools.RecipeTree
{
    [CreateAssetMenu(menuName = "Tree/RecipeTree/FoodData")]
    public class FoodData: ScriptableObject
    {
        [SerializeField] private string _foodName;
        [SerializeField] private GameObject _prefab;
        [SerializeField] private GameObject _failedFood;

        public string GetFoodName() => _foodName;

        public GameObject GetPrefab() => _prefab;

        public GameObject GetFailedFood() => _failedFood;
    }
}