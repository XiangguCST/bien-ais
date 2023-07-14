using System;
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
        InitSkillSlots();
        _player1.InitCharacter();
        _player2.InitCharacter();

        _player1.AttachSkill(KeyCode.J, SkillLibrary.GetSkill("迷雾斩"));
        _player1.AttachSkill(KeyCode.J, SkillLibrary.GetSkill("后滚翻"));
        _player1.AttachToggleSkill(KeyCode.J, SkillLibrary.GetSkill("木业疾斩"), KeyCode.S);
        _player1.AttachToggleSkill(KeyCode.J, SkillLibrary.GetSkill("木业闪现"), KeyCode.S);
        _player1.AttachSkill(KeyCode.K, SkillLibrary.GetSkill("刺心"));
        _player1.AttachToggleSkill(KeyCode.K, SkillLibrary.GetSkill("侧身闪"), KeyCode.S);
        _player1.AttachSkill(KeyCode.L, SkillLibrary.GetSkill("瞬步"));
        _player1.AttachSkill(KeyCode.U, SkillLibrary.GetSkill("潜行"));
        _player1.AttachToggleSkill(KeyCode.U, SkillLibrary.GetSkill("莲华脚"), KeyCode.S);
        _player1.AttachSkill(KeyCode.I, SkillLibrary.GetSkill("替身术"));
        _player1.AttachSkill(KeyCode.O, SkillLibrary.GetSkill("闪光"));
        _player1.AttachSkill(KeyCode.S, SkillLibrary.GetSkill("逆风行"), true);
        _player1.AttachSkill(KeyCode.S, SkillLibrary.GetSkill("幽灵步"), true);
        _player1.AttachSkill(KeyCode.K, SkillLibrary.GetSkill("空手入白刃"));
        _player1.AttachSkill(KeyCode.K, SkillLibrary.GetSkill("朔月脚"));
        _player1.ApplyAllSkills();

        _player2.AttachSkill(KeyCode.Keypad1, SkillLibrary.GetSkill("迷雾斩"));
        _player2.AttachSkill(KeyCode.Keypad1, SkillLibrary.GetSkill("后滚翻"));
        _player2.AttachToggleSkill(KeyCode.Keypad1, SkillLibrary.GetSkill("木业疾斩"), KeyCode.DownArrow);
        _player2.AttachToggleSkill(KeyCode.Keypad1, SkillLibrary.GetSkill("木业闪现"), KeyCode.DownArrow);
        _player2.AttachSkill(KeyCode.Keypad2, SkillLibrary.GetSkill("刺心"));
        _player2.AttachToggleSkill(KeyCode.Keypad2, SkillLibrary.GetSkill("侧身闪"), KeyCode.DownArrow);
        _player2.AttachSkill(KeyCode.Keypad3, SkillLibrary.GetSkill("瞬步"));
        _player2.AttachSkill(KeyCode.Keypad4, SkillLibrary.GetSkill("潜行"));
        _player2.AttachToggleSkill(KeyCode.Keypad4, SkillLibrary.GetSkill("莲华脚"), KeyCode.DownArrow);
        _player2.AttachSkill(KeyCode.Keypad5, SkillLibrary.GetSkill("替身术"));
        _player2.AttachSkill(KeyCode.Keypad6, SkillLibrary.GetSkill("闪光"));
        _player2.AttachSkill(KeyCode.DownArrow, SkillLibrary.GetSkill("逆风行"), true);
        _player2.AttachSkill(KeyCode.DownArrow, SkillLibrary.GetSkill("幽灵步"), true);
        _player2.AttachSkill(KeyCode.Keypad2, SkillLibrary.GetSkill("空手入白刃"));
        _player2.AttachSkill(KeyCode.Keypad2, SkillLibrary.GetSkill("朔月脚"));
        _player2.ApplyAllSkills();
    }

    public SkillSlot GetSkillSlotByHotKey(KeyCode hotKey)
    {
        SkillSlot ret;
        _skillSlots.TryGetValue(hotKey, out ret);
        return ret;
    }

    private void InitSkillSlots()
    {
        var slots = GameObject.FindObjectsOfType<SkillSlot>();
        foreach (var slot in slots)
        {
            _skillSlots.Add(slot._hotKey, slot);
        }
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

        // 对所有技能键进行遍历
        foreach (var pair in _skillSlots)
        {
            var hotKey = pair.Key;
            var skillSlot = pair.Value;
            if (skillSlot != null && skillSlot._skill != null)
            {
                // 如果技能槽需要双击且双击了该技能键，则激活对应的技能
                if (skillSlot._skill._bDoubleClick && _doubleClickDetector.IsDoubleClick(hotKey))
                {
                    skillSlot.Activate();
                }
                // 如果技能槽不需要双击且按下了该技能键，则激活对应的技能
                else if (!skillSlot._skill._bDoubleClick && Input.GetKeyDown(hotKey))
                {
                    skillSlot.Activate();
                }
            }
        }
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
        Time.timeScale = 0.05f; // 减慢游戏运行
        _isGameOver = true;
        StartCoroutine(ShowGameOverPanelAfterDelay());
    }

    IEnumerator ShowGameOverPanelAfterDelay()
    {
        yield return new WaitForSeconds(0.1f);
        Time.timeScale = 0f; // 将时间缩放因子设为0，暂停游戏运行
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
    public GameObject _pausePanel;
    public GameObject _resultPanel;
    private bool isPaused = false;
    public bool _isGameOver = false;
    private Dictionary<KeyCode, SkillSlot> _skillSlots = new Dictionary<KeyCode, SkillSlot>();
    private DoubleClickDetector _doubleClickDetector = new DoubleClickDetector(0.3f);
}
