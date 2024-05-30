using System;
using UnityEngine;

namespace Script.Controller.Task._0_Opening
{
    public class CupParentController : MonoBehaviour
    {
        public EatPillCupController eatPillCupController;

        private void OnCollisionEnter(Collision other)
        {
            eatPillCupController.OnParentCollisionEnter(other);
        }
    }
}