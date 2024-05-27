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
        public TextMeshProUGUI subtitleText;
        private List<SubtitleEntity> _playingSubtitles = new List<SubtitleEntity>();

        private bool _isPlaying = false;

        // private int _currentSubtitleIndex = -1;
        public bool UpdateSubtitleText(string text)
        {
            if (subtitleText == null)
            {
                Debug.LogError("Subtitle Text is null");
                return false;
            }

            subtitleText.text = text;
            return true;
        }

        public void AddSubtitleInSequence(SubtitleEntity subtitle)
        {
            if (_playingSubtitles.Exists((x) => x.Key == subtitle.Key)) return;

            _playingSubtitles.Add(subtitle);
            if (!_isPlaying) StartPlaySubtitle();
        }

        public void StartPlaySubtitle()
        {
            StartCoroutine(PlayCurrentSubtitle());
        }

        private IEnumerator PlayCurrentSubtitle()
        {
            _isPlaying = true;
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
                _isPlaying = false;
            }
        }


        private void Update()
        {
        }
    }
}