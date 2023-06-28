using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YFrameWork;

public class BaseController : IController
{
    public IApp GetApp()
    {
        return BnsApp.Instance;
    }
}

public class BaseMonoController : MonoBehaviour, IController
{
    public IApp GetApp()
    {
        return BnsApp.Instance;
    }
}
