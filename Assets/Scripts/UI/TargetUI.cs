using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TargetUI : MonoBehaviour
{
    private void Awake()
    {
        _text = GetComponentInChildren<Text>();
    }

    public void UpdateDistance(float distance)
    {
        _text.text = distance.ToString("f1") + "米";
    }

    public void Hide()
    {
        _text.text = "";
    }

    public void Init()
    {
        Hide();
    }

    private void Update()
    {
        if (_target == null) return;
        transform.position = _target.position;
    }
    public void SetTarget(Transform target)
    {
        _target = target;
    }

    Text _text;
    Transform _target;
}
