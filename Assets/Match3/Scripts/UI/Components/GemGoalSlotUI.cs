using Match3.Scripts.Core;
using Match3.Scripts.LevelSystem.Goals;
using Match3.Scripts.UI.Base;
using TMPro;
using UnityCoreModules.Services;
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
            if (goal is not GemMatchGoal gemMatchGoal) return;
            Debug.Log($"Goal is set to {gemMatchGoal.TargetCount} for {gemMatchGoal.GoalGemType.ToString()}");
            _icon.sprite = ServiceLocator.Get<GemSpriteProvider>().GetSprite(gemMatchGoal.GoalGemType);
            _targetCountText.text = gemMatchGoal.TargetCount.ToString();
        }
    }
}