using System;
using System.Collections.Generic;
using System.Text;

public partial class TableCheckHelper
{
    /// <summary>
    /// 用于string型按指定格式解析后，取值必须在另一字段（可能还是这张表格也可能跨表）中有对应值的检查
    /// ref2:table[entry_dungeon_drop.drop_id](split{1;x;y;z}(;))(except{0})
    /// </summary>
    /// <param name="fieldInfo">字段信息</param>
    /// <param name="checkRule">检查信息</param>
    /// <param name="errorString">错误信息</param>
    /// <returns></returns>
    public static bool CheckRefStr(FieldInfo fieldInfo, FieldCheckRule checkRule, out string errorString)
    {
        int IDPosition = 0;
        bool DataTypeString = true;
        // 首先要求字段类型只能为int、long、float或string型
        if (!(fieldInfo.DataType == DataType.String))
        {
            errorString = string.Format("值引用检查规则只适用于string类型的字段，要检查的这列类型为{0}\n", fieldInfo.DataType.ToString());
            return false;
        }
        else
        {
            string tableName;
            string fieldIndexDefine;

            // 解析ref2规则中目标列所在表格以及字段名
            const string START_STRING = "refStr:";
            if (!checkRule.CheckRuleString.StartsWith(START_STRING, StringComparison.CurrentCultureIgnoreCase))
            {
                errorString = string.Format("值引用检查规则声明错误test，必须以\"{0}\"开头，后面跟表格名-字段名test\n", START_STRING);
                return false;
            }
            else
            {
                string temp = checkRule.CheckRuleString.Substring(START_STRING.Length).Trim();//去掉前面的fef2:字符
                if (string.IsNullOrEmpty(temp))
                {
                    errorString = string.Format("值引用检查规则声明错误，\"{0}\"的后面必须跟表格名-字段名\n", START_STRING);
                    return false;
                }
                else
                {
                    // 判断是否在最后以(except{xx,xx})的格式声明无需ref2规则检查的特殊值
                    List<object> exceptValues = new List<object>();
                    int leftBracketIndex = temp.LastIndexOf("(except");
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

                    // 判断是否在最后以(split{1;x;y;z}(;))的格式声明无需refStr规则检查的特殊值
                    List<object> splitValues = new List<object>();
                    int splitleftBracketIndex = temp.IndexOf("(split");
                    int splitrightBracketIndex = temp.LastIndexOf(')');
                    if (splitleftBracketIndex != -1 && splitrightBracketIndex > splitleftBracketIndex)
                    {
                        // 取出括号中的排除值声明
                        const string SPLIT_DEFINE_START_STRING = "split";
                        string splitDefineString = temp.Substring(splitleftBracketIndex + 1, splitrightBracketIndex - splitleftBracketIndex - 1).Trim();//提取split括号内的声明内容
                        if (!splitDefineString.StartsWith(SPLIT_DEFINE_START_STRING, StringComparison.CurrentCultureIgnoreCase))
                        {
                            errorString = string.Format("值引用检查规则声明错误，若要声明refStr检查所忽略的特殊值，需以(split{1;x;y;z}(;))的形式声明，而你在括号中声明为\"{0}\"\n", splitDefineString);
                            return false;
                        }
                        else
                        {
                            // 检查排除值的声明（即有效值声明格式）是否合法
                            string splitValuesDefine = splitDefineString.Substring(SPLIT_DEFINE_START_STRING.Length).Trim();
                            splitValues = Utils.GetEffectiveValue(splitValuesDefine, fieldInfo.DataType, out errorString);
                            if (errorString != null)
                            {
                                errorString = string.Format("值引用检查规则声明错误，排除值的声明非法，{0}\n", errorString);
                                return false;
                            }
                            if (splitValues.Count < 2)
                            {
                                errorString = string.Format("值引用检查规则声明错误，split{1;x;y;z}内至少包含2个元素，而你在括号中声明为\"{0}\"\n", splitDefineString);
                                return false;
                            }

                            try
                            {
                                IDPosition = int.Parse(splitValues[0].ToString());
                            }
                            catch
                            {
                                errorString = string.Format("值引用检查规则声明错误，split{1;x;y;z}中第1个元素必须为正整数，而你在括号中声明为\"{0}\"\n", splitDefineString);
                                return false;
                            }
                            if (IDPosition == 0)
                            {
                                errorString = string.Format("值引用检查规则声明错误，split{1;x;y;z}中第1个元素必须为正整数,不能为0，而你在括号中声明为\"{0}\"\n", splitDefineString);
                                return false;
                            }
                            // 将定义字符串去掉except声明部分
                            temp = temp.Substring(0, splitleftBracketIndex).Trim();
                        }
                    }

                    FieldInfo targetFieldInfo = null;


                    const string START_STRING2 = "table[";
                    int rightBracketIndex2 = temp.LastIndexOf(']');
                    if (temp.StartsWith(START_STRING2, StringComparison.CurrentCultureIgnoreCase))//如果是以 refStr:table开头则
                    {
                        temp = temp.Substring(START_STRING2.Length, rightBracketIndex2 - 6).Trim();//提交[]内的表名和字段
                        if (string.IsNullOrEmpty(temp))
                        {
                            errorString = string.Format("值引用检查规则声明错误，\"{0}\"的后面必须跟[表格名.字段名,表格名.字段名]\n", START_STRING2);
                            return false;
                        }
                        //检查表名和字段
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
                            if (targetFieldInfo.DataType==DataType.Int)
                            {
                                DataTypeString = false;
                                //errorString = string.Format("值引用检查规则声明错误，表格\"{0}\"中通过索引字符串\"{1}\"找到的参考字段的数据类型为{2}，而要检查字段的数据类型为{3}，无法进行不同数据类型字段的引用检查\n", tableName, fieldIndexDefine, targetFieldInfo.DataType.ToString(), fieldInfo.DataType.ToString());
                                //return false;
                            }
                        }

                        Dictionary<string, object> unreferencedInfo = new Dictionary<string, object>();
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
                            Dictionary<string, object> tempunreferencedInfo = new Dictionary<string, object>();

                            if (DataTypeString == false)
                            {
                                string FirstSplit = splitValues[1].ToString();//第一分隔符
                                string SecondSplit = null;

                                for (int i = 0; i < fieldInfo.Data.Count; ++i)
                                {
                                    // 忽略无效集合元素下属子类型的空值以及空字符串
                                    if (fieldInfo.Data[i] == null || string.IsNullOrEmpty(fieldInfo.Data[i].ToString()))
                                        continue;
                                    // 忽略不进行ref检查的排除值
                                    else if (exceptValues.Contains(fieldInfo.Data[i]))
                                        continue;

                                    string[] FirstSplitData = null;//使用第一分隔符，获得的数组
                                    int z = 0;
                                    int splitDataInt = 0;
                                    switch (splitValues.Count)
                                    {
                                        case 2:

                                            FirstSplitData = fieldInfo.Data[i].ToString().Split(FirstSplit[0]);
                                            foreach (string splitData in FirstSplitData)
                                            {
                                                z++;
                                                if (splitData == null)
                                                {
                                                    tempunreferencedInfo.Add(i + "_" + z.ToString(), splitDataInt);
                                                    continue;
                                                }

                                                try
                                                {
                                                    splitDataInt = int.Parse(splitData);
                                                }
                                                catch
                                                {
                                                    tempunreferencedInfo.Add(i + "_" + z.ToString(), splitData);
                                                    continue;
                                                }

                                                if (!targetFieldData.Contains(splitDataInt))
                                                    tempunreferencedInfo.Add(i + "_" + z.ToString(), splitDataInt);
                                            }
                                            break;

                                        case 3:
                                            FirstSplitData = fieldInfo.Data[i].ToString().Split(FirstSplit[0]);

                                            SecondSplit = splitValues[2].ToString();

                                            foreach (string splitData in FirstSplitData)
                                            {
                                                z++;
                                                if (splitData == null)
                                                {
                                                    tempunreferencedInfo.Add(i + "_" + z.ToString(), splitDataInt);
                                                    continue;
                                                }

                                                string[] SecondSplitData = splitData.Split(SecondSplit[0]);

                                                if (SecondSplitData.Length < IDPosition - 1)
                                                {
                                                    tempunreferencedInfo.Add(i + "_" + z.ToString(), SecondSplitData.ToString());
                                                    continue;
                                                }

                                                try
                                                {
                                                    splitDataInt = int.Parse(SecondSplitData[IDPosition - 1].ToString());
                                                }
                                                catch
                                                {
                                                    tempunreferencedInfo.Add(i + "_" + z.ToString(), SecondSplitData[IDPosition - 1].ToString());
                                                    continue;
                                                }

                                                if (!targetFieldData.Contains(splitDataInt))
                                                    tempunreferencedInfo.Add(i + "_" + z.ToString(), splitDataInt);
                                            }
                                            break;

                                        default:
                                            break;
                                    }
                                }
                            }

                            if (tempunreferencedInfo.Count == 0)
                            {
                                break;
                            }
                            if (unreferencedInfo.Count > 0)
                            {
                                List<string> tempList = new List<string>();
                                foreach (KeyValuePair<string, object> kvp in unreferencedInfo)
                                {
                                    if (tempunreferencedInfo.ContainsKey(kvp.Key))
                                    {
                                        //存在处理
                                    }
                                    else
                                    {
                                        tempList.Add(kvp.Key);
                                    }
                                }
                                foreach (string k in tempList)
                                {
                                    unreferencedInfo.Remove(k);//不存在就移除
                                }
                            }
                            else
                            {
                                if (tempunreferencedInfo.Count > 0)
                                {
                                    foreach (KeyValuePair<string, object> kvp in tempunreferencedInfo)
                                    {
                                        unreferencedInfo.Add(kvp.Key, kvp.Value);
                                    }
                                }
                            }
                        }

                        if (unreferencedInfo.Count > 0)
                        {
                            StringBuilder errorStringBuild = new StringBuilder();
                            errorStringBuild.AppendLine("存在以下未找到引用关系的数据行：");
                            foreach (var item in unreferencedInfo)
                            {
                                string[] item2 = item.Key.ToString().Split('_');

                                int irow = int.Parse(item2[0]) + AppValues.DATA_FIELD_DATA_START_INDEX + 1;
                                errorStringBuild.AppendFormat("第{0}行第{1}组数据\"{2}\"在对应参考列不存在\n", irow.ToString(), item2[1], item.Value);
                            }

                            errorString = errorStringBuild.ToString();
                            return false;
                        }
                        else
                        {
                            errorString = null;
                            return true;
                        }

                    }
                    errorString = null;
                    return true;
                }
            }
        }
    }
}