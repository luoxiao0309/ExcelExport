using System;
using System.Collections.Generic;
using System.Text;


public partial class TableCheckHelper
{
    /// <summary>
    /// 用于数据唯一性检查，适用于string、int、long、float或lang类型
    /// 注意：string型、lang型如果填写或者找到的value为空字符串，允许出现多次为空的情况
    /// 注意：lang型默认只检查key不能重复，如果还想检查不同key对应的value也不能相同则需要声明为unique[value]
    /// </summary>
    public static bool CheckUnique(FieldInfo fieldInfo, FieldCheckRule checkRule, out string errorString)
    {
        if (fieldInfo.DataType == DataType.Int || fieldInfo.DataType == DataType.Long || fieldInfo.DataType == DataType.Float || fieldInfo.DataType == DataType.String || fieldInfo.DataType == DataType.Date || fieldInfo.DataType == DataType.Time)
        {
            _CheckInputDataUnique(fieldInfo.DataType, fieldInfo.Data, out errorString);
            if (errorString == null)
                return true;
            else
            {
                errorString = string.Format("数据类型为{0}的字段中，存在以下重复数据：\n", fieldInfo.DataType.ToString()) + errorString;
                return false;
            }
        }
        else if (fieldInfo.DataType == DataType.Lang)
        {
            // 只检查key则与string、int、float型的操作相同
            if ("unique".Equals(checkRule.CheckRuleString, StringComparison.CurrentCultureIgnoreCase))
            {
                _CheckInputDataUnique(fieldInfo.DataType, fieldInfo.LangKeys, out errorString);
                if (errorString == null)
                    return true;
                else
                {
                    errorString = "要求仅检查lang型数据的key值不重复，但存在以下重复key：\n" + errorString;
                    return false;
                }
            }
            else if ("unique[value]".Equals(checkRule.CheckRuleString, StringComparison.CurrentCultureIgnoreCase))
            {
                _CheckInputDataUnique(fieldInfo.DataType, fieldInfo.Data, out errorString);
                if (errorString == null)
                    return true;
                else
                {
                    errorString = "要求检查lang型数据的key值与Value值均不重复，但存在以下重复数据：\n" + errorString;
                    return false;
                }
            }
            else
            {
                errorString = string.Format("唯一性检查规则用于lang型的声明错误，输入的检查规则字符串为{0}。而lang型声明unique仅检查key不重复，声明为unique[value]一并检查value不重复\n", checkRule.CheckRuleString);
                return false;
            }
        }
        else
        {
            errorString = string.Format("唯一性检查规则只适用于string、int、long、float、lang、date或time类型的字段，要检查的这列类型为{0}\n", fieldInfo.DataType.ToString());
            return false;
        }
    }

    /// <summary>
    /// 用于检查List中的数据（类型为int、long、reffloat或string）是否唯一
    /// 该函数需传入List而不直接传入FieldInfo是因为对于lang型的检查分为只检查key和一并检查value不能重复，传List则可针对两种情况灵活处理
    /// </summary>
    private static bool _CheckInputDataUnique(DataType dataType, List<object> data, out string errorString)
    {
        // 存储每个数据对应的index（key：data， value：index）
        Dictionary<object, int> dataToIndex = new Dictionary<object, int>();
        // 存储已经重复的数据所在的所有行数
        Dictionary<object, List<int>> repeatedDataInfo = new Dictionary<object, List<int>>();

        for (int i = 0; i < data.Count; ++i)
        {
            object oneData = data[i];
            // 如果值为null说明其属于标为无效集合的子数据，或者是数值型字段中的空值，跳过unique检查。如果值为string.Empty说明其属于string类型列中的空数据也跳过检查
            if (oneData == null || string.IsNullOrEmpty(oneData.ToString()))
                continue;

            if (dataToIndex.ContainsKey(oneData))
            {
                if (!repeatedDataInfo.ContainsKey(oneData))
                    repeatedDataInfo.Add(oneData, new List<int>());
                List<int> repeatedRowIndex = repeatedDataInfo[oneData];
                repeatedRowIndex.Add(i);
            }
            else
                dataToIndex.Add(oneData, i);
        }
        // 此时repeatedDataInfo存储的重复行索引中并不包含最早出现这个重复数据的行，需要从dataToIndex中找到
        foreach (var item in repeatedDataInfo)
        {
            var repeatedData = item.Key;
            var repeatedRowIndex = item.Value;
            int index = dataToIndex[repeatedData];
            repeatedRowIndex.Insert(0, index);
        }

        if (repeatedDataInfo.Count > 0)
        {
            StringBuilder repeatedLineInfo = new StringBuilder();
            foreach (var item in repeatedDataInfo)
            {
                if (dataType == DataType.Date)
                    repeatedLineInfo.AppendFormat("数据\"{0}\"重复，重复的行号为：", ((DateTime)(item.Key)).ToString(AppValues.APP_DEFAULT_DATE_FORMAT));
                if (dataType == DataType.Time)
                    repeatedLineInfo.AppendFormat("数据\"{0}\"重复，重复的行号为：", ((DateTime)(item.Key)).ToString(AppValues.APP_DEFAULT_TIME_FORMAT));
                else
                    repeatedLineInfo.AppendFormat("数据\"{0}\"重复，重复的行号为：", item.Key);

                List<int> lineIndex = item.Value;
                foreach (int lineNum in lineIndex)
                    repeatedLineInfo.Append(lineNum + AppValues.DATA_FIELD_DATA_START_INDEX + 1 + ", ");

                // 去掉最后多余的", "
                repeatedLineInfo.Remove(repeatedLineInfo.Length - 2, 2);
                // 换行
                repeatedLineInfo.AppendLine();
            }

            errorString = repeatedLineInfo.ToString();
            return false;
        }
        else
        {
            errorString = null;
            return true;
        }
    }

}

