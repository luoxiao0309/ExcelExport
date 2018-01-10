using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

public class TableExportToTxtHelper
{
    /// <summary>
    /// txt特殊方式导出
    /// tableExportTxtConfig
    /// System:extension=hrl|-define(|{systemName}|,|{systemId}|).|    %%|{txet}
    /// </summary>
    /// <param name="tableInfo"></param>
    /// <param name="exportRule"></param>
    /// <param name="errorString"></param>
    /// <returns></returns>
    public static bool SpecialExportTableTxt(TableInfo tableInfo, string exportRule, out string errorString)
    {
        exportRule = exportRule.Trim();
        // 解析按这种方式导出后的txt文件名
        int colonIndex = exportRule.IndexOf(':');
        if (colonIndex == -1)
        {
            errorString = string.Format("导出配置\"{0}\"定义错误，必须在开头声明导出txt文件名\n", exportRule);
            return false;
        }
        string fileName = exportRule.Substring(0, colonIndex).Trim();//文件名

        // 解析依次作为索引的字段名
        string indexFieldNameString = exportRule.Substring(colonIndex + 1);

        string[] indexFieldDefine = indexFieldNameString.Split(new char[] { '|' }, System.StringSplitOptions.RemoveEmptyEntries);

        // 用于索引的字段列表
        List<FieldInfo> indexField = new List<FieldInfo>();
        // 索引字段对应的完整性检查规则
        List<string> integrityCheckRules = new List<string>();
        if (indexFieldDefine.Length < 1)
        {
            errorString = string.Format("导出配置\"{0}\"定义错误，用于索引的字段不能为空，请按fileName:indexFieldName1-indexFieldName2{otherFieldName1,otherFieldName2}的格式配置\n", exportRule);
            return false;
        }
        // 存储每一行数据生成的txt文件内容
        List<StringBuilder> rowContentList = new List<StringBuilder>();

        string SpecialExportTxtExtension = null;
        string SpecialExportTxtPath = null;
        int SpecialTopCommentRows = 0;
        string SpecialTopComment = null;

        // 生成主键列的同时，对每行的StringBuilder进行初始化，主键列只能为int、long或string型，且值不允许为空，直接转为字符串即可
        FieldInfo keyColumnFieldInfo = tableInfo.GetKeyColumnFieldInfo();
        int rowCount = keyColumnFieldInfo.Data.Count;
        for (int row = 0; row < rowCount; ++row)
        {
            StringBuilder stringBuilder = new StringBuilder();
            // 检查字段是否存在且为int、float、string或lang型
            foreach (string fieldDefine in indexFieldDefine)
            {
                if (fieldDefine.StartsWith(AppValues.SPECIAL_EXPORT_TXT_PARAM_EXTENSION_PARAM_STRING))//文件后缀，如 extension=txt
                {
                    string[] extension = fieldDefine.Split(new char[] { '=' });
                    SpecialExportTxtExtension = extension[1].ToString().Trim();
                    continue;
                }
                if (fieldDefine.StartsWith(AppValues.SPECIAL_EXPORT_TXT_PARAM_EXPORT_PATH_PARAM_STRING))//文件导出目录，如exportPath = C:\Users\Administrator\Desktop
                {
                    string[] exportTxtPath = fieldDefine.Split(new char[] { '=' });
                    SpecialExportTxtPath = exportTxtPath[1].ToString().Trim();
                    continue;
                }
                if (fieldDefine.StartsWith(AppValues.SPECIAL_EXPORT_TXT_PARAM_TOPCOMMENTROWS_PARAM_STRING))//配置导出文件上方注释说明的行数
                {
                    string[] TopCommentRows = fieldDefine.Split(new char[] { '=' });
                    SpecialTopCommentRows = int.Parse(TopCommentRows[1].ToString().Trim());
                    continue;
                }
                if (fieldDefine.StartsWith(AppValues.SPECIAL_EXPORT_TXT_PARAM_TOPCOMMENT_PARAM_STRING))//配置导出文件上方注释说明内容
                {
                    string[] TopComment = fieldDefine.Split(new char[] { '=' });
                    SpecialTopComment = TopComment[1].ToString().Trim();
                    continue;
                }
                if (fieldDefine == "\\r\\n")
                {
                    //stringBuilder.Append("\\r\\n");
                    stringBuilder.AppendLine("");
                    continue;
                }
                if (fieldDefine.StartsWith("{") && fieldDefine.EndsWith("}"))
                {
                    string fieldName = fieldDefine.Substring(1, fieldDefine.Length - 2);
                    FieldInfo fieldInfo = tableInfo.GetFieldInfoByFieldName(fieldName);
                    stringBuilder.Append(fieldInfo.Data[row]);
                }

                else
                {
                    stringBuilder.Append(fieldDefine);
                }
            }
            rowContentList.Add(stringBuilder);
        }

        if(SpecialExportTxtExtension==null)
        {
            SpecialExportTxtExtension = AppValues.SpecialExportTxtExtension;
        }
        if (SpecialExportTxtPath == null)//如果不存在文件夹就创建
        {
            string p = SpecialExportTxtExtension.ToUpper();
            if (!System.IO.Directory.Exists(p))
            {
                System.IO.Directory.CreateDirectory(p);

            }
            SpecialExportTxtPath = p;
        }
        if (SpecialTopCommentRows > 0)//默认注释文本内容
        {
            if(SpecialTopComment==null)
            {
                SpecialTopComment = AppValues.SPECIAL_EXPORT_TXT_PARAM_TOPCOMMENT_STRING;
            }
        }

        for (int i = 0; i < SpecialTopCommentRows; i++)
        {
            StringBuilder stringbuilder = new StringBuilder();
            stringbuilder.Append(SpecialTopComment);
            rowContentList.Insert(0, stringbuilder);
        }
        // stringbuilder = stringbuilder.Remove(stringbuilder.Length - 3, 2);
        

        // 保存为txt文件 SpecialExportTxtExtension
        if (Utils.SaveTxtFile( rowContentList, SpecialExportTxtPath, fileName, SpecialExportTxtExtension))
        {
            errorString = null;

            return true;
        }
        else
        {
            errorString = string.Format("保存为{0}文件失败\n", SpecialExportTxtExtension);

            return false;
        }
    }

    public static bool ExportTableToTxt(TableInfo tableInfo, out string errorString)
    {
        // 存储每一行数据生成的txt文件内容
        List<StringBuilder> rowContentList = new List<StringBuilder>();

        // 生成主键列的同时，对每行的StringBuilder进行初始化，主键列只能为int、long或string型，且值不允许为空，直接转为字符串即可
        FieldInfo keyColumnFieldInfo = tableInfo.GetKeyColumnFieldInfo();
        int rowCount = keyColumnFieldInfo.Data.Count;
        for (int row = 0; row < rowCount; ++row)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append(keyColumnFieldInfo.Data[row]);
            rowContentList.Add(stringBuilder);
        }
        // 生成其他列的内容（将array、dict这样的集合类型下属字段作为独立字段处理）
        List<FieldInfo> allFieldInfoIgnoreSetDataStructure = tableInfo.GetAllClientFieldInfoIgnoreSetDataStructure();
        for (int i = 1; i < allFieldInfoIgnoreSetDataStructure.Count; ++i)
            _GetOneFieldTxtContent(allFieldInfoIgnoreSetDataStructure[i], rowContentList);

        // 如果声明了要在txt中显示 Lua等客户端数据类型
        if (AppValues.ExportTxtIsExportDatabseField == true)
        {
            StringBuilder tempStringBuilder = new StringBuilder();
            for (int i = 0; i < allFieldInfoIgnoreSetDataStructure.Count; ++i)
            {
                tempStringBuilder.Append(AppValues.ExportTxtSplitChar);
                FieldInfo fieldInfo = allFieldInfoIgnoreSetDataStructure[i];
                tempStringBuilder.Append(fieldInfo.DatabaseFieldName == null ? null : fieldInfo.DatabaseFieldName + "(" + fieldInfo.DatabaseFieldType + ")");
            }

            // 去掉开头多加的一个分隔符
            rowContentList.Insert(0, tempStringBuilder.Remove(0, 1));
        }
        // 如果声明了要在txt中显示 声明字段检查字符串
        if (AppValues.ExportTxtIsExportCheckRule== true)
        {
            StringBuilder tempStringBuilder = new StringBuilder();
            for (int i = 0; i < allFieldInfoIgnoreSetDataStructure.Count; ++i)
            {
                tempStringBuilder.Append(AppValues.ExportTxtSplitChar);
                FieldInfo fieldInfo = allFieldInfoIgnoreSetDataStructure[i];
                tempStringBuilder.Append(fieldInfo.CheckRule);
            }

            // 去掉开头多加的一个分隔符
            rowContentList.Insert(0, tempStringBuilder.Remove(0, 1));
        }
        // 如果声明了要在txt中显示 Lua等客户端数据类型
        if (AppValues.ExportTxtIsExportDataType == true)
        {
            StringBuilder tempStringBuilder = new StringBuilder();
            for (int i = 0; i < allFieldInfoIgnoreSetDataStructure.Count; ++i)
            {
                tempStringBuilder.Append(AppValues.ExportTxtSplitChar);
                FieldInfo fieldInfo = allFieldInfoIgnoreSetDataStructure[i];
                tempStringBuilder.Append(fieldInfo.DataType);
            }

            // 去掉开头多加的一个分隔符
            rowContentList.Insert(0, tempStringBuilder.Remove(0, 1));
        }

        // 如果声明了要在txt中显示 英文字段名
        if (AppValues.ExportTxtIsExportFieldName == true)
        {
            StringBuilder tempStringBuilder = new StringBuilder();
            for (int i = 0; i < allFieldInfoIgnoreSetDataStructure.Count; ++i)
            {
                tempStringBuilder.Append(AppValues.ExportTxtSplitChar);
                FieldInfo fieldInfo = allFieldInfoIgnoreSetDataStructure[i];
                // 如果是array下属的子元素，字段名生成格式为“array字段名[从1开始的下标序号]”。dict下属的子元素，生成格式为“dict字段名.下属字段名”
                if (fieldInfo.ParentField != null)
                {
                    String fieldName = fieldInfo.FieldName;
                    FieldInfo tempField = fieldInfo;
                    while (tempField.ParentField != null)
                    {
                        if (tempField.ParentField.DataType == DataType.Array)
                            fieldName = string.Concat(tempField.ParentField.FieldName, fieldName);
                        else if (tempField.ParentField.DataType == DataType.Dict)
                            fieldName = string.Concat(tempField.ParentField.FieldName, ".", fieldName);

                        tempField = tempField.ParentField;
                    }

                    tempStringBuilder.Append(fieldName);
                }
                else
                    tempStringBuilder.Append(fieldInfo.FieldName);
            }

            // 去掉开头多加的一个分隔符
            rowContentList.Insert(0, tempStringBuilder.Remove(0, 1));
            //rowContentList.Insert(0, columnNameStringBuilder.Remove(0, AppValues.ExportTxtSplitChar.Length));
        }

        // 如果声明了要在txt中显示 中文字段名
        if (AppValues.ExportTxtIsExportDesc == true)
        {
            StringBuilder tempStringBuilder = new StringBuilder();
            for (int i = 0; i < allFieldInfoIgnoreSetDataStructure.Count; ++i)
            {
                tempStringBuilder.Append(AppValues.ExportTxtSplitChar);
                FieldInfo fieldInfo = allFieldInfoIgnoreSetDataStructure[i];
                tempStringBuilder.Append(fieldInfo.Desc.Replace("\n", "\\n"));
            }

            // 去掉开头多加的一个分隔符
            rowContentList.Insert(0, tempStringBuilder.Remove(0, 1));
        }


        //config表
        StringBuilder tempStringBuilder2 = new StringBuilder();
        tempStringBuilder2.Append("");
        rowContentList.Add(tempStringBuilder2);
        rowContentList.Add(tempStringBuilder2);
        rowContentList.Add(tempStringBuilder2);
        rowContentList.Add(tempStringBuilder2);
        rowContentList.Add(tempStringBuilder2);
        StringBuilder tempStringBuilder3 = new StringBuilder();
        tempStringBuilder3.Append("============以下为config表中的配置================");
        rowContentList.Add(tempStringBuilder3);

        DataTable dt = tableInfo.TableConfigData;
        rowCount = dt.Rows.Count;
        int columnCount = dt.Columns.Count;
        for (int row = 0; row < rowCount; ++row)
        {
            string str = null;
            for (int column = 0; column < columnCount; ++column)
            {
                str = str + dt.Rows[row][column].ToString() + AppValues.ExportTxtSplitChar;
            }

            //str.Remove(str.Length-1, 1);
            str = str.TrimEnd(AppValues.ExportTxtSplitChar);
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append(str);
            rowContentList.Add(stringBuilder);
            // 去掉开头多加的一个分隔符
            // rowContentList.Insert(AppValues.ExportTxtIsExportColumnName == true ? 1 : 0, );
        }



        string ExportTxtPath = null;
        if (AppValues.ExportTxtPath == null)
        {
            ExportTxtPath = tableInfo.ExcelDirectoryName;
        }
        else
        {
            ExportTxtPath = AppValues.ExportTxtPath;
        }
        // 保存为txt文件
        if (Utils.SaveTxtFile( rowContentList, ExportTxtPath, tableInfo.TableName, null))
        {
            errorString = null;
            return true;
        }
        else
        {
            errorString = "保存为txt文件失败\n";
            return false;
        }
    }

    public static bool ExportTableConfigDataToTxt(TableInfo tableInfo, out string errorString)
    {
        DataTable dt = tableInfo.TableConfigData;
        int rowCount = dt.Rows.Count;
        int columnCount = dt.Columns.Count;

        // 存储每一行数据生成的txt文件内容
        List<StringBuilder> rowContentList = new List<StringBuilder>();
        //if (columnCount == 0)
        //{
        //    StringBuilder stringBuilder = new StringBuilder();
        //    string param = null;
        //    stringBuilder.Append(param);
        //    rowContentList.Add(stringBuilder);
        //}
        //else
        //{
            for (int row = 0; row < rowCount; ++row)
            {
            string str = null;
                for (int column = 0; column < columnCount; ++column)
                {
                str = str+ dt.Rows[row][column].ToString()+ AppValues.ExportTxtSplitChar;
                }

            //str.Remove(str.Length-1, 1);
            str=str.TrimEnd(AppValues.ExportTxtSplitChar);
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append(str);
            rowContentList.Add(stringBuilder);
                // 去掉开头多加的一个分隔符
                // rowContentList.Insert(AppValues.ExportTxtIsExportColumnName == true ? 1 : 0, );
            }
        // }
        string ExportTxtPath = null;
        if (AppValues.ExportTxtPath == null)
        {
            ExportTxtPath = tableInfo.ExcelDirectoryName;
        }
        else
        {
            ExportTxtPath = AppValues.ExportTxtPath;
        }
        string tableName = tableInfo.TableName + "_Config";
        // 保存为txt文件
        if (Utils.SaveTxtFile(rowContentList, ExportTxtPath, tableName, null))
        {
            errorString = null;
            return true;
        }
        else
        {
            errorString = "保存为txt文件失败\n";
            return false;
        }
    }

    private static void _GetOneFieldTxtContent(FieldInfo fieldInfo, List<StringBuilder> rowContentList)
    {
        int rowCount = fieldInfo.Data.Count;

        switch (fieldInfo.DataType)
        {
            case DataType.Int:
            case DataType.Long:
            case DataType.Float:
            case DataType.String:
                {
                    for (int row = 0; row < rowCount; ++row)
                    {
                        StringBuilder stringBuilder = rowContentList[row];
                        // 先增加与上一字段间的分隔符
                        stringBuilder.Append(AppValues.ExportTxtSplitChar);
                        // 再生成本行对应的内容
                        if (fieldInfo.Data[row] != null)

                            stringBuilder.Append(fieldInfo.Data[row].ToString().Replace("\n", "\\n"));
                    }
                    break;
                }
            case DataType.Lang:
            case DataType.TableString:
                {
                    for (int row = 0; row < rowCount; ++row)
                    {
                        StringBuilder stringBuilder = rowContentList[row];
                        // 先增加与上一字段间的分隔符
                        stringBuilder.Append(AppValues.ExportTxtSplitChar);
                        // 再生成本行对应的内容
                        if (fieldInfo.Data[row] != null)

                            stringBuilder.Append(fieldInfo.Data[row].ToString().Replace("\n", "\\n"));
                    }
                    break;
                }
            case DataType.Bool:
                {
                    for (int row = 0; row < rowCount; ++row)
                    {
                        StringBuilder stringBuilder = rowContentList[row];
                        stringBuilder.Append(AppValues.ExportTxtSplitChar);
                        if (fieldInfo.Data[row] != null)
                        {
                            if ((bool)fieldInfo.Data[row] == true)
                                stringBuilder.Append("true");
                            else
                                stringBuilder.Append("false");
                        }
                    }
                    break;
                }
            case DataType.Json:
                {
                    for (int row = 0; row < rowCount; ++row)
                    {
                        StringBuilder stringBuilder = rowContentList[row];
                        stringBuilder.Append(AppValues.ExportTxtSplitChar);
                        if (fieldInfo.Data[row] != null)
                            stringBuilder.Append(fieldInfo.JsonString[row]);
                    }
                    break;
                }
            case DataType.Date:
                {
                    DateFormatType dateFormatType = TableAnalyzeHelper.GetDateFormatType(fieldInfo.ExtraParam[AppValues.TABLE_INFO_EXTRA_PARAM_KEY_DATE_TO_LUA_FORMAT].ToString());
                    string exportFormatString = null;
                    // 若date型声明toLua的格式为dateTable，则按input格式进行导出
                    if (dateFormatType == DateFormatType.DataTable)
                    {
                        dateFormatType = TableAnalyzeHelper.GetDateFormatType(fieldInfo.ExtraParam[AppValues.TABLE_INFO_EXTRA_PARAM_KEY_DATE_INPUT_FORMAT].ToString());
                        exportFormatString = fieldInfo.ExtraParam[AppValues.TABLE_INFO_EXTRA_PARAM_KEY_DATE_INPUT_FORMAT].ToString();
                    }
                    else
                        exportFormatString = fieldInfo.ExtraParam[AppValues.TABLE_INFO_EXTRA_PARAM_KEY_DATE_TO_LUA_FORMAT].ToString();

                    switch (dateFormatType)
                    {
                        case DateFormatType.FormatString:
                            {
                                for (int row = 0; row < rowCount; ++row)
                                {
                                    StringBuilder stringBuilder = rowContentList[row];
                                    stringBuilder.Append(AppValues.ExportTxtSplitChar);
                                    if (fieldInfo.Data[row] != null)
                                        stringBuilder.Append(((DateTime)(fieldInfo.Data[row])).ToString(exportFormatString));
                                }
                                break;
                            }
                        case DateFormatType.ReferenceDateSec:
                            {
                                for (int row = 0; row < rowCount; ++row)
                                {
                                    StringBuilder stringBuilder = rowContentList[row];
                                    stringBuilder.Append(AppValues.ExportTxtSplitChar);
                                    if (fieldInfo.Data[row] != null)
                                        stringBuilder.Append(((DateTime)(fieldInfo.Data[row]) - AppValues.REFERENCE_DATE).TotalSeconds);
                                }
                                break;
                            }
                        case DateFormatType.ReferenceDateMsec:
                            {
                                for (int row = 0; row < rowCount; ++row)
                                {
                                    StringBuilder stringBuilder = rowContentList[row];
                                    stringBuilder.Append(AppValues.ExportTxtSplitChar);
                                    if (fieldInfo.Data[row] != null)
                                        stringBuilder.Append(((DateTime)(fieldInfo.Data[row]) - AppValues.REFERENCE_DATE).TotalMilliseconds);
                                }
                                break;
                            }
                        default:
                            {
                                Utils.LogErrorAndExit("用_GetOneFieldTxtContent函数导出txt文件的date型的DateFormatType非法");
                                break;
                            }
                    }
                    break;
                }
            case DataType.Time:
                {
                    TimeFormatType timeFormatType = TableAnalyzeHelper.GetTimeFormatType(fieldInfo.ExtraParam[AppValues.TABLE_INFO_EXTRA_PARAM_KEY_TIME_TO_LUA_FORMAT].ToString());
                    switch (timeFormatType)
                    {
                        case TimeFormatType.FormatString:
                            {
                                for (int row = 0; row < rowCount; ++row)
                                {
                                    StringBuilder stringBuilder = rowContentList[row];
                                    stringBuilder.Append(AppValues.ExportTxtSplitChar);
                                    if (fieldInfo.Data[row] != null)
                                        stringBuilder.Append(((DateTime)(fieldInfo.Data[row])).ToString(fieldInfo.ExtraParam[AppValues.TABLE_INFO_EXTRA_PARAM_KEY_TIME_TO_LUA_FORMAT].ToString()));
                                }
                                break;
                            }
                        case TimeFormatType.ReferenceTimeSec:
                            {
                                for (int row = 0; row < rowCount; ++row)
                                {
                                    StringBuilder stringBuilder = rowContentList[row];
                                    stringBuilder.Append(AppValues.ExportTxtSplitChar);
                                    if (fieldInfo.Data[row] != null)
                                        stringBuilder.Append(((DateTime)(fieldInfo.Data[row]) - AppValues.REFERENCE_DATE).TotalSeconds);
                                }
                                break;
                            }
                        default:
                            {
                                Utils.LogErrorAndExit("错误：用_GetOneFieldTxtContent函数导出txt文件的time型的TimeFormatType非法");
                                break;
                            }
                    }
                    break;
                }
            case DataType.Array:
            case DataType.Dict:
                {
                    for (int row = 0; row < rowCount; ++row)
                    {
                        StringBuilder stringBuilder = rowContentList[row];
                        stringBuilder.Append(AppValues.ExportTxtSplitChar);
                        if ((bool)fieldInfo.Data[row] == false)
                            stringBuilder.Append("-1");
                    }
                    break;
                }
            default:
                {
                    Utils.LogErrorAndExit(string.Format("_GetOneFieldTxtContent函数中未定义{0}类型数据导出至txt文件的形式", fieldInfo.DataType));
                    break;
                }
        }
    }
}