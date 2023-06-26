using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YFrameWork;

public class InputController : BaseController
{
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

        //// 攻击
        //if (Input.GetKeyDown(KeyCode.J))
        //{

        //}
    }

    public Player _player1;
    public Player _player2;
}
