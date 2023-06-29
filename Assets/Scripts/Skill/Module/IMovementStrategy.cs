﻿using UnityEngine;

/// <summary>
/// 技能移动策略
/// </summary>
public interface IMovementStrategy
{
    void BeforeSkillCast(Player owner, Skill skill);
    void OnSkillCasting(Player owner, Skill skill);
    void AfterSkillCast(Player owner, Skill skill);
}

/// <summary>
/// 位移
/// </summary>
public class FixedDirectionMovement : IMovementStrategy
{
    public FixedDirectionMovement(MovementDirection movementDirection, float movementDistance)
    {
        _movementDirection = movementDirection;
        _movementDistance = movementDistance;
    }

    public void BeforeSkillCast(Player owner, Skill skill)
    {
        Vector3 movementVector = MovementUtils.GetMovementVector(owner._dir, _movementDirection);
        _targetMovePosition = owner.transform.position + movementVector * _movementDistance;
        _movementSpeed = _movementDistance / skill._castTime; // 计算移动速度
    }

    public void AfterSkillCast(Player owner, Skill skill)
    {
        owner.transform.position = _targetMovePosition; // 移动到目标位置
    }

    public void OnSkillCasting(Player owner, Skill skill)
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

    public MovementDirection _movementDirection; // 位移方向
    public float _movementDistance; // 位移距离

    Vector3 _targetMovePosition; // 目标移动位置
    float _movementSpeed; // 移动速度
}

/// <summary>
/// 突进
/// </summary>
public class RushToTargetMovement : IMovementStrategy
{
    public RushToTargetMovement()
    {
        _rushDistance = 1;// 突进到目标1米处
    }

    public void BeforeSkillCast(Player owner, Skill skill)
    {
        var target = owner._targetFinder._nearestEnemy;

        // 计算移动的目标位置，即目标的位置减去距离目标的距离
        _targetMovePosition = target.transform.position - (target.transform.position - owner.transform.position).normalized * _rushDistance;

        // 计算移动速度
        float distance = Vector3.Distance(owner.transform.position, _targetMovePosition);
        _movementSpeed = distance / skill._castTime;
    }

    public void OnSkillCasting(Player owner, Skill skill)
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

    public void AfterSkillCast(Player owner, Skill skill)
    {
        // 技能释放结束后不做额外操作
    }

    private float _rushDistance; // 距离目标的距离
    private Vector3 _targetMovePosition; // 目标移动位置
    private float _movementSpeed; // 移动速度
}

/// <summary>
/// 无移动
/// </summary>
public class NoMovement : IMovementStrategy
{
    public void AfterSkillCast(Player owner, Skill skill)
    {
    }

    public void BeforeSkillCast(Player owner, Skill skill)
    {
    }

    public void OnSkillCasting(Player owner, Skill skill)
    {
    }
}

/// <summary>
/// 位移方向
/// </summary>
public enum MovementDirection
{
    Forward,
    Backward
}