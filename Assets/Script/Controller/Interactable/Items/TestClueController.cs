using System;
using Script.Manager;
using Script.ScriptableObjects;
using UnityEngine;
using SceneManager = UnityEngine.SceneManagement.SceneManager;

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
            
            // 去场景2
            SceneManager.LoadScene("SampleScene");

            // ClueArchiveManager.Instance.RecordClue(clueScriptable);
        }
    }
}