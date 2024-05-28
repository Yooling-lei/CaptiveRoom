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

        private void Update()
        {
            if (_targetVisible < 0)
            {
                _subtitleCanvasGroup.alpha -= Time.deltaTime;
                if (_subtitleCanvasGroup.alpha <= 0)
                {
                    _subtitleCanvasGroup.alpha = 0;
                    _targetVisible = 0;
                }
            }
            else if (_targetVisible > 0)
            {
                _subtitleCanvasGroup.alpha += Time.deltaTime;
                if (_subtitleCanvasGroup.alpha >= 1)
                {
                    _subtitleCanvasGroup.alpha = 1;
                    _targetVisible = 0;
                }
            }
        }

        private void OnEnable()
        {
            _subtitleCanvasGroup = GetComponent<CanvasGroup>();
            _subtitleText = GetComponentInChildren<TextMeshProUGUI>();
        }

        private void ChangeSubtitleVisible(bool visible)
        {
            _targetVisible = visible ? 1 : -1;
        }


        // private int _currentSubtitleIndex = -1;
        public bool UpdateSubtitleText(string text)
        {
            if (_subtitleText is null)
            {
                Debug.LogError("Subtitle Text is null");
                return false;
            }

            _subtitleText.text = text;
            return true;
        }

        public void AddSubtitleInSequence(SubtitleEntity subtitle)
        {
            if (_playingSubtitles.Exists((x) => x.Key == subtitle.Key)) return;

            _playingSubtitles.Add(subtitle);
            if (!IsPlaying) StartPlaySubtitle();
        }

        public void StartPlaySubtitle()
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
                UpdateSubtitleText("");
                IsPlaying = false;
            }
        }
    }
}