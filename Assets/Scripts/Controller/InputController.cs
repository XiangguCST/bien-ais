using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YFrameWork;

public class InputController : MonoSingleton<InputController>
{
    void Awake()
    {
        // 初始化技能栏
        _skillBar1.AddSkill(KeyCode.J, new Skill(_skillBar1, "普攻", 1, 2.5f, 0.67f, 0.67f, 0.2f, 0));
        _skillBar1.ApplySkills(_player1);
        _skillBar2.AddSkill(KeyCode.Keypad1, new Skill(_skillBar2, "普攻", 1, 2.5f, 0.67f, 0.67f, 0.2f, 0));
        _skillBar2.ApplySkills(_player2);
    }

    void Update()
    {
        if (_isGameOver)
            return;

        // 移动
        if(Input.GetKey(KeyCode.A))
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

        // 使用技能
        foreach (var slot in _skillBar1._skills)
        {
            if(Input.GetKey(slot._hotKey))
            {
                _skillBar1.ActivateSkill(slot._skill);
            }
        }
        foreach (var slot in _skillBar2._skills)
        {
            if (Input.GetKey(slot._hotKey))
            {
                _skillBar2.ActivateSkill(slot._skill);
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
