using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using System;

/// <summary>
/// 用于直接控制 TextMeshProPlus 进行文本测试
/// </summary>
public class TestTMPPlus : MonoBehaviour, IPointerClickHandler
{
    public static TestTMPPlus Instance;
    public Sprite[] spriteArray;

    public TextMeshProPlus tmpPlus;
    [TextArea]
    public string contentTxt;

    private void Awake()
    {
        Instance = this;
        InitSpriteArray();

        // tmpPlus = this.GetComponent<TextMeshProPlus>();
    }

    private void Start()
    {
        tmpPlus.SetTextPlus(contentTxt);
    }

    //<link=ID>点击测试</link>
    public void OnPointerClick(PointerEventData eventData)
    {
        int linkIndex = TMP_TextUtilities.FindIntersectingLink(tmpPlus, eventData.position, null);
        if (linkIndex != -1)
        {
            TMP_LinkInfo linkInfo = tmpPlus.textInfo.linkInfo[linkIndex];
            Debug.LogError($"点击了这个事件：{linkInfo.GetLinkID()}");
        }
    }

    //temp - 加载Sprite
    private void InitSpriteArray()
    {
        spriteArray = Resources.LoadAll<Sprite>("");
    }
    public static Sprite GetSprite(int id)
    {
        return Instance.spriteArray[id];
    }

    //点击本组件在inspector界面右边的三个点即可看到此选项
    [ContextMenu("测试文本")]
    public void RunTMP()
    {
        InitSpriteArray();
        tmpPlus.SetTextPlus(contentTxt);
    }
}
