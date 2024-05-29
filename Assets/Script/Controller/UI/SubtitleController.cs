using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Script.Entity;
using TMPro;
using UnityEngine;

namespace Script.Controller.UI
{
    public class SubtitleController : MonoBehaviour
    {
        [Tooltip("字幕淡入淡出速度")] public float FadeSpeed = 1.0f;
        
        // textMeshPro
        private List<SubtitleEntity> _playingSubtitles = new List<SubtitleEntity>();
        private TextMeshProUGUI _subtitleText;
        private CanvasGroup _subtitleCanvasGroup;

        private int _targetVisible = 0;
        private bool _isPlaying = false;
        
        private bool IsPlaying
        {
            get => _isPlaying;
            set
            {
                if (value == _isPlaying) return;
                // 切换字幕展示
                ChangeSubtitleVisible(value);
                _isPlaying = value;
            }
        }

        private void OnEnable()
        {
            _subtitleCanvasGroup = GetComponent<CanvasGroup>();
            _subtitleText = GetComponentInChildren<TextMeshProUGUI>();
        }

        private void Update()
        {
            UpdateSubtitleAlpha(ref _targetVisible, _subtitleCanvasGroup);
        }


        private void ChangeSubtitleVisible(bool visible)
        {
            _targetVisible = visible ? 1 : -1;
        }

        /// <summary>
        /// 根据目标展示隐藏来更新字幕的透明度
        /// </summary>
        private void UpdateSubtitleAlpha(ref int targetVisible, CanvasGroup canvasGroup)
        {
            if (targetVisible < 0)
            {
                canvasGroup.alpha -= Time.deltaTime * FadeSpeed;
                if (canvasGroup.alpha <= 0)
                {
                    canvasGroup.alpha = 0;
                    targetVisible = 0;
                }
            }
            else if (targetVisible > 0)
            {
                canvasGroup.alpha += Time.deltaTime * FadeSpeed;
                if (canvasGroup.alpha >= 1)
                {
                    canvasGroup.alpha = 1;
                    targetVisible = 0;
                }
            }
        }

        
        // 更新显示的字幕文本
        private bool UpdateSubtitleText(string text)
        {
            if (_subtitleText is null)
            {
                Debug.LogError("Subtitle Text is null");
                return false;
            }

            _subtitleText.text = text;
            return true;
        }

        // 添加字幕到播放队列
        public void AddSubtitleInSequence(SubtitleEntity subtitle)
        {
            if (_playingSubtitles.Exists((x) => x.Key == subtitle.Key)) return;

            _playingSubtitles.Add(subtitle);
            if (!IsPlaying) StartPlaySubtitle();
        }

        // 开始播放字幕
        private void StartPlaySubtitle()
        {
            StartCoroutine(PlayCurrentSubtitle());
        }

        private IEnumerator PlayCurrentSubtitle()
        {
            IsPlaying = true;
            var entity = _playingSubtitles.First();
            var duration = entity.Duration;
            var elapsedTime = 0.0f;

            UpdateSubtitleText(entity.SubtitleText);
            while (elapsedTime < duration)
            {   
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            _playingSubtitles.RemoveAt(0);
            if (_playingSubtitles.Count > 0)
            {
                yield return StartCoroutine(PlayCurrentSubtitle());
            }
            else
            {
                // UpdateSubtitleText("");
                IsPlaying = false;
            }
        }
    }
}