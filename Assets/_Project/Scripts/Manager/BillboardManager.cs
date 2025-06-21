using System.Collections;
using System.Collections.Generic;
using UCExtension;
using UnityEngine;

public class BillboardManager : Singleton<BillboardManager>
{
    private Camera mainCam = null;
    protected List<Billboard> billboards = null;

    protected override void Awake()
    {
        base.Awake();
        mainCam = Camera.main;
        billboards = new List<Billboard>();
    }

    public void Add(Billboard bill)
    {
        billboards.Add(bill);
    }

    public void Remove(Billboard bill)
    {
        billboards.Remove(bill);
    }

    private void FixedUpdate()
    {
        foreach (var bill in billboards)
                bill.LookAtCamera(mainCam);
    }
}
