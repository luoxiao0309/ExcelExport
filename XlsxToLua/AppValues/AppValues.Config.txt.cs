public partial class AppValues
{
    // 以下为config配置文件中配置项的key名
  
    /// <summary>
    /// MySQL连接字符串
    /// </summary>
    public const string APP_CONFIG_KEY_MYSQL_CONNECT_STRING = "connectMySQLString";

    /// <summary>
    /// 创建MySQL数据库表格时额外指定的参数字符串
    /// </summary>
    public const string APP_CONFIG_KEY_CREATE_DATABASE_TABLE_EXTRA_PARAM = "createDatabaseTableExtraParam";

    /// <summary>
    /// 未声明date型的输入格式时所采用的默认格式
    /// </summary>
    public const string APP_CONFIG_KEY_DEFAULT_DATE_INPUT_FORMAT = "defaultDateInputFormat";

    /// <summary>
    /// 未声明date型导出至lua文件的格式时所采用的默认格式
    /// </summary>
    public const string APP_CONFIG_KEY_DEFAULT_DATE_TO_LUA_FORMAT = "defaultDateToLuaFormat";

    /// <summary>
    /// 未声明date型导出至MySQL数据库的格式时所采用的默认格式
    /// </summary>
    public const string APP_CONFIG_KEY_DEFAULT_DATE_TO_DATABASE_FORMAT = "defaultDateToDatabaseFormat";

    /// <summary>
    /// 未声明time型的输入格式时所采用的默认格式
    /// </summary>
    public const string APP_CONFIG_KEY_DEFAULT_TIME_INPUT_FORMAT = "defaultTimeInputFormat";

    /// <summary>
    /// 
    /// </summary>
    public const string APP_CONFIG_KEY_DEFAULT_TIME_TO_LUA_FORMAT = "defaultTimeToLuaFormat";

    /// <summary>
    /// 未声明time型导出至MySQL数据库的格式时所采用的默认格式
    /// </summary>
    public const string APP_CONFIG_KEY_DEFAULT_TIME_TO_DATABASE_FORMAT = "defaultTimeToDatabaseFormat";
}