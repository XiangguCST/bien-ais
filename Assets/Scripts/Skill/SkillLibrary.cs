using System.Collections.Generic;

public class SkillLibrary
{
    private static Dictionary<string, Skill> skillDictionary = new Dictionary<string, Skill>();

    static SkillLibrary()
    {
        skillDictionary = new Dictionary<string, Skill>();

        // 添加技能到字典
        var attack = AddSkill(new Skill("迷雾斩", "attack", 0, 1, 1, 0.5f, 0.33f, 0.1f, 0, 
            SkillUsabilityPriority.Normal, SkillInterruptPriority.Normal, false,
            new NonChainSkillStrategy()));
        attack.AddComponent(new FaceTargetHitCheckStrategy(2.5f));
        var cixin = AddSkill(new Skill("刺心", "cixin", 3, 0, 12, 1f, 0.83f, 0.5f, 0,
            SkillUsabilityPriority.Normal, SkillInterruptPriority.Normal, false,
             new NonChainSkillStrategy()));
        cixin.AddComponent(new TargetRequired(3f));
        cixin.AddComponent(new FaceTargetHitCheckStrategy(3f));
        cixin.AddComponent(new StatusAdditionEffect(CharacterStatusType.Weakness, 2f));
        var shunbu = AddSkill(new Skill("瞬步", "shunbu", 0, 1, 0, 16f, 0.2f, 0f, 0,
            SkillUsabilityPriority.Normal, SkillInterruptPriority.High, false,
            new NonChainSkillStrategy()));
            shunbu.AddComponent(new FixedDirectionMovement(MovementDirection.Forward, 6));
        shunbu.AddComponent(new AddBuffDurationEffect(BuffType.ImmunityAll, 1));
        var ceshenshan = AddSkill(new Skill("侧身闪", "ceshenshan", 0, 0, 0, 8f, 0.33f, 0.3f, 0,
           SkillUsabilityPriority.Normal, SkillInterruptPriority.High, false,
          new NonChainSkillStrategy())
          );
        ceshenshan.AddComponent(new TargetRequired(6f));
        ceshenshan.AddComponent(new RushToBackTargetMovement());
        ceshenshan.AddComponent(new AddBuffDurationEffect(BuffType.ImmunityAll));
        var qianxing = AddSkill(new Skill("潜行", "qianxing", 0, 2, 0, 6f, 0.33f, 0.3f, 0,
            SkillUsabilityPriority.Normal, SkillInterruptPriority.Normal, false,
            new NonChainSkillStrategy())
            );
           qianxing.AddComponent(new TargetRequired(6f));
           qianxing.AddComponent(new RushToTargetMovement());
        var lianhuajiao = AddSkill(new Skill("莲华脚", "lianhuajiao", 2, 0, 12, 24f, 0.22f, 0.2f, 0,
            SkillUsabilityPriority.Normal, SkillInterruptPriority.Normal, false,
              new NonChainSkillStrategy()));
        lianhuajiao.AddComponent(new TargetRequired(6f));
        lianhuajiao.AddComponent(new RangeHitCheckStrategy(-1f));
        lianhuajiao.AddComponent(new StatusAdditionEffect(CharacterStatusType.Stun, 2f));
        lianhuajiao.AddComponent(new AddBuffDurationEffect(BuffType.ImmunityAll, 1));
            lianhuajiao.AddComponent(new RushToTargetMovement());
        var tishenshu = AddSkill(new Skill("替身术", "tishenshu", 0, 0, 0, 8f, 0.5f, 0.5f, 0, 
            SkillUsabilityPriority.Normal, SkillInterruptPriority.Low, false, 
            new NonChainSkillStrategy()));
        tishenshu.AddComponent(new AddBuffDurationEffect(BuffType.ShadowClone));
        var hougunfan = AddSkill(new Skill("后滚翻", "hougunfan", 0, 0, 0, 12f, 0.83f, 0f, 0,
            SkillUsabilityPriority.Conditional, SkillInterruptPriority.High, false,
            new NonChainSkillStrategy()));
           hougunfan.AddComponent(new FixedDirectionMovement(MovementDirection.Backward, 3f));
        hougunfan.AddComponent(new StatusRemovalEffect(new List<CharacterStatusType> { CharacterStatusType.Knockdown, CharacterStatusType.Weakness }));
        hougunfan.AddComponent(new AddBuffDurationEffect(BuffType.ImmunityAll));
        var tab = AddSkill(new Skill("闪光", "tab", 0, 5, 0, 36f, 0.83f, 0f, 0,
            SkillUsabilityPriority.Normal, SkillInterruptPriority.High, true,
            new NonChainSkillStrategy()));
            tab.AddComponent(new FixedDirectionMovement(MovementDirection.Backward, 6));
        tab.AddComponent(new RangeHitCheckStrategy(3f));
        tab.AddComponent(new StatusRemovalEffect());
        tab.AddComponent(new StatusAdditionEffect(CharacterStatusType.Stun, 2f));
        tab.AddComponent(new AddBuffDurationEffect(BuffType.ImmunityAll));
        var nifengxing = AddSkill(new Skill("逆风行", "nifengxing", 0, 0, 0, 8f, 0.43f, 0f, 0,
            SkillUsabilityPriority.Normal, SkillInterruptPriority.High, false,
            new NonChainSkillStrategy()));
        nifengxing.AddComponent(new FixedDirectionMovement(MovementDirection.Backward, 6));
        nifengxing.AddComponent(new AddBuffDurationEffect(BuffType.ImmunityAll));
        var kongshourubairen = AddSkill(new Skill("空手入白刃", "kongshourubairen", 0, 0, 1, 9f, 0.75f, 0.37f, 0,
            SkillUsabilityPriority.ChainHigh1, SkillInterruptPriority.High, true,
             new NonChainSkillStrategy()));
        kongshourubairen.AddComponent(new TargetRequired(3f));
        kongshourubairen.AddComponent(new TargetBuffRequired(BuffType.ShadowClone));
        kongshourubairen.AddComponent(new RangeHitCheckStrategy(-1f));
        kongshourubairen.AddComponent(new StatusAdditionEffect(CharacterStatusType.Stun, 2f));
        var shuoyuejiao = AddSkill(new Skill("朔月脚", "shuoyuejiao", 0, 0, 12, 24f, 0.38f, 0.19f, 0,
            SkillUsabilityPriority.Chain, SkillInterruptPriority.High, false,
            new ChainSkillStrategy()));
        shuoyuejiao.AddComponent(new FixedDirectionMovement(MovementDirection.Forward, 12));
        shuoyuejiao.AddComponent(new RangeHitCheckStrategy(6f));
        shuoyuejiao.AddComponent(new StatusAdditionEffect(CharacterStatusType.Knockdown, 2f));
        var youlingbu = AddSkill(new Skill("幽灵步", "youlingbu", 0, 0, 0, 6f, 0.43f, 0f, 0,
            SkillUsabilityPriority.Chain, SkillInterruptPriority.High, false, 
            new ChainSkillStrategy(true)));
        youlingbu.AddComponent(new FixedDirectionMovement(MovementDirection.Backward, 6));
        youlingbu.AddComponent(new AddBuffDurationEffect(BuffType.ImmunityAll));

        tishenshu.AddChainSkill(shuoyuejiao);
        nifengxing.AddChainSkill(shuoyuejiao);
        youlingbu.AddChainSkill(shuoyuejiao);
        nifengxing.AddChainSkill(youlingbu);
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
