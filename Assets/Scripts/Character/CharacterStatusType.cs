using System.ComponentModel;

public enum CharacterStatusType
{
    [Description("无异常状态")]
    None,
    [Description("眩晕")]
    Stun = 2,
    [Description("虚弱")]
    Weakness = 3,
    [Description("击倒")]
    Knockdown = 4,
    [Description("沉默")]
    Silence = 5,
}
