using UnityEngine;

namespace Jobworld
{
    public class TutorialSelectionMenuPresenter : MonoBehaviour
    {
        [SerializeField] private MainMenuPageNavigator pageNavigator;
        [SerializeField] private TutorialSelectionMenuView view;

        void OnEnable()
        {
            view.backButtonClicked += OnBackButtonClicked;
        }

        void OnDisable()
        {
            view.backButtonClicked -= OnBackButtonClicked;
        }

        private void OnBackButtonClicked()
        {
            pageNavigator.OpenJoinMenu();
        }
    }
}