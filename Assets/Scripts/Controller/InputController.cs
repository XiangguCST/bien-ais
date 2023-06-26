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
            _player1.Move(-1);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            _player1.Move(1);
        }
        else
        {
            _player1.Move(0);

        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            _player2.Move(-1);
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            _player2.Move(1);
        }
        else
        {
            _player2.Move(0);
        }

        //// 攻击
        //if (Input.GetKeyDown(KeyCode.J))
        //{

        //}
    }

    public Player _player1;
    public Player _player2;
}
