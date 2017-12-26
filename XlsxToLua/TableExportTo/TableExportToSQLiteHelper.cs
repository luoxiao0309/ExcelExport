using System.Data.SQLite;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.IO;

public class TableExportToSQLiteHelper
{
    private static SQLiteConnection _conn = null;
    private static string _schemaName = null;
    // 导出数据前获取的数据库中已存在的表名
    private static List<string> _existTableNames = new List<string>();

    // SQLite支持的用于定义Schema名的参数名
    private static string[] _DEFINE_SCHEMA_NAME_PARAM = { "Database", "Initial Catalog" };

    private const string _CREATE_TABLE_SQL = "CREATE TABLE {0}({1})";

    private const string _DROP_TABLE_SQL = "drop table if exists `{0}`";
    private const string _INSERT_DATA_SQL = "INSERT INTO {0} ({1}) VALUES {2};";

    public static bool ConnectToDatabase(out string errorString)
    {
        string connectString = AppValues.ExportSQLiteConnectString;
        try
        {
                _conn = new SQLiteConnection(connectString);
                _conn.Open();

            if (_conn.State == System.Data.ConnectionState.Open)
            {
                // 获取已经存在的表格名
                DataTable schemaInfo = _conn.GetSchema("TABLES");
				if (schemaInfo != null && schemaInfo.Rows.Count > 0)
                {
			                foreach (DataRow info in schemaInfo.Rows)
                    _existTableNames.Add(info["TABLE_NAME"].ToString());
					
				}
                errorString = null;
                return true;
            }
            else
            {
                errorString = "数据库连接失败,请检查连接字符串是否正确";
                return true;
            }
        }
        catch (SQLiteException exception)
        {
            errorString = exception.Message;
            return false;
        }
    }

    public static bool ExportTableToDatabase(TableInfo tableInfo, out string errorString)
    {
        Utils.Log(string.Format("导入SQLite数据库 \"{0}\"：", tableInfo.TableName), ConsoleColor.Green);
        if (tableInfo.TableConfig != null && tableInfo.TableConfig.ContainsKey(AppValues.CONFIG_NAME_EXPORT_DATABASE_TABLE_NAME))
        {
            List<string> inputParams = tableInfo.TableConfig[AppValues.CONFIG_NAME_EXPORT_DATABASE_TABLE_NAME];
            if (inputParams == null || inputParams.Count < 1 || string.IsNullOrEmpty(inputParams[0]))
            {
                Utils.LogWarning("警告：未在表格配置中声明该表导出到_QLITE数据库中的表名，此表将不被导出到数据库，请确认是否真要如此");
                errorString = null;
                return true;
            }
            string tableName = inputParams[0];
            // 检查数据库中是否已经存在同名表格，若存在删除旧表
            if (_existTableNames.Contains(tableName))
            {
                _DropTable(tableName, out errorString);
                if (!string.IsNullOrEmpty(errorString))
                {
                    errorString = string.Format("SQLITE数据库中存在同名表格，但删除旧表失败，{0}", errorString);
                    return false;
                }
            }
            // 警告未设置导出到数据库信息的字段，这些字段将不被导出
            const string WARNING_INFO_FORMAT = "第{0}列（字段名为{1}）";
            List<string> warningInfo = new List<string>();
            foreach (FieldInfo fieldInfo in tableInfo.GetAllFieldInfo())
            {
                if (fieldInfo.DataType != DataType.Array && fieldInfo.DataType != DataType.Dict && fieldInfo.DatabaseFieldName == null)
                    warningInfo.Add(string.Format(WARNING_INFO_FORMAT, Utils.GetExcelColumnName(fieldInfo.ColumnSeq + 1), fieldInfo.FieldName));
            }
            if (warningInfo.Count > 0)
            {
                Utils.LogWarning("警告：以下字段未设置导出SQLite数据库的信息，将被忽略：");
                Utils.LogWarning(Utils.CombineString(warningInfo, " ,"));
            }
            // 按Excel表格中字段定义新建数据库表格
			//string comment = tableInfo.TableConfig != null && tableInfo.TableConfig.ContainsKey(AppValues.CONFIG_NAME_EXPORT_DATABASE_TABLE_COMMENT_SQLITE) && tableInfo.TableConfig[AppValues.CONFIG_NAME_EXPORT_DATABASE_TABLE_COMMENT_SQLITE].Count > 0 ? tableInfo.TableConfig[AppValues.CONFIG_NAME_EXPORT_DATABASE_TABLE_COMMENT_SQLITE ][0] : string.Empty;
			string comment =string.Empty;
			if(tableInfo.TableConfig != null)
			{
				if(tableInfo.TableConfig.ContainsKey(AppValues.CONFIG_NAME_EXPORT_DATABASE_TABLE_COMMENT))
				{
					if(tableInfo.TableConfig[AppValues.CONFIG_NAME_EXPORT_DATABASE_TABLE_COMMENT].Count > 0)
					{
                        comment = tableInfo.TableConfig[AppValues.CONFIG_NAME_EXPORT_DATABASE_TABLE_COMMENT][0];
					}
				}
			}
            
            _CreateTable(tableName, tableInfo, comment, out errorString);
            if (string.IsNullOrEmpty(errorString))
            {
                // 将Excel表格中的数据添加至数据库
                _InsertData(tableName, tableInfo, out errorString);
                if (string.IsNullOrEmpty(errorString))
                {
                    Utils.Log("成功");

                    errorString = null;
                    return true;
                }
                else
                {
                    errorString = string.Format("插入数据失败，{0}", errorString);
                    return false;
                }
            }
            else
            {
                errorString = string.Format("创建表格失败，{0}", errorString);
                return false;
            }
        }
        else
        {
            Utils.LogWarning("警告：未在表格配置中声明该表导出到数据库中的表名，此表将不被导出到数据库，请确认是否真要如此");
            errorString = null;
            return true;
        }
    }

    private static bool _InsertData(string tableName, TableInfo tableInfo, out string errorString)
    {
        List<FieldInfo> allDatabaseFieldInfo = GetAllDatabaseFieldInfo(tableInfo);

        // 生成所有字段名对应的定义字符串
        List<string> fileNames = new List<string>();
        foreach (FieldInfo fieldInfo in allDatabaseFieldInfo)
            fileNames.Add(string.Format("{0}", _CombineDatabaseTableFullName(fieldInfo.DatabaseFieldName)));

        string fieldNameDefineString = Utils.CombineString(fileNames, ", ");

        // 用户是否配置该表中string型字段中的空单元格导出至SQLite中为NULL，默认为空字符串
        bool isWriteNullForEmptyString = tableInfo.TableConfig != null && tableInfo.TableConfig.ContainsKey(AppValues.CONFIG_NAME_EXPORT_DATABASE_WRITE_NULL_FOR_EMPTY_STRING) && tableInfo.TableConfig[AppValues.CONFIG_NAME_EXPORT_DATABASE_WRITE_NULL_FOR_EMPTY_STRING].Count > 0 && "true".Equals(tableInfo.TableConfig[AppValues.CONFIG_NAME_EXPORT_DATABASE_WRITE_NULL_FOR_EMPTY_STRING][0], StringComparison.CurrentCultureIgnoreCase);

        // 逐行生成插入数据的SQL语句中的value定义部分
        StringBuilder valueDefineStringBuilder = new StringBuilder();
        int count = tableInfo.GetKeyColumnFieldInfo().Data.Count;
        if (count > 0)
        {
            for (int i = 0; i < count; ++i)
            {
                List<string> values = new List<string>();
                foreach (FieldInfo fieldInfo in allDatabaseFieldInfo)
                {
                    if (fieldInfo.Data[i] == null)
                        values.Add("NULL");
                    else if (fieldInfo.DataType == DataType.Date)
                    {
                        string toDatabaseFormatDefine = fieldInfo.ExtraParam[AppValues.TABLE_INFO_EXTRA_PARAM_KEY_DATE_TO_DATABASE_FORMAT].ToString();
                        DateFormatType toDatabaseFormatType = TableAnalyzeHelper.GetDateFormatType(toDatabaseFormatDefine);
                        if (toDatabaseFormatType == DateFormatType.FormatString)
                        {
                            // 注意SQLite中的时间型，datetime和time型后面可用括号进行具体设置，date型没有
                            // SQLite中的date型插入数据时不允许含有时分秒，否则会报错，故这里强制采用SQLite默认的yyyy-MM-dd格式插入
                            if (fieldInfo.DatabaseFieldType.Equals("date", StringComparison.CurrentCultureIgnoreCase))
                                values.Add(string.Format("'{0}'", ((DateTime)(fieldInfo.Data[i])).ToString(AppValues.APP_DEFAULT_ONLY_DATE_FORMAT)));
                            else if (fieldInfo.DatabaseFieldType.StartsWith("datetime", StringComparison.CurrentCultureIgnoreCase))
                                values.Add(string.Format("'{0}'", ((DateTime)(fieldInfo.Data[i])).ToString(AppValues.APP_DEFAULT_DATE_FORMAT)));
                            // date型导出到SQLite中的其他数据类型字段如varchar，采用声明的指定格式
                            else
                                values.Add(string.Format("'{0}'", ((DateTime)(fieldInfo.Data[i])).ToString(fieldInfo.ExtraParam[AppValues.TABLE_INFO_EXTRA_PARAM_KEY_DATE_TO_DATABASE_FORMAT].ToString())));
                        }
                        else if (toDatabaseFormatType == DateFormatType.ReferenceDateSec)
                            values.Add(string.Format("'{0}'", ((DateTime)fieldInfo.Data[i] - AppValues.REFERENCE_DATE).TotalSeconds));
                        else if (toDatabaseFormatType == DateFormatType.ReferenceDateMsec)
                            values.Add(string.Format("'{0}'", ((DateTime)fieldInfo.Data[i] - AppValues.REFERENCE_DATE).TotalMilliseconds));
                        else
                        {
                            errorString = "date型导出至SQLite的格式定义非法";
                            Utils.LogErrorAndExit(errorString);
                            return false;
                        }
                    }
                    else if (fieldInfo.DataType == DataType.Time)
                    {
                        string toDatabaseFormatDefine = fieldInfo.ExtraParam[AppValues.TABLE_INFO_EXTRA_PARAM_KEY_TIME_TO_DATABASE_FORMAT].ToString();
                        TimeFormatType toDatabaseFormatType = TableAnalyzeHelper.GetTimeFormatType(toDatabaseFormatDefine);
                        if (toDatabaseFormatType == TimeFormatType.FormatString)
                        {
                            if (fieldInfo.DatabaseFieldType.StartsWith("time", StringComparison.CurrentCultureIgnoreCase))
                                values.Add(string.Format("'{0}'", ((DateTime)(fieldInfo.Data[i])).ToString(AppValues.APP_DEFAULT_TIME_FORMAT)));
                            else
                                values.Add(string.Format("'{0}'", ((DateTime)(fieldInfo.Data[i])).ToString(fieldInfo.ExtraParam[AppValues.TABLE_INFO_EXTRA_PARAM_KEY_TIME_TO_DATABASE_FORMAT].ToString())));
                        }
                        else if (toDatabaseFormatType == TimeFormatType.ReferenceTimeSec)
                            values.Add(string.Format("'{0}'", ((DateTime)fieldInfo.Data[i] - AppValues.REFERENCE_DATE).TotalSeconds));
                        else
                        {
                            errorString = "time型导出至SQLite的格式定义非法";
                            Utils.LogErrorAndExit(errorString);
                            return false;
                        }
                    }
                    else if (fieldInfo.DataType == DataType.Bool)
                    {
                        bool inputData = (bool)fieldInfo.Data[i];
                        // 如果数据库用bit数据类型表示bool型，比如要写入true，SQL语句中的1不能加单引号
                        if (fieldInfo.DatabaseFieldType.Equals("bit", StringComparison.CurrentCultureIgnoreCase) || fieldInfo.DatabaseFieldType.StartsWith("bit(", StringComparison.CurrentCultureIgnoreCase))
                        {
                            if (inputData == true)
                                values.Add("1");
                            else
                                values.Add("0");
                        }
                        else
                        {
                            // 如果数据库用tinyint(1)数据类型表示bool型，比如要写入true，SQL语句中可以写为'1'或者不带单引号的true
                            if (inputData == true)
                                values.Add("'1'");
                            else
                                values.Add("'0'");
                        }
                    }
                    else if (fieldInfo.DataType == DataType.Json)
                    {
                        // json型直接向数据库写入原始json字符串，但需要对\进行转义
                        values.Add(string.Format("'{0}'", fieldInfo.JsonString[i]).Replace("\\", "\\\\"));
                    }
                    else if (fieldInfo.DataType == DataType.MapString)
                    {
                        // mapString型也直接写入原始mapString数据字符串，并对\进行转义
                        values.Add(string.Format("'{0}'", fieldInfo.JsonString[i]).Replace("\\", "\\\\"));
                    }
                    // 这里需要自行处理数据库中某些数据类型（如datetime）中不允许插入空字符串的情况，以及用户设置的string型中空单元格导出至数据库的形式
                    else if (string.IsNullOrEmpty(fieldInfo.Data[i].ToString()))
                    {
                        if (fieldInfo.DatabaseFieldType.StartsWith("datetime", StringComparison.CurrentCultureIgnoreCase))
                            values.Add("NULL");
                        else if (fieldInfo.DataType == DataType.String && isWriteNullForEmptyString == true)
                            values.Add("NULL");
                        else
                            values.Add(string.Format("{0}", fieldInfo.Data[i].ToString()));
                    }
                    else
                    {
                        // 注意对\进行转义
                        values.Add(string.Format("'{0}'", fieldInfo.Data[i].ToString()).Replace("\\", "\\\\"));
                    }
                }

                valueDefineStringBuilder.AppendFormat("({0}),", Utils.CombineString(values, ","));
            }
            // 去掉末尾多余的逗号
            string valueDefineString = valueDefineStringBuilder.ToString();
            valueDefineString = valueDefineString.Substring(0, valueDefineString.Length - 1);

            string insertSqlString = string.Format(_INSERT_DATA_SQL,tableName, fieldNameDefineString, valueDefineString);

            // 执行插入操作
            try
            {
                SQLiteCommand cmd = new SQLiteCommand(insertSqlString, _conn);
                int insertCount = cmd.ExecuteNonQuery();
                if (insertCount < count)
                {
                    errorString = string.Format("需要插入{0}条数据但仅插入了{1}条");
                    return false;
                }
                else
                {
                    errorString = null;
                    return true;
                }
            }
            catch (SQLiteException exception)
            {
                errorString = exception.Message;
                return false;
            }
        }
        else
        {
            errorString = null;
            return true;
        }
    }

    /// <summary>
    /// 获取某张表格中对应要导出到数据库的字段集合
    /// </summary>
    public static List<FieldInfo> GetAllDatabaseFieldInfo(TableInfo tableInfo)
    {
        List<FieldInfo> allFieldInfo = new List<FieldInfo>();
        foreach (FieldInfo fieldInfo in tableInfo.GetAllFieldInfo())
            _GetOneDatabaseFieldInfo(fieldInfo, allFieldInfo);

        return allFieldInfo;
    }

    private static void _GetOneDatabaseFieldInfo(FieldInfo fieldInfo, List<FieldInfo> allFieldInfo)
    {
        if (fieldInfo.DataType == DataType.Array || fieldInfo.DataType == DataType.Dict)
        {
            foreach (FieldInfo childFieldInfo in fieldInfo.ChildField)
                _GetOneDatabaseFieldInfo(childFieldInfo, allFieldInfo);
        }
        // 忽略未配置导出到数据库信息的字段
        else if (fieldInfo.DatabaseFieldName != null)
            allFieldInfo.Add(fieldInfo);
    }

    /// <summary>
    /// 将表名或字段名中的，sqlite保留关键字用方括号包裹起来
    /// </summary>
    private static string _CombineDatabaseTableFullName(string tableName)
    {
        if (tableName.ToLower() == "set" || tableName.ToLower() == "drop")
            return string.Format("[{0}]", tableName);
        else
            return tableName;
    }

    private static bool _CreateTable(string tableName, TableInfo tableInfo, string comment, out string errorString)
    {
        // 生成在创建数据表时所有字段的声明
        StringBuilder fieldDefineStringBuilder = new StringBuilder();
        foreach (FieldInfo fieldInfo in GetAllDatabaseFieldInfo(tableInfo))
        {
            // 在这里并不对每种本工具的数据类型是否能导出为指定的SQLite数据类型进行检查（比如本工具中的string型应该导出为SQLite中的文本类型如varchar，而不应该导出为数值类型）
            if (fieldInfo.DataType == DataType.Date)
            {
                string toDatabaseFormatDefine = fieldInfo.ExtraParam[AppValues.TABLE_INFO_EXTRA_PARAM_KEY_DATE_TO_DATABASE_FORMAT].ToString();
                DateFormatType toDatabaseFormatType = TableAnalyzeHelper.GetDateFormatType(toDatabaseFormatDefine);
                if (fieldInfo.DatabaseFieldType.StartsWith("time", StringComparison.CurrentCultureIgnoreCase))
                {
                    errorString = string.Format("date型字段\"{0}\"（列号：{1}）声明导出到SQLite中的数据类型错误，不允许为time型，如果仅需要时分秒部分，请在Excel中将该字段在本工具中的数据类型改为time型", fieldInfo.FieldName, Utils.GetExcelColumnName(fieldInfo.ColumnSeq + 1));
                    return false;
                }
                if (toDatabaseFormatType == DateFormatType.ReferenceDateSec || toDatabaseFormatType == DateFormatType.ReferenceDateMsec)
                {
                    if (fieldInfo.DatabaseFieldType.StartsWith("datetime", StringComparison.CurrentCultureIgnoreCase) || fieldInfo.DatabaseFieldType.Equals("date", StringComparison.CurrentCultureIgnoreCase))
                    {
                        errorString = string.Format("date型字段\"{0}\"（列号：{1}）声明导出到SQLite中的形式为距1970年的时间（{2}），但所填写的导出到SQLite中的格式为时间型的{3}，请声明为SQLite中的数值型", fieldInfo.FieldName, Utils.GetExcelColumnName(fieldInfo.ColumnSeq + 1), toDatabaseFormatDefine, fieldInfo.DatabaseFieldType);
                        return false;
                    }
                }
            }
            else if (fieldInfo.DataType == DataType.Time)
            {
                string toDatabaseFormatDefine = fieldInfo.ExtraParam[AppValues.TABLE_INFO_EXTRA_PARAM_KEY_TIME_TO_DATABASE_FORMAT].ToString();
                TimeFormatType toDatabaseFormatType = TableAnalyzeHelper.GetTimeFormatType(toDatabaseFormatDefine);
                if (fieldInfo.DatabaseFieldType.StartsWith("datetime", StringComparison.CurrentCultureIgnoreCase) || fieldInfo.DatabaseFieldType.Equals("date", StringComparison.CurrentCultureIgnoreCase))
                {
                    errorString = string.Format("time型字段\"{0}\"（列号：{1}）声明导出到SQLite中的数据类型错误，不允许为datetime或date型，如果需要年月日部分，请在Excel中将该字段在本工具中的数据类型改为date型", fieldInfo.FieldName, Utils.GetExcelColumnName(fieldInfo.ColumnSeq + 1));
                    return false;
                }
                if (toDatabaseFormatType == TimeFormatType.ReferenceTimeSec && fieldInfo.DatabaseFieldType.StartsWith("time", StringComparison.CurrentCultureIgnoreCase))
                {
                    errorString = string.Format("time型字段\"{0}\"（列号：{1}）声明导出到SQLite中的形式为距0点的秒数（#sec），但所填写的导出到SQLite中的格式为时间型的time，请声明为SQLite中的数值型", fieldInfo.FieldName, Utils.GetExcelColumnName(fieldInfo.ColumnSeq + 1));
                    return false;
                }
            }

            fieldDefineStringBuilder.AppendFormat("{0} {1},", _CombineDatabaseTableFullName(fieldInfo.DatabaseFieldName), fieldInfo.DatabaseFieldType);
        }
        fieldDefineStringBuilder.Remove(fieldDefineStringBuilder.Length - 1, 1);
        string createTableExtraParam = AppValues.ConfigData.ContainsKey(AppValues.APP_CONFIG_KEY_CREATE_DATABASE_TABLE_EXTRA_PARAM) ? AppValues.ConfigData[AppValues.APP_CONFIG_KEY_CREATE_DATABASE_TABLE_EXTRA_PARAM] : string.Empty;
        string createTableSql = string.Format(_CREATE_TABLE_SQL, tableName, fieldDefineStringBuilder.ToString());

        try
        {
            SQLiteCommand cmd = new SQLiteCommand(createTableSql, _conn);
            cmd.ExecuteNonQuery();
            errorString = null;
            return true;
        }
        catch (SQLiteException exception)
        {
            errorString = exception.Message;
            return false;
        }
    }

    private static bool _DropTable(string tableName, out string errorString)
    {
        try
        {
            SQLiteCommand cmd = new SQLiteCommand(string.Format(_DROP_TABLE_SQL, _CombineDatabaseTableFullName(tableName)), _conn);
            cmd.ExecuteNonQuery();
            errorString = null;
            return true;
        }
        catch (SQLiteException exception)
        {
            errorString = exception.Message;
            return false;
        }
    }
}
