using System.Collections.Generic;
using System.Text;

public partial class TableCheckHelper
{
    /// <summary>
    /// 用于检查按自定义索引字段导出lua文件的表格中相关索引字段的数据完整性
    /// </summary>
    public static bool CheckTableIntegrity(List<FieldInfo> indexField, Dictionary<object, object> data, List<string> integrityCheckRules, out string errorString)
    {
        StringBuilder errorStringBuilder = new StringBuilder();

        // 解析各个需要进行数据完整性检查的字段声明的有效值
        List<List<object>> effectiveValues = new List<List<object>>();
        for (int i = 0; i < integrityCheckRules.Count; ++i)
        {
            if (integrityCheckRules[i] == null)
                effectiveValues.Add(null);
            else
            {
                List<object> oneFieldEffectiveValues = Utils.GetEffectiveValue(integrityCheckRules[i], indexField[i].DataType, out errorString);
                if (errorString != null)
                {
                    errorStringBuilder.AppendFormat("字段\"{0}\"（列号：{1}）的数据完整性检查规则定义错误，{2}\n", indexField[i].FieldName, Utils.GetExcelColumnName(indexField[i].ColumnSeq + 1), errorString);
                    errorString = null;
                }
                else
                    effectiveValues.Add(oneFieldEffectiveValues);
            }
        }
        errorString = errorStringBuilder.ToString();
        if (string.IsNullOrEmpty(errorString))
            errorString = null;
        else
            return false;

        // 进行数据完整性检查
        List<object> parentKeys = new List<object>();
        for (int i = 0; i < integrityCheckRules.Count; ++i)
            parentKeys.Add(null);

        int currentLevel = 0;
        _CheckIntegrity(indexField, data, parentKeys, effectiveValues, ref currentLevel, errorStringBuilder);
        errorString = errorStringBuilder.ToString();
        if (string.IsNullOrEmpty(errorString))
        {
            errorString = null;
            return true;
        }
        else
            return false;
    }
}