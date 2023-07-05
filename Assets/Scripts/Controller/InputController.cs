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

        _player1.AttachSkill(KeyCode.J, SkillLibrary.CreateSkillByName("迷雾斩"));
        _player1.AttachSkill(KeyCode.K, SkillLibrary.CreateSkillByName("刺心"));
        _player1.AttachSkill(KeyCode.L, SkillLibrary.CreateSkillByName("瞬步"));
        _player1.AttachSkill(KeyCode.U, SkillLibrary.CreateSkillByName("莲华脚"));
        _player1.AttachSkill(KeyCode.I, SkillLibrary.CreateSkillByName("替身术"));
        _player1.AttachSkill(KeyCode.O, SkillLibrary.CreateSkillByName("闪光"));
        _player1.AttachSkill(KeyCode.W, SkillLibrary.CreateSkillByName("潜行"));
        _player1.AttachSkill(KeyCode.S, SkillLibrary.CreateSkillByName("逆风行"));
        _player1.AttachSkill(KeyCode.K, SkillLibrary.CreateSkillByName("空手入白刃"));

        _player2.AttachSkill(KeyCode.Keypad1, SkillLibrary.CreateSkillByName("迷雾斩"));
        _player2.AttachSkill(KeyCode.Keypad2, SkillLibrary.CreateSkillByName("刺心"));
        _player2.AttachSkill(KeyCode.Keypad3, SkillLibrary.CreateSkillByName("瞬步"));
        _player2.AttachSkill(KeyCode.Keypad4, SkillLibrary.CreateSkillByName("莲华脚"));
        _player2.AttachSkill(KeyCode.Keypad5, SkillLibrary.CreateSkillByName("替身术"));
        _player2.AttachSkill(KeyCode.Keypad6, SkillLibrary.CreateSkillByName("闪光"));
        _player2.AttachSkill(KeyCode.UpArrow, SkillLibrary.CreateSkillByName("潜行"));
        _player2.AttachSkill(KeyCode.DownArrow, SkillLibrary.CreateSkillByName("逆风行"));
        _player2.AttachSkill(KeyCode.Keypad2, SkillLibrary.CreateSkillByName("空手入白刃"));
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

        foreach (var pair in _skillSlots)
        {
            var _hotKey = pair.Key;
            var slot = pair.Value;
            if (Input.GetKeyDown(slot._hotKey))
            {
                slot.Activate();
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
    Dictionary<KeyCode, SkillSlot> _skillSlots = new Dictionary<KeyCode, SkillSlot>();
}
