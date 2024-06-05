using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;

public class TmpPlusUtility
{
    //temp - 项目为异步加载资源
    public static async Task<Sprite> LoadSpriteById(int id)
    {
        Sprite sprite = TestTMPPlus.GetSprite(id);
        await Task.Delay(10);
        return sprite;
    }
    
    //获得Sprite原图片的宽高比
    public static float GetSpriteWidthHeightRate(Sprite sprite)
    {
        Rect rect = sprite.rect;
        return rect.width / rect.height;
    }
    
    //基于组件字体大小，获得Sprite在组件中的默认宽高
    public static Vector2 GetSpriteWidthHeight(float lineHeight, Sprite sprite)
    {
        float whRate = GetSpriteWidthHeightRate(sprite);
        return new Vector2(whRate * lineHeight, lineHeight);
    }
    
    public static Image AddSubImageObject(TextMeshProUGUI textComponent)
    {
        GameObject go = new GameObject("TMP_PLUS_SUB_IMAGE", typeof(RectTransform));
        go.hideFlags = HideFlags.DontSave;
        
        go.transform.SetParent(textComponent.transform, false);
        go.transform.SetAsFirstSibling();
        go.layer = textComponent.gameObject.layer;
        
        RectTransform rectTransform = go.GetComponent<RectTransform>();
        Vector2 pivotCenter = Vector2.one * 0.5f;
        rectTransform.anchorMin = pivotCenter;
        rectTransform.anchorMax = pivotCenter;
        rectTransform.pivot = textComponent.rectTransform.pivot;
        
        LayoutElement layoutElement = go.AddComponent<LayoutElement>();
        layoutElement.ignoreLayout = true;

        Image image = go.AddComponent<Image>();
        return image;
    }
}
