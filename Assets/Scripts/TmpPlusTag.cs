using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TmpPlusImgTag
{
    public Image subImage;  //被创建出来的Image组件对象
    public Sprite sprite;   //异步加载得到的sprite对象
    public int beginIndex;  //字符串位置
    public int replaceSpaceCount;
    
    //标签生成
    public int spriteId;
    public float x;
    public float y;
    public float width;
    public float height;
    
    public TmpPlusImgTag(Image image)
    {
        this.subImage = image;
        this.SetUnActive();
    }

    public void OnDestroy()
    {
        sprite = null;
        GameObject.Destroy(subImage);
    }

    public string GetReplaceString(float spaceWidth)
    {
        int spaceCount = (int)Mathf.Ceil(width / spaceWidth);
        if (spaceCount < 1)
            spaceCount = 1;
        this.replaceSpaceCount = spaceCount;
        return new string(' ', spaceCount);
    }

    public void SetUnActive()
    {
        sprite = null;
        subImage.enabled = false;
    }
    public void SetActive()
    {
        spriteId = -1;
        x = 0;
        y = 0;
        width = -1;
        height = -1;
    }

    public void SetBeginIndex(int index)
    {
        if (index < 0)
        {
            index = 0;
        }
        this.beginIndex = index;
    }

    public void SetSprite(Sprite sprite)
    {
        subImage.sprite = sprite;
        subImage.enabled = true;
    }

    public void SetWidthHeight(float w, float h)
    {
        if (width < 0)
        {
            width = w;
        }
        if (height < 0)
        {
            height = h;
        }
        subImage.rectTransform.sizeDelta = new Vector2(width, height);
    }

    public void SetPos(Vector3 pos)
    {
        subImage.rectTransform.localPosition = new Vector3(pos.x + x, pos.y + y, pos.z);
    }
}