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

        _owner.SetPlayerUI(this);
    }

    public void UpdateUI()
    {
        _txtHP.text = _owner._hp.ToString();
        _txtNeiLi.text = _owner._neiLi.ToString();
        _hpSlider.value = (float)_owner._hp / _owner._attr.maxHP;
        _neiLiSlider.value = (float)_owner._neiLi/ _owner._attr.maxNeiLi;
    }

    public Slider _hpSlider;
    public Text _txtHP;
    public Slider _neiLiSlider;
    public Text _txtNeiLi;
    public Player _owner;
}
