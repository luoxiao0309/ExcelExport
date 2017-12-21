using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;

class XMLToLuaHelper
{
    static Dictionary<string, string> m_dicAllType;
    public static void ExportTableToLua(string strXmlPath, out string errorString)
    {
        errorString = "";
        m_dicAllType = GetAllType(strXmlPath);
        StringBuilder content = new StringBuilder();

        // 生成数据内容开头
        content.AppendLine("return {");

        // 当前缩进量
        int currentLevel = 1;

        XmlDocument doc = new XmlDocument();
        doc.Load(strXmlPath);    //加载Xml文件 
        XmlElement root = doc.DocumentElement;   //获取根节点  
        content.Append(GetNodeInfo(root, currentLevel, 0, true));

        content.AppendLine("}");

        Utils.SaveLuaFile(Path.GetFileNameWithoutExtension(strXmlPath), content.ToString());        
    }

    static Dictionary<string, string> GetAllType(string strXmlPath)
    {
        Dictionary<string, string> dicAllType = new Dictionary<string, string>();
        XmlDocument doc = new XmlDocument();
        doc.Load(strXmlPath);    //加载Xml文件 
        XmlElement root = doc.DocumentElement;   //获取根节点  
        XmlNodeList definedNodes = root.GetElementsByTagName("DefinedType"); //获取Person子节点集合
        if(definedNodes != null)
        {
            foreach (XmlNode node in definedNodes)
            {
                XmlNodeList typeNodes = ((XmlElement)node).GetElementsByTagName("Type");
                if(typeNodes != null)
                {
                    foreach (XmlNode typeNode in typeNodes)
                    {
                        XmlAttributeCollection attrbutes = typeNode.Attributes;
                        for(int i = 0; i < attrbutes.Count; ++i)
                        {
                            XmlAttribute attrbute = attrbutes[i];
                            string strType = attrbute.Name;
                            string strTypeVlue = attrbute.Value;
                            string[] strNameArray = strTypeVlue.Split(",".ToCharArray());
                            if(strTypeVlue != null)
                            {
                                for(int j = 0; j < strNameArray.Length; ++j)
                                {
                                    if (!dicAllType.ContainsKey(strNameArray[j]))
                                        dicAllType.Add(strNameArray[j], strType);
                                }
                            }
                        }
                    }
                }
            }
        }
        return dicAllType;
    }

    //检测类型是否合法
    static bool CheckLegitimate(string strTypeName, string strValue)
    {
        return true;
    }

   
    static string GetNodeInfo(XmlNode node, int curLevel, int nIndex, bool bMax)
    {
        StringBuilder content = new StringBuilder();
        if (node != null && node.Name != "DefinedType")
        {
            if (node.PreviousSibling != null && !string.IsNullOrEmpty(node.PreviousSibling.Value))
            {
                content.Append(TableExportToLuaHelper._GetLuaTableIndentation(curLevel));
                content.Append(string.Format("-- {0}\n", node.PreviousSibling.Value));
            }
            if(nIndex == 0 || nIndex == 1)
            {
                content.Append(TableExportToLuaHelper._GetLuaTableIndentation(curLevel));
                content.Append(string.Format("{0} = ", node.Name) + "{ \n");
            }

            //多层同样结点数组
            if (nIndex > 0)
            {
                curLevel++;
                content.Append(TableExportToLuaHelper._GetLuaTableIndentation(curLevel));
                content.Append(string.Format("[{0}] = ", nIndex) + "{ \n");
            }

            content.Append(GetAttrbutesInfo(node, curLevel + 1));

            XmlNodeList nodeList = node.ChildNodes;
            if (nodeList != null && nodeList.Count > 0)
            {
                Dictionary<string, int> dicSameNode = new Dictionary<string, int>();
                foreach (XmlNode nodeChild in nodeList)
                {
                    if (nodeChild != null && !(nodeChild is XmlComment))
                    {
                        if (!dicSameNode.ContainsKey(nodeChild.Name))
                            dicSameNode.Add(nodeChild.Name, 0);
                        dicSameNode[nodeChild.Name]++;
                    }
                }

                foreach(KeyValuePair<string, int> it in dicSameNode)
                {
                    XmlNodeList nodeChildList1 = node.SelectNodes(it.Key);
                    
                    int nChildIndex = 1;
                    foreach (XmlNode nodeChild in nodeChildList1)
                    {
                        content.Append(GetNodeInfo(nodeChild, curLevel + 1, nodeChildList1.Count > 1 ? nChildIndex : 0, nodeChildList1.Count == nChildIndex));
                        nChildIndex++;
                    }
                }
            }
            if(nIndex > 0)
            {
                content.Append(TableExportToLuaHelper._GetLuaTableIndentation(curLevel));
                content.Append("},\n");
                curLevel--;
            }
            if (bMax)
            {
                content.Append(TableExportToLuaHelper._GetLuaTableIndentation(curLevel));
                content.Append("},\n");
            }
        }
        return content.ToString();
    }

    //获取属性信息
    static string GetAttrbutesInfo(XmlNode node, int curLevel)
    {
        StringBuilder content = new StringBuilder();

        if (node != null && node.Name != "DefinedType" && node.Attributes.Count > 0)
        {
            XmlAttributeCollection attrbutes = node.Attributes;
            for (int i = 0; i < attrbutes.Count; ++i)
            {
                XmlAttribute attrbute = attrbutes[i];
                string strName = attrbute.Name;
                string strValue = attrbute.Value;
                if (!m_dicAllType.ContainsKey(strName) || !CheckLegitimate(m_dicAllType[strName], strValue))
                    continue;
                content.Append(TableExportToLuaHelper._GetLuaTableIndentation(curLevel));
                switch(m_dicAllType[strName])
                {
                    case   "string":
                    case    "time":
                    case    "date":
                        {
                            strValue = string.Format("\"{0}\"", strValue);
                        }
                        break;
                }
                content.Append(string.Format("{0} = {1},\n", strName, strValue));
            }
        }
        return content.ToString();
    }
}
