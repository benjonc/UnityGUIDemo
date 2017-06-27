using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollViewTest1 : MonoBehaviour
{
    public UIScrollView ScrollView;
    public UIWrapContent Wrap;
    public int DataLen;
    public bool Reset;

    private Dictionary<Transform, ItemConfig> _itemsConfig;
    private Dictionary<Transform, ItemInfo> _itemsInfo;

    private struct ItemConfig
    {
        public Transform it;
        public Transform root;
        public UISprite sp;
        public UILabel lb;
    }

    private struct ItemInfo
    {
        public Transform it;
        public int index;
    }
    
    void Start()
    {
        Reset = true;
        DataLen = 6;
        InitItems();
        SetWrap();
    }

    
    void Update()
    {
        if(Reset)
        {
            Reset = false;
            SetWrap();
        }
    }

    private void InitItems()
    {
        Wrap.onInitializeItem = OnWrapUpdate;
        _itemsConfig = new Dictionary<Transform, ItemConfig>();
        _itemsInfo = new Dictionary<Transform, ItemInfo>();
        int len = Wrap.transform.childCount;
        for(int i = 0; i < len; i++)
        {
            string name = string.Format("Item{0}", i + 1);
            var item = Wrap.transform.FindChild(name);
            if(item)
            {
                var root = item.FindChild("ItemRoot");
                var sp = root.FindChild("Sprite").GetComponent<UISprite>();
                var lb = sp.transform.FindChild("Label").GetComponent<UILabel>();
                ItemConfig itcg = new ItemConfig();
                itcg.it = item;
                itcg.root = root;
                itcg.sp = sp;
                itcg.lb = lb;
                _itemsConfig.Add(item, itcg);
                UIEventListener.Get(item.gameObject).onClick = OnItemClick;
            }
        }
    }

    private void SetWrap()
    {
        ScrollView.ResetPosition();
        ScrollView.transform.localPosition = Vector3.zero;
        ScrollView.panel.clipOffset = Vector2.zero;
        int minIndex = -DataLen;
        Wrap.minIndex = minIndex;
        Wrap.SortBasedOnScrollMovement();
    }

    private void OnWrapUpdate(GameObject go, int wrapIndex, int realIndex)
    {
        int index = -realIndex + 1;
        var itcg = _itemsConfig[go.transform];
        if(index > DataLen)
        {
            itcg.root.gameObject.SetActive(false);
        }
        else
        {
            itcg.root.gameObject.SetActive(true);
            itcg.lb.text = index.ToString();
            ItemInfo itfo = new ItemInfo();
            itfo.it = go.transform;
            itfo.index = index;
            if(_itemsInfo.ContainsKey(go.transform))
            {
                _itemsInfo[go.transform] = itfo;
            }
            else
            {
                _itemsInfo.Add(go.transform, itfo);
            }
        }
    }

    private void OnItemClick(GameObject go)
    {
        var itfo = _itemsInfo[go.transform];
        Debug.Log(string.Format("You on click item index is :{0}", itfo.index));
    }
}
