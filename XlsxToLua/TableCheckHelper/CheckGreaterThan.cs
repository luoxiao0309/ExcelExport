using System;
using System.Collections.Generic;
using System.Text;


public partial class TableCheckHelper
{
    /// <summary>
    /// 用于int、long、float、date或time型同一行某字段值必须大于等于或大于另一字段值的检查
    /// 注意：要进行比较的两个字段可以为同种数据类型，也可以任意比较int、long、float三种数值型大小
    /// </summary>
    public static bool CheckGreaterThan(FieldInfo fieldInfo, FieldCheckRule checkRule, out string errorString)
    {
        bool isNumberDataType = false;
        if (fieldInfo.DataType == DataType.Int || fieldInfo.DataType == DataType.Long || fieldInfo.DataType == DataType.Float)
            isNumberDataType = true;
        else if (fieldInfo.DataType == DataType.Date || fieldInfo.DataType == DataType.Time)
            isNumberDataType = false;
        else
        {
            errorString = string.Format("值大小比较检查规则只能用于int、long、float三种数值类型或date、time两种时间类型的字段，而该字段为{0}型\n", fieldInfo.DataType);
            return false;
        }

        bool isContainsEqual = checkRule.CheckRuleString.StartsWith(">=");
        TableInfo tableInfo = AppValues.TableInfo[fieldInfo.TableName];
        string comparedFieldString = null;
        if (isContainsEqual == true)
            comparedFieldString = checkRule.CheckRuleString.Substring(2).Trim();
        else
            comparedFieldString = checkRule.CheckRuleString.Substring(1).Trim();

        // 根据索引字符串定义，找到要与其比较的字段
        FieldInfo comparedField = GetFieldByIndexDefineString(comparedFieldString, tableInfo, out errorString);
        if (errorString != null)
        {
            errorString = string.Format("值大小比较检查规则定义错误：{0}\n", errorString);
            return false;
        }
        // 检查与其比较的字段是否类型匹配
        if (comparedField.DataType == DataType.Int || comparedField.DataType == DataType.Long || comparedField.DataType == DataType.Float)
        {
            if (isNumberDataType == false)
            {
                errorString = string.Format("值大小比较检查规则定义错误：该字段为{0}型，而声明的与其进行比较的字段为{1}型，不支持数值型与时间型的比较\n", fieldInfo.DataType, comparedField.DataType);
                return false;
            }
        }
        else if (comparedField.DataType == DataType.Date || comparedField.DataType == DataType.Time)
        {
            if (isNumberDataType == true)
            {
                errorString = string.Format("值大小比较检查规则定义错误：该字段为{0}型，而声明的与其进行比较的字段为{1}型，不支持数值型与时间型的比较\n", fieldInfo.DataType, comparedField.DataType);
                return false;
            }
            if (comparedField.DataType != fieldInfo.DataType)
            {
                errorString = string.Format("值大小比较检查规则定义错误：该字段为{0}型，而声明的与其进行比较的字段为{1}型，date型无法与time型进行比较\n", fieldInfo.DataType, comparedField.DataType);
                return false;
            }
        }
        // 对这两个字段中的每行数据进行值大小比较检查（任一字段中某行数据为无效数据则忽略对该行两字段数值的比较）
        // 记录检查出的不满足要求的数据，其中object数组含3个元素，分别为未通过检查的数据所在Excel的行号、该字段的值、与其比较的字段的值
        List<object[]> illegalValue = new List<object[]>();
        if (isNumberDataType == true)
        {
            if (isContainsEqual == true)
            {
                for (int i = 0; i < fieldInfo.Data.Count; ++i)
                {
                    if (fieldInfo.Data[i] == null || comparedField.Data[i] == null)
                        continue;

                    double fieldDataValue = Convert.ToDouble(fieldInfo.Data[i]);
                    double comparedFieldDataValue = Convert.ToDouble(comparedField.Data[i]);
                    if (fieldDataValue < comparedFieldDataValue)
                        illegalValue.Add(new object[3] { i + AppValues.DATA_FIELD_DATA_START_INDEX + 1, fieldInfo.Data[i], comparedField.Data[i] });
                }
            }
            else
            {
                for (int i = 0; i < fieldInfo.Data.Count; ++i)
                {
                    if (fieldInfo.Data[i] == null || comparedField.Data[i] == null)
                        continue;

                    double fieldDataValue = Convert.ToDouble(fieldInfo.Data[i]);
                    double comparedFieldDataValue = Convert.ToDouble(comparedField.Data[i]);
                    if (fieldDataValue <= comparedFieldDataValue)
                        illegalValue.Add(new object[3] { i + AppValues.DATA_FIELD_DATA_START_INDEX + 1, fieldInfo.Data[i], comparedField.Data[i] });
                }
            }
        }
        else if (fieldInfo.DataType == DataType.Date)
        {
            if (isContainsEqual == true)
            {
                for (int i = 0; i < fieldInfo.Data.Count; ++i)
                {
                    if (fieldInfo.Data[i] == null || comparedField.Data[i] == null)
                        continue;

                    DateTime fieldDataValue = (DateTime)fieldInfo.Data[i];
                    DateTime comparedFieldDataValue = (DateTime)comparedField.Data[i];
                    if (fieldDataValue < comparedFieldDataValue)
                        illegalValue.Add(new object[3] { i + AppValues.DATA_FIELD_DATA_START_INDEX + 1, fieldDataValue.ToString(AppValues.APP_DEFAULT_DATE_FORMAT), comparedFieldDataValue.ToString(AppValues.APP_DEFAULT_DATE_FORMAT) });
                }
            }
            else
            {
                for (int i = 0; i < fieldInfo.Data.Count; ++i)
                {
                    if (fieldInfo.Data[i] == null || comparedField.Data[i] == null)
                        continue;

                    DateTime fieldDataValue = (DateTime)fieldInfo.Data[i];
                    DateTime comparedFieldDataValue = (DateTime)comparedField.Data[i];
                    if (fieldDataValue <= comparedFieldDataValue)
                        illegalValue.Add(new object[3] { i + AppValues.DATA_FIELD_DATA_START_INDEX + 1, fieldDataValue.ToString(AppValues.APP_DEFAULT_DATE_FORMAT), comparedFieldDataValue.ToString(AppValues.APP_DEFAULT_DATE_FORMAT) });
                }
            }
        }
        else if (fieldInfo.DataType == DataType.Time)
        {
            if (isContainsEqual == true)
            {
                for (int i = 0; i < fieldInfo.Data.Count; ++i)
                {
                    if (fieldInfo.Data[i] == null || comparedField.Data[i] == null)
                        continue;

                    DateTime fieldDataValue = (DateTime)fieldInfo.Data[i];
                    DateTime comparedFieldDataValue = (DateTime)comparedField.Data[i];
                    if (fieldDataValue < comparedFieldDataValue)
                        illegalValue.Add(new object[3] { i + AppValues.DATA_FIELD_DATA_START_INDEX + 1, fieldDataValue.ToString(AppValues.APP_DEFAULT_TIME_FORMAT), comparedFieldDataValue.ToString(AppValues.APP_DEFAULT_TIME_FORMAT) });
                }
            }
            else
            {
                for (int i = 0; i < fieldInfo.Data.Count; ++i)
                {
                    if (fieldInfo.Data[i] == null || comparedField.Data[i] == null)
                        continue;

                    DateTime fieldDataValue = (DateTime)fieldInfo.Data[i];
                    DateTime comparedFieldDataValue = (DateTime)comparedField.Data[i];
                    if (fieldDataValue <= comparedFieldDataValue)
                        illegalValue.Add(new object[3] { i + AppValues.DATA_FIELD_DATA_START_INDEX + 1, fieldDataValue.ToString(AppValues.APP_DEFAULT_TIME_FORMAT), comparedFieldDataValue.ToString(AppValues.APP_DEFAULT_TIME_FORMAT) });
                }
            }
        }

        if (illegalValue.Count > 0)
        {
            StringBuilder errorStringBuilder = new StringBuilder();
            errorStringBuilder.AppendFormat("以下行中数据不满足{0}的值大小比较检查规则\n", isContainsEqual == true ? ">=" : ">");
            for (int i = 0; i < illegalValue.Count; ++i)
            {
                object[] oneIllegalValue = illegalValue[i];
                errorStringBuilder.AppendFormat("第{0}行中，本字段所填值为\"{1}\"，与其比较的的字段所填值为\"{2}\"\n", oneIllegalValue[0], oneIllegalValue[1], oneIllegalValue[2]);
            }

            errorString = errorStringBuilder.ToString();
            return false;
        }
        else
        {
            errorString = null;
            return true;
        }
    }

}

