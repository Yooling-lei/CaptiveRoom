using Script.Interface;
using UnityEngine;

namespace Script.Controller.Interactable.Items
{
    public class TestCubeController : MonoBehaviour, IUsableItem
    {
        public void OnItemUse()
        {
            Debug.Log("Use TestCube");
        }
    }
}