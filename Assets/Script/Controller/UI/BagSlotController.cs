using System;
using UnityEngine;
using UnityEngine.UI;

namespace Script.Controller.UI
{
    public class BagSlotController : MonoBehaviour
    {
        public Color selectColor;
        public Color idleColor;
        [HideInInspector] public Button slotButton;
        [HideInInspector] private Image slotImage;

        private void Awake()
        {
            slotButton = GetComponent<Button>();
            slotImage = GetComponent<Image>();
        }

        public void ChangeColor(bool select)
        {
            slotImage.color = select ? selectColor : idleColor;
        }
    }
}