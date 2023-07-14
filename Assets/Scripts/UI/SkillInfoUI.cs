﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillInfoUI : MonoBehaviour
{
    void Awake()
    {
        _txtSkillName = transform.Find("SkillName").GetComponent<Text>();
        _txtEnergyCost = transform.Find("EnergyCost").GetComponent<Text>();
        _mainDescription = transform.Find("MainDescription").GetComponent<Text>();
        _subDescription = transform.Find("SubDescription").GetComponent<Text>();
        _otherDescription = transform.Find("OtherDescription").GetComponent<Text>();


        _txtHotKey = transform.Find("Skill/Icon/HotKey").GetComponent<Text>();
        _imgSkillIcon = transform.Find("Skill/Icon").GetComponent<Image>();

        _txtDistance = transform.Find("InfoSlots/Distance/Info/Text").GetComponent<Text>();
        _txtRange = transform.Find("InfoSlots/Range/Info/Text").GetComponent<Text>();
        _txtCoolDown = transform.Find("InfoSlots/CoolDown/Info/Text").GetComponent<Text>();


    }

    public void UpdateUI(SkillSlot skillSlot)
    {
        SkillInstance skill = skillSlot._skill;
        if (skill == null) return;

        int energyCost = skill.SkillInfo._energyCost;
        int energyRecover = skill.SkillInfo._energyRecover;
        string name = skill.SkillInfo._name;
        float rate = skill.SkillInfo._rate; // 伤害倍率
        float coolDownTime = skill.SkillInfo._cooldownTime; // 冷却时间
        int lowDamage;
        int highDamage;
        SkillDamageCalculator.GetDamageRange(skill, out lowDamage, out highDamage);

        _txtSkillName.text = name;
        if(energyRecover > 0)
        {
            _txtEnergyCost.text = $"内力恢复{energyRecover}";
        }
        else if(energyCost > 0)
        {
            _txtEnergyCost.text = $"内力消耗{energyCost}";
        }
        else
        {
            _txtEnergyCost.text = $"";
        }

        if (rate != 0)
        {
            _mainDescription.text = $"{lowDamage}-{highDamage}伤害";
        }
        else if(skill.SkillInfo.HasComponent<IBuffAdditionEffect>() && skill.SkillInfo.GetComponent<IBuffAdditionEffect>().BuffType == BuffType.ShadowClone)
        {
            _mainDescription.text = $"在{skill.SkillInfo._castTime}秒内反击";
        }
        else if (skill.SkillInfo.HasComponent<StatusRemovalEffect>())
        {
            var statusRemove = skill.SkillInfo.GetComponent<StatusRemovalEffect>();
            _mainDescription.text = $"脱离";
            for (int i = 0; i < statusRemove.RemovalStatus.Count; ++i)
            {
                CharacterStatusType status = statusRemove.RemovalStatus[i];
                if (i != 0) _mainDescription.text += "、";
                _mainDescription.text += status.GetDescription();
            }
            
            _mainDescription.text += $"状态";
        }
        else if (skill.SkillInfo.HasComponent<FixedDirectionMovement>())
        {
            FixedDirectionMovement fixedDirectionMovement = skill.SkillInfo.GetComponent<FixedDirectionMovement>();
            
            _mainDescription.text = fixedDirectionMovement.MovementDirection.GetDescription() + fixedDirectionMovement.MovementDistance.ToString("f0") + "米";
        }
        else if (skill.SkillInfo.HasComponent<RushToTargetMovement>())
        {
            _mainDescription.text = $"向敌人突进";
        }
        else if (skill.SkillInfo.HasComponent<RushToBackTargetMovement>())
        {
            _mainDescription.text = $"移动到敌人后方";
        }
        else
        {
            _mainDescription.text = $"无效果";
        }

        if(skill.SkillInfo.HasComponent<StatusAdditionEffect>())
        {
            var addStatus = skill.SkillInfo.GetComponent<StatusAdditionEffect>();
            _subDescription.text = $"命中后 {addStatus.AddStatus.GetDescription()}{addStatus.StatusTime}秒";
        }
        else
        {
            _subDescription.text = $"";
        }

        _otherDescription.text = $"";

        if(skill.SkillInfo.HasComponent<AddBuffDurationEffect>() && skill.SkillInfo.GetComponent<AddBuffDurationEffect>().BuffType == BuffType.ImmunityAll)
        {
            AddBuffDurationEffect addBuffDuration = skill.SkillInfo.GetComponent<AddBuffDurationEffect>();
            _otherDescription.text += $"施展状态下抵抗伤害和异常状态\n";
            if (addBuffDuration._duration != -1)
            {
                _otherDescription.text += $"施展后在{addBuffDuration._duration}秒内抵抗伤害和异常状态\n";
            }
        }
        
        _txtHotKey.text = CommonUtility.GetHotKeyString(skillSlot._hotKey);
        _imgSkillIcon.sprite = skillSlot._imgIcon.sprite;


        if(skill.SkillInfo.HasComponent<TargetRequired>())
        {
            var targetRequired = skill.SkillInfo.GetComponent<TargetRequired>();
            _txtDistance.text = $"{targetRequired.RequiredTargetDistance}m";
        }
        else
        {
            _txtDistance.text = $"原地";
        }

        if (skill.SkillInfo.HasComponent<IHitCheckStrategy>() 
            && skill.SkillInfo.GetComponent<IHitCheckStrategy>().IsAOESkill()
            && skill.SkillInfo.HasComponent<FixedDirectionMovement>()
            && skill.SkillInfo.GetComponent<FixedDirectionMovement>().MovementDirection == MovementDirection.Forward)
        {
            var movement = skill.SkillInfo.GetComponent<FixedDirectionMovement>();
            _txtRange.text = $"前方{movement.MovementDistance}m";
        }
        else if (skill.SkillInfo.HasComponent<IHitCheckStrategy>()
            && skill.SkillInfo.GetComponent<IHitCheckStrategy>().IsAOESkill()
            && skill.SkillInfo.HasComponent<StatusRemovalEffect>()
          )
        {
            var rangeHitCheck = skill.SkillInfo.GetComponent<RangeHitCheckStrategy>();
            _txtRange.text = $"半径{rangeHitCheck.Range}m";
        }
        else
        {
            _txtRange.text = $"单一目标";
        }
        _txtCoolDown.text = $"{coolDownTime}秒";
    }

    Text _txtSkillName;
    Text _txtEnergyCost;
    Text _mainDescription;
    Text _subDescription;
    Text _otherDescription;

    Text _txtHotKey;
    Image _imgSkillIcon;

    Text _txtDistance;
    Text _txtRange;
    Text _txtCoolDown;
}
