﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YFrameWork;

public class InputController : MonoSingleton<InputController>
{
    void Awake()
    {
        // 初始化技能栏
        _skillBar1.AddSkill(KeyCode.J, new Skill("普攻", "attack", 0, 1, 1, 0.5f, 0.33f, 0.1f, 0, new DoNotRemoveStatuses(), new DoNotAddStatusEffect(), new NoTargetRequired(), new NoMovement(), new FaceTargetHitCheck(2.5f)));
        _skillBar1.AddSkill(KeyCode.K, new Skill("刺心", "cixin", 3, 0, 12,  1f, 0.83f, 0.5f, 0, new DoNotRemoveStatuses(), new AddStatusEffect(CharacterStatusType.Weakness, 2f), new TargetRequired(3f), new NoMovement(), new FaceTargetHitCheck(3f)));
        _skillBar1.AddSkill(KeyCode.S, new Skill("逆风行", "nifengxing", 0, 0, 0,  8f, 0.43f, 0f, 0, new DoNotRemoveStatuses(), new DoNotAddStatusEffect(), new NoTargetRequired(), new FixedDirectionMovement(MovementDirection.Backward, 6), new FaceTargetHitCheck(0f)));
        _skillBar1.AddSkill(KeyCode.O, new Skill("闪光", "tab", 0, 0, 0,  1f, 0.83f, 0.2f, 0, new RemoveAllStatuses(), new AddStatusEffect(CharacterStatusType.Stun, 2f), new NoTargetRequired(), new FixedDirectionMovement(MovementDirection.Backward, 6), new RangeHitCheck(3f)));
        _skillBar1.ApplySkills(_player1);

        _skillBar2.AddSkill(KeyCode.Keypad1, new Skill("普攻", "attack", 0, 1, 1, 0.5f, 0.33f, 0.1f, 0, new DoNotRemoveStatuses(), new DoNotAddStatusEffect(), new NoTargetRequired(), new NoMovement(), new FaceTargetHitCheck(2.5f)));
        _skillBar2.AddSkill(KeyCode.Keypad2, new Skill("刺心", "cixin", 3, 0, 12, 1f, 0.83f, 0.5f, 0, new DoNotRemoveStatuses(), new AddStatusEffect(CharacterStatusType.Weakness, 2f), new TargetRequired(3f), new NoMovement(), new FaceTargetHitCheck(3f)));
        _skillBar2.AddSkill(KeyCode.DownArrow, new Skill("逆风行", "nifengxing", 0, 0, 0, 8f, 0.43f, 0f, 0, new DoNotRemoveStatuses(), new DoNotAddStatusEffect(), new NoTargetRequired(), new FixedDirectionMovement(MovementDirection.Backward, 6), new FaceTargetHitCheck(0f)));
        _skillBar2.AddSkill(KeyCode.Keypad6, new Skill("闪光", "tab", 0, 0, 0, 1f, 0.83f, 0.2f, 0, new RemoveAllStatuses(), new AddStatusEffect(CharacterStatusType.Stun, 2f), new NoTargetRequired(), new FixedDirectionMovement(MovementDirection.Backward, 6), new RangeHitCheck(3f)));
        _skillBar2.ApplySkills(_player2);
    }

    void Update()
    {
        if (_isGameOver)
            return;

        if(_player1._stateManager.GetCurrentStatus() == CharacterStatusType.None)
        {
            if (Input.GetKey(KeyCode.A))
            {
                _player1.Move(CharacterDir.eLeft);
            }
            else if (Input.GetKey(KeyCode.D))
            {
                _player1.Move(CharacterDir.eRight);
            }
            else
            {
                _player1.Stand();
            }
        }
        if (_player2._stateManager.GetCurrentStatus() == CharacterStatusType.None)
        {
            if (Input.GetKey(KeyCode.LeftArrow))
            {
                _player2.Move(CharacterDir.eLeft);
            }
            else if (Input.GetKey(KeyCode.RightArrow))
            {
                _player2.Move(CharacterDir.eRight);
            }
            else
            {
                _player2.Stand();
            }
        }
        foreach (var slot in _player1._skillBar._skills)
        {
            if (Input.GetKeyDown(slot._hotKey))
            {
                _player1._skillBar.ActivateSkill(slot._skill);
            }
        }
        foreach (var slot in _player2._skillBar._skills)
        {
            if (Input.GetKeyDown(slot._hotKey))
            {
                _player2._skillBar.ActivateSkill(slot._skill);
            }
        }
    }

    public void SetGameOver(bool bGameOver)
    {
        _isGameOver = (bGameOver);
    }

    public Player _player1;
    public Player _player2;
    public SkillBar _skillBar1;
    public SkillBar _skillBar2;

    bool _isGameOver = false;
}
