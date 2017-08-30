using System.Collections.Generic;

public partial class AppValues
{
    /// <summary>
    /// 声明将指定的Excel文件额外导出为csv文件
    /// </summary>
    public const string EXPORT_CSV_PARAM_STRING = "-exportCsv";

    /// <summary>
    /// 声明导出csv文件时的参数
    /// </summary>
    public const string EXPORT_CSV_PARAM_PARAM_STRING = "-exportCsvParam";

    /// <summary>
    /// 导出csv文件参数下属的具体参数，用于配置导出路径
    /// </summary>
    public const string EXPORT_CSV_PARAM_EXPORT_PATH_PARAM_STRING = "exportPath";

    /// <summary>
    /// 导出csv文件参数下属的具体参数，用于配置导出文件的扩展名
    /// </summary>
    public const string EXPORT_CSV_PARAM_EXTENSION_PARAM_STRING = "extension";

    /// <summary>
    /// 导出csv文件参数下属的具体参数，用于配置字段间的分隔符
    /// </summary>
    public const string EXPORT_CSV_PARAM_SPLIT_STRING_PARAM_STRING = "splitString";

    /// <summary>
    /// 导出csv文件参数下属的具体参数，用于配置是否在首行列举字段名称
    /// </summary>
    public const string EXPORT_CSV_PARAM_IS_EXPORT_COLUMN_NAME_PARAM_STRING = "isExportColumnName";

    /// <summary>
    /// 导出csv文件参数下属的具体参数，用于配置是否在其后列举字段数据类型
    /// </summary>
    public const string EXPORT_CSV_PARAM_IS_EXPORT_COLUMN_DATA_TYPE_PARAM_STRING = "isExportColumnDataType";

    /// <summary>
    /// 存储本次要额外导出为csv文件的Excel文件名
    /// </summary>
    public static List<string> ExportCsvTableNames = new List<string>();

    /// <summary>
    /// 导出csv文件的存储路径
    /// </summary>
    public static string ExportCsvPath = null;

    /// <summary>
    /// 导出csv文件的扩展名（不含点号），默认为csv
    /// </summary>
    public static string ExportCsvExtension = "csv";

    /// <summary>
    /// 导出csv文件中的字段分隔符，默认为英文逗号
    /// </summary>
    public static string ExportCsvSplitString = ",";

    /// <summary>
    /// 导出的csv文件中是否在首行列举字段名称，默认为是
    /// </summary>
    public static bool ExportCsvIsExportColumnName = true;

    /// <summary>
    /// 导出的csv文件中是否在其后列举字段数据类型，默认为是
    /// </summary>
    public static bool ExportCsvIsExportColumnDataType = true;
}