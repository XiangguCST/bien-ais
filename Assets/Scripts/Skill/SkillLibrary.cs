using System.Collections.Generic;

public class SkillLibrary
{
    private static Dictionary<string, Skill> skillDictionary = new Dictionary<string, Skill>();

    static SkillLibrary()
    {
        skillDictionary = new Dictionary<string, Skill>();

        // 添加技能到字典
        AddSkill(new Skill("迷雾斩", "attack", 0, 1, 1, 0.5f, 0.33f, 0.1f, 0, false, true, new DoNotRemoveStatuses(), new DoNotAddStatusEffect(), new NoTargetRequired(), new NoMovement(), new FaceTargetHitCheck(2.5f), new DoNotAddBuff()));
        AddSkill(new Skill("刺心", "cixin", 3, 0, 12, 1f, 0.83f, 0.5f, 0, false, true, new DoNotRemoveStatuses(), new AddStatusEffect(CharacterStatusType.Weakness, 2f), new TargetRequired(3f), new NoMovement(), new FaceTargetHitCheck(3f), new DoNotAddBuff()));
        AddSkill(new Skill("瞬步", "shunbu", 0, 1, 0, 16f, 0.2f, 0f, 0, true, false, new DoNotRemoveStatuses(), new DoNotAddStatusEffect(), new NoTargetRequired(), new FixedDirectionMovement(MovementDirection.Forward, 6), new FaceTargetHitCheck(0f), new AddBuffDuration(BuffType.ImmunityAll, 1)));
        AddSkill(new Skill("潜行", "qianxing", 0, 2, 1, 6f, 0.33f, 0.3f, 0, false, false, new DoNotRemoveStatuses(), new DoNotAddStatusEffect(), new TargetRequired(6f), new RushToTargetMovement(), new FaceTargetHitCheck(2.5f), new DoNotAddBuff()));
        AddSkill(new Skill("莲华脚", "lianhuajiao", 2, 0, 12, 24f, 0.22f, 0.2f, 0, false, true, new DoNotRemoveStatuses(), new AddStatusEffect(CharacterStatusType.Stun, 2f), new TargetRequired(6f), new RushToTargetMovement(), new RangeHitCheck(-1f), new AddBuffDuration(BuffType.ImmunityAll, 1)));
        AddSkill(new Skill("替身术", "tishenshu", 0, 0, 0, 8f, 0.5f, 0.5f, 0, false, true, new DoNotRemoveStatuses(), new DoNotAddStatusEffect(), new NoTargetRequired(), new NoMovement(), new FaceTargetHitCheck(0f), new AddBuffDuration(BuffType.ShadowClone, 0.5f)));
        AddSkill(new Skill("闪光", "tab", 0, 5, 0, 36f, 0.83f, 0f, 0, true, false, new RemoveAllStatuses(), new AddStatusEffect(CharacterStatusType.Stun, 2f), new NoTargetRequired(), new FixedDirectionMovement(MovementDirection.Backward, 6), new RangeHitCheck(3f), new AddBuffDuration(BuffType.ImmunityAll, 0.83f)));
        AddSkill(new Skill("逆风行", "nifengxing", 0, 0, 0, 8f, 0.43f, 0f, 0, true, false, new DoNotRemoveStatuses(), new DoNotAddStatusEffect(), new NoTargetRequired(), new FixedDirectionMovement(MovementDirection.Backward, 6), new FaceTargetHitCheck(0f), new AddBuffDuration(BuffType.ImmunityAll, 0.43f)));
    }

    private static void AddSkill(Skill skill)
    {
        if (skill != null)
            skillDictionary.Add(skill._name, skill);
    }


    public static Skill CreateSkillByName(string name)
    {
        Skill skill;
        if (skillDictionary.TryGetValue(name, out skill))
        {
            // 注意：此处返回技能的一个新的实例，防止不同的地方修改了同一个技能
            return new Skill(skill._name, skill._animName, skill._energyCost, skill._energyRecover, skill._rate, skill._cooldownTime, 
                skill._castTime, skill._damageDelay, skill._globalCooldownTime, skill._canInterruptOtherSkills, skill._canBeInterrupted,
                skill._statusRemovalStrategy, skill._statusAdditionStrategy, skill._targetRequirementStrategy, skill._movementStrategy, 
                skill._hitCheckStrategy, skill._buffAdditionStrategy);
        }

        return null;  // 如果字典中不存在，则返回null
    }
}
