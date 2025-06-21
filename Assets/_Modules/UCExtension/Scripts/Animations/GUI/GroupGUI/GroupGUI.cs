using System.Collections;
using System.Collections.Generic;
using UCExtension.GUI;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public abstract class GroupGUI<T> : BasePopupGUI
{
    [SerializeField] BaseTab<T> tabPrefab;

    [SerializeField] Transform tabsParent;

    List<BaseTab<T>> tabs = new List<BaseTab<T>>();

    BaseTab<T> selectedTab;

    protected abstract List<T> Datas { get; }

    protected abstract List<string> Titles { get; }

    protected abstract void Display(T data);

    public virtual int OpenTabIndex => 0;

    protected override void SetUp()
    {
        base.SetUp();
        SetDatas(Datas);
    }

    public override void Open()
    {
        base.Open();
        SetTab(OpenTabIndex);
    }

    public void SetTab(int index)
    {
        SetSelectedTab(tabs[index]);
    }

    public void SetPosition(int position)
    {
        GoTo(position);

    }

    protected virtual void GoTo(int position)
    {

    }

    public void SetDatas(List<T> datas)
    {
        int index = 0;
        foreach (var item in datas)
        {
            var tab = Instantiate(tabPrefab, tabsParent);
            tab.OnSelect += OnSelectTab;
            tab.SetData(item);
            tab.SetSelect(false);
            tab.SetTitle(Titles[index]);
            tabs.Add(tab);
            index++;
        }
    }

    void OnSelectTab(BaseTab<T> tab)
    {
        SetSelectedTab(tab);
    }

    void SetSelectedTab(BaseTab<T> tab)
    {
        if (selectedTab)
        {
            selectedTab.SetSelect(false);
        }
        selectedTab = tab;
        selectedTab.SetSelect(true);
        Display(selectedTab.Data);
    }
}
