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
/// 位移方向
/// </summary>
public enum MovementDirection
{
    [Description("向前方移动")]
    Forward,
    [Description("向后方移动")]
    Backward
}

public class AllowMovementDuringSkill : ISkillEffect
{

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
/// 绕目标旋转
/// </summary>
public class CircleAroundTargetMovement : IMovementEffect
{
    private Character _target; // 目标
    private Vector3 _startPosition; // 角色旋转开始前的位置
    private float _elapsedTime; // 旋转已经进行的时间
    private float _totalTravelTime; // 角色旋转需要的总时间
    private Quaternion _initialRotation; // 角色旋转开始前的朝向
    private Vector3 _initialLocalTargetDirection; // 角色旋转开始前，目标相对于角色的本地坐标系的方向
    private Vector3 _initialWorldTargetDirection; // 角色旋转开始前，目标在世界坐标系中的方向
    private float _startAngle; // 新增，表示角色初始位置相对于目标的方向（在水平面上）

    public void BeforeMove(Character owner, SkillInstance skill)
    {
        _target = owner._targetFinder.GetTarget(); // 找到目标
        _startPosition = owner.transform.position; // 记录下角色旋转开始前的位置
        _elapsedTime = 0; // 初始化已经旋转的时间为0
        _totalTravelTime = skill.SkillInfo._castTime; // 旋转需要的总时间由技能释放时间决定
        _initialRotation = owner.transform.rotation; // 记录下角色旋转开始前的朝向

        // 计算出角色旋转开始前，目标相对于角色的本地坐标系的方向
        _initialLocalTargetDirection = owner.transform.InverseTransformDirection(_target.transform.position - _startPosition);

        // 计算出角色旋转开始前，目标在世界坐标系中的方向
        _initialWorldTargetDirection = owner.transform.TransformDirection(_initialLocalTargetDirection);

        // 计算初始角度
        Vector3 startDirection = (_startPosition - _target.transform.position).normalized;
        _startAngle = Mathf.Atan2(startDirection.z, startDirection.x);
    }

    public void OnMoving(Character owner, SkillInstance skill)
    {
        _elapsedTime += Time.deltaTime; // 更新已经旋转的时间

        // 计算当前时间对应的旋转角度，这个角度等于 (已经旋转的时间 / 旋转需要的总时间) * 2π
        // 注意这里是将已经旋转的角度和初始角度相加
        float angle = (_startAngle + _elapsedTime * 2 * Mathf.PI / _totalTravelTime) % (2 * Mathf.PI);

        // 计算角色旋转的半径，这个半径等于角色开始旋转前的位置与目标的距离
        float radius = Vector3.Distance(_startPosition, _target.transform.position);

        // 根据旋转的半径和当前时间对应的旋转角度，计算出角色的新位置
        Vector3 newPosition = _target.transform.position + new Vector3(radius * Mathf.Cos(angle), 0, radius * Mathf.Sin(angle));

        // 移动角色到新位置
        owner.transform.position = newPosition;

        // 通过旋转初始时角色的本地朝向来得到当前应该面向的方向
        Vector3 currentLocalTargetDirection = Quaternion.Euler(0, angle * Mathf.Rad2Deg, 0) * _initialLocalTargetDirection;
        // 将本地朝向转换为世界坐标系中的朝向
        Vector3 currentWorldTargetDirection = owner.transform.TransformDirection(currentLocalTargetDirection);

        // 让角色面向目标
        LookAtWithCustomForward(owner.transform, _initialLocalTargetDirection, _target.transform.position);
    }

    public void AfterMove(Character owner, SkillInstance skill)
    {
        // 技能释放结束后，立即将角色的位置和朝向恢复到旋转开始前的状态
        owner.transform.position = _startPosition;
        owner.transform.rotation = _initialRotation;
    }

    public void LookAtWithCustomForward(Transform transform, Vector3 forwardInLocalSpace, Vector3 targetPosition)
    {
        // 计算目标方向
        Vector3 targetDirection = (targetPosition - transform.position).normalized;

        // 计算当前朝向
        Vector3 currentForward = transform.TransformDirection(forwardInLocalSpace).normalized;

        // 计算旋转轴
        Vector3 rotationAxis = Vector3.Cross(currentForward, targetDirection);

        // 计算需要旋转的角度
        float angle = Vector3.SignedAngle(currentForward, targetDirection, rotationAxis);

        // 进行旋转
        transform.Rotate(rotationAxis, angle, Space.World);
    }
}














