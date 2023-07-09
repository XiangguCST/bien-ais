using System.Collections.Generic;

public class SkillLibrary
{
    private static Dictionary<string, Skill> skillDictionary = new Dictionary<string, Skill>();

    static SkillLibrary()
    {
        skillDictionary = new Dictionary<string, Skill>();

        // 添加技能到字典
        AddSkill(new Skill("迷雾斩", "attack", 0, 1, 1, 0.5f, 0.33f, 0.1f, 0, SkillUsabilityPriority.Normal, SkillInterruptPriority.Normal, false, new DoNotRemoveStatuses(), new DoNotAddStatusEffect(), new NoMovement(), new FaceTargetHitCheck(2.5f), new DoNotAddBuff()));
        AddSkill(new Skill("刺心", "cixin", 3, 0, 12, 1f, 0.83f, 0.5f, 0, SkillUsabilityPriority.Normal, SkillInterruptPriority.Normal, false, new DoNotRemoveStatuses(), new AddStatusEffect(CharacterStatusType.Weakness, 2f), new NoMovement(), new FaceTargetHitCheck(3f), new DoNotAddBuff()).AddSkillUsability(new TargetRequired(3f)));
        AddSkill(new Skill("瞬步", "shunbu", 0, 1, 0, 16f, 0.2f, 0f, 0, SkillUsabilityPriority.Normal, SkillInterruptPriority.High, false, new DoNotRemoveStatuses(), new DoNotAddStatusEffect(), new FixedDirectionMovement(MovementDirection.Forward, 6), new DoNotHitCheck(), new AddBuffDuration(BuffType.ImmunityAll, 1)));
        AddSkill(new Skill("潜行", "qianxing", 0, 2, 0, 6f, 0.33f, 0.3f, 0, SkillUsabilityPriority.Normal, SkillInterruptPriority.Normal, false, new DoNotRemoveStatuses(), new DoNotAddStatusEffect(), new RushToTargetMovement(), new DoNotHitCheck(), new DoNotAddBuff()).AddSkillUsability(new TargetRequired(6f)));
        AddSkill(new Skill("莲华脚", "lianhuajiao", 2, 0, 12, 24f, 0.22f, 0.2f, 0, SkillUsabilityPriority.Normal, SkillInterruptPriority.Normal, false, new DoNotRemoveStatuses(), new AddStatusEffect(CharacterStatusType.Stun, 2f), new RushToTargetMovement(), new RangeHitCheck(-1f), new AddBuffDuration(BuffType.ImmunityAll, 1)).AddSkillUsability(new TargetRequired(6f)));
        var tishenshu = AddSkill(new Skill("替身术", "tishenshu", 0, 0, 0, 8f, 0.5f, 0.5f, 0, SkillUsabilityPriority.Normal, SkillInterruptPriority.Low, false, new DoNotRemoveStatuses(), new DoNotAddStatusEffect(), new NoMovement(), new DoNotHitCheck(), new AddBuffDuration(BuffType.ShadowClone, 0.5f)));
        AddSkill(new Skill("闪光", "tab", 0, 5, 0, 36f, 0.83f, 0f, 0, SkillUsabilityPriority.Normal, SkillInterruptPriority.High, true, new RemoveAllStatuses(), new AddStatusEffect(CharacterStatusType.Stun, 2f), new FixedDirectionMovement(MovementDirection.Backward, 6), new RangeHitCheck(3f), new AddBuffDuration(BuffType.ImmunityAll, 0.83f)));
        var nifengxing = AddSkill(new Skill("逆风行", "nifengxing", 0, 0, 0, 8f, 0.43f, 0f, 0, SkillUsabilityPriority.Normal, SkillInterruptPriority.High, false, new DoNotRemoveStatuses(), new DoNotAddStatusEffect(), new FixedDirectionMovement(MovementDirection.Backward, 6), new DoNotHitCheck(), new AddBuffDuration(BuffType.ImmunityAll, 0.43f)));
        AddSkill(new Skill("空手入白刃", "kongshourubairen", 0, 0, 1, 9f, 0.75f, 0.37f, 0, SkillUsabilityPriority.Conditional, SkillInterruptPriority.High, true, new DoNotRemoveStatuses(), new AddStatusEffect(CharacterStatusType.Stun, 2f), new NoMovement(), new RangeHitCheck(-1f), new DoNotAddBuff()).AddSkillUsability(new TargetRequired(3f)).AddSkillUsability(new TargetBuffRequired(BuffType.ShadowClone)));
        var shuoyuejiao = AddSkill(new Skill("朔月脚", "shuoyuejiao", 0, 0, 12, 24f, 0.38f, 0.19f, 0, SkillUsabilityPriority.Chain, SkillInterruptPriority.High, false, new DoNotRemoveStatuses(), new AddStatusEffect(CharacterStatusType.Weakness, 2f), new FixedDirectionMovement(MovementDirection.Forward, 12), new RangeHitCheck(6f), new DoNotAddBuff()));
        tishenshu.AddChainSkill(shuoyuejiao);
        nifengxing.AddChainSkill(shuoyuejiao);
    }

    private static Skill AddSkill(Skill skill)
    {
        if (skill != null)
            skillDictionary.Add(skill._name, skill);
        return skill;
    }


    public static Skill GetSkill(string name)
    {
        Skill skill;
        if (skillDictionary.TryGetValue(name, out skill))
        {
            return skill;
        }

        return null;  // 如果字典中不存在，则返回null
    }
}
