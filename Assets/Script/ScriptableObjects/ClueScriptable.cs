using UnityEngine;

namespace Script.ScriptableObjects
{
    [CreateAssetMenu(fileName = "NewClue", menuName = "ScriptableObjects/ClueScriptableObject", order = 1)]
    public class ClueScriptable : ScriptableObject
    {
        public string title;

        public string detail;

    }
}