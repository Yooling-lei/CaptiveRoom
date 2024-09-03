using System;
using Script.Manager;
using Script.ScriptableObjects;
using UnityEngine;

namespace Script.Controller.Interactable.Items
{
    public class TestClueController : BaseInteractableController
    {
        public ClueScriptable clueScriptable;

        // private void Start()
        // {
        //     Debug.Log("clue...........");
        //     Debug.Log(clueScriptable.title);
        // }


        public override void OnInteract()
        {
            Debug.Log("收集线索............");
            Debug.Log(clueScriptable.title);

            ClueArchiveManager.Instance.RecordClue(clueScriptable);
        }
    }
}