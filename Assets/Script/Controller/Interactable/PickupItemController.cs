using System.Collections;
using Script.Controller.Common;
using Script.Manager;
using UnityEngine;
using UnityEngine.Serialization;

namespace Script.Controller.Interactable
{
    public enum EItemStatus
    {
        Normal,
        PickedUp,
    }

    /// <summary>
    /// 控制游戏中可交互的物体
    /// </summary>
    public class PickupItemController : BaseInteractableController
    {
        public string itemName;
        public float scaleInBag = 1f;
        public EItemStatus itemStatus = EItemStatus.Normal;

        private Collider _collider;
        protected override void Awake()
        {
            base.Awake();
            itemStatus = EItemStatus.Normal;
            _collider = GetComponent<Collider>();
        }

        public override void OnInteract()
        {
            if (itemStatus != EItemStatus.Normal) return;

            // base.OnInteract();
            Debug.Log("捡起一个物体");
            itemStatus = EItemStatus.PickedUp;

            // 让物体飞向玩家
            if (_collider is not null) _collider.enabled = false;
            StartCoroutine(PickUpItem());
        }

        private IEnumerator PickUpItem()
        {
            yield return StartCoroutine(FlyToPlayer());
            yield return StartCoroutine(AddItemToPackage());
        }

        private IEnumerator FlyToPlayer()
        {
            var time = 0.5f;
            var elapsedTime = 0.0f;
            var startPos = transform.position;
            var targetPos = GameManager.Instance.player.transform.position;
            while (elapsedTime < time)
            {
                elapsedTime += Time.deltaTime;
                transform.position = Vector3.Lerp(startPos, targetPos, elapsedTime / time);
                yield return null;
            }
        }

        private IEnumerator AddItemToPackage()
        {
            GameManager.Instance.AddItemToPackage(itemName, this);
            Destroy(gameObject);
            yield return null;
        }
    }
}