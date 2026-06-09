using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Jobworld
{
    public class TutorialSelectionElement : MonoBehaviour, ITutorialSelectionModel
    {
        [Header("UI")]
        [SerializeField] private Image bannerImage;
        [SerializeField] private TMP_Text jobNameText;
        [SerializeField] private TMP_Text descriptionText;
        [SerializeField] private Button startButton;
        
        [SerializeField] private JobSelectionPanelInfo _info;
        public JobSelectionPanelInfo info => _info;
        public event Action<string> OnTutorialStart;

        private void Awake()
        {
            ApplyInfo();
            startButton.onClick.RemoveAllListeners();
            startButton.onClick.AddListener(OnStartClicked);
        }

        private void ApplyInfo()
        {
            if (_info == null) return;
            if (bannerImage != null && _info.banner != null)
                bannerImage.sprite = _info.banner;
            if (jobNameText != null)
                jobNameText.text = _info.jobName;
            if (descriptionText != null)
                descriptionText.text = _info.description;
        }

        public void StartTutorial(string jobName)
        {
            OnTutorialStart?.Invoke(jobName);
            if (_info != null && !string.IsNullOrEmpty(_info.tutorialSceneName))
            {
                SafeLoading.LoadSceneWithLoading(_info.tutorialSceneName);
            }
        }

        private void OnStartClicked()
        {
            StartTutorial(_info.jobName);
        }
    }
}

