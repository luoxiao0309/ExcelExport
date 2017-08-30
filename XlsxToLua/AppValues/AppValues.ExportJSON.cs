using System.Collections.Generic;

public partial class AppValues
{
    /// <summary>
    /// 对导出csv对应C#或Java类文件进行自动命名的参数
    /// </summary>
    public const string AUTO_NAME_CSV_CLASS_PARAM_STRING = "-autoNameCsvClassParam";

    /// <summary>
    /// 对导出csv对应C#或Java类文件自动命名参数下属的具体参数，用于配置在类名前增加的前缀
    /// </summary>
    public const string AUTO_NAME_CSV_CLASS_PARAM_CLASS_NAME_PREFIX_PARAM_STRING = "classNamePrefix";

    /// <summary>
    /// 对导出csv对应C#或Java类文件自动命名参数下属的具体参数，用于配置在类名前增加的后缀
    /// </summary>
    public const string AUTO_NAME_CSV_CLASS_PARAM_CLASS_NAME_POSTFIX_PARAM_STRING = "classNamePostfix";

    // 声明某张表格导出csv对应C#或Java文件的类名
    public const string CONFIG_NAME_EXPORT_CSV_CLASS_NAME = "exportCsvClassName";

    /// <summary>
    /// 导出csv对应C#或Java类的类名前缀
    /// </summary>
    public static string ExportCsvClassClassNamePrefix = null;

    /// <summary>
    /// 导出csv对应C#或Java类的类名后缀
    /// </summary>
    public static string ExportCsvClassClassNamePostfix = null;

    /// <summary>
    /// 声明将指定的Excel文件额外导出为json文件
    /// </summary>
    public const string EXPORT_JSON_PARAM_STRING = "-exportJson";

    /// <summary>
    /// 声明导出json文件时的参数
    /// </summary>
    public const string EXPORT_JSON_PARAM_PARAM_STRING = "-exportJsonParam";

    /// <summary>
    /// 导出json文件参数下属的具体参数，用于配置导出路径
    /// </summary>
    public const string EXPORT_JSON_PARAM_EXPORT_PATH_PARAM_STRING = "exportPath";

    /// <summary>
    /// 导出json文件参数下属的具体参数，用于配置导出文件的扩展名
    /// </summary>
    public const string EXPORT_JSON_PARAM_EXTENSION_PARAM_STRING = "extension";

    /// <summary>
    /// 导出json文件参数下属的具体参数，用于配置是否将生成的json字符串整理为带缩进格式的形式
    /// </summary>
    public const string EXPORT_JSON_PARAM_IS_FORMAT_PARAM_STRING = "isFormat";

    /// <summary>
    /// 导出json文件参数下属的具体参数，用于配置是否生成为各行数据对应的json object包含在一个json array的形式
    /// </summary>
    public const string EXPORT_JSON_PARAM_IS_EXPORT_JSON_ARRAY_FORMAT_PARAM_STRING = "isExportJsonArrayFormat";

    /// <summary>
    /// 导出json文件参数下属的具体参数，用于配置若生成包含在一个json object的形式，是否使每行字段信息对应的json object中包含主键列对应的键值对
    /// </summary>
    public const string EXPORT_JSON_PARAM_IS_MAP_INCLUDE_KEY_COLUMN_VALUE_PARAM_STRING = "isMapIncludeKeyColumnValue";

    /// <summary>
    /// 存储本次要额外导出为json文件的Excel文件名
    /// </summary>
    public static List<string> ExportJsonTableNames = new List<string>();

    /// <summary>
    /// 导出json文件的存储路径
    /// </summary>
    public static string ExportJsonPath = null;

    /// <summary>
    /// 导出json文件的扩展名（不含点号），默认为txt
    /// </summary>
    public static string ExportJsonExtension = "txt";

    /// <summary>
    /// 导出的json文件中是否将json字符串整理为带缩进格式的形式，默认为否
    /// </summary>
    public static bool ExportJsonIsFormat = false;

    /// <summary>
    /// 导出的json文件是否生成为各行数据对应的json object包含在一个json array的形式，默认为是
    /// </summary>
    public static bool ExportJsonIsExportJsonArrayFormat = true;

    /// <summary>
    /// 导出的json文件，若生成包含在一个json object的形式，是否使每行字段信息对应的json object中包含主键列对应的键值对，默认为是
    /// </summary>
    public static bool ExportJsonIsExportJsonMapIncludeKeyColumnValue = true;
}