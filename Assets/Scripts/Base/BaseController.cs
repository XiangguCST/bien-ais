using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YFrameWork;

public class BaseController : MonoBehaviour, IController
{
    public IApp GetApp()
    {
        return BnsApp.Instance;
    }
}

