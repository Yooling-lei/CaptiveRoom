using System.Collections.Generic;
using Script.ScriptableObjects;

namespace Script.Manager
{
    public class ClueArchiveManager : Singleton<ClueArchiveManager>
    {
        public List<ClueScriptable> clueEntities = new List<ClueScriptable>();

        public void RecordClue(ClueScriptable clueEntity)
        {
            clueEntities.Add(clueEntity);
        }
        
        // TODO: 如何使用UI展示这个页面
        
        
    }
}