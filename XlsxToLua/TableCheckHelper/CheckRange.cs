using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

public partial class TableCheckHelper
{
    /// <summary>
    /// 用于int、long、float三种数值类型或string类型或date、time两种时间类型的范围检查
    /// </summary>
    public static bool CheckRange(FieldInfo fieldInfo, FieldCheckRule checkRule, out string errorString)
    {
        bool isNumberDataType = fieldInfo.DataType == DataType.Int || fieldInfo.DataType == DataType.Long || fieldInfo.DataType == DataType.Float;
        bool isStringDataType = fieldInfo.DataType == DataType.String;
        bool isTimeDataType = fieldInfo.DataType == DataType.Date || fieldInfo.DataType == DataType.Time;
        if (isNumberDataType == false && isStringDataType==false && isTimeDataType == false)
        {
            errorString = string.Format("值范围检查只能用于int、long、float三种数值类型或String类型或date、time两种时间类型的字段，而该字段为{0}型\n", fieldInfo.DataType);
            return false;
        }
        // 检查填写的检查规则是否正确
        bool isIncludeFloor;
        bool isIncludeCeil;
        bool isCheckFloor;
        bool isCheckCeil;
        double floorValue = 0;
        double ceilValue = 0;
        int intfloorValue = 0;
        int intceilValue = 0;
        DateTime floorDateTime = AppValues.REFERENCE_DATE;
        DateTime ceilDateTime = AppValues.REFERENCE_DATE;
        // 规则首位必须为方括号或者圆括号
        if (checkRule.CheckRuleString.StartsWith("("))
            isIncludeFloor = false;
        else if (checkRule.CheckRuleString.StartsWith("["))
            isIncludeFloor = true;
        else
        {
            errorString = "值范围检查定义错误：必须用英文(或[开头，表示有效范围是否包含等于下限的情况\n";
            return false;
        }
        // 规则末位必须为方括号或者圆括号
        if (checkRule.CheckRuleString.EndsWith(")"))
            isIncludeCeil = false;
        else if (checkRule.CheckRuleString.EndsWith("]"))
            isIncludeCeil = true;
        else
        {
            errorString = "值范围检查定义错误：必须用英文)或]结尾，表示有效范围是否包含等于上限的情况\n";
            return false;
        }
        // 去掉首尾的括号
        string temp = checkRule.CheckRuleString.Substring(1, checkRule.CheckRuleString.Length - 2);
        // 通过英文逗号分隔上下限
        string[] floorAndCeilString = temp.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
        if (floorAndCeilString.Length != 2)
        {
            errorString = "值范围检查定义错误：必须用一个英文逗号分隔值范围的上下限\n";
            return false;
        }
        string floorString = floorAndCeilString[0].Trim();
        string ceilString = floorAndCeilString[1].Trim();
        // 提取下限数值
        if (floorString.Equals("*"))
            isCheckFloor = false;
        else
        {
            isCheckFloor = true;
            if (isNumberDataType == true)
            {
                if (double.TryParse(floorString, out floorValue) == false)
                {
                    errorString = string.Format("值范围检查定义错误：下限不是合法的数字，你输入的为{0}\n", floorString);
                    return false;
                }
            }
            else if(isStringDataType==true)
            {
                if (int.TryParse(floorString, out intfloorValue) == false)
                {
                    errorString = string.Format("值范围检查定义错误：下限不是合法的数字，你输入的为{0}\n", floorString);
                    return false;
                }
            }
            else
            {
                if (fieldInfo.DataType == DataType.Date)
                {
                    try
                    {
                        DateTimeFormatInfo dateTimeFormat = new DateTimeFormatInfo();
                        dateTimeFormat.ShortDatePattern = AppValues.APP_DEFAULT_DATE_FORMAT;
                        floorDateTime = Convert.ToDateTime(floorString, dateTimeFormat);
                    }
                    catch
                    {
                        errorString = string.Format("值范围检查定义错误：date型下限声明不合法，必须按{0}的形式填写，你输入的为{1}\n", AppValues.APP_DEFAULT_DATE_FORMAT, floorString);
                        return false;
                    }
                }
                else
                {
                    try
                    {
                        DateTimeFormatInfo dateTimeFormat = new DateTimeFormatInfo();
                        dateTimeFormat.ShortTimePattern = AppValues.APP_DEFAULT_TIME_FORMAT;
                        // 此函数会将DateTime的日期部分自动赋值为当前时间
                        DateTime tempDateTime = Convert.ToDateTime(floorString, dateTimeFormat);
                        floorDateTime = new DateTime(AppValues.REFERENCE_DATE.Year, AppValues.REFERENCE_DATE.Month, AppValues.REFERENCE_DATE.Day, tempDateTime.Hour, tempDateTime.Minute, tempDateTime.Second);
                    }
                    catch
                    {
                        errorString = string.Format("值范围检查定义错误：time型下限声明不合法，必须按{0}的形式填写，你输入的为{1}\n", AppValues.APP_DEFAULT_TIME_FORMAT, floorString);
                        return false;
                    }
                }
            }
        }
        // 提取上限数值
        if (ceilString.Equals("*"))
            isCheckCeil = false;
        else
        {
            isCheckCeil = true;
            if (isNumberDataType == true)
            {
                if (double.TryParse(ceilString, out ceilValue) == false)
                {
                    errorString = string.Format("值范围检查定义错误：上限不是合法的数字，你输入的为{0}\n", ceilString);
                    return false;
                }
            }
            else if(isStringDataType==true)
            {
                if (int.TryParse(ceilString, out intceilValue) == false)
                {
                    errorString = string.Format("值范围检查定义错误：上限不是合法的数字，你输入的为{0}\n", ceilString);
                    return false;
                }
            }
            else
            {
                if (fieldInfo.DataType == DataType.Date)
                {
                    try
                    {
                        DateTimeFormatInfo dateTimeFormat = new DateTimeFormatInfo();
                        dateTimeFormat.ShortDatePattern = AppValues.APP_DEFAULT_DATE_FORMAT;
                        ceilDateTime = Convert.ToDateTime(ceilString, dateTimeFormat);
                    }
                    catch
                    {
                        errorString = string.Format("值范围检查定义错误：date型上限声明不合法，必须按{0}的形式填写，你输入的为{1}\n", AppValues.APP_DEFAULT_DATE_FORMAT, ceilString);
                        return false;
                    }
                }
                else
                {
                    try
                    {
                        DateTimeFormatInfo dateTimeFormat = new DateTimeFormatInfo();
                        dateTimeFormat.ShortDatePattern = AppValues.APP_DEFAULT_TIME_FORMAT;
                        // 此函数会将DateTime的日期部分自动赋值为当前时间
                        DateTime tempDateTime = Convert.ToDateTime(ceilString, dateTimeFormat);
                        ceilDateTime = new DateTime(AppValues.REFERENCE_DATE.Year, AppValues.REFERENCE_DATE.Month, AppValues.REFERENCE_DATE.Day, tempDateTime.Hour, tempDateTime.Minute, tempDateTime.Second);
                    }
                    catch
                    {
                        errorString = string.Format("值范围检查定义错误：time型上限声明不合法，必须按{0}的形式填写，你输入的为{1}\n", AppValues.APP_DEFAULT_TIME_FORMAT, ceilString);
                        return false;
                    }
                }
            }
        }
        // 判断上限是否大于下限
        if (isNumberDataType == true)
        {
            if (isCheckFloor == true && isCheckCeil == true && floorValue >= ceilValue)
            {
                errorString = string.Format("值范围检查定义错误：上限值必须大于下限值，你输入的下限为{0}，上限为{1}\n", floorString, ceilString);
                return false;
            }
        }
        else if(isStringDataType==true)
        {
            if (isCheckFloor == true && isCheckCeil == true && intfloorValue >= intceilValue && intceilValue>=0)
            {
                errorString = string.Format("值范围检查定义错误：上下限必须为正整数，且上限值必须大于下限值，你输入的下限为{0}，上限为{1}\n", floorString, ceilString);
                return false;
            }
        }
        else
        {
            if (isCheckFloor == true && isCheckCeil == true && floorDateTime >= ceilDateTime)
            {
                errorString = string.Format("值范围检查定义错误：上限值必须大于下限值，你输入的下限为{0}，上限为{1}\n", floorDateTime.ToString(AppValues.APP_DEFAULT_DATE_FORMAT), ceilDateTime.ToString(AppValues.APP_DEFAULT_DATE_FORMAT));
                return false;
            }
        }

        // 进行检查
        // 存储检查出的非法值（key：数据索引， value：填写值）
        Dictionary<int, object> illegalValue = new Dictionary<int, object>();
        if (isCheckFloor == true && isCheckCeil == false)
        {
            if (isIncludeFloor == true)
            {
                for (int i = 0; i < fieldInfo.Data.Count; ++i)
                {
                    // 忽略无效集合元素下属子类型的空值或本身为空值
                    if (fieldInfo.Data[i] == null)
                        continue;

                    if (isNumberDataType == true)
                    {
                        double inputValue = Convert.ToDouble(fieldInfo.Data[i]);
                        if (inputValue < floorValue)
                            illegalValue.Add(i, fieldInfo.Data[i]);
                    }
                    else if(isStringDataType==true)
                    {
                        int inputValue = System.Text.Encoding.Default.GetBytes(fieldInfo.Data[i].ToString().ToCharArray()).Length;
                        if (inputValue < intfloorValue)
                            illegalValue.Add(i, fieldInfo.Data[i]);
                    }
                    else
                    {
                        DateTime inputValue = (DateTime)fieldInfo.Data[i];
                        if (inputValue < floorDateTime)
                            illegalValue.Add(i, inputValue);
                    }
                }
            }
            else
            {
                for (int i = 0; i < fieldInfo.Data.Count; ++i)
                {
                    if (fieldInfo.Data[i] == null)
                        continue;

                    if (isNumberDataType == true)
                    {
                        double inputValue = Convert.ToDouble(fieldInfo.Data[i]);
                        if (inputValue <= floorValue)
                            illegalValue.Add(i, fieldInfo.Data[i]);
                    }
                    else if(isStringDataType==true)
                    {
                        int inputValue = System.Text.Encoding.Default.GetBytes(fieldInfo.Data[i].ToString().ToCharArray()).Length;
                        if (inputValue <= intfloorValue)
                            illegalValue.Add(i, fieldInfo.Data[i]);
                    }
                    else
                    {
                        DateTime inputValue = (DateTime)fieldInfo.Data[i];
                        if (inputValue <= floorDateTime)
                            illegalValue.Add(i, inputValue);
                    }
                }
            }
        }
        else if ((isCheckFloor == false && isCheckCeil == true))
        {
            if (isIncludeCeil == true)
            {
                for (int i = 0; i < fieldInfo.Data.Count; ++i)
                {
                    if (fieldInfo.Data[i] == null)
                        continue;

                    if (isNumberDataType == true)
                    {
                        double inputValue = Convert.ToDouble(fieldInfo.Data[i]);
                        if (inputValue > ceilValue)
                            illegalValue.Add(i, fieldInfo.Data[i]);
                    }
                    else if (isStringDataType == true)
                    {
                        int inputValue = System.Text.Encoding.Default.GetBytes(fieldInfo.Data[i].ToString().ToCharArray()).Length;
                        if (inputValue > intceilValue)
                            illegalValue.Add(i, fieldInfo.Data[i]);
                    }
                    else
                    {
                        DateTime inputValue = (DateTime)fieldInfo.Data[i];
                        if (inputValue > ceilDateTime)
                            illegalValue.Add(i, inputValue);
                    }
                }
            }
            else
            {
                for (int i = 0; i < fieldInfo.Data.Count; ++i)
                {
                    if (fieldInfo.Data[i] == null)
                        continue;

                    if (isNumberDataType == true)
                    {
                        double inputValue = Convert.ToDouble(fieldInfo.Data[i]);
                        if (inputValue >= ceilValue)
                            illegalValue.Add(i, fieldInfo.Data[i]);
                    }
                    else if (isStringDataType == true)
                    {
                        int inputValue = System.Text.Encoding.Default.GetBytes(fieldInfo.Data[i].ToString().ToCharArray()).Length;
                        if (inputValue >= intceilValue)
                            illegalValue.Add(i, fieldInfo.Data[i]);
                    }
                    else
                    {
                        DateTime inputValue = (DateTime)fieldInfo.Data[i];
                        if (inputValue >= ceilDateTime)
                            illegalValue.Add(i, inputValue);
                    }
                }
            }
        }
        else if ((isCheckFloor == true && isCheckCeil == true))
        {
            if (isIncludeFloor == false && isIncludeCeil == false)
            {
                for (int i = 0; i < fieldInfo.Data.Count; ++i)
                {
                    if (fieldInfo.Data[i] == null)
                        continue;

                    if (isNumberDataType == true)
                    {
                        double inputValue = Convert.ToDouble(fieldInfo.Data[i]);
                        if (inputValue <= floorValue || inputValue >= ceilValue)
                            illegalValue.Add(i, fieldInfo.Data[i]);
                    }
                    else if (isStringDataType == true)
                    {
                        int inputValue = System.Text.Encoding.Default.GetBytes(fieldInfo.Data[i].ToString().ToCharArray()).Length;
                        if (inputValue <= intfloorValue || inputValue >= intceilValue)
                            illegalValue.Add(i, fieldInfo.Data[i]);
                    }
                    else
                    {
                        DateTime inputValue = (DateTime)fieldInfo.Data[i];
                        if (inputValue <= floorDateTime || inputValue >= ceilDateTime)
                            illegalValue.Add(i, inputValue);
                    }
                }
            }
            else if (isIncludeFloor == true && isIncludeCeil == false)
            {
                for (int i = 0; i < fieldInfo.Data.Count; ++i)
                {
                    if (fieldInfo.Data[i] == null)
                        continue;

                    if (isNumberDataType == true)
                    {
                        double inputValue = Convert.ToDouble(fieldInfo.Data[i]);
                        if (inputValue < floorValue || inputValue >= ceilValue)
                            illegalValue.Add(i, fieldInfo.Data[i]);
                    }
                    else if (isStringDataType == true)
                    {
                        int inputValue = System.Text.Encoding.Default.GetBytes(fieldInfo.Data[i].ToString().ToCharArray()).Length;
                        if (inputValue < intfloorValue || inputValue >= intceilValue)
                            illegalValue.Add(i, fieldInfo.Data[i]);
                    }
                    else
                    {
                        DateTime inputValue = (DateTime)fieldInfo.Data[i];
                        if (inputValue < floorDateTime || inputValue >= ceilDateTime)
                            illegalValue.Add(i, inputValue);
                    }
                }
            }
            else if (isIncludeFloor == false && isIncludeCeil == true)
            {
                for (int i = 0; i < fieldInfo.Data.Count; ++i)
                {
                    if (fieldInfo.Data[i] == null)
                        continue;

                    if (isNumberDataType == true)
                    {
                        double inputValue = Convert.ToDouble(fieldInfo.Data[i]);
                        if (inputValue <= floorValue || inputValue > ceilValue)
                            illegalValue.Add(i, fieldInfo.Data[i]);
                    }
                    else if (isStringDataType == true)
                    {
                        int inputValue = System.Text.Encoding.Default.GetBytes(fieldInfo.Data[i].ToString().ToCharArray()).Length;
                        if (inputValue <= intfloorValue || inputValue > intceilValue)
                            illegalValue.Add(i, fieldInfo.Data[i]);
                    }
                    else
                    {
                        DateTime inputValue = (DateTime)fieldInfo.Data[i];
                        if (inputValue <= floorDateTime || inputValue > ceilDateTime)
                            illegalValue.Add(i, inputValue);
                    }
                }
            }
            else if (isIncludeFloor == true && isIncludeCeil == true)
            {
                for (int i = 0; i < fieldInfo.Data.Count; ++i)
                {
                    if (fieldInfo.Data[i] == null)
                        continue;

                    if (isNumberDataType == true)
                    {
                        double inputValue = Convert.ToDouble(fieldInfo.Data[i]);
                        if (inputValue < floorValue || inputValue > ceilValue)
                            illegalValue.Add(i, fieldInfo.Data[i]);
                    }
                    else if (isStringDataType == true)
                    {
                        int inputValue = System.Text.Encoding.Default.GetBytes(fieldInfo.Data[i].ToString().ToCharArray()).Length;
                        if (inputValue < intfloorValue || inputValue > intceilValue)
                            illegalValue.Add(i, fieldInfo.Data[i]);
                    }
                    else
                    {
                        DateTime inputValue = (DateTime)fieldInfo.Data[i];
                        if (inputValue < floorDateTime || inputValue > ceilDateTime)
                            illegalValue.Add(i, inputValue);
                    }
                }
            }
        }

        if (illegalValue.Count > 0)
        {
            StringBuilder illegalValueInfo = new StringBuilder();
            if (isNumberDataType == true)
            {
                foreach (var item in illegalValue)
                    illegalValueInfo.AppendFormat("第{0}行数据\"{1}\"不满足要求\n", item.Key + AppValues.DATA_FIELD_DATA_START_INDEX + 1, item.Value);
            }
            else if(isStringDataType==true)
            {
                foreach (var item in illegalValue)
                    illegalValueInfo.AppendFormat("第{0}行数据\"{1}\"不满足要求\n", item.Key + AppValues.DATA_FIELD_DATA_START_INDEX + 1, item.Value);
            }
            else if (fieldInfo.DataType == DataType.Date)
            {
                foreach (var item in illegalValue)
                    illegalValueInfo.AppendFormat("第{0}行数据\"{1}\"不满足要求\n", item.Key + AppValues.DATA_FIELD_DATA_START_INDEX + 1, ((DateTime)(item.Value)).ToString(AppValues.APP_DEFAULT_DATE_FORMAT));
            }
            else if (fieldInfo.DataType == DataType.Time)
            {
                foreach (var item in illegalValue)
                    illegalValueInfo.AppendFormat("第{0}行数据\"{1}\"不满足要求\n", item.Key + AppValues.DATA_FIELD_DATA_START_INDEX + 1, ((DateTime)(item.Value)).ToString(AppValues.APP_DEFAULT_TIME_FORMAT));
            }

            errorString = illegalValueInfo.ToString();
            return false;
        }
        else
        {
            errorString = null;
            return true;
        }
    }
}