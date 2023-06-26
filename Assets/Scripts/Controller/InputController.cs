using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YFrameWork;

public class InputController : BaseController
{
    private void Awake()
    {
        _skillBar1.AddSkill(KeyCode.J, new Skill(_skillBar1, "普攻", 1, 0, 0));
        _skillBar1.ApplySkills();
    }

    void Update()
    {
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
        
    }

    public Player _player1;
    public Player _player2;
    public SkillBar _skillBar1;
    public SkillBar _skillBar2;
}
