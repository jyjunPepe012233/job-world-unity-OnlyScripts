using System;
using UnityEngine;
using UnityEngine.UI;


namespace Jobworld
{
    public class TutorialSelectionMenuView : MonoBehaviour
    {
        [SerializeField] private Transform elementContainer;
        [SerializeField] private Button backButton;

        public event Action backButtonClicked;

        void Awake()
        {
            if (backButton != null)
            {
                backButton.onClick.RemoveAllListeners();
                backButton.onClick.AddListener(() => backButtonClicked?.Invoke());
            }
        }

        public Transform ElementContainer => elementContainer;
    }
}

