using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Timeline;
using UnityEngine;
using UnityEngine.UI;
using System.Text.RegularExpressions;
using TMPro;
using Match = System.Text.RegularExpressions.Match;
using UnityEngine.Pool;
using System.Threading.Tasks;

public class TextMeshProPlus : TextMeshProUGUI
{
    protected List<TmpPlusImgTag> _imgTagList = new List<TmpPlusImgTag>();
    public int lastUnActiveImgTagIndex = 0;

    protected override void GenerateTextMesh()
    {
        base.GenerateTextMesh();
        this.PostDealImgTag();
    }

    public void SetTextPlus(string txt)
    {
        this.ClearImgTag();

        this.InnerAsyncSetText(txt);
    }

    private void InnerAsyncSetText(string txt)
    {
        if (!m_isRichText)
        {
            this.SetText(txt);
            return;
        }

        ParseImgTagString(txt, _InnerSetText);
    }
    private void _InnerSetText(string txt)
    {
    }

    #region ImgTag部分

    public TmpPlusImgTag GetUnActiveImgTag()
    {
        if (_imgTagList.Count <= lastUnActiveImgTagIndex)
        {
            TmpPlusImgTag imgTag = new TmpPlusImgTag(TmpPlusUtility.AddSubImageObject(this));
            _imgTagList.Add(imgTag);
            lastUnActiveImgTagIndex++;
            return imgTag;
        }
        else
        {
            TmpPlusImgTag imgTag = _imgTagList[lastUnActiveImgTagIndex];
            lastUnActiveImgTagIndex++;
            return imgTag;
        }
    }

    public void ClearImgTag()
    {
        lastUnActiveImgTagIndex = 0;
        foreach (var imgTag in _imgTagList)
        {
            imgTag.SetUnActive();
        }
    }

    //解析<img id=1 x=0 y=0 width=100 height=100>
    public void ParseImgTag(string text, TmpPlusImgTag imgTagData)
    {
        text = text.TrimStart('<').TrimEnd('>');
        string[] strs = text.Split(' ');
        foreach (string str in strs)
        {
            string[] valueStrs = str.Split('=');
            int intValue;
            if (valueStrs.Length == 2)
            {
                switch (valueStrs[0])
                {
                    case "id": //图片id
                        intValue = int.Parse(valueStrs[1]);
                        imgTagData.spriteId = intValue;
                        break;
                    case "x": //图片x偏移
                        intValue = int.Parse(valueStrs[1]);
                        imgTagData.x = intValue;
                        break;
                    case "y": //图片y偏移
                        intValue = int.Parse(valueStrs[1]);
                        imgTagData.y = intValue;
                        break;
                    case "w": //图片宽度
                        intValue = int.Parse(valueStrs[1]);
                        imgTagData.width = intValue;
                        break;
                    case "h": //图片高度
                        intValue = int.Parse(valueStrs[1]);
                        imgTagData.height = intValue;
                        break;
                    default:
                        throw new Exception($"TMPPlus:img-tag解析错误：{text}");
                        break;
                }
            }
        }
    }

    //imgTag处理为 TmpPlusImgTag List
    public async void ParseImgTagString(string text, Action<string> callback)
    {
        Regex regex = new Regex("<img([^>]+)>");
        Match match = regex.Match(text);

        while (match.Success)
        {
            string value = match.Value;

            TmpPlusImgTag imgTagData = this.GetUnActiveImgTag();
            imgTagData.SetActive();
            this.ParseImgTag(value, imgTagData);
            imgTagData.SetBeginIndex(match.Index - 1);

            Task<Sprite> task = TmpPlusUtility.LoadSpriteById(imgTagData.spriteId);
            await task;
            Sprite sprite = task.Result;

            Vector2 wh = TmpPlusUtility.GetSpriteWidthHeight(this.m_fontSize, sprite);
            imgTagData.SetWidthHeight(wh.x, wh.y);
            imgTagData.SetSprite(sprite);
            string replaceStr = imgTagData.GetReplaceString(this.GetSpaceWidth());
            text = regex.Replace(text, replaceStr, 1);

            match = regex.Match(text);
        }

        this.SetText(text);
    }

    //文本Mesh生成后，遍历 TmpPlusImgTag List 将图片摆放至正确的位置
    public void PostDealImgTag()
    {
        var info = m_textInfo;

        for (int i = 0; i < _imgTagList.Count; i++)
        {
            TmpPlusImgTag tagData = _imgTagList[i];
            int beginIndex = tagData.beginIndex;
            TMP_CharacterInfo cInfo = info.characterInfo[beginIndex];
            if (beginIndex != cInfo.index)
            {
                for (int ci = beginIndex - 1; ci >= 0; ci--)
                {
                    var tempCInfo = info.characterInfo[ci];
                    if (beginIndex >= tempCInfo.index && tempCInfo.scale != 0)
                    {
                        cInfo = info.characterInfo[ci];
                        break;
                    }
                }
            }

            float beginPosX = cInfo.xAdvance;
            if (cInfo.character == ' ' && cInfo.index == 0) //行首空格按origin作为起点
            {
                beginPosX = cInfo.origin;
            }
            Vector3 imgPos = new Vector3(
                beginPosX + ((tagData.replaceSpaceCount+2) * this.GetSpaceWidth() / 2),
                (cInfo.ascender + cInfo.descender) / 2,
                this.rectTransform.position.z
            );

            //获取图片高度以改变行高
            int lineIndex = cInfo.lineNumber;
            float lineHeight = info.lineInfo[lineIndex].lineHeight;
            info.lineInfo[lineIndex].lineHeight = Mathf.Max(lineHeight, tagData.height);

            tagData.SetPos(imgPos);
        }
    }

    #endregion

    #region Utility

    //获取空格宽度 = tabWidth * 字体缩放
    public float GetSpaceWidth()
    {
        float sw = font.faceInfo.tabWidth * (m_fontSize / font.faceInfo.pointSize);
        return sw;
    }

    #endregion
}
