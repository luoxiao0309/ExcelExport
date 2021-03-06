﻿public partial class AppValues
{
    /// <summary>
    /// 声明将表格导出到MySQL数据库的命令参数
    /// </summary>
    public const string EXPORT_MYSQL_PARAM_STRING = "-exportMySQL";
    /// <summary>
    /// 导出MySQL连接字符串
    /// </summary>
    public static string ExportMySQLConnectString = null;

    /// <summary>
    /// 声明将表格导出到SQLite数据库的命令参数
    /// </summary>
    public const string EXPORT_SQLITE_PARAM_STRING = "-exportSQLite";
    /// <summary>
    /// 导出SQLite连接字符串
    /// </summary>
    public static string ExportSQLiteConnectString = null;
}