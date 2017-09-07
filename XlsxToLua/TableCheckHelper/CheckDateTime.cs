using System.Collections.Generic;

public partial class TableCheckHelper
{
    /// <summary>
    /// 检查date型的输入格式定义
    /// </summary>
    public static bool CheckDateInputDefine(string defineString, out string errorString)
    {
        defineString = defineString.Trim();
        if (string.IsNullOrEmpty(defineString))
        {
            errorString = "未进行格式声明";
            return false;
        }
        DateFormatType formatType = TableAnalyzeHelper.GetDateFormatType(defineString);
        if (!(formatType == DateFormatType.FormatString || formatType == DateFormatType.ReferenceDateMsec || formatType == DateFormatType.ReferenceDateSec))
        {
            errorString = "不属于合法的date型输入格式类型";
            return false;
        }

        errorString = null;
        return true;
    }

    /// <summary>
    /// 检查date型导出至lua文件的格式定义
    /// </summary>
    public static bool CheckDateToLuaDefine(string defineString, out string errorString)
    {
        defineString = defineString.Trim();
        if (string.IsNullOrEmpty(defineString))
        {
            errorString = "未进行格式声明";
            return false;
        }

        errorString = null;
        return true;
    }

    /// <summary>
    /// 检查date型导出至MySQL数据库的格式定义
    /// </summary>
    public static bool CheckDateToDatabaseDefine(string defineString, out string errorString)
    {
        defineString = defineString.Trim();
        if (string.IsNullOrEmpty(defineString))
        {
            errorString = "未进行格式声明";
            return false;
        }
        DateFormatType formatType = TableAnalyzeHelper.GetDateFormatType(defineString);
        if (!(formatType == DateFormatType.FormatString || formatType == DateFormatType.ReferenceDateMsec || formatType == DateFormatType.ReferenceDateSec))
        {
            errorString = "不属于合法的date型导出至MySQL数据库的格式类型";
            return false;
        }

        errorString = null;
        return true;
    }

    /// <summary>
    /// 检查time型的格式定义
    /// </summary>
    public static bool CheckTimeDefine(string defineString, out string errorString)
    {
        defineString = defineString.Trim();
        if (string.IsNullOrEmpty(defineString))
        {
            errorString = "未进行格式声明";
            return false;
        }
        TimeFormatType formatType = TableAnalyzeHelper.GetTimeFormatType(defineString);
        if (formatType == TimeFormatType.FormatString)
        {
            // 检查time型的格式字符串声明，不允许出现代表年月日的y、M、d
            List<string> errorInfo = new List<string>();
            if (defineString.IndexOf('y') != -1)
                errorInfo.Add("代表年的y");
            if (defineString.IndexOf('M') != -1)
                errorInfo.Add("代表月的M");
            if (defineString.IndexOf('d') != -1)
                errorInfo.Add("代表日的d");

            if (errorInfo.Count > 0)
            {
                errorString = string.Format("time类型的格式定义中不允许出现以下与年月日相关的日期型格式定义字符：{0}", Utils.CombineString(errorInfo, "，"));
                return false;
            }
        }

        errorString = null;
        return true;
    }
}