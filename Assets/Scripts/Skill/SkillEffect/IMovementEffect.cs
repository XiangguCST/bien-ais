using System.ComponentModel;
using UnityEngine;

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
        MovementDirection = movementDirection;
        MovementDistance = movementDistance;
    }

    public void BeforeMove(Character owner, SkillInstance skill)
    {
        Vector3 movementVector = GetMovementVector(owner._dir, MovementDirection);
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

    public MovementDirection MovementDirection { get; set; } // 位移方向
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
        var target = owner._targetFinder._nearestEnemy;
        _targetMovePosition = target.transform.position - (target.transform.position - owner.transform.position).normalized * _rushDistance;
        _movementSpeed = Vector3.Distance(owner.transform.position, _targetMovePosition) / skill.SkillInfo._castTime;
    }

    public void OnMoving(Character owner, SkillInstance skill)
    {
        if (owner == null) return;

        // 如果敌人距离移动目标位置小于等于突进距离，说明敌人已经移动，需要更新突进目标位置
        var target = owner._targetFinder._nearestEnemy;
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
    private float _averageSpeed; // 平均速度

    public RushToBackTargetMovement()
    {
        _rushDistance = 1; // 突进到目标1米处
    }

    public void BeforeMove(Character owner, SkillInstance skill)
    {
        var target = owner._targetFinder._nearestEnemy;
        _targetMovePosition = target.transform.position - (owner.transform.position - target.transform.position).normalized * _rushDistance;
        _totalTravelTime = skill.SkillInfo._castTime;
        _elapsedTime = 0;
    }

    public void OnMoving(Character owner, SkillInstance skill)
    {
        if (owner == null) return;

        var target = owner._targetFinder._nearestEnemy;
        if (target == null) return;

        _elapsedTime += Time.deltaTime;

        // 通过SmoothStep函数来让速度由慢到快再到慢
        float normalizedTime = _elapsedTime / _totalTravelTime;
        float speedFactor = Mathf.SmoothStep(0, 1, normalizedTime);

        float targetSpeed = Vector3.Distance(owner.transform.position, _targetMovePosition) * speedFactor / (_totalTravelTime - _elapsedTime);

        owner.transform.position = Vector3.MoveTowards(owner.transform.position, _targetMovePosition, targetSpeed * Time.deltaTime);
    }

    public void AfterMove(Character owner, SkillInstance skill)
    {
        // 技能释放结束后立即转变方向
        if (owner == null) return;
        owner.FlipDirection();
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