using System.Collections.Generic;
using System.IO;
using System.Text;

public partial class TableCheckHelper
{
    /// <summary>
    /// 用于检查string型的文件路径对应的文件是否存在
    /// </summary>
    public static bool CheckFile2(FieldInfo fieldInfo, FieldCheckRule checkRule, out string errorString)
    {
        // 先判断是否输入了Client目录的路径
        if (AppValues.ClientPath == null)
        {
            errorString = "文件存在性检查无法进行：必须在程序运行时传入Client目录的路径\n";
            return false;
        }
        else if (fieldInfo.DataType == DataType.String)
        {
            int colonIndex = checkRule.CheckRuleString.IndexOf(":");
            if (colonIndex == -1)
            {
                errorString = "文件存在性检查定义错误：必须在英文冒号后声明相对于Client目录的路径\n";
                return false;
            }
            else
            {
                // 存储不存在的文件信息（key：行号， value：输入的文件名）
                Dictionary<int, string> inexistFileInfo = new Dictionary<int, string>();
                // 存储含有\或/的非法文件名信息
                List<int> illegalFileNames = new List<int>();

                // 判断规则中填写的文件的路径与Client路径组合后是否为一个已存在路径
                string inputPath = checkRule.CheckRuleString.Substring(colonIndex + 1, checkRule.CheckRuleString.Length - colonIndex - 1).Trim();
                string pathString = Utils.CombinePath(AppValues.ClientPath, inputPath);
                if (!Directory.Exists(pathString))
                {
                    errorString = string.Format("文件存在性检查定义错误：声明的文件所在目录不存在，请检查定义的路径是否正确，最终拼得的路径为{0}\n", pathString);
                    return false;
                }
                // 提取file和冒号之间的字符串，判断是否声明扩展名
                const string START_STRING = "file2";
                string extensionString = checkRule.CheckRuleString.Substring(START_STRING.Length, colonIndex - START_STRING.Length).Trim();
                // 如果声明了扩展名，则遍历出目标目录下所有该扩展名的文件，然后逐行判断文件是否存在
                if (string.IsNullOrEmpty(extensionString))
                {
                    // 如果没有声明扩展名，则每行数据都用File.Exists判断是否存在
                    for (int i = 0; i < fieldInfo.Data.Count; ++i)
                    {
                        // 忽略无效集合元素下属子类型的空值
                        if (fieldInfo.Data[i] == null)
                            continue;

                        // 文件名中不允许含有\或/，即不支持文件在填写路径的非同级目录
                        string inputFileName = fieldInfo.Data[i].ToString().Trim();
                        if (string.IsNullOrEmpty(inputFileName))
                            continue;
                        else if (inputFileName.IndexOf('\\') != -1 || inputFileName.IndexOf('/') != -1)
                            illegalFileNames.Add(i);
                        else
                        {
                            //string path = Utils.CombinePath(pathString, inputFileName);
                            string[] paths = Utils.GetAllFolders(pathString, "", false);
                            bool exists = ((System.Collections.IList)paths).Contains(pathString + "\\" + inputFileName);
                            if(!exists)
                            inexistFileInfo.Add(i, inputFileName);
                        }
                    }
                }
                else
                {
                    if (extensionString.StartsWith("(") && extensionString.EndsWith(")"))
                    {
                        // 提取括号中定义的扩展名
                        string extension = extensionString.Substring(1, extensionString.Length - 2).Trim();
                        if (extension != "*")
                        {
                            // 判断扩展名是否合法（只能为数字或小写英文字母）
                            foreach (char c in extension)
                            {
                                if (!((c >= 'a' && c <= 'z') || (c >= '0' && c <= '9')))
                                {
                                    errorString = string.Format("文件存在性检查定义错误：声明文件扩展名不合法，文件扩展名只能为*号或由小写英文字母和数字组成，你填写的为{0}\n", extension);
                                    return false;
                                }
                            }
                        }
                            Utils.GetDicFile(extension, pathString, out errorString);
                        if (errorString !=null)
                        {
                            return false;
                        }
                        for (int i = 0; i < fieldInfo.Data.Count; ++i)
                        {
                            // 忽略无效集合元素下属子类型的空值
                            if (fieldInfo.Data[i] == null|| fieldInfo.Data[i].ToString() == "0")
                                    continue;

                                // 文件名中不允许含有\或/，即不支持文件在填写路径的非同级目录
                                string inputFileName = fieldInfo.Data[i].ToString().Trim();
                                if (string.IsNullOrEmpty(inputFileName))
                                    continue;
                                else if (inputFileName.IndexOf('\\') != -1 || inputFileName.IndexOf('/') != -1)
                                    illegalFileNames.Add(i);
                                else
                                {
                                    if (!AppValues.FlieNames.ContainsKey(pathString + "（." + extension + "）" + inputFileName))
                                        inexistFileInfo.Add(i, inputFileName);
                                }
                            }
                        }
                    else
                    {
                        errorString = "文件存在性检查定义错误：如果要声明扩展名，\"file2\"与英文冒号之间必须在英文括号内声明扩展名\n";
                        return false;
                    }
                }

                

                

                if (inexistFileInfo.Count > 0 || illegalFileNames.Count > 0)
                {
                    StringBuilder errorStringBuild = new StringBuilder();
                    if (illegalFileNames.Count > 0)
                    {
                        errorStringBuild.Append("单元格中填写的文件名中不允许含有\"\\\"或\"/\"，即要求填写的文件必须在填写路径的同级目录，以下行对应的文件名不符合此规则：");
                        string separator = ", ";
                        foreach (int lineNum in illegalFileNames)
                            errorStringBuild.AppendFormat("{0}{1}", lineNum + AppValues.DATA_FIELD_DATA_START_INDEX + 1, separator);

                        // 去掉末尾多余的", "
                        errorStringBuild.Remove(errorStringBuild.Length - separator.Length, separator.Length);

                        errorStringBuild.Append("\n");
                    }
                    if (inexistFileInfo.Count > 0)
                    {
                        errorStringBuild.AppendLine("存在以下找不到的文件：");
                        foreach (var item in inexistFileInfo)
                            errorStringBuild.AppendFormat("第{0}行数据，填写的文件名为{1}\n", item.Key + AppValues.DATA_FIELD_DATA_START_INDEX + 1, item.Value);
                    }

                    errorString = errorStringBuild.ToString();
                    return false;
                }
                else
                {
                    errorString = null;
                    return true;
                }
            }
        }
        else
        {
            errorString = string.Format("文件存在性检查定义只能用于string型的字段，而该字段为{0}型\n", fieldInfo.DataType);
            return false;
        }
    }
  
}