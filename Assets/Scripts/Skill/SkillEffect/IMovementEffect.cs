using System.ComponentModel;
using UnityEngine;

public class AllowMovementDuringSkill : ISkillEffect
{

}

/// <summary>
/// 技能移动组件
/// </summary>
public interface IMovementEffect : ISkillEffect
{
    void BeforeMove(Character owner, SkillInstance skill);
    void OnMoving(Character owner, SkillInstance skill);
    void AfterMove(Character owner, SkillInstance skill);
}

/// <summary>
/// 位移
/// </summary>
public class FixedDirectionMovement : IMovementEffect
{
    public FixedDirectionMovement(MovementDirection movementDirection, float movementDistance)
    {
        MovementDir = movementDirection;
        MovementDistance = movementDistance;
    }

    public void BeforeMove(Character owner, SkillInstance skill)
    {
        Vector3 movementVector = GetMovementVector(owner.GetDirection(), MovementDir);
        _targetMovePosition = owner.transform.position + movementVector * MovementDistance;
        _movementSpeed = MovementDistance / skill.SkillInfo._castTime; // 计算移动速度
    }

    public void AfterMove(Character owner, SkillInstance skill)
    {
        owner.transform.position = _targetMovePosition; // 移动到目标位置
    }

    public void OnMoving(Character owner, SkillInstance skill)
    {
        // 计算当前移动距离
        float distanceToTarget = Vector3.Distance(owner.transform.position, _targetMovePosition);

        // 判断是否到达目标位置
        if (distanceToTarget >= 0.01f)
        {
            // 移动技能释放者
            owner.transform.position = Vector3.MoveTowards(owner.transform.position, _targetMovePosition, _movementSpeed * Time.deltaTime);
        }
    }

    private Vector2 GetMovementVector(CharacterDir characterDir, MovementDirection movementDirection)
    {
        Vector2 movementVector = Vector2.zero;

        // 根据人物朝向和移动方向设置移动向量
        switch (characterDir)
        {
            case CharacterDir.eLeft:
                movementVector.x = (movementDirection == MovementDirection.Forward) ? -1f : 1f;
                break;
            case CharacterDir.eRight:
                movementVector.x = (movementDirection == MovementDirection.Forward) ? 1f : -1f;
                break;
        }

        return movementVector;
    }

    public MovementDirection MovementDir { get; set; } // 位移方向
    public float MovementDistance { get; set; } // 位移距离

    Vector3 _targetMovePosition; // 目标移动位置
    float _movementSpeed; // 移动速度
}

public class RushToTargetMovement : IMovementEffect
{
    public RushToTargetMovement()
    {
        _rushDistance = 1;// 突进到目标1米处
    }

    public void BeforeMove(Character owner, SkillInstance skill)
    {
        var target = owner._targetFinder.GetTarget();
        _targetMovePosition = target.transform.position - (target.transform.position - owner.transform.position).normalized * _rushDistance;
        _movementSpeed = Vector3.Distance(owner.transform.position, _targetMovePosition) / skill.SkillInfo._castTime;
    }

    public void OnMoving(Character owner, SkillInstance skill)
    {
        if (owner == null) return;

        // 如果敌人距离移动目标位置小于等于突进距离，说明敌人已经移动，需要更新突进目标位置
        var target = owner._targetFinder.GetTarget();
        if (target == null) return;
        if (Vector3.Distance(target.transform.position, _targetMovePosition) <= _rushDistance)
        {
            _targetMovePosition = target.transform.position - (target.transform.position - owner.transform.position).normalized * _rushDistance;
        }

        float distanceToTarget = Vector3.Distance(owner.transform.position, _targetMovePosition);

        if (distanceToTarget >= 0.01f)
        {
            owner.transform.position = Vector3.MoveTowards(owner.transform.position, _targetMovePosition, _movementSpeed * Time.deltaTime);
        }
    }

    public void AfterMove(Character owner, SkillInstance skill)
    {
        // 技能释放结束后不做额外操作
    }

    private float _rushDistance; // 距离目标的距离
    private Vector3 _targetMovePosition; // 目标移动位置
    private float _movementSpeed; // 移动速度
}

public class RushToBackTargetMovement : IMovementEffect
{
    private float _rushDistance; // 距离目标的距离
    private Vector3 _targetMovePosition; // 目标移动位置
    private float _totalTravelTime; // 总的旅行时间
    private float _elapsedTime; // 已经过去的时间
    private bool _hasPassedTarget; // 是否已经经过目标

    public RushToBackTargetMovement()
    {
        _rushDistance = 1; // 突进到目标1米处
    }

    public void BeforeMove(Character owner, SkillInstance skill)
    {
        var target = owner._targetFinder.GetTarget();
        _targetMovePosition = target.transform.position - (owner.transform.position - target.transform.position).normalized * _rushDistance;
        _totalTravelTime = skill.SkillInfo._castTime;
        _elapsedTime = 0;
        _hasPassedTarget = false;
    }

    public void OnMoving(Character owner, SkillInstance skill)
    {
        if (owner == null) return;

        var target = owner._targetFinder.GetTarget();
        if (target == null) return;

        _elapsedTime += Time.deltaTime;

        // 通过SmoothStep函数来让速度由慢到快再到慢
        float normalizedTime = _elapsedTime / _totalTravelTime;
        float speedFactor = Mathf.SmoothStep(0, 1, normalizedTime);

        float targetSpeed = Vector3.Distance(owner.transform.position, _targetMovePosition) * speedFactor / (_totalTravelTime - _elapsedTime);

        owner.transform.position = Vector3.MoveTowards(owner.transform.position, _targetMovePosition, targetSpeed * Time.deltaTime);

        // 如果已经经过了目标，立即转向
        if (!_hasPassedTarget && Vector3.Distance(owner.transform.position, _targetMovePosition) <= _rushDistance)
        {
            owner.FlipDirection();
            _hasPassedTarget = true;
        }
    }

    public void AfterMove(Character owner, SkillInstance skill)
    {
        // 技能释放结束后，如果还没转向，立即转向
        if (owner != null && !_hasPassedTarget)
        {
            owner.FlipDirection();
        }
    }
}


/// <summary>
/// 闪现到目标后方
/// </summary>
public class BlinkBehindTargetMovement : IMovementEffect
{
    private float _blinkDistance; // 距离目标的距离
    private Vector3 _targetBlinkPosition; // 目标闪现位置
    private float _delayElapsed; // 已经过去的延迟时间
    private bool _hasBlinked; // 是否已经完成闪现

    public float MovementDelay { get; set; } // 延迟多少秒再闪现

    public BlinkBehindTargetMovement(float movementDelay = 0)
    {
        _blinkDistance = 1; // 闪现到目标1米处
        this.MovementDelay = movementDelay;
    }

    public void BeforeMove(Character owner, SkillInstance skill)
    {
        var target = owner._targetFinder.GetTarget();
        _targetBlinkPosition = target.transform.position + (target.transform.position - owner.transform.position).normalized * _blinkDistance;
        _delayElapsed = 0;
        _hasBlinked = false;
    }

    public void OnMoving(Character owner, SkillInstance skill)
    {
        _delayElapsed += Time.deltaTime;

        // 如果已经完成闪现或者延迟时间未到，不进行移动
        if (_hasBlinked || _delayElapsed < MovementDelay)
        {
            return;
        }

        // 闪现到目标位置并立即转变方向
        owner.transform.position = _targetBlinkPosition;
        if (owner != null)
        {
            owner.FlipDirection();
        }

        _hasBlinked = true;
    }

    public void AfterMove(Character owner, SkillInstance skill)
    {
        // 确保移动到目标位置
        owner.transform.position = _targetBlinkPosition;
    }
}


/// <summary>
/// 和目标交换位置
/// </summary>
public class SwapWithTargetMovement : IMovementEffect
{
    private Character _target; // 目标
    private Vector3 _ownerOriginalPosition; // 主角原始位置
    private Vector3 _targetOriginalPosition; // 目标原始位置
    private float _elapsedTime; // 已经过去的时间
    private bool _hasSwapped; // 是否已经完成交换
    private bool _hasPassedTarget; // 是否已经经过目标

    public void BeforeMove(Character owner, SkillInstance skill)
    {
        _target = owner._targetFinder.GetTarget();
        _ownerOriginalPosition = owner.transform.position;
        _targetOriginalPosition = _target.transform.position;
        _elapsedTime = 0f;
        _hasSwapped = false;
        _hasPassedTarget = false;
    }

    public void OnMoving(Character owner, SkillInstance skill)
    {
        _elapsedTime += Time.deltaTime;

        // 如果已经完成交换或者延迟时间未到，不进行移动
        if (_hasSwapped || _elapsedTime < skill.SkillInfo._damageDelay)
        {
            return;
        }

        // 计算交换位置所剩余的时间
        float swapRemainingTime = skill.SkillInfo._castTime - skill.SkillInfo._damageDelay;
        // (_elapsedTime - skill.SkillInfo._damageDelay) / swapRemainingTime 是交换开始后经过的时间的百分比
        float t = (_elapsedTime - skill.SkillInfo._damageDelay) / swapRemainingTime;

        Vector3 ownerNextPosition = Vector3.Lerp(_ownerOriginalPosition, _targetOriginalPosition, t);
        Vector3 targetNextPosition = Vector3.Lerp(_targetOriginalPosition, _ownerOriginalPosition, t);

        if (t <= 1)
        {
            if (!_hasPassedTarget && Vector3.Dot(ownerNextPosition - _target.transform.position, _ownerOriginalPosition - _targetOriginalPosition) < 0)
            {
                owner.FlipDirection();
                _target.SetDirection(owner.GetDirection()); // 设置目标朝向与角色一致
                _hasPassedTarget = true;
            }

            owner.transform.position = ownerNextPosition;
            _target.transform.position = targetNextPosition;
        }
        else if (!_hasSwapped) // 确保交换只进行一次
        {
            owner.transform.position = _targetOriginalPosition;
            _target.transform.position = _ownerOriginalPosition;

            _hasSwapped = true;
        }
    }

    public void AfterMove(Character owner, SkillInstance skill)
    {
        // 如果技能释放结束后还没有交换位置，立即交换位置
        if (!_hasSwapped)
        {
            owner.transform.position = _targetOriginalPosition;
            _target.transform.position = _ownerOriginalPosition;
            _hasSwapped = true;
        }
    }
}




/// <summary>
/// 位移方向
/// </summary>
public enum MovementDirection
{
    [Description("向前方移动")]
    Forward,
    [Description("向后方移动")]
    Backward
}