using System.Collections.Generic;

public partial class AppValues
{
    /// <summary>
    /// 声明将指定的Excel文件额外导出为csv对应Java类文件
    /// </summary>
    public const string EXPORT_JAVA_CLASS_PARAM_STRING = "-exportJavaClass";

    /// <summary>
    /// 声明导出与csv对应Java类文件的参数
    /// </summary>
    public const string EXPORT_JAVA_CLASS_PARAM_PARAM_STRING = "-exportJavaClassParam";

    /// <summary>
    /// 导出csv对应Java类文件参数下属的具体参数，用于配置导出路径
    /// </summary>
    public const string EXPORT_JAVA_CLASS_PARAM_EXPORT_PATH_PARAM_STRING = "exportPath";

    /// <summary>
    /// 导出csv对应Java类文件参数下属的具体参数，用于配置包名
    /// </summary>
    public const string EXPORT_JAVA_CLASS_PARAM_PACKAGE_PARAM_STRING = "package";

    /// <summary>
    /// 导出csv对应Java类文件参数下属的具体参数，用于配置引用类库
    /// </summary>
    public const string EXPORT_JAVA_CLASS_PARAM_IMPORT_PARAM_STRING = "import";

    /// <summary>
    /// 导出csv对应Java类文件参数下属的具体参数，用于配置是否将日期型转为Date而不是Calendar类型
    /// </summary>
    public const string EXPORT_JAVA_CLASS_PARAM_IS_USE_DATE_PARAM_STRING = "isUseDate";

    /// <summary>
    /// 导出csv对应Java类文件参数下属的具体参数，用于配置是否生成无参构造函数
    /// </summary>
    public const string EXPORT_JAVA_CLASS_PARAM_IS_GENERATE_CONSTRUCTOR_WITHOUT_FIELDS_PARAM_STRING = "isGenerateConstructorWithoutFields";

    /// <summary>
    /// 导出csv对应Java类文件参数下属的具体参数，用于配置是否生成含全部参数的构造函数
    /// </summary>
    public const string EXPORT_JAVA_CLASS_PARAM_IS_GENERATE_CONSTRUCTOR_WITH_ALL_FIELDS_PARAM_STRING = "isGenerateConstructorWithAllFields";

    /// <summary>
    /// 导出csv对应Java类文件的扩展名（不含点号）
    /// </summary>
    public static string EXPORT_JAVA_CLASS_FILE_EXTENSION = "java";

    /// <summary>
    /// 存储本次要额外导出为csv对应Java类文件的Excel文件名
    /// </summary>
    public static List<string> ExportJavaClassTableNames = new List<string>();

    /// <summary>
    /// 导出csv对应Java类文件的存储路径
    /// </summary>
    public static string ExportJavaClassPath = null;

    /// <summary>
    /// 导出csv对应Java类文件的包名
    /// </summary>
    public static string ExportJavaClassPackage = null;

    /// <summary>
    /// 导出csv对应Java类文件中的引用类库
    /// </summary>
    public static List<string> ExportJavaClassImport = null;

    /// <summary>
    /// 导出csv对应Java类文件中，是否将时间型转为Date而不是Calendar，默认为true
    /// </summary>
    public static bool ExportJavaClassIsUseDate = true;

    /// <summary>
    /// 导出csv对应Java类文件中，是否生成无参构造函数，默认为false
    /// </summary>
    public static bool ExportJavaClassisGenerateConstructorWithoutFields = false;

    /// <summary>
    /// 导出csv对应Java类文件中，是否生成含全部参数的构造函数，默认为false
    /// </summary>
    public static bool ExportJavaClassIsGenerateConstructorWithAllFields = false;
}