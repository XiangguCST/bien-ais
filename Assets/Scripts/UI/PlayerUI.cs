using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{

    private void Awake()
    {
        var hp = transform.Find("HP");
        var neiLi = transform.Find("NeiLi");
        _hpSlider = hp.GetComponent<Slider>();
        _neiLiSlider = neiLi.GetComponent<Slider>();
        _txtHP = hp.GetComponentInChildren<Text>();
        _txtNeiLi = neiLi.GetComponentInChildren<Text>();
    }

    void Start()
    {
        _owner.OnStatusBarsChanged += UpdateUI;
    }

    void OnDestroy()
    {
        _owner.OnStatusBarsChanged -= UpdateUI;
    }

    public void UpdateUI(Character owner)
    {
        _txtHP.text = owner._hp.ToString();
        _txtNeiLi.text = owner._energy.ToString();
        _hpSlider.value = (float)owner._hp / owner._attr.maxHP;
        _neiLiSlider.value = (float)owner._energy/ owner._attr.maxEnergy;
    }

    public Slider _hpSlider;
    public Text _txtHP;
    public Slider _neiLiSlider;
    public Text _txtNeiLi;
    public Player _owner;
}
