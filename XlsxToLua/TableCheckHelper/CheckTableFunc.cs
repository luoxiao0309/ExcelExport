using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

public partial class TableCheckHelper
{
    /// <summary>
    /// 用于将整张表格用自定义函数进行检查
    /// </summary>
    public static bool CheckTableFunc(TableInfo tableInfo, out string errorString)
    {
        if (tableInfo.TableConfig == null || !tableInfo.TableConfig.ContainsKey(AppValues.CONFIG_NAME_CHECK_TABLE))
        {
            errorString = null;
            return true;
        }
        List<string> checkTableFuncNames = tableInfo.TableConfig[AppValues.CONFIG_NAME_CHECK_TABLE];
        if (checkTableFuncNames.Count < 1)
        {
            errorString = null;
            Utils.LogWarning(string.Format("警告：表格{0}中声明了整表检查参数但没有配置任何检查函数，请确认是否遗忘", tableInfo.TableName));
            return true;
        }
        Type myCheckFunctionClassType = typeof(MyCheckFunction);
        if (myCheckFunctionClassType == null)
        {
            errorString = string.Format("自定义函数检查规则无法使用，找不到{0}类\n", myCheckFunctionClassType.Name);
            return false;
        }
        StringBuilder errorStringBuilder = new StringBuilder();
        foreach (string funcName in checkTableFuncNames)
        {
            MethodInfo dynMethod = myCheckFunctionClassType.GetMethod(funcName, BindingFlags.Public | BindingFlags.Static, null, new Type[] { typeof(TableInfo), typeof(string).MakeByRefType() }, null);
            if (dynMethod == null)
            {
                errorStringBuilder.AppendFormat("自定义整表函数检查规则声明错误，{0}.cs中找不到符合要求的名为\"{1}\"的函数，函数必须形如public static bool funcName(TableInfo tableInfo, out string errorString)\n", myCheckFunctionClassType.Name, funcName);
                continue;
            }
            else
            {
                string tempErrorString = null;
                object[] inputParams = new object[] { tableInfo, tempErrorString };
                bool checkResult = true;
                try
                {
                    checkResult = (bool)dynMethod.Invoke(null, inputParams);
                }
                catch (Exception exception)
                {
                    errorString = string.Format("运行自定义整表检查函数{0}错误，请修正代码后重试\n{1}", funcName, exception);
                    return false;
                }
                if (inputParams[1] != null)
                    tempErrorString = inputParams[1].ToString();

                if (checkResult == false)
                    errorStringBuilder.AppendFormat("未通过自定义整表检查函数{0}的检查，存在以下错误：\n{1}\n", funcName, tempErrorString);
            }
        }

        errorString = errorStringBuilder.ToString();
        if (string.IsNullOrEmpty(errorString))
        {
            errorString = null;
            return true;
        }
        else
            return false;
    }
}