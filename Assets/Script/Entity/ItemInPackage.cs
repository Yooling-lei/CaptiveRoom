using System;
using UnityEngine;

namespace Script.Entity
{
    public class ItemInPackage
    {
        public string ItemName { get; set; }

        public int Count { get; set; }

        // 关联的GameObject
        public GameObject LinkGameObject { get; set; }
    
        // 关联的展示图片
        public Sprite LinkSprite { get; set; }


        // TODO: 实现捡东西
    }
}