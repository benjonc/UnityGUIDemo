using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollViewTest : MonoBehaviour
{
    public UIScrollView ScrollView;
    public UIWrapContent Wrap;
    public bool Reset;    
    public int DataLen;

    void Start()
    {
        Reset = true;
        _items = new Dictionary<Transform, List<ItemConfig>>();
        _itemInfos = new Dictionary<Transform, ItemInfo>();
        InitItems();

        Wrap.onInitializeItem = OnWrapUpdate;
    }

    
    void Update()
    {
        if(Reset)
        {
            Reset = false;
            SetWrap(DataLen);
        }
    }

    

    private void SetWrap(int dataLen)
    {
        ScrollView.ResetPosition();
        ScrollView.transform.localPosition = Vector3.zero;
        ScrollView.panel.clipOffset = Vector2.zero;
        Wrap.minIndex = GetMinIndexByLen(dataLen);
        Wrap.SortBasedOnScrollMovement();
    }

    // ------------------------------多行多列
    private Dictionary<Transform, List<ItemConfig>> _items;
    private Dictionary<Transform, ItemInfo> _itemInfos;
    private int _col, _row;
    private struct ItemConfig
    {
        public Transform it;
        public UISprite sp;
        public UILabel lb;
    }

    private struct ItemInfo
    {
        public Transform it;
        public int index;
    }

    // 先取出来,不用在Update的时候再去取,会非常耗费运算量
    private void InitItems()
    {
        // 横行竖列 行row 列 column
        _col = 6;
        _row = 9;
        for(int i = 0; i < _row; i++)
        {
            string name1 = string.Format("Items{0}", i + 1);
            var items = Wrap.transform.FindChild(name1);
            List<ItemConfig> itemList = new List<ItemConfig>();
            for (int j = 0; j < _col; j++)
            {
                string name2 = string.Format("Item{0}", j + 1);
                var item = items.FindChild(name2);

                var sp = item.FindChild("Sprite").GetComponent<UISprite>();
                var lb = sp.transform.FindChild("Label").GetComponent<UILabel>();

                UIEventListener.Get(item.gameObject).onClick = OnClickItem;

                ItemConfig it = new ItemConfig();
                it.sp = sp;
                it.lb = lb;
                it.it = item;

                itemList.Add(it);
            }
            _items.Add(items, itemList);
        }

    }

    private int GetMinIndexByLen(int dataLen)
    {
        float min = (float)dataLen / (float)_col;
        int index = Mathf.CeilToInt(min);
        Debug.Log(string.Format("Min index is :{0}", index));
        return -index;
    }

    private void OnWrapUpdate(GameObject go, int wrapIndex, int realIndex)
    {        
        for(int i = 0; i < _col; i++)
        {
            int index = -realIndex * _col + (i + 1);
            var itemInfo = _items[go.transform];
            itemInfo[i].lb.text = index.ToString();
            itemInfo[i].it.gameObject.SetActive(true);
            if (index > DataLen)
            {
                itemInfo[i].it.gameObject.SetActive(false);
            }
            ItemInfo itfo = new ItemInfo();
            itfo.it = itemInfo[i].it;
            itfo.index = index;

            if (!_itemInfos.ContainsKey(itemInfo[i].it))
            {
                _itemInfos.Add(itemInfo[i].it, itfo);
            }
            else
            {
                _itemInfos[itemInfo[i].it] = itfo;
            }
        }
    }

    private void OnClickItem(GameObject go)
    {
        var info = _itemInfos[go.transform];
        Debug.Log(string.Format("You clcik the item real index is : {0}", info.index));
    }
    // ------------------------------多行多列
}
