using System;
using System.Collections.Generic;
using System.Text;


public partial class TableCheckHelper
{
    /// <summary>
    /// 用于int、long、float、string、date或time型取值不允许为指定集合值中的一个的检查
    /// </summary>
    public static bool CheckIllegal(FieldInfo fieldInfo, FieldCheckRule checkRule, out string errorString)
    {
        List<object> repeatedSetValue = null;
        List<int> errorDataIndex = null;
        string setValueDefineString = checkRule.CheckRuleString.Substring(1).Trim();
        _GetValueIsInSet(fieldInfo.Data, fieldInfo.DataType, setValueDefineString, false, out repeatedSetValue, out errorDataIndex, out errorString);

        if (errorString == null)
        {
            if (repeatedSetValue.Count > 0)
            {
                foreach (object setValue in repeatedSetValue)
                {
                    if (fieldInfo.DataType == DataType.Int || fieldInfo.DataType == DataType.Long || fieldInfo.DataType == DataType.Float || fieldInfo.DataType == DataType.String)
                        Utils.LogWarning(string.Format("警告：字段{0}（列号：{1}）的非法值检查规则定义中，出现了相同的非法值\"{2}\"，本工具忽略此问题继续进行检查，需要你之后修正规则定义错误\n", fieldInfo.FieldName, Utils.GetExcelColumnName(fieldInfo.ColumnSeq + 1), setValue));
                    else if (fieldInfo.DataType == DataType.Date)
                    {
                        DateTime dataTimeSetValue = (DateTime)setValue;
                        Utils.LogWarning(string.Format("警告：字段{0}（列号：{1}）的非法值检查规则定义中，出现了相同的非法值\"{2}\"，本工具忽略此问题继续进行检查，需要你之后修正规则定义错误\n", fieldInfo.FieldName, Utils.GetExcelColumnName(fieldInfo.ColumnSeq + 1), dataTimeSetValue.ToString(AppValues.APP_DEFAULT_DATE_FORMAT)));
                    }
                    else if (fieldInfo.DataType == DataType.Time)
                    {
                        DateTime dataTimeSetValue = (DateTime)setValue;
                        Utils.LogWarning(string.Format("警告：字段{0}（列号：{1}）的非法值检查规则定义中，出现了相同的非法值\"{2}\"，本工具忽略此问题继续进行检查，需要你之后修正规则定义错误\n", fieldInfo.FieldName, Utils.GetExcelColumnName(fieldInfo.ColumnSeq + 1), dataTimeSetValue.ToString(AppValues.APP_DEFAULT_TIME_FORMAT)));
                    }
                }
            }
            if (errorDataIndex.Count > 0)
            {
                StringBuilder illegalValueInfo = new StringBuilder();
                foreach (int dataIndex in errorDataIndex)
                {
                    if (fieldInfo.DataType == DataType.Int || fieldInfo.DataType == DataType.Long || fieldInfo.DataType == DataType.Float || fieldInfo.DataType == DataType.String)
                        illegalValueInfo.AppendFormat("第{0}行数据\"{1}\"属于非法取值中的一个\n", dataIndex + AppValues.DATA_FIELD_DATA_START_INDEX + 1, fieldInfo.Data[dataIndex]);
                    else if (fieldInfo.DataType == DataType.Date)
                    {
                        DateTime dataTimeValue = (DateTime)fieldInfo.Data[dataIndex];
                        illegalValueInfo.AppendFormat("第{0}行数据\"{1}\"属于非法取值中的一个\n", dataIndex + AppValues.DATA_FIELD_DATA_START_INDEX + 1, dataTimeValue.ToString(AppValues.APP_DEFAULT_DATE_FORMAT));
                    }
                    else if (fieldInfo.DataType == DataType.Time)
                    {
                        DateTime dataTimeValue = (DateTime)fieldInfo.Data[dataIndex];
                        illegalValueInfo.AppendFormat("第{0}行数据\"{1}\"属于非法取值中的一个\n", dataIndex + AppValues.DATA_FIELD_DATA_START_INDEX + 1, dataTimeValue.ToString(AppValues.APP_DEFAULT_TIME_FORMAT));
                    }
                }

                errorString = illegalValueInfo.ToString();
                return false;
            }
            else
                return true;
        }
        else
        {
            errorString = errorString + "\n";
            return false;
        }
    }

}

