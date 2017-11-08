using System.Text;

public partial class TableCheckHelper
{
    /// <summary>
    /// 通过索引定义字符串，找到某张表格中的指定字段，若字段为dict的子元素，用“.”进行层层索引，若为array的子元素，用进行[x]索引
    /// </summary>
    public static FieldInfo GetFieldByIndexDefineString(string indexDefineString, TableInfo tableInfo, out string errorString)
    {
        FieldInfo fieldInfo = null;
        indexDefineString = indexDefineString.Trim();
        if (string.IsNullOrEmpty(indexDefineString))
        {
            errorString = "输入的索引定义字符串不允许为空";
            return null;
        }
        // 如果是独立字段
        if (indexDefineString.IndexOf('.') == -1 && indexDefineString.IndexOf('[') == -1)
        {
            fieldInfo = tableInfo.GetFieldInfoByFieldName(indexDefineString);
            if (fieldInfo == null)
            {
                errorString = string.Format("此表格中不存在名为\"{0}\"的字段，若此字段为dict或array的子元素，请通过.或[x]的形式索引到该子字段", indexDefineString);
                return null;
            }
        }
        else
        {
            StringBuilder tempFieldNameBuilder = new StringBuilder();
            StringBuilder tempArrayIndexBuilder = new StringBuilder();
            bool isInBracket = false;
            bool isAfterDot = false;
            for (int i = 0; i < indexDefineString.Length; ++i)
            {
                char currentChar = indexDefineString[i];
                if (currentChar == '.')
                {
                    if (isInBracket == true)
                    {
                        errorString = string.Format("索引array的[]中不允许出现小数点，截止到出错位置的定义字符串为{0}", indexDefineString.Substring(0, i + 1));
                        return null;
                    }
                    if (i - 1 >= 0 && indexDefineString[i - 1] == '.')
                    {
                        errorString = string.Format("索引定义字符串中出现了连续的小数点，截止到出错位置的定义字符串为{0}", indexDefineString.Substring(0, i + 1));
                        return null;
                    }

                    string tempFieldName = tempFieldNameBuilder.ToString();
                    // 处理array嵌套dict，形如rewardList[1].rewardType的情况
                    if (string.IsNullOrEmpty(tempFieldName))
                    {
                        if (fieldInfo == null)
                        {
                            errorString = string.Format("小数点必须声明在dict型字段的变量名之后，用于索引dict型字段，截止到出错位置的定义字符串为{0}", indexDefineString.Substring(0, i + 1));
                            return null;
                        }
                        if (fieldInfo.DataType != DataType.Dict)
                        {
                            if (fieldInfo.DataType == DataType.Array)
                                errorString = string.Format("用小数点只能索引dict型字段，而你索引的是array型字段\"{0}\"，请使用[]来索引array型字段", fieldInfo.FieldName);
                            else
                                errorString = string.Format("用小数点只能索引dict型字段，而你索引的是{0}型字段\"{1}\"", fieldInfo.DataType, fieldInfo.FieldName);

                            return null;
                        }
                    }
                    else
                    {
                        // 处理dict嵌套dict，形如systemConfigDict.audioConfigDict.isOpen的情况
                        if (fieldInfo != null && fieldInfo.DataType == DataType.Dict)
                        {
                            bool isFoundDictChildField = false;
                            foreach (FieldInfo dictChildField in fieldInfo.ChildField)
                            {
                                if (tempFieldName.Equals(dictChildField.FieldName))
                                {
                                    fieldInfo = dictChildField;
                                    isFoundDictChildField = true;
                                    break;
                                }
                            }
                            if (isFoundDictChildField == false)
                            {
                                errorString = string.Format("dict型字段中不存在名为\"{0}\"的子元素，截止到出错位置的定义字符串为{1}", tempFieldName, indexDefineString.Substring(0, i + 1));
                                return null;
                            }
                        }
                        else
                        {
                            fieldInfo = tableInfo.GetFieldInfoByFieldName(tempFieldName);
                            if (fieldInfo == null)
                            {
                                errorString = string.Format("此表格中不存在名为\"{0}\"的字段，截止到出错位置的定义字符串为{1}", tempFieldName, indexDefineString.Substring(0, i + 1));
                                return null;
                            }
                            if (fieldInfo.DataType != DataType.Dict)
                            {
                                if (fieldInfo.DataType == DataType.Array)
                                    errorString = string.Format("用小数点只能索引dict型字段，而你索引的是array型字段\"{0}\"，请使用[]来索引array型字段", tempFieldName);
                                else
                                    errorString = string.Format("用小数点只能索引dict型字段，而你索引的是{0}型字段\"{1}\"", fieldInfo.DataType, tempFieldName);

                                return null;
                            }
                        }
                    }

                    isAfterDot = true;
                    tempFieldNameBuilder = new StringBuilder();
                }
                else if (currentChar == '[')
                {
                    if (isInBracket == true)
                    {
                        errorString = string.Format("索引定义中括号不匹配，截止到出错位置的定义字符串为{0}", indexDefineString.Substring(0, i + 1));
                        return null;
                    }
                    // 排除array嵌套array，形如array[1][1]的情况
                    if (!(fieldInfo != null && fieldInfo.DataType == DataType.Array))
                    {
                        // 处理dict嵌套array，形如pveBattleConfig.eliteBattleConfig[1]的情况
                        if (fieldInfo != null && fieldInfo.DataType == DataType.Dict)
                        {
                            string tempFieldName = tempFieldNameBuilder.ToString();
                            bool isFoundDictChildField = false;
                            foreach (FieldInfo dictChildField in fieldInfo.ChildField)
                            {
                                if (tempFieldName.Equals(dictChildField.FieldName))
                                {
                                    fieldInfo = dictChildField;
                                    isFoundDictChildField = true;
                                    break;
                                }
                            }
                            if (isFoundDictChildField == false)
                            {
                                errorString = string.Format("dict型字段\"{0}\"中不存在名为\"{1}\"的子元素", fieldInfo.FieldName, tempFieldName);
                                return null;
                            }
                        }
                        else
                        {
                            string tempFieldName = tempFieldNameBuilder.ToString();
                            fieldInfo = tableInfo.GetFieldInfoByFieldName(tempFieldName);
                            if (fieldInfo == null)
                            {
                                errorString = string.Format("此表格中不存在名为\"{0}\"的字段，截止到出错位置的定义字符串为{1}", tempFieldName, indexDefineString.Substring(0, i + 1));
                                return null;
                            }
                            if (fieldInfo.DataType != DataType.Array)
                            {
                                if (fieldInfo.DataType == DataType.Dict)
                                    errorString = string.Format("用[]只能索引array型字段，而你索引的是dict型字段\"{0}\"，请使用小数点来索引dict型字段", tempFieldName);
                                else
                                    errorString = string.Format("用[]只能索引array型字段，而你索引的是{0}型字段\"{1}\"", fieldInfo.DataType, tempFieldName);

                                return null;
                            }
                        }
                    }

                    isInBracket = true;
                    isAfterDot = false;
                    tempFieldNameBuilder = new StringBuilder();
                }
                else if (currentChar == ']')
                {
                    if (isInBracket == false)
                    {
                        errorString = string.Format("索引定义中括号不匹配，截止到出错位置的定义字符串为{0}", indexDefineString.Substring(0, i + 1));
                        return null;
                    }
                    string arrayIndexString = tempArrayIndexBuilder.ToString();
                    int arrayIndex = -1;
                    if (int.TryParse(arrayIndexString, out arrayIndex) == true)
                    {
                        if (arrayIndex > 0)
                        {
                            int arrayChildCount = fieldInfo.ChildField.Count;
                            if (arrayIndex > arrayChildCount)
                            {
                                errorString = string.Format("对array型字段\"{0}\"进行索引的数字非法，其只有{1}个子元素，而你要取第{2}个子元素", fieldInfo.FieldName, arrayChildCount, arrayIndex);
                                return null;
                            }
                            else
                                fieldInfo = fieldInfo.ChildField[arrayIndex - 1];
                        }
                        else
                        {
                            errorString = string.Format("对array型字段\"{0}\"进行索引的数字非法必须为大于0的数字，你输入的为{1}", fieldInfo.FieldName, arrayIndexString);
                            return null;
                        }
                    }
                    else
                    {
                        errorString = string.Format("对array型字段\"{0}\"进行索引的数字非法，你输入的为{1}", fieldInfo.FieldName, arrayIndexString);
                        return null;
                    }

                    isInBracket = false;
                    tempArrayIndexBuilder = new StringBuilder();
                }
                else
                {
                    if (isInBracket == false)
                        tempFieldNameBuilder.Append(currentChar);
                    else
                        tempArrayIndexBuilder.Append(currentChar);
                }
            }
            if (isAfterDot == true)
            {
                string tempFieldName = tempFieldNameBuilder.ToString();
                // 处理最后通过小数点索引Dict中子元素的情况
                if (fieldInfo != null && fieldInfo.DataType == DataType.Dict)
                {
                    bool isFoundDictChildField = false;
                    foreach (FieldInfo dictChildField in fieldInfo.ChildField)
                    {
                        if (tempFieldName.Equals(dictChildField.FieldName))
                        {
                            fieldInfo = dictChildField;
                            isFoundDictChildField = true;
                            break;
                        }
                    }
                    if (isFoundDictChildField == false)
                    {
                        if (fieldInfo.ParentField != null && fieldInfo.ParentField.DataType == DataType.Array)
                            errorString = string.Format("dict型字段\"{0}\"中不存在名为\"{1}\"的子元素", string.Concat(fieldInfo.ParentField.FieldName, fieldInfo.FieldName), tempFieldName);
                        else
                            errorString = string.Format("dict型字段\"{0}\"中不存在名为\"{1}\"的子元素", fieldInfo.FieldName, tempFieldName);

                        return null;
                    }
                }
            }
            if (isInBracket == true)
            {
                errorString = string.Format("索引定义中括号不匹配，你输入的索引定义字符串为{0}", indexDefineString);
                return null;
            }
        }

        errorString = null;
        return fieldInfo;
    }
}