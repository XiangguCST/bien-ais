using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using YFrameWork;

public class InputController : MonoSingleton<InputController>
{
    void Awake()
    {
        // 初始化技能栏
        _skillBar1.InitSkillBar();
        _skillBar2.InitSkillBar();

        _skillBar1.AttachSkill(KeyCode.J, new Skill("普攻", "attack", 0, 1, 1, 0.5f, 0.33f, 0.1f, 0, false, true, new DoNotRemoveStatuses(), new DoNotAddStatusEffect(), new NoTargetRequired(), new NoMovement(), new FaceTargetHitCheck(2.5f), new DoNotAddBuff()));
        _skillBar1.AttachSkill(KeyCode.K, new Skill("刺心", "cixin", 3, 0, 12, 1f, 0.83f, 0.5f, 0, false, true, new DoNotRemoveStatuses(), new AddStatusEffect(CharacterStatusType.Weakness, 2f), new TargetRequired(3f), new NoMovement(), new FaceTargetHitCheck(3f), new DoNotAddBuff()));
        _skillBar1.AttachSkill(KeyCode.L, new Skill("瞬步", "shunbu", 0, 1, 0, 16f, 0.2f, 0f, 0, true, false, new DoNotRemoveStatuses(), new DoNotAddStatusEffect(), new NoTargetRequired(), new FixedDirectionMovement(MovementDirection.Forward, 6), new FaceTargetHitCheck(0f), new AddBuffDuration(BuffType.ImmunityAll, 1)));
        _skillBar1.AttachSkill(KeyCode.U, new Skill("莲华脚", "lianhuajiao", 2, 0, 12, 24f, 0.22f, 0.2f, 0, false, true, new DoNotRemoveStatuses(), new AddStatusEffect(CharacterStatusType.Stun, 2f), new TargetRequired(6f), new RushToTargetMovement(), new FaceTargetHitCheck(3f), new AddBuffDuration(BuffType.ImmunityAll, 1)));
        _skillBar1.AttachSkill(KeyCode.S, new Skill("逆风行", "nifengxing", 0, 0, 0,  8f, 0.43f, 0f, 0, true, false, new DoNotRemoveStatuses(), new DoNotAddStatusEffect(), new NoTargetRequired(), new FixedDirectionMovement(MovementDirection.Backward, 6), new FaceTargetHitCheck(0f), new AddBuffDuration(BuffType.ImmunityAll, 0.43f)));
        _skillBar1.AttachSkill(KeyCode.O, new Skill("闪光", "tab", 0, 5, 0,  36f, 0.83f, 0f, 0, true, false, new RemoveAllStatuses(), new AddStatusEffect(CharacterStatusType.Stun, 2f), new NoTargetRequired(), new FixedDirectionMovement(MovementDirection.Backward, 6), new RangeHitCheck(3f), new AddBuffDuration(BuffType.ImmunityAll, 0.83f)));

        _skillBar2.AttachSkill(KeyCode.Keypad1, new Skill("普攻", "attack", 0, 1, 1, 0.5f, 0.33f, 0.1f, 0, false, true, new DoNotRemoveStatuses(), new DoNotAddStatusEffect(), new NoTargetRequired(), new NoMovement(), new FaceTargetHitCheck(2.5f), new DoNotAddBuff()));
        _skillBar2.AttachSkill(KeyCode.Keypad2, new Skill("刺心", "cixin", 3, 0, 12, 1f, 0.83f, 0.5f, 0, false, true, new DoNotRemoveStatuses(), new AddStatusEffect(CharacterStatusType.Weakness, 2f), new TargetRequired(3f), new NoMovement(), new FaceTargetHitCheck(3f), new DoNotAddBuff()));
        _skillBar2.AttachSkill(KeyCode.Keypad3, new Skill("瞬步", "shunbu", 0, 1, 0, 16f, 0.2f, 0f, 0, true, false, new DoNotRemoveStatuses(), new DoNotAddStatusEffect(), new NoTargetRequired(), new FixedDirectionMovement(MovementDirection.Forward, 6), new FaceTargetHitCheck(0f), new AddBuffDuration(BuffType.ImmunityAll, 1)));
        _skillBar2.AttachSkill(KeyCode.Keypad4, new Skill("莲华脚", "lianhuajiao", 2, 0, 12, 24f, 0.22f, 0.2f, 0, false, true, new DoNotRemoveStatuses(), new AddStatusEffect(CharacterStatusType.Stun, 2f), new TargetRequired(6f), new RushToTargetMovement(), new FaceTargetHitCheck(3f), new AddBuffDuration(BuffType.ImmunityAll, 1)));
        _skillBar2.AttachSkill(KeyCode.DownArrow, new Skill("逆风行", "nifengxing", 0, 0, 0, 8f, 0.43f, 0f, 0, true, false, new DoNotRemoveStatuses(), new DoNotAddStatusEffect(), new NoTargetRequired(), new FixedDirectionMovement(MovementDirection.Backward, 6), new FaceTargetHitCheck(0f), new AddBuffDuration(BuffType.ImmunityAll, 0.43f)));
        _skillBar2.AttachSkill(KeyCode.Keypad6, new Skill("闪光", "tab", 0, 5, 0, 36f, 0.83f, 0f, 0, true, false, new RemoveAllStatuses(), new AddStatusEffect(CharacterStatusType.Stun, 2f), new NoTargetRequired(), new FixedDirectionMovement(MovementDirection.Backward, 6), new RangeHitCheck(3f), new AddBuffDuration(BuffType.ImmunityAll, 0.83f)));
    }

    void Update()
    {
        if (_isGameOver)
            return;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }

        if (_player1._stateManager.GetCurrentStatus() == CharacterStatusType.None)
        {
            if (Input.GetKey(KeyCode.A))
            {
                _player1.Move(CharacterDir.eLeft);
            }
            else if (Input.GetKey(KeyCode.D))
            {
                _player1.Move(CharacterDir.eRight);
            }
            else
            {
                _player1.Stand();
            }
        }
        if (_player2._stateManager.GetCurrentStatus() == CharacterStatusType.None)
        {
            if (Input.GetKey(KeyCode.LeftArrow))
            {
                _player2.Move(CharacterDir.eLeft);
            }
            else if (Input.GetKey(KeyCode.RightArrow))
            {
                _player2.Move(CharacterDir.eRight);
            }
            else
            {
                _player2.Stand();
            }
        }
        _player1._skillBar.UpdateSkillBar(Time.deltaTime);
        _player2._skillBar.UpdateSkillBar(Time.deltaTime);

    }

    public void ReloadScene()
    {
        Time.timeScale = 1f; // 将时间缩放因子设为1，恢复游戏运行
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentSceneIndex);
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
    }

    public void SetGameOver()
    {
        Time.timeScale = 0f; // 将时间缩放因子设为0，暂停游戏运行
        _isGameOver = true;
        _resultPanel.SetActive(true);
    }

    public void PauseGame()
    {
        Time.timeScale = 0f; // 将时间缩放因子设为0，暂停游戏运行
        _pausePanel.SetActive(true);
        isPaused = true;
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f; // 将时间缩放因子设为1，恢复游戏运行
        _pausePanel.SetActive(false);
        isPaused = false;
    }

    public Player _player1;
    public Player _player2;
    public SkillBar _skillBar1;
    public SkillBar _skillBar2;
    public GameObject _pausePanel;
    public GameObject _resultPanel;
    private bool isPaused = false;
    public bool _isGameOver = false;
}
