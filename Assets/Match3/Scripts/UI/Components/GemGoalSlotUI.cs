using System.Collections.Generic;
using Match3.Scripts.Core;
using Match3.Scripts.Enums;
using Match3.Scripts.Systems.Level.Base;
using Match3.Scripts.Systems.Level.Goals;
using Match3.Scripts.UI.Base;
using TMPro;
using UnityCoreModules.Services;
using UnityEngine;
using UnityEngine.UI;

namespace Match3.Scripts.UI.Components
{
    public class GemGoalSlotUI : GoalSlotUIBase
    {
        [SerializeField] private TextMeshProUGUI _targetCountText;
        [SerializeField] private Image _icon;
        private int _currentCount;
        private GemType _gemType;

        protected internal override void Set(LevelGoalBase goal)
        {
            if (goal is not GemMatchGoal gemMatchGoal)
                return;
            Debug.Log($"Goal is set to {gemMatchGoal.TargetCount} for {gemMatchGoal.GoalGemType.ToString()}");
            _icon.sprite = ServiceLocator.Get<GemSpriteProvider>().GetSprite(gemMatchGoal.GoalGemType);
            _currentCount = gemMatchGoal.TargetCount;
            _gemType = gemMatchGoal.GoalGemType;
            _targetCountText.text = _currentCount.ToString();
        }

        protected internal override void UpdateCount(GemType gemType, int collectedCount)
        {
            if (_gemType != gemType)
                return;
            if (_currentCount <= 0)
                return;

            _currentCount -= collectedCount;
            if (_currentCount < 0)
                _currentCount = 0;
            _targetCountText.text = _currentCount.ToString();
        }
    }
}