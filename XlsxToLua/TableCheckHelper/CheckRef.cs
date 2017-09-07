using System;
using System.Collections.Generic;
using System.Text;

public partial class TableCheckHelper
{
    /// <summary>
    /// 用于int、long、float或string型取值必须在另一字段（可能还是这张表格也可能跨表）中有对应值的检查
    /// </summary>
    public static bool CheckRef(FieldInfo fieldInfo, FieldCheckRule checkRule, out string errorString)
    {
        // 首先要求字段类型只能为int、long、float或string型
        if (!(fieldInfo.DataType == DataType.Int || fieldInfo.DataType == DataType.Long || fieldInfo.DataType == DataType.Float || fieldInfo.DataType == DataType.String))
        {
            errorString = string.Format("值引用检查规则只适用于int、long、float或string类型的字段，要检查的这列类型为{0}\n", fieldInfo.DataType.ToString());
            return false;
        }
        else
        {
            string tableName;
            string fieldIndexDefine;

            // 解析ref规则中目标列所在表格以及字段名
            const string START_STRING = "ref:";
            if (!checkRule.CheckRuleString.StartsWith(START_STRING, StringComparison.CurrentCultureIgnoreCase))
            {
                errorString = string.Format("值引用检查规则声明错误，必须以\"{0}\"开头，后面跟表格名-字段名\n", START_STRING);
                return false;
            }
            else
            {
                string temp = checkRule.CheckRuleString.Substring(START_STRING.Length).Trim();//去掉前面的fef:字符
                if (string.IsNullOrEmpty(temp))
                {
                    errorString = string.Format("值引用检查规则声明错误，\"{0}\"的后面必须跟表格名-字段名\n", START_STRING);
                    return false;
                }
                else
                {
                    // 判断是否在最后以(except{xx,xx})的格式声明无需ref规则检查的特殊值
                    List<object> exceptValues = new List<object>();
                    int leftBracketIndex = temp.IndexOf('(');
                    int rightBracketIndex = temp.LastIndexOf(')');
                    if (leftBracketIndex != -1 && rightBracketIndex > leftBracketIndex)
                    {
                        // 取出括号中的排除值声明
                        const string EXCEPT_DEFINE_START_STRING = "except";
                        string exceptDefineString = temp.Substring(leftBracketIndex + 1, rightBracketIndex - leftBracketIndex - 1).Trim();//提取except括号内的声明内容
                        if (!exceptDefineString.StartsWith(EXCEPT_DEFINE_START_STRING, StringComparison.CurrentCultureIgnoreCase))
                        {
                            errorString = string.Format("值引用检查规则声明错误，若要声明ref检查所忽略的特殊值，需在最后以(except{xx,xx})的形式声明，而你在括号中声明为\"{0}\"\n", exceptDefineString);
                            return false;
                        }
                        else
                        {
                            // 检查排除值的声明（即有效值声明格式）是否合法
                            string exceptValuesDefine = exceptDefineString.Substring(EXCEPT_DEFINE_START_STRING.Length).Trim();
                            exceptValues = Utils.GetEffectiveValue(exceptValuesDefine, fieldInfo.DataType, out errorString);
                            if (errorString != null)
                            {
                                errorString = string.Format("值引用检查规则声明错误，排除值的声明非法，{0}\n", errorString);
                                return false;
                            }

                            // 将定义字符串去掉except声明部分
                            temp = temp.Substring(0, leftBracketIndex).Trim();
                        }
                    }

                    FieldInfo targetFieldInfo = null;

                    #region 多表多字段情况 ref:table[entry_item.item_id,entry_item_weapon.weapon_id,entry_partner.entry_id](except{0})

                    const string START_STRING2 = "table[";
                    int rightBracketIndex2 = temp.LastIndexOf(']');
                    if (temp.StartsWith(START_STRING2, StringComparison.CurrentCultureIgnoreCase))//如果是以 ref:table开头则
                    {
                        temp = temp.Substring(START_STRING2.Length, rightBracketIndex2 - 6).Trim();//提交[]内的表名和字段
                        if (string.IsNullOrEmpty(temp))
                        {
                            errorString = string.Format("值引用检查规则声明错误，\"{0}\"的后面必须跟[表格名.字段名,表格名.字段名]\n", START_STRING2);
                            return false;
                        }
                        //检查表名和字段
                        //   FieldInfo targetFieldInfo2 = null;
                        string[] values = temp.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                        for (int i = 0; i < values.Length; ++i)
                        {
                            string tempNameField = values[i].Trim();
                            // 解析参考表名、列名声明
                            targetFieldInfo = null;
                            int hyphenIndex = tempNameField.LastIndexOf('.');
                            if (hyphenIndex == -1)
                            {
                                tableName = tempNameField;
                                fieldIndexDefine = null;
                            }
                            else
                            {
                                tableName = tempNameField.Substring(0, hyphenIndex).Trim();
                                fieldIndexDefine = tempNameField.Substring(hyphenIndex + 1, tempNameField.Length - hyphenIndex - 1);
                            }

                            if (!AppValues.TableInfo.ContainsKey(tableName))
                            {
                                errorString = string.Format("值引用检查规则声明错误，找不到名为 {0} 的表格\n", tableName);
                                return false;
                            }
                            if (string.IsNullOrEmpty(fieldIndexDefine))
                                targetFieldInfo = AppValues.TableInfo[tableName].GetKeyColumnFieldInfo();
                            else
                            {
                                TableInfo targetTableInfo = AppValues.TableInfo[tableName];
                                targetFieldInfo = GetFieldByIndexDefineString(fieldIndexDefine, targetTableInfo, out errorString);
                                if (errorString != null)
                                {
                                    errorString = string.Format("值引用检查规则声明错误，表格\"{0}\"中无法根据索引字符串\"{1}\"找到要参考的字段，错误信息为：{2}\n", tableName, fieldIndexDefine, errorString);
                                    return false;
                                }
                            }
                            // 检查目标字段必须为相同的数据类型
                            if (fieldInfo.DataType != targetFieldInfo.DataType)
                            {
                                errorString = string.Format("值引用检查规则声明错误，表格\"{0}\"中通过索引字符串\"{1}\"找到的参考字段的数据类型为{2}，而要检查字段的数据类型为{3}，无法进行不同数据类型字段的引用检查\n", tableName, fieldIndexDefine, targetFieldInfo.DataType.ToString(), fieldInfo.DataType.ToString());
                                return false;
                            }
                        }

                        Dictionary<int, object> unreferencedInfo = new Dictionary<int, object>();
                        for (int j = 0; j < values.Length; ++j)
                        {
                            string tempNameField = values[j].Trim();
                            // 解析参考表名、列名声明
                            targetFieldInfo = null;
                            int hyphenIndex = tempNameField.LastIndexOf('.');
                            if (hyphenIndex == -1)
                            {
                                tableName = tempNameField;
                                fieldIndexDefine = null;
                            }
                            else
                            {
                                tableName = tempNameField.Substring(0, hyphenIndex).Trim();
                                fieldIndexDefine = tempNameField.Substring(hyphenIndex + 1, tempNameField.Length - hyphenIndex - 1);
                            }

                            if (string.IsNullOrEmpty(fieldIndexDefine))
                                targetFieldInfo = AppValues.TableInfo[tableName].GetKeyColumnFieldInfo();
                            else
                            {
                                TableInfo targetTableInfo = AppValues.TableInfo[tableName];
                                targetFieldInfo = GetFieldByIndexDefineString(fieldIndexDefine, targetTableInfo, out errorString);
                            }

                            List<object> targetFieldData = targetFieldInfo.Data;
                            // 存储找不到引用对应关系的数据信息（key：行号， value：填写的数据）
                            Dictionary<int, object> tempunreferencedInfo = new Dictionary<int, object>();

                            if (fieldInfo.DataType == DataType.Int || fieldInfo.DataType == DataType.Long || fieldInfo.DataType == DataType.Float)
                            {
                                for (int i = 0; i < fieldInfo.Data.Count; ++i)
                                {
                                    // 忽略无效集合元素下属子类型的空值或本身为空值
                                    if (fieldInfo.Data[i] == null)
                                        continue;
                                    // 忽略不进行ref检查的排除值
                                    else if (exceptValues.Contains(fieldInfo.Data[i]))
                                        continue;

                                    if (!targetFieldData.Contains(fieldInfo.Data[i]))
                                        tempunreferencedInfo.Add(i, fieldInfo.Data[i]);
                                }
                            }
                            else if (fieldInfo.DataType == DataType.String)
                            {
                                for (int i = 0; i < fieldInfo.Data.Count; ++i)
                                {
                                    // 忽略无效集合元素下属子类型的空值以及空字符串
                                    if (fieldInfo.Data[i] == null || string.IsNullOrEmpty(fieldInfo.Data[i].ToString()))
                                        continue;
                                    // 忽略不进行ref检查的排除值
                                    else if (exceptValues.Contains(fieldInfo.Data[i]))
                                        continue;

                                    if (!targetFieldData.Contains(fieldInfo.Data[i]))
                                        tempunreferencedInfo.Add(i, fieldInfo.Data[i]);
                                }
                            }
                            if (tempunreferencedInfo.Count == 0)
                            {
                                break;
                            }
                            //  var dz = unreferencedInfo.Keys.Intersect(tempunreferencedInfo.Keys);
                            if (unreferencedInfo.Count > 0)
                            {
                                List<int> tempList = new List<int>();
                                foreach (KeyValuePair<int, object> kvp in unreferencedInfo)
                                {
                                    if (tempunreferencedInfo.ContainsKey(kvp.Key))
                                    {
                                        //存在处理
                                    }
                                    else
                                    {
                                        tempList.Add(kvp.Key);
                                        // unreferencedInfo.Remove(kvp.Key);//不存在就移除
                                    }
                                }
                                foreach (int k in tempList)
                                {
                                    unreferencedInfo.Remove(k);//不存在就移除
                                }
                            }
                            else
                            {
                                if (tempunreferencedInfo.Count > 0)
                                {
                                    foreach (KeyValuePair<int, object> kvp in tempunreferencedInfo)
                                    {
                                        unreferencedInfo.Add(kvp.Key, kvp.Value);
                                    }
                                }
                                else
                                {
                                }
                            }
                        }

                        if (unreferencedInfo.Count > 0)
                        {
                            StringBuilder errorStringBuild = new StringBuilder();
                            errorStringBuild.AppendLine("存在以下未找到引用关系的数据行：");
                            foreach (var item in unreferencedInfo)
                                errorStringBuild.AppendFormat("第{0}行数据\"{1}\"在对应参考列不存在\n", item.Key + AppValues.DATA_FIELD_DATA_START_INDEX + 1, item.Value);

                            errorString = errorStringBuild.ToString();
                            return false;
                        }
                        else
                        {
                            errorString = null;
                            return true;
                        }
                    }

                    #endregion 多表多字段情况 ref:table[entry_item.item_id,entry_item_weapon.weapon_id,entry_partner.entry_id](except{0})

                    // 解析参考表名、列名声明

                    int hyphenIndex2 = temp.LastIndexOf('-');
                    if (hyphenIndex2 == -1)
                    {
                        tableName = temp;
                        fieldIndexDefine = null;
                    }
                    else
                    {
                        tableName = temp.Substring(0, hyphenIndex2).Trim();
                        fieldIndexDefine = temp.Substring(hyphenIndex2 + 1, temp.Length - hyphenIndex2 - 1);
                    }

                    if (!AppValues.TableInfo.ContainsKey(tableName))
                    {
                        errorString = string.Format("值引用检查规则声明错误，找不到名为{0}的表格\n", START_STRING);
                        return false;
                    }
                    if (string.IsNullOrEmpty(fieldIndexDefine))
                        targetFieldInfo = AppValues.TableInfo[tableName].GetKeyColumnFieldInfo();
                    else
                    {
                        TableInfo targetTableInfo = AppValues.TableInfo[tableName];
                        targetFieldInfo = GetFieldByIndexDefineString(fieldIndexDefine, targetTableInfo, out errorString);
                        if (errorString != null)
                        {
                            errorString = string.Format("值引用检查规则声明错误，表格\"{0}\"中无法根据索引字符串\"{1}\"找到要参考的字段，错误信息为：{2}\n", tableName, fieldIndexDefine, errorString);
                            return false;
                        }
                    }
                    // 检查目标字段必须为相同的数据类型
                    if (fieldInfo.DataType != targetFieldInfo.DataType)
                    {
                        errorString = string.Format("值引用检查规则声明错误，表格\"{0}\"中通过索引字符串\"{1}\"找到的参考字段的数据类型为{2}，而要检查字段的数据类型为{3}，无法进行不同数据类型字段的引用检查\n", tableName, fieldIndexDefine, targetFieldInfo.DataType.ToString(), fieldInfo.DataType.ToString());
                        return false;
                    }
                    else
                    {
                        List<object> targetFieldData = targetFieldInfo.Data;
                        // 存储找不到引用对应关系的数据信息（key：行号， value：填写的数据）
                        Dictionary<int, object> unreferencedInfo = new Dictionary<int, object>();

                        if (fieldInfo.DataType == DataType.Int || fieldInfo.DataType == DataType.Long || fieldInfo.DataType == DataType.Float)
                        {
                            for (int i2 = 0; i2 < fieldInfo.Data.Count; ++i2)
                            {
                                // 忽略无效集合元素下属子类型的空值或本身为空值
                                if (fieldInfo.Data[i2] == null)
                                    continue;
                                // 忽略不进行ref检查的排除值
                                else if (exceptValues.Contains(fieldInfo.Data[i2]))
                                    continue;

                                if (!targetFieldData.Contains(fieldInfo.Data[i2]))
                                    unreferencedInfo.Add(i2, fieldInfo.Data[i2]);
                            }
                        }
                        else if (fieldInfo.DataType == DataType.String)
                        {
                            for (int i3 = 0; i3 < fieldInfo.Data.Count; ++i3)
                            {
                                // 忽略无效集合元素下属子类型的空值以及空字符串
                                if (fieldInfo.Data[i3] == null || string.IsNullOrEmpty(fieldInfo.Data[i3].ToString()))
                                    continue;
                                // 忽略不进行ref检查的排除值
                                else if (exceptValues.Contains(fieldInfo.Data[i3]))
                                    continue;

                                if (!targetFieldData.Contains(fieldInfo.Data[i3]))
                                    unreferencedInfo.Add(i3, fieldInfo.Data[i3]);
                            }
                        }

                        if (unreferencedInfo.Count > 0)
                        {
                            StringBuilder errorStringBuild = new StringBuilder();
                            errorStringBuild.AppendLine("存在以下未找到引用关系的数据行：");
                            foreach (var item in unreferencedInfo)
                                errorStringBuild.AppendFormat("第{0}行数据\"{1}\"在对应参考列不存在\n", item.Key + AppValues.DATA_FIELD_DATA_START_INDEX + 1, item.Value);

                            errorString = errorStringBuild.ToString();
                            return false;
                        }
                        else
                        {
                            errorString = null;
                            return true;
                        }
                    }
                }
            }
        }
    }
}