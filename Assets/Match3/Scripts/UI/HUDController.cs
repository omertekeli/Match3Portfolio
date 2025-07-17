using System.Collections.Generic;
using System.Threading.Tasks;
using Match3.Scripts.Enums;
using Match3.Scripts.LevelSystem.Data;
using Match3.Scripts.LevelSystem.Goals;
using UnityEngine;

namespace Match3.Scripts.UI
{
    public class HUDController: MonoBehaviour
    {
        #region Fields
        
        [SerializeField] private GameObject _hud;
        [SerializeField] private GoalPanelUI _goalPanel;
        //[SerializeField] private MoveCounterUI _moveCounter;
        //[SerializeField] private ScoreUI _scoreUI;

        //[SerializeField] private TextMeshProUGUI _remainingMovesText;
        //[SerializeField] private TextMeshProUGUI _scoreText;
        //[SerializeField] private Slider _progressBar;
        
        #endregion

        public void ToggleHUD(GameState state)
        {
            bool shouldShow = state != GameState.MainMenu && state != GameState.Loading;
            Debug.Log($"ShouldShow HUD: {shouldShow}");
            _hud.SetActive(shouldShow);
        }
        public void SetupUI(LevelDataSO leveldata, List<LevelGoalBase> goals)
        {
            _goalPanel.SetGoals(goals);
        }
        
    }
}