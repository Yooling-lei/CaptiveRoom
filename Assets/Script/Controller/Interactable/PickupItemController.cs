using System;
using System.Collections;
using System.ComponentModel;
using Script.Manager;
using UnityEngine;

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
        public float flyTime = 1f;

        [Description("若物体曾经被捡起,则不创建")] public bool destroyIfPicked;

        private Collider _collider;

        protected override void Awake()
        {
            base.Awake();
            itemStatus = EItemStatus.Normal;
            _collider = GetComponent<Collider>();
        }

        protected virtual void Start()
        {
            if (destroyIfPicked && BagManager.Instance.HasItemPicked(itemName))
            {
                Destroy(gameObject);
            }
        }

        public override void OnInteract()
        {
            if (itemStatus != EItemStatus.Normal) return;

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
            var elapsedTime = 0.0f;
            var startPos = transform.position;
            var targetPos = GameManager.Instance.player.transform.position;
            while (elapsedTime < flyTime)
            {
                elapsedTime += Time.deltaTime;
                transform.position = Vector3.Lerp(startPos, targetPos, elapsedTime / flyTime);
                yield return null;
            }
        }

        private IEnumerator AddItemToPackage()
        {
            BagManager.Instance.OnItemPickup(itemName, this);
            yield return null;
            Destroy(gameObject);
            yield return null;
        }
    }
}