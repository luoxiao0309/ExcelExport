using System.Collections.Generic;

public partial class AppValues
{
    /// <summary>
    /// 声明将指定的Excel文件额外导出为csv对应C#类文件
    /// </summary>
    public const string EXPORT_CS_CLASS_PARAM_STRING = "-exportCsClass";

    /// <summary>
    /// 声明导出与csv对应C#类文件的参数
    /// </summary>
    public const string EXPORT_CS_CLASS_PARAM_PARAM_STRING = "-exportCsClassParam";

    /// <summary>
    /// 导出csv对应C#类文件参数下属的具体参数，用于配置导出路径
    /// </summary>
    public const string EXPORT_CS_CLASS_PARAM_EXPORT_PATH_PARAM_STRING = "exportPath";

    /// <summary>
    /// 导出csv对应C#类文件参数下属的具体参数，用于配置命名空间
    /// </summary>
    public const string EXPORT_CS_CLASS_PARAM_NAMESPACE_PARAM_STRING = "namespace";

    /// <summary>
    /// 导出csv对应C#类文件参数下属的具体参数，用于配置引用类库
    /// </summary>
    public const string EXPORT_CS_CLASS_PARAM_USING_PARAM_STRING = "using";

    /// <summary>
    /// 导出csv对应C#类文件的扩展名（不含点号）
    /// </summary>
    public static string EXPORT_CS_CLASS_FILE_EXTENSION = "cs";

    /// <summary>
    /// 存储本次要额外导出为csv对应C#类文件的Excel文件名
    /// </summary>
    public static List<string> ExportCsClassTableNames = new List<string>();

    /// <summary>
    /// 导出csv对应C#类文件的存储路径
    /// </summary>
    public static string ExportCsClassPath = null;

    /// <summary>
    /// 导出csv对应C#类文件中的命名空间
    /// </summary>
    public static string ExportCsClassNamespace = null;

    /// <summary>
    /// 导出csv对应C#类文件中的引用类库
    /// </summary>
    public static List<string> ExportCsClassUsing = null;
}