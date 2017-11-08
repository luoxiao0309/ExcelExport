using System;
using System.Collections.Generic;

public partial class TableCheckHelper
{
    /// <summary>
    /// 解析一个字段的所有表格检查规则
    /// </summary>
    public static List<FieldCheckRule> GetCheckRules(string checkRuleString, out string errorString)
    {
        if (string.IsNullOrEmpty(checkRuleString))
        {
            errorString = null;
            return null;
        }
        else
        {
            List<FieldCheckRule> checkRules = new List<FieldCheckRule>();
            errorString = null;

            // 不同检查规则通过&&分隔
            string[] ruleString = checkRuleString.Split(new string[] { "&&" }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < ruleString.Length; ++i)
            {
                string oneRule = ruleString[i].Trim();
                if (oneRule == string.Empty)
                    continue;

                List<FieldCheckRule> oneCheckRule = _GetOneCheckRule(oneRule, out errorString);
                if (errorString != null)
                    return null;
                else
                    checkRules.AddRange(oneCheckRule);
            }

            return checkRules;
        }
    }

    /// <summary>
    /// 解析一条表格检查规则
    /// 注意：要把config配置的规则彻底解析为TABLE_CHECK_TYPE定义的基本的检查规则，故要考虑如果是config配置的规则中继续嵌套config配置规则的情况
    /// </summary>
    private static List<FieldCheckRule> _GetOneCheckRule(string ruleString, out string errorString)
    {
        List<FieldCheckRule> oneCheckRule = new List<FieldCheckRule>();
        errorString = null;

        if (ruleString.StartsWith("notEmpty", StringComparison.CurrentCultureIgnoreCase))
        {
            FieldCheckRule checkRule = new FieldCheckRule();
            checkRule.CheckType = TableCheckType.NotEmpty;
            checkRule.CheckRuleString = ruleString;
            oneCheckRule.Add(checkRule);
        }
        else if (ruleString.StartsWith("unique", StringComparison.CurrentCultureIgnoreCase))
        {
            FieldCheckRule checkRule = new FieldCheckRule();
            checkRule.CheckType = TableCheckType.Unique;
            checkRule.CheckRuleString = ruleString;
            oneCheckRule.Add(checkRule);
        }
        else if (ruleString.StartsWith("refStr", StringComparison.CurrentCultureIgnoreCase))
        {
            FieldCheckRule checkRule = new FieldCheckRule();
            checkRule.CheckType = TableCheckType.RefStr;
            checkRule.CheckRuleString = ruleString;
            oneCheckRule.Add(checkRule);
        }
        else if (ruleString.StartsWith("ref", StringComparison.CurrentCultureIgnoreCase))
        {
            FieldCheckRule checkRule = new FieldCheckRule();
            checkRule.CheckType = TableCheckType.Ref;
            checkRule.CheckRuleString = ruleString;
            oneCheckRule.Add(checkRule);
        }
        else if (ruleString.StartsWith("mapString", StringComparison.CurrentCultureIgnoreCase))
        {
            FieldCheckRule checkRule = new FieldCheckRule();
            checkRule.CheckType = TableCheckType.MapString;
            checkRule.CheckRuleString = ruleString;
            oneCheckRule.Add(checkRule);
        }
        else if (ruleString.StartsWith(">") || ruleString.StartsWith(">="))
        {
            FieldCheckRule checkRule = new FieldCheckRule();
            checkRule.CheckType = TableCheckType.GreaterThan;
            checkRule.CheckRuleString = ruleString;
            oneCheckRule.Add(checkRule);
        }
        else if (ruleString.StartsWith("func", StringComparison.CurrentCultureIgnoreCase))
        {
            FieldCheckRule checkRule = new FieldCheckRule();
            checkRule.CheckType = TableCheckType.Func;
            checkRule.CheckRuleString = ruleString;
            oneCheckRule.Add(checkRule);
        }
        else if (ruleString.StartsWith("file", StringComparison.CurrentCultureIgnoreCase))
        {
            FieldCheckRule checkRule = new FieldCheckRule();
            checkRule.CheckType = TableCheckType.File;
            checkRule.CheckRuleString = ruleString;
            oneCheckRule.Add(checkRule);
        }
        else if (ruleString.StartsWith("$"))
        {
            // 到config文件中找到对应的检查规则
            if (AppValues.ConfigData.ContainsKey(ruleString))
            {
                string configRuleString = AppValues.ConfigData[ruleString];
                // 不同检查规则通过&&分隔
                string[] ruleStringInConfigRule = configRuleString.Split(new string[] { "&&" }, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < ruleStringInConfigRule.Length; ++i)
                {
                    string oneRule = ruleStringInConfigRule[i].Trim();
                    if (oneRule.Equals(string.Empty))
                        continue;

                    // 递归调用自身，解析config中配置的检查规则
                    List<FieldCheckRule> configCheckRules = _GetOneCheckRule(oneRule, out errorString);
                    if (errorString != null)
                    {
                        errorString = string.Format("config文件中名为\"{0}\"的配置\"{1}\"有误：", ruleString, configRuleString) + errorString;
                        return null;
                    }
                    else
                        oneCheckRule.AddRange(configCheckRules);
                }
            }
            else
            {
                errorString = string.Format("config文件中找不到名为\"{0}\"的检查规则配置", ruleString);
                return null;
            }
        }
        else if (ruleString.StartsWith("!") && ruleString.IndexOf("{") > 0)
        {
            FieldCheckRule checkRule = new FieldCheckRule();
            checkRule.CheckType = TableCheckType.Illegal;
            checkRule.CheckRuleString = ruleString;
            oneCheckRule.Add(checkRule);
        }
        else if (ruleString.StartsWith("{"))
        {
            FieldCheckRule checkRule = new FieldCheckRule();
            checkRule.CheckType = TableCheckType.Effective;
            checkRule.CheckRuleString = ruleString;
            oneCheckRule.Add(checkRule);
        }
        else if (ruleString.StartsWith("(") || ruleString.StartsWith("["))
        {
            FieldCheckRule checkRule = new FieldCheckRule();
            checkRule.CheckType = TableCheckType.Range;
            checkRule.CheckRuleString = ruleString;
            oneCheckRule.Add(checkRule);
        }
        else
        {
            errorString = "未知的检查规则";
            return null;
        }

        return oneCheckRule;
    }
}