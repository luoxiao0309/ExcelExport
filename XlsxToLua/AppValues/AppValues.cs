using System;
using System.Collections.Generic;
using System.Text;

/// <summary>
/// 定义本工具中的常量以及供各个类之间读取的常量
/// </summary>
public partial class AppValues
{
    /// <summary>
    /// 本工具所在目录，不能用System.Environment.CurrentDirectory因为当本工具被其他程序调用时取得的CurrentDirectory将是调用者的路径
    /// </summary>
    public static string PROGRAM_FOLDER_PATH = System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase;

    /// <summary>
    /// 用户输入的要导出的Excel文件所在目录
    /// </summary>
    public static string ExcelFolderPath = null;


    /// <summary>
    /// Excel文件中存放数据的工作簿Sheet名。除预设功能的特殊Sheet表之外，其余Sheet表可自定义内容，不会被本工具导出
    /// </summary>
    public const string EXCEL_DATA_SHEET_NAME = "data$";

    /// <summary>
    /// Excel文件中存放该表格配置的工作簿Sheet名
    /// </summary>
    public const string EXCEL_CONFIG_SHEET_NAME = "config$";

    /// <summary>
    /// Excel临时文件的文件名前缀
    /// </summary>
    public const string EXCEL_TEMP_FILE_FILE_NAME_START_STRING = "~$";

    /// <summary>
    /// 声明将Excel所在文件夹下属子文件夹中的Excel文件也进行导出的命令参数
    /// </summary>
    public const string EXPORT_INCLUDE_SUBFOLDER_PARAM_STRING = "-exportIncludeSubfolder";

    /// <summary>
    /// 声明将生成的文件按原Excel文件所在的目录结构进行存储的命令参数
    /// </summary>
    public const string EXPORT_KEEP_DIRECTORY_STRUCTURE_PARAM_STRING = "-exportKeepDirectoryStructure";

    /// <summary>
    /// 声明不进行表格检查的命令参数
    /// </summary>
    public const string UNCHECKED_PARAM_STRING = "-unchecked";

    /// <summary>
    /// 声明不指定项目Client目录的命令参数
    /// </summary>
    public const string NO_CLIENT_PATH_STRING = "-noClient";

    /// <summary>
    /// 声明不含有lang文件的命令参数
    /// </summary>
    public const string NO_LANG_PARAM_STRING = "-noLang";

    /// <summary>
    /// 声明当lang型数据key在lang文件中找不到对应值时，在lua文件输出字段值为空字符串的命令参数
    /// </summary>
    public const string LANG_NOT_MATCHING_PRINT_PARAM_STRING = "-printEmptyStringWhenLangNotMatching";

    /// <summary>
    /// 声明只将部分指定Excel表进行导出的命令参数
    /// </summary>
    public const string PART_EXPORT_PARAM_STRING = "-part";

    /// <summary>
    /// 声明忽略对指定Excel表进行导出的命令参数
    /// </summary>
    public const string EXCEPT_EXPORT_PARAM_STRING = "-except";

    /// <summary>
    /// 声明允许int、float型字段中存在空值的命令参数
    /// </summary>
    public const string ALLOWED_NULL_NUMBER_PARAM_STRING = "-allowedNullNumber";

    /// <summary>
    /// 声明额外导出为其他格式文件时，涉及所有文件
    /// </summary>
    public const string EXPORT_ALL_TO_EXTRA_FILE_PARAM_STRING = "$all";

    /// <summary>
    /// 配置文件（配置自定义的检查规则）的文件名
    /// </summary>
    public const string CONFIG_FILE_NAME = "config.txt";
    /// <summary>
    /// 检查png文件是否存在重复
    /// </summary>
    public const string CHECK_PNG = "-checkPng";
    /// <summary>
    /// 检查png文件是否存在重复,png文件路径
    /// </summary>
    public const string CHECK_PNG_PATH = "chekPath";
    // 每张数据表前五行分别声明字段描述、字段变量名、字段数据类型、字段检查规则、导出到数据库中的字段名及类型（行编号从0开始）
    public const int DATA_FIELD_DESC_INDEX = 0;
    public const int DATA_FIELD_NAME_INDEX = 1;
    public const int DATA_FIELD_DATA_TYPE_INDEX = 2;
    public const int DATA_FIELD_CHECK_RULE_INDEX = 3;
    public const int DATA_FIELD_EXPORT_DATABASE_FIELD_INFO = 4;
    public const int DATA_FIELD_DATA_START_INDEX = 5;

    // 每张配置表中的一列为一个配置参数的声明，其中第一行声明参数名，其余行声明具体参数（行编号从0开始）
    public const int CONFIG_FIELD_DEFINE_INDEX = 0;
    public const int CONFIG_FIELD_PARAM_START_INDEX = 1;

    // 声明整表检查的配置参数名
    public const string CONFIG_NAME_CHECK_TABLE = "tableCheckRule";

    // 声明某张表格导出到数据库中的表名
    public const string CONFIG_NAME_EXPORT_DATABASE_TABLE_NAME = "exportDatabaseTableName";
    // 声明某张表格导出到数据库中的说明信息
    public const string CONFIG_NAME_EXPORT_DATABASE_TABLE_COMMENT = "exportDatabaseTableComment";
    // 声明某张表格导出到数据库中时string型字段中的空白单元格导出为数据库中的NULL
    public const string CONFIG_NAME_EXPORT_DATABASE_WRITE_NULL_FOR_EMPTY_STRING = "exportDatabaseWriteNullForEmptyString";



    // 以下为TableInfo的ExtraParam所支持的key声明
    // date型的输入格式
    public const string TABLE_INFO_EXTRA_PARAM_KEY_DATE_INPUT_FORMAT = "dateInputFormat";
    // date型导出至lua文件的格式
    public const string TABLE_INFO_EXTRA_PARAM_KEY_DATE_TO_LUA_FORMAT = "dateToLuaFormat";
    // date型导出至MySQL数据库的格式
    public const string TABLE_INFO_EXTRA_PARAM_KEY_DATE_TO_DATABASE_FORMAT = "dateToDatabaseFormat";
    // time型的输入格式
    public const string TABLE_INFO_EXTRA_PARAM_KEY_TIME_INPUT_FORMAT = "timeInputFormat";
    // time型导出至lua文件的格式
    public const string TABLE_INFO_EXTRA_PARAM_KEY_TIME_TO_LUA_FORMAT = "timeToLuaFormat";
    // time型导出至MySQL数据库的格式
    public const string TABLE_INFO_EXTRA_PARAM_KEY_TIME_TO_DATABASE_FORMAT = "timeToDatabaseFormat";

    // 将MySQL中datetime、date型的默认格式作为本工具对date、time两种时间型进行检查并发现错误后的输出格式
    public const string APP_DEFAULT_DATE_FORMAT = "yyyy-MM-dd HH:mm:ss";
    public const string APP_DEFAULT_TIME_FORMAT = "HH:mm:ss";
    // 导出数据到MySQL中的date型字段的默认格式
    public const string APP_DEFAULT_ONLY_DATE_FORMAT = "yyyy-MM-dd";

    

    /// <summary>
    /// 以1970年1月1日作为计算距今秒数的参考时间，并且作为存储time型的DateTime变量的日期部分
    /// </summary>
    public static DateTime REFERENCE_DATE = new DateTime(1970, 1, 1);

    /// <summary>
    /// 未对某字段命名时，默认给予的字段名前缀
    /// </summary>
    public static string AUTO_FIELD_NAME_PREFIX = "未命名字段";

    /// <summary>
    /// 用户选择的是否要导出Excel文件夹下属子文件夹中的Excel文件（默认为不包含子文件夹）
    /// </summary>
    public static bool IsExportIncludeSubfolder = false;

    /// <summary>
    /// 用户选择的当导出Excel文件夹下属子文件夹中的Excel文件时，是否将生成的文件按原Excel文件所在的目录结构进行存储（默认为直接存储在同级目录下）
    /// </summary>
    public static bool IsExportKeepDirectoryStructure = false;

    /// <summary>
    /// 用户输入的国际化文件所在路径
    /// </summary>
    public static string LangFilePath = null;

    /// <summary>
    /// 用户输入的Client目录所在路径
    /// </summary>
    public static string ClientPath = null;

    /// <summary>
    /// 用户输入的是否需要检查表格（默认为检查）
    /// </summary>
    public static bool IsNeedCheck = true;

    /// <summary>
    /// 当lang型数据key在lang文件中找不到对应值时，是否在lua文件输出字段值为空字符（默认为输出nil）
    /// </summary>
    public static bool IsPrintEmptyStringWhenLangNotMatching = false;

    /// <summary>
    /// 用户输入的是否导出表格数据到MySQL数据库
    /// </summary>
    public static bool IsExportMySQL = false;

    /// <summary>
    /// 用户输入的是否允许int、float型字段中存在空值
    /// </summary>
    public static bool IsAllowedNullNumber = false;

    /// <summary>
    /// 未声明date型的输入格式时所采用的默认格式
    /// </summary>
    public static string DefaultDateInputFormat = null;

    /// <summary>
    /// 未声明date型导出至lua文件的格式时所采用的默认格式
    /// </summary>
    public static string DefaultDateToLuaFormat = null;

    /// <summary>
    /// 未声明date型导出至MySQL数据库的格式时所采用的默认格式
    /// </summary>
    public static string DefaultDateToDatabaseFormat = null;

    /// <summary>
    /// 未声明time型的输入格式时所采用的默认格式
    /// </summary>
    public static string DefaultTimeInputFormat = null;

    /// <summary>
    /// 未声明time型导出至lua文件的格式时所采用的默认格式
    /// </summary>
    public static string DefaultTimeToLuaFormat = null;

    /// <summary>
    /// 未声明time型导出至MySQL数据库的格式时所采用的默认格式
    /// </summary>
    public static string DefaultTimeToDatabaseFormat = null;

    /// <summary>
    /// lang文件转为键值对形式（key：lang文件中的key名， value：对应的在指定语言下的翻译）
    /// </summary>
    public static Dictionary<string, string> LangData = new Dictionary<string, string>();

    /// <summary>
    /// config文件转为键值对形式（key：配置文件中的key名， value：对应的配置规则字符串）
    /// </summary>
    public static Dictionary<string, string> ConfigData = new Dictionary<string, string>();

    /// <summary>
    /// 存储每张Excel表格解析成的本工具所需的数据结构（key：表名）
    /// </summary>
    public static Dictionary<string, TableInfo> TableInfo = new Dictionary<string, TableInfo>();

    /// <summary>
    /// 存储本次要导出的Excel文件名对应的文件所在路径（key：表名， value：文件所在路径）
    /// </summary>
    public static Dictionary<string, List<string>> ExportTableNameAndPath = new Dictionary<string, List<string>>();

    /// <summary>
    /// 存储本次要导出的Excel文件名
    /// </summary>
    public static List<string> ExportTableNames = new List<string>();

    /// <summary>
    /// 存储本次忽略导出的Excel文件名
    /// </summary>
    public static List<string> ExceptExportTableNames = new List<string>();

    /// <summary>
    /// 存储运行时打印的所有信息，在程序运行完毕后输出为txt文件，从而解决如果输出内容过多控制台无法显示全部信息的问题
    /// </summary>
    public static StringBuilder LogContent = new StringBuilder();

    /// <summary>
    /// 存储路径集合
    /// </summary>
    public static List<string> FileFolde = new List<string>();
    /// <summary>
    /// 存储指定目录及子目录下所有文件名和对应的文件路径；
    /// 文件名组成：指定路径+文件名
    /// </summary>
    public static Dictionary<string, List<string>> FlieNames = new Dictionary<string, List<string>>();

    /// <summary>
    /// Dictionary<路径+(.扩展名)TopDirectoryOnly/AllDirectories, Dictionary<文件名, List<该文件对应所有完整文件路径>>>
    /// </summary>
    public static Dictionary<string, Dictionary<string, List<string>>> AllFileInfos = new Dictionary<string, Dictionary<string, List<string>>>();
    /// <summary>
    /// 存储_drop掉落表
    /// </summary>
    public static Dictionary<string, Table_drop> TableDropInfo = new Dictionary<string, Table_drop>();
    /// <summary>
    /// 存储_drop_group掉落表
    /// </summary>
    public static Dictionary<string, Table_drop_reward> TableDropRewardInfo = new Dictionary<string, Table_drop_reward>();
}
