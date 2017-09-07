using System;
using System.Collections.Generic;
using System.Globalization;

public partial class TableCheckHelper
{
    /// <summary>
    /// 获取某字段的所有数据中属于或不属于指定集合中取值的数据所在索引
    /// </summary>
    /// <param name="data">字段数据</param>
    /// <param name="dataType">字段数据类型</param>
    /// <param name="setValueDefineString">集合取值的定义字符串</param>
    /// <param name="isInSet">获取属于还是不属于集合中数据所在索引</param>
    /// <param name="repeatedSetValue">集合定义字符串中存在的重复定义值</param>
    /// <param name="errorDataIndex">不满足属于或不属于要求的数据所在索引</param>
    /// <param name="errorString">检查规则定义错误信息</param>
    private static void _GetValueIsInSet(List<object> data, DataType dataType, string setValueDefineString, bool isInSet, out List<object> repeatedSetValue, out List<int> errorDataIndex, out string errorString)
    {
        repeatedSetValue = new List<object>();
        errorDataIndex = new List<int>();
        errorString = null;

        if (dataType == DataType.Int || dataType == DataType.Long || dataType == DataType.Float || dataType == DataType.Date || dataType == DataType.Time)
        {
            // 去除首尾花括号后，通过英文逗号分隔每个集合值即可
            if (!(setValueDefineString.StartsWith("{") && setValueDefineString.EndsWith("}")))
            {
                errorString = "集合值定义错误：必须在首尾用一对花括号包裹整个定义内容";
                return;
            }
            string temp = setValueDefineString.Substring(1, setValueDefineString.Length - 2).Trim();
            if (string.IsNullOrEmpty(temp))
            {
                errorString = "集合值定义错误：至少需要输入一个值";
                return;
            }

            string[] values = temp.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            if (dataType == DataType.Int || dataType == DataType.Long)
            {
                // 存储用户设置的取值集合（key：取值， value：恒为true）
                Dictionary<long, bool> setValues = new Dictionary<long, bool>();
                for (int i = 0; i < values.Length; ++i)
                {
                    string oneValueString = values[i].Trim();
                    long oneValue;
                    if (long.TryParse(oneValueString, out oneValue) == true)
                    {
                        // 记录集合定义字符串中的重复值
                        if (setValues.ContainsKey(oneValue))
                            repeatedSetValue.Add(oneValue);
                        else
                            setValues.Add(oneValue, true);
                    }
                    else
                    {
                        errorString = string.Format("集合值定义中出现了非{0}型数据，其为\"{1}\"\n", dataType, oneValueString);
                        return;
                    }
                }
                // 对本列所有数据进行检查
                if (isInSet == true)
                {
                    for (int i = 0; i < data.Count; ++i)
                    {
                        // 忽略无效集合元素下属子类型的空值或本身为空值
                        if (data[i] == null)
                            continue;

                        long inputData = Convert.ToInt64(data[i]);
                        if (!setValues.ContainsKey(inputData))
                            errorDataIndex.Add(i);
                    }
                }
                else
                {
                    for (int i = 0; i < data.Count; ++i)
                    {
                        if (data[i] == null)
                            continue;

                        long inputData = Convert.ToInt64(data[i]);
                        if (setValues.ContainsKey(inputData))
                            errorDataIndex.Add(i);
                    }
                }
            }
            else if (dataType == DataType.Float)
            {
                Dictionary<float, bool> setValues = new Dictionary<float, bool>();
                for (int i = 0; i < values.Length; ++i)
                {
                    string oneValueString = values[i].Trim();
                    float oneValue;
                    if (float.TryParse(oneValueString, out oneValue) == true)
                    {
                        if (setValues.ContainsKey(oneValue))
                            repeatedSetValue.Add(oneValue);
                        else
                            setValues.Add(oneValue, true);
                    }
                    else
                    {
                        errorString = string.Format("集合值定义中出现了非float型数据，其为\"{0}\"", oneValueString);
                        return;
                    }
                }

                if (isInSet == true)
                {
                    for (int i = 0; i < data.Count; ++i)
                    {
                        if (data[i] == null)
                            continue;

                        float inputData = Convert.ToSingle(data[i]);
                        if (!setValues.ContainsKey(inputData))
                            errorDataIndex.Add(i);
                    }
                }
                else
                {
                    for (int i = 0; i < data.Count; ++i)
                    {
                        if (data[i] == null)
                            continue;

                        float inputData = Convert.ToSingle(data[i]);
                        if (setValues.ContainsKey(inputData))
                            errorDataIndex.Add(i);
                    }
                }
            }
            else if (dataType == DataType.Date)
            {
                DateTimeFormatInfo dateTimeFormat = new DateTimeFormatInfo();
                dateTimeFormat.ShortDatePattern = AppValues.APP_DEFAULT_DATE_FORMAT;

                Dictionary<DateTime, bool> setValues = new Dictionary<DateTime, bool>();
                for (int i = 0; i < values.Length; ++i)
                {
                    string oneValueString = values[i].Trim();
                    try
                    {
                        DateTime oneValue = Convert.ToDateTime(oneValueString, dateTimeFormat);
                        if (setValues.ContainsKey(oneValue))
                            repeatedSetValue.Add(oneValue);
                        else
                            setValues.Add(oneValue, true);
                    }
                    catch
                    {
                        errorString = string.Format("集合值定义中出现了非法的date型数据，其为\"{0}\"，请按{1}的形式填写", oneValueString, AppValues.APP_DEFAULT_DATE_FORMAT);
                        return;
                    }
                }

                if (isInSet == true)
                {
                    for (int i = 0; i < data.Count; ++i)
                    {
                        if (data[i] == null)
                            continue;

                        DateTime inputData = (DateTime)data[i];
                        if (!setValues.ContainsKey(inputData))
                            errorDataIndex.Add(i);
                    }
                }
                else
                {
                    for (int i = 0; i < data.Count; ++i)
                    {
                        if (data[i] == null)
                            continue;

                        DateTime inputData = (DateTime)data[i];
                        if (setValues.ContainsKey(inputData))
                            errorDataIndex.Add(i);
                    }
                }
            }
            else if (dataType == DataType.Time)
            {
                DateTimeFormatInfo dateTimeFormat = new DateTimeFormatInfo();
                dateTimeFormat.ShortTimePattern = AppValues.APP_DEFAULT_TIME_FORMAT;

                Dictionary<DateTime, bool> setValues = new Dictionary<DateTime, bool>();
                for (int i = 0; i < values.Length; ++i)
                {
                    string oneValueString = values[i].Trim();
                    try
                    {
                        // 此函数会将DateTime的日期部分自动赋值为当前时间
                        DateTime tempDateTime = Convert.ToDateTime(oneValueString, dateTimeFormat);
                        DateTime oneValue = new DateTime(AppValues.REFERENCE_DATE.Year, AppValues.REFERENCE_DATE.Month, AppValues.REFERENCE_DATE.Day, tempDateTime.Hour, tempDateTime.Minute, tempDateTime.Second);
                        if (setValues.ContainsKey(oneValue))
                            repeatedSetValue.Add(oneValue);
                        else
                            setValues.Add(oneValue, true);
                    }
                    catch
                    {
                        errorString = string.Format("集合值定义中出现了非法的time型数据，其为\"{0}\"，请按{1}的形式填写", oneValueString, AppValues.APP_DEFAULT_TIME_FORMAT);
                        return;
                    }
                }

                if (isInSet == true)
                {
                    for (int i = 0; i < data.Count; ++i)
                    {
                        if (data[i] == null)
                            continue;

                        DateTime inputData = (DateTime)data[i];
                        if (!setValues.ContainsKey(inputData))
                            errorDataIndex.Add(i);
                    }
                }
                else
                {
                    for (int i = 0; i < data.Count; ++i)
                    {
                        if (data[i] == null)
                            continue;

                        DateTime inputData = (DateTime)data[i];
                        if (setValues.ContainsKey(inputData))
                            errorDataIndex.Add(i);
                    }
                }
            }
            else
            {
                errorString = "用_GetValueIsInSet函数检查了非int、long、float、date、time型的字段";
                Utils.LogErrorAndExit(errorString);
                return;
            }
        }
        else if (dataType == DataType.String)
        {
            // 用于分隔集合值定义的字符，默认为英文逗号
            char separator = ',';
            // 去除首尾花括号后整个集合值定义内容
            string tempSetValueDefineString = setValueDefineString;

            // 右边花括号的位置
            int rightBraceIndex = setValueDefineString.LastIndexOf('}');
            if (rightBraceIndex == -1)
            {
                errorString = "string型集合值定义错误：必须用一对花括号包裹整个定义内容";
                return;
            }
            // 如果声明了分隔集合值的字符
            if (rightBraceIndex != setValueDefineString.Length - 1)
            {
                int leftBracketIndex = setValueDefineString.LastIndexOf('(');
                int rightBracketIndex = setValueDefineString.LastIndexOf(')');
                if (leftBracketIndex < rightBraceIndex || rightBracketIndex < leftBracketIndex)
                {
                    errorString = "string型集合值定义错误：需要在最后面的括号中声明分隔各个集合值的一个字符，如果使用默认的英文逗号作为分隔符，则不必在最后面用括号声明自定义分隔字符";
                    return;
                }
                string separatorString = setValueDefineString.Substring(leftBracketIndex + 1, rightBracketIndex - leftBracketIndex - 1);
                if (separatorString.Length > 1)
                {
                    errorString = string.Format("string型集合值定义错误：自定义集合值的分隔字符只能为一个字符，而你输入的为\"{0}\"", separatorString);
                    return;
                }
                separator = separatorString[0];

                // 取得前面用花括号包裹的集合值定义
                tempSetValueDefineString = setValueDefineString.Substring(0, rightBraceIndex + 1).Trim();
            }

            // 去除花括号
            tempSetValueDefineString = tempSetValueDefineString.Substring(1, tempSetValueDefineString.Length - 2);
            if (string.IsNullOrEmpty(tempSetValueDefineString))
            {
                errorString = "string型集合值定义错误：至少需要输入一个集合值";
                return;
            }

            string[] setValueDefine = tempSetValueDefineString.Split(new char[] { separator }, StringSplitOptions.RemoveEmptyEntries);
            if (setValueDefine.Length == 0)
            {
                errorString = "string型集合值定义错误：至少需要输入一个集合值";
                return;
            }

            // 存储定义的集合值（key：集合值， value：恒为true）
            Dictionary<string, bool> setValues = new Dictionary<string, bool>();
            foreach (string setValue in setValueDefine)
            {
                if (setValues.ContainsKey(setValue))
                    repeatedSetValue.Add(setValue);
                else
                    setValues.Add(setValue, true);
            }

            // 对本列所有数据进行检查
            if (isInSet == true)
            {
                for (int i = 0; i < data.Count; ++i)
                {
                    // 忽略无效集合元素下属子类型的空值
                    if (data[i] == null)
                        continue;

                    string inputData = data[i].ToString();
                    if (!setValues.ContainsKey(inputData))
                        errorDataIndex.Add(i);
                }
            }
            else
            {
                for (int i = 0; i < data.Count; ++i)
                {
                    if (data[i] == null)
                        continue;

                    string inputData = data[i].ToString();
                    if (setValues.ContainsKey(inputData))
                        errorDataIndex.Add(i);
                }
            }
        }
        else
        {
            errorString = string.Format("该检查只能用于int、long、float、string、date或time型的字段，而该字段为{0}型", dataType);
            return;
        }
    }
}