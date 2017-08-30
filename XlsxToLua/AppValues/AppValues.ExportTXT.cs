using System.Collections.Generic;

public partial class AppValues
{
    #region 全局 常量配置
    //"-exportTxtParam(exportPath=TestExcel/txt|extension=txt|isExportColumnName=true|isExportColumnDataType=true)" 

    /// <summary>
    /// 声明将指定的Excel文件额外导出为txt文件
    /// </summary>
    public const string EXPORT_TXT_PARAM_STRING = "-exportTxt";
    /// <summary>
    /// 声明导出txt文件时的参数
    /// </summary>
    public const string EXPORT_TXT_PARAM_PARAM_STRING = "-exportTxtParam";
    /// <summary>
    /// 导出txt文件参数下属的具体参数，用于配置导出路径
    /// </summary>
    public const string EXPORT_TXT_PARAM_EXPORT_PATH_PARAM_STRING = "exportPath";

    /// <summary>
    /// 导出txt文件参数下属的具体参数，用于配置导出文件的扩展名
    /// </summary>
    public const string EXPORT_TXT_PARAM_EXTENSION_PARAM_STRING = "extension";

    /// <summary>
    /// 导出txt文件参数下属的具体参数，用于配置字段间的分隔符
    /// </summary>
    public const string EXPORT_TXT_PARAM_SPLIT_STRING_PARAM_STRING = "splitString";

    /// <summary>
    /// 导出txt文件参数下属的具体参数，用于配置是否在首行列举字段名称
    /// </summary>
    public const string EXPORT_TXT_PARAM_IS_EXPORT_COLUMN_NAME_PARAM_STRING = "isExportColumnName";

    /// <summary>
    /// 导出txt文件参数下属的具体参数，用于配置是否在其后列举字段数据类型
    /// </summary>
    public const string EXPORT_TXT_PARAM_IS_EXPORT_COLUMN_DATA_TYPE_PARAM_STRING = "isExportColumnDataType";
    #endregion

    #region 全局 初始值 默认值
    /// <summary>
    /// 存储本次要额外导出为txt文件的Excel文件名
    /// </summary>
    public static List<string> ExportTxtTableNames = new List<string>();

    /// <summary>
    /// 导出txt文件的存储路径
    /// </summary>
    public static string ExportTxtPath = null;

    /// <summary>
    /// 导出txt文件的扩展名（不含点号），默认为txt
    /// </summary>
    public static string ExportTxtExtension = "txt";
    /// <summary>
    /// 导出txt文件中的字段分隔符，默认为Tab键
    /// </summary>
    public static char ExportTxtSplitChar = '\t';

    /// <summary>
    /// 导出的txt文件中是否在首行列举字段名称，默认为是
    /// </summary>
    public static bool ExportTxtIsExportColumnName = true;

    /// <summary>
    /// 导出的txt文件中是否在其后列举字段数据类型，默认为是
    /// </summary>
    public static bool ExportTxtIsExportColumnDataType = true;
    #endregion

    #region 单表 常量 特殊方式导出
    /// <summary>
    /// 声明对某张表格设置特殊导出Txt规则的配置参数名
    /// </summary>
    public const string CONFIG_NAME_EXPORTTXT = "tableExportTxtConfig";

    /// <summary>
    /// 声明对某张表格不进行默认导出TXT的参数配置
    /// </summary>
    public const string CONFIG_PARAM_NOT_EXPORTTXT_ORIGINAL_TABLE = "-notExportTxtOriginalTable";

    /// <summary>
    /// 特殊方式导出txt文件参数下属的具体参数，用于配置导出路径
    /// </summary>
    public const string SPECIAL_EXPORT_TXT_PARAM_EXPORT_PATH_PARAM_STRING = "exportPath";

    /// <summary>
    /// 特殊方式导出txt文件参数下属的具体参数，用于配置导出文件的扩展名
    /// </summary>
    public const string SPECIAL_EXPORT_TXT_PARAM_EXTENSION_PARAM_STRING = "extension";

    /// <summary>
    /// 特殊方式导出txt文件参数下属的具体参数，用于配置导出文件上方注释说明的行数
    /// </summary>
    public const string SPECIAL_EXPORT_TXT_PARAM_TOPCOMMENTROWS_PARAM_STRING = "TopCommentRows";

    /// <summary>
    /// 特殊方式导出txt文件参数下属的具体参数，用于配置导出文件上方注释说明的行数
    /// </summary>
    public const int SPECIAL_EXPORT_TXT_PARAM_TOPCOMMENTROWS_INT = 0;

    /// <summary>
    /// 特殊方式导出txt文件参数下属的具体参数，用于配置导出文件上方注释说明内容
    /// </summary>
    public const string SPECIAL_EXPORT_TXT_PARAM_TOPCOMMENT_PARAM_STRING = "TopComment";

    /// <summary>
    /// 特殊方式导出txt文件参数下属的具体参数，用于配置导出文件上方注释说明的默认内容
    /// </summary>
    public const string SPECIAL_EXPORT_TXT_PARAM_TOPCOMMENT_STRING = "%%配置导表自动生成，请不要随意手动修改！！！";
    #endregion


    #region 单表 初始值 默认值 特殊方式导出
    /// <summary>
    /// 特殊方式导出txt文件的扩展名（不含点号），默认为hrl
    /// </summary>
    public static string SpecialExportTxtExtension = "hrl";
    #endregion

}