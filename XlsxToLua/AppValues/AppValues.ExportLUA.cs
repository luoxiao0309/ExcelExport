public partial class AppValues
{
    /// <summary>
    /// 声明在生成的lua文件开头以注释形式展示列信息的命令参数
    /// </summary>
    public const string NEED_COLUMN_INFO_PARAM_STRING = "-columnInfo";

    // 声明对某张表格设置特殊导出Lua规则的配置参数名
    public const string CONFIG_NAME_EXPORT = "tableExportConfig";

    // 声明某张表格导出为lua table时，是否将主键列的值作为table中的元素
    public const string CONFIG_NAME_ADD_KEY_TO_LUA_TABLE = "addKeyToLuaTable";

    // 声明对某张表格不进行默认导出Lua的参数配置
    public const string CONFIG_PARAM_NOT_EXPORT_ORIGINAL_TABLE = "-notExportOriginalTable";

    /// <summary>
    /// 用户输入的要生成的lua文件存放路径
    /// </summary>
    public static string ExportLuaFilePath = null;

    /// <summary>
    /// 用户输入的是否需要在生成lua文件的最上方用注释形式显示列信息（默认为不需要）
    /// </summary>
    public static bool IsNeedColumnInfo = false;
}