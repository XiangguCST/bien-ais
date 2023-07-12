using System;
using System.ComponentModel;

/// <summary>
/// 枚举特性(Attribute)
/// 
/// 示例：
/// 1.定义：
///public enum MovementDirection
///{
///    [Description("向前方移动")]
///    Forward,
///    [Description("向后方移动")]
///    Backward
///}
///public enum CharacterStatusType
///{
///    [Description("无异常状态")]
///    None,
///    [Description("沉默状态")]
///    Silence,
///    [Description("眩晕")]
///    Stun,
///    [Description("虚弱")]
///    Weakness,
///    [Description("击倒")]
///    Knockdown
///}
/// 2.使用：
/// CharacterStatusType status = CharacterStatusType.Silence;
/// string statusString = status.GetDescription();
/// MovementDirection direction = MovementDirection.Forward;
/// string directionString = direction.GetDescription();
/// 
/// </summary>
public static class EnumExtensions
{
    public static string GetDescription<TEnum>(this TEnum enumValue) where TEnum : Enum
    {
        var type = typeof(TEnum);
        var field = type.GetField(enumValue.ToString());
        if (field != null)
        {
            var attributes = field.GetCustomAttributes(typeof(DescriptionAttribute), false);
            if (attributes.Length > 0)
            {
                return ((DescriptionAttribute)attributes[0]).Description;
            }
        }

        return enumValue.ToString();
    }
}
