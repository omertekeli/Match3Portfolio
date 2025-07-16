using Match3.Scripts.LevelSystem.Goals;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Match3.Scripts.UI
{
    public class GemGoalSlotUI: GoalSlotUIBase
    {
        [SerializeField] private TextMeshProUGUI _targetCountText;
        [SerializeField] private Image _icon;
        
        protected internal override void Set(LevelGoalBase goal)
        {
            if (goal is GemMatchGoal gemMatchGoal)
            {
                //_icon.sprite = GemDataBase.GetSprite(gemMatchGoal.GoalGemType);
                _targetCountText.text = gemMatchGoal.TargetCount.ToString();
            }
        }
    }
}