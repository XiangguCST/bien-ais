using UnityEngine;

public class MovementUtils
{
    public static Vector2 GetMovementVector(CharacterDir characterDir, MovementDirection movementDirection)
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
}
