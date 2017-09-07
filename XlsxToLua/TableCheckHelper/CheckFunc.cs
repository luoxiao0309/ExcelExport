using System;
using System.Reflection;

public partial class TableCheckHelper
{
    /// <summary>
    /// 用于将指定的字段用自定义函数进行检查
    /// </summary>
    public static bool CheckFunc(FieldInfo fieldInfo, FieldCheckRule checkRule, out string errorString)
    {
        // 提取func:后声明的自定义函数名
        const string START_STRING = "func:";
        if (!checkRule.CheckRuleString.StartsWith(START_STRING, StringComparison.CurrentCultureIgnoreCase))
        {
            errorString = string.Format("自定义函数检查规则声明错误，必须以\"{0}\"开头，后面跟MyCheckFunction.cs中声明的函数名\n", START_STRING);
            return false;
        }
        else
        {
            string funcName = checkRule.CheckRuleString.Substring(START_STRING.Length, checkRule.CheckRuleString.Length - START_STRING.Length).Trim();
            Type myCheckFunctionClassType = typeof(MyCheckFunction);
            if (myCheckFunctionClassType != null)
            {
                MethodInfo dynMethod = myCheckFunctionClassType.GetMethod(funcName, BindingFlags.Public | BindingFlags.Static, null, new Type[] { typeof(FieldInfo), typeof(string).MakeByRefType() }, null);
                if (dynMethod == null)
                {
                    errorString = string.Format("自定义函数检查规则声明错误，{0}.cs中找不到符合要求的名为\"{1}\"的函数，函数必须形如public static bool funcName(FieldInfo fieldInfo, out string errorString)\n", myCheckFunctionClassType.Name, funcName);
                    return false;
                }
                else
                {
                    errorString = null;
                    object[] inputParams = new object[] { fieldInfo, errorString };
                    bool checkResult = true;
                    try
                    {
                        checkResult = (bool)dynMethod.Invoke(null, inputParams);
                    }
                    catch (Exception exception)
                    {
                        errorString = string.Format("运行自定义检查函数{0}错误，请修正代码后重试\n{1}", funcName, exception);
                        return false;
                    }
                    if (inputParams[1] != null)
                        errorString = inputParams[1].ToString();

                    if (checkResult == true)
                        return true;
                    else
                    {
                        errorString = string.Format("未通过自定义函数规则检查，存在以下错误：\n{0}\n", errorString);
                        return false;
                    }
                }
            }
            else
            {
                errorString = string.Format("自定义函数检查规则无法使用，找不到{0}类\n", myCheckFunctionClassType.Name);
                return false;
            }
        }
    }
}