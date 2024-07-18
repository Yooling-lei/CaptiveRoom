using System.Collections.Generic;
using Script.Entity;

namespace Script.Interface
{
    public interface IMergeableItem
    {
        public bool MergeCheck(List<ItemInPackage> items);
        
        public void OnMergeCheckFailed();
        
        public void OnMergeSuccess();
    }
}