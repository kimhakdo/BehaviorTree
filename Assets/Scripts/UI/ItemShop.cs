using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ItemShop : MonoBehaviour
{
    [SerializeField] private Image thumbnail;
    [SerializeField] private Text name;
    [SerializeField] private Text desc;
    [SerializeField] private Text price;

    UnityAction<ShopData> callback;
    ShopData data;

    public void Init(ShopData data, UnityAction<ShopData> callback)
    {
        this.data = data;
        var itemData = DataManager.Instance.GetData<ItemData>(data.target);
        this.callback = callback;
        this.name.text = data.displayName;
        thumbnail.sprite = ResourceManager.Instance.LoadAsset<Sprite>(itemData.rcode, ResourceType.Thumbnail);
        this.desc.text = itemData.desc;
        price.text = $"АЁАн : {data.price}";
        gameObject.SetActive(true);
    }

    public void OnClickBuy()
    {
        callback.Invoke(data);
    }
}
