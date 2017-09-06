using System;
using System.Collections.Generic;
using System.Text;


public partial class TableCheckHelper
{

    /// <summary>
    /// 用于输入数据的非空检查，适用于int、long、float、string、lang、date、time、json或tableString型
    /// 注意：string类型要求字符串不能为空但允许为连续空格字符串，如果也不允许为连续空格字符串，需要声明为notEmpty[trim]
    /// 注意：lang类型声明notEmpty[key]只检查是否填写了key值，声明notEmpty[value]只检查填写的key在相应的lang文件中能找到对应的value，声明notEmpty[key|value]或notEmpty则包含这两个要求
    /// </summary>
    public static bool CheckNotEmpty(FieldInfo fieldInfo, FieldCheckRule checkRule, out string errorString)
    {
        // 存储检查出的空值对应的行号
        List<int> emptyDataLines = new List<int>();

        if (fieldInfo.DataType == DataType.String)
        {
            if (checkRule.CheckRuleString.Equals("notEmpty", StringComparison.CurrentCultureIgnoreCase))
            {
                _CheckInputDataNotEmpty(fieldInfo.Data, emptyDataLines, false);
            }
            else if (checkRule.CheckRuleString.Equals("notEmpty[trim]", StringComparison.CurrentCultureIgnoreCase))
            {
                _CheckInputDataNotEmpty(fieldInfo.Data, emptyDataLines, true);
            }
            else
            {
                errorString = string.Format("数据非空检查规则用于string型的声明错误，输入的检查规则字符串为{0}。而string型声明notEmpty要求字符串不能为空但允许为连续空格字符串，如果也不允许为连续空格字符串，需要声明为notEmpty[trim]\n", checkRule.CheckRuleString);
                return false;
            }
        }
        else if (fieldInfo.DataType == DataType.Lang)
        {
            if (checkRule.CheckRuleString.Equals("notEmpty[key]", StringComparison.CurrentCultureIgnoreCase))
            {
                _CheckInputDataNotEmpty(fieldInfo.LangKeys, emptyDataLines, true);
            }
            else if (checkRule.CheckRuleString.Equals("notEmpty[value]", StringComparison.CurrentCultureIgnoreCase))
            {
                for (int i = 0; i < fieldInfo.LangKeys.Count; ++i)
                {
                    // 忽略无效集合元素下属子类型的空值，忽略未填写key的
                    if (fieldInfo.LangKeys[i] == null || string.IsNullOrEmpty(fieldInfo.LangKeys[i].ToString()))
                        continue;

                    if (fieldInfo.Data[i] == null)
                        emptyDataLines.Add(i);
                }
            }
            else if (checkRule.CheckRuleString.Equals("notEmpty", StringComparison.CurrentCultureIgnoreCase) || checkRule.CheckRuleString.Equals("notEmpty[key|value]", StringComparison.CurrentCultureIgnoreCase))
            {
                for (int i = 0; i < fieldInfo.LangKeys.Count; ++i)
                {
                    // 忽略无效集合元素下属子类型的空值
                    if (fieldInfo.LangKeys[i] == null)
                        continue;

                    // 填写的key不能为空或连续空格字符串，且必须能在lang文件中找到对应的value
                    if (string.IsNullOrEmpty(fieldInfo.LangKeys[i].ToString().Trim()) || fieldInfo.Data[i] == null)
                        emptyDataLines.Add(i);
                }
            }
            else
            {
                errorString = string.Format("数据非空检查规则用于lang型的声明错误，输入的检查规则字符串为{0}。而lang型声明notEmpty[key]只检查是否填写了key值，声明notEmpty[value]只检查填写的key在相应的lang文件中能找到对应的value，声明notEmpty[key|value]或notEmpty则包含这两个要求\n", checkRule.CheckRuleString);
                return false;
            }
        }
        else if (fieldInfo.DataType == DataType.Int || fieldInfo.DataType == DataType.Long || fieldInfo.DataType == DataType.Float)
        {
            if (AppValues.IsAllowedNullNumber == true)
            {
                for (int i = 0; i < fieldInfo.Data.Count; ++i)
                {
                    // 如果int、long、float型字段下取值为null，可能填写的为空值，也可能是父集合类型标为无效
                    if (fieldInfo.ParentField != null)
                    {
                        if ((bool)fieldInfo.ParentField.Data[i] == false)
                            continue;
                        else if (fieldInfo.ParentField.ParentField != null && (bool)fieldInfo.ParentField.ParentField.Data[i] == false)
                            continue;
                    }
                    else if (fieldInfo.Data[i] == null)
                        emptyDataLines.Add(i);
                }
            }
        }
        else if (fieldInfo.DataType == DataType.Date || fieldInfo.DataType == DataType.Time)
        {
            for (int i = 0; i < fieldInfo.Data.Count; ++i)
            {
                // 如果date、time型字段下取值为null，可能填写的为空值，也可能是父集合类型标为无效
                if (fieldInfo.ParentField != null)
                {
                    if ((bool)fieldInfo.ParentField.Data[i] == false)
                        continue;
                    else if (fieldInfo.ParentField.ParentField != null && (bool)fieldInfo.ParentField.ParentField.Data[i] == false)
                        continue;
                }
                else if (fieldInfo.Data[i] == null)
                    emptyDataLines.Add(i);
            }
        }
        else if (fieldInfo.DataType == DataType.Json || fieldInfo.DataType == DataType.TableString)
        {
            for (int i = 0; i < fieldInfo.Data.Count; ++i)
            {
                // json、tableString类型必为独立字段
                if (fieldInfo.Data[i] == null)
                    emptyDataLines.Add(i);
            }
        }
        else
        {
            errorString = string.Format("数据非空检查规则只适用于int、long、float、string、lang、date、time、json或tableString类型的字段，要检查的这列类型为{0}\n", fieldInfo.DataType.ToString());
            return false;
        }

        if (emptyDataLines.Count > 0)
        {
            StringBuilder errorStringBuild = new StringBuilder();
            errorStringBuild.Append("存在以下空数据，行号分别为：");
            string separator = ", ";
            foreach (int lineNum in emptyDataLines)
                errorStringBuild.AppendFormat("{0}{1}", lineNum + AppValues.DATA_FIELD_DATA_START_INDEX + 1, separator);

            // 去掉末尾多余的", "
            errorStringBuild.Remove(errorStringBuild.Length - separator.Length, separator.Length);

            errorStringBuild.Append("\n");
            errorString = errorStringBuild.ToString();

            return false;
        }
        else
        {
            errorString = null;
            return true;
        }
    }

    /// <summary>
    /// 检查List中的string型的数据是否为空，传入的needTrim表示检查前是否对字符串进行trim操作
    /// </summary>
    private static bool _CheckInputDataNotEmpty(List<object> data, List<int> emptyDataLines, bool needTrim)
    {
        if (needTrim == true)
        {
            for (int i = 0; i < data.Count; ++i)
            {
                // 忽略无效集合元素下属子类型的空值
                if (data[i] == null)
                    continue;

                if (string.IsNullOrEmpty(data[i].ToString().Trim()))
                    emptyDataLines.Add(i);
            }
        }
        else
        {
            for (int i = 0; i < data.Count; ++i)
            {
                // 忽略无效集合元素下属子类型的空值
                if (data[i] == null)
                    continue;

                if (string.IsNullOrEmpty(data[i].ToString()))
                    emptyDataLines.Add(i);
            }
        }

        return data.Count == 0;
    }

}

