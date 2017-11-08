using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;

public partial class TableCheckHelper
{
    /// <summary>
    /// 用于递归对表格进行数据完整性检查
    /// </summary>
    public static void _CheckIntegrity(List<FieldInfo> indexField, Dictionary<object, object> parentDict, List<object> parentKeys, List<List<object>> effectiveValues, ref int currentLevel, StringBuilder errorStringBuilder)
    {
        if (effectiveValues[currentLevel] != null)
        {
            List<object> inputData = new List<object>(parentDict.Keys);
            foreach (object value in effectiveValues[currentLevel])
            {
                if (!inputData.Contains(value))
                {
                    if (currentLevel > 0)
                    {
                        StringBuilder parentKeyInfoBuilder = new StringBuilder();
                        for (int i = 0; i <= currentLevel - 1; ++i)
                            parentKeyInfoBuilder.AppendFormat("{0}={1},", indexField[i].FieldName, parentKeys[i]);

                        string parentKeyInfo = parentKeyInfoBuilder.ToString().Substring(0, parentKeyInfoBuilder.Length - 1);
                        errorStringBuilder.AppendFormat("字段\"{0}\"（列号：{1}）缺少在{2}情况下值为\"{3}\"的数据\n", indexField[currentLevel].FieldName, Utils.GetExcelColumnName(indexField[currentLevel].ColumnSeq + 1), parentKeyInfo, value);
                    }
                    else
                        errorStringBuilder.AppendFormat("字段\"{0}\"（列号：{1}）缺少值为\"{2}\"的数据\n", indexField[currentLevel].FieldName, Utils.GetExcelColumnName(indexField[currentLevel].ColumnSeq + 1), value);
                }
            }
        }

        if (currentLevel < effectiveValues.Count - 1)
        {
            foreach (var key in parentDict.Keys)
            {
                parentKeys[currentLevel] = key;
                ++currentLevel;
                _CheckIntegrity(indexField, (Dictionary<object, object>)(parentDict[key]), parentKeys, effectiveValues, ref currentLevel, errorStringBuilder);
                --currentLevel;
            }
        }
    }

    public static bool CheckTable(TableInfo tableInfo, out string errorString)
    {
        StringBuilder errorStringBuilder = new StringBuilder();
        // 字段检查
        StringBuilder fieldErrorBuilder = new StringBuilder();
        foreach (FieldInfo fieldInfo in tableInfo.GetAllFieldInfo())
        {
            CheckOneField(fieldInfo, out errorString);
            if (errorString != null)
            {
                fieldErrorBuilder.Append(errorString);
                errorString = null;
            }
        }
        string fieldErrorString = fieldErrorBuilder.ToString();
        if (!string.IsNullOrEmpty(fieldErrorString))
            errorStringBuilder.Append("字段检查中发现以下错误：\n").Append(fieldErrorString);

        // 整表检查
        TableCheckHelper.CheckTableFunc(tableInfo, out errorString);
        if (errorString != null)
        {
            errorStringBuilder.Append("整表检查中发现以下错误：\n").Append(errorString);
            errorString = null;
        }

        errorString = errorStringBuilder.ToString();
        if (string.IsNullOrEmpty(errorString))
        {
            errorString = null;
            return true;
        }
        else
            return false;
    }

    private static bool CheckOneField(FieldInfo fieldInfo, out string errorString)
    {
        StringBuilder errorStringBuilder = new StringBuilder();

        // 解析检查规则
        List<FieldCheckRule> checkRules = GetCheckRules(fieldInfo.CheckRule, out errorString);
        if (errorString != null)
        {
            errorStringBuilder.AppendFormat("字段\"{0}\"（列号：{1}）填写的检查规则\"{2}\"不合法：{3}，不得不跳过对该字段的检查\n\n", fieldInfo.FieldName, Utils.GetExcelColumnName(fieldInfo.ColumnSeq + 1), fieldInfo.CheckRule, errorString);
            errorString = null;
        }
        else if (checkRules != null)
        {
            CheckByRules(checkRules, fieldInfo, out errorString);
            if (errorString != null)
            {
                errorStringBuilder.Append(errorString);
                errorString = null;
            }
        }

        // array或dict下属子元素字段的检查
        if (fieldInfo.DataType == DataType.Array || fieldInfo.DataType == DataType.Dict)
        {
            foreach (FieldInfo childField in fieldInfo.ChildField)
            {
                CheckOneField(childField, out errorString);
                if (errorString != null)
                {
                    errorStringBuilder.Append(errorString);
                    errorString = null;
                }
            }
        }

        errorString = errorStringBuilder.ToString();
        if (string.IsNullOrEmpty(errorString))
        {
            errorString = null;
            return true;
        }
        else
            return false;
    }

    public static bool CheckByRules(List<FieldCheckRule> checkRules, FieldInfo fieldInfo, out string errorString)
    {
        StringBuilder errorStingBuilder = new StringBuilder();
        errorString = null;

        foreach (FieldCheckRule checkRule in checkRules)
        {
            switch (checkRule.CheckType)
            {
                case TableCheckType.Unique:
                    {
                        CheckUnique(fieldInfo, checkRule, out errorString);
                        break;
                    }
                case TableCheckType.NotEmpty:
                    {
                        CheckNotEmpty(fieldInfo, checkRule, out errorString);
                        break;
                    }
                case TableCheckType.RefStr:
                    {
                        CheckRefStr(fieldInfo, checkRule, out errorString);
                        break;
                    }
                case TableCheckType.Ref:
                    {
                        CheckRef(fieldInfo, checkRule, out errorString);
                        break;
                    }
                case TableCheckType.Range:
                    {
                        CheckRange(fieldInfo, checkRule, out errorString);
                        break;
                    }
                case TableCheckType.Effective:
                    {
                        CheckEffective(fieldInfo, checkRule, out errorString);
                        break;
                    }
                case TableCheckType.Illegal:
                    {
                        CheckIllegal(fieldInfo, checkRule, out errorString);
                        break;
                    }
                case TableCheckType.GreaterThan:
                    {
                        CheckGreaterThan(fieldInfo, checkRule, out errorString);
                        break;
                    }
                case TableCheckType.File:
                    {
                        CheckFile(fieldInfo, checkRule, out errorString);
                        break;
                    }
                case TableCheckType.MapString:
                    {
                        MapStringCheckHelper.CheckMapString(fieldInfo, checkRule, out errorString);
                        break;
                    }
                case TableCheckType.Func:
                    {
                        CheckFunc(fieldInfo, checkRule, out errorString);
                        break;
                    }
                default:
                    {
                        Utils.LogErrorAndExit(string.Format("用CheckTable函数解析出了检查规则，但没有对应的检查函数，检查规则类型为{0}", checkRule.CheckType));
                        break;
                    }
            }

            if (errorString != null)
            {
                errorStingBuilder.AppendFormat("字段\"{0}\"（列号：{1}）未通过\"{2}\"的检查规则\n{3}\n", fieldInfo.FieldName, Utils.GetExcelColumnName(fieldInfo.ColumnSeq + 1), checkRule.CheckRuleString, errorString);
                errorString = null;
            }
        }

        if (string.IsNullOrEmpty(errorStingBuilder.ToString()))
        {
            errorString = null;
            return true;
        }
        else
        {
            errorString = errorStingBuilder.ToString();
            return false;
        }
    }


}

/// <summary>
/// 表格检查规则类型
/// </summary>
public enum TableCheckType
{
    Invalid,

    Range,        // 数值范围检查
    Effective,    // 值有效性检查（填写值必须是几个合法值中的一个）
    Illegal,      // 非法值检查（填写值不允许为几个非法值中的一个）
    NotEmpty,     // 值非空检查
    Unique,       // 值唯一性检查
    RefStr,          // 值引用检查（某个数值必须为另一个表格中某字段中存在的值）
    Ref,          // 值引用检查（某个数值必须为另一个表格中某字段中存在的值）
    GreaterThan,  // 值大小比较检查（同一行中某个字段的值必须大于另一字段的值）
    Func,         // 自定义检查函数
    File,         // 文件存在性检查
    MapString,    // mapString类型的内容检查
}

public struct FieldCheckRule
{
    public TableCheckType CheckType;
    public string CheckRuleString;
}
