using System.Collections.Generic;
using System.IO;
using System.Text;

// col是竖行,row是横排，防止我忘了
public class LTCSVLoader
{

    private TextReader inStream = null;

    private List<string> vContent;

    private List<List<string>> table;

    /// <summary>
    /// 只支持GBK2312的编码（WPS直接保存的编码支持,仅提供给Windows使用）
    /// </summary>
    /// <param name="fileName"></param>
    public void ReadFile(string fileName)
    {
        inStream = new StreamReader(fileName, Encoding.GetEncoding("GBK"));
        table = new List<List<string>>();
        List<string> temp = this.getLineContentVector();
        while (null != temp)
        {
            List<string> tempList = new List<string>();
            for (int i = 0; i < temp.Count; ++i)
            {
                tempList.Add(temp[i]);
            }
            table.Add(tempList);
            temp = this.getLineContentVector();
        }
    }

    /// <summary>
    /// 目前只支持UTF-8的编码（WPS直接保存的编码不支持）
    /// </summary>
    /// <param name="str"></param>
    public void ReadMultiLine(string str)
    {
        inStream = new StringReader(str);

        table = new List<List<string>>();
        List<string> temp = this.getLineContentVector();
        while (null != temp)
        {
            List<string> tempList = new List<string>();
            for (int i = 0; i < temp.Count; ++i)
            {
                tempList.Add(temp[i]);
            }
            table.Add(tempList);
            temp = this.getLineContentVector();
        }
    }

    private int containsNumber(string parentStr, string parameter)
    {
        int containNumber = 0;
        if (parentStr == null || parentStr.Equals(""))
        {
            return 0;
        }
        if (parameter == null || parameter.Equals(""))
        {
            return 0;
        }
        for (int i = 0; i < parentStr.Length; i++)
        {
            i = parentStr.IndexOf(parameter, i);
            if (i > -1)
            {
                i = i + parameter.Length;
                i--;
                containNumber = containNumber + 1;
            }
            else
            {
                break;
            }
        }
        return containNumber;
    }

    private bool isQuoteAdjacent(string p_String)
    {
        bool ret = false;
        string temp = p_String;
        temp = temp.Replace("\"\"", "");
        if (temp.IndexOf("\"") == -1)
        {
            ret = true;
        }
        return ret;
    }

    private bool isQuoteContained(string p_String)
    {
        bool ret = false;
        if (p_String == null || p_String.Equals(""))
        {
            return false;
        }
        if (p_String.IndexOf("\"") > -1)
        {
            ret = true;
        }
        return ret;
    }

    private string[] readAtomString(string lineStr)
    {
        string atomString = "";// 要读取的原子字符串
        string orgString = "";// 保存第一次读取下一个逗号时的未经任何处理的字符串
        string[] ret = new string[2];// 要返回到外面的数组
        bool isAtom = false;// 是否是原子字符串的标志
        string[] commaStr = lineStr.Split(new char[] { ',' });
        while (!isAtom)
        {
            foreach (string str in commaStr)
            {
                if (!atomString.Equals(""))
                {
                    atomString = atomString + ",";
                }
                atomString = atomString + str;
                orgString = atomString;
                if (!isQuoteContained(atomString))
                {
                    // 如果字符串中不包含引号，则为正常，返回
                    isAtom = true;
                    break;
                }
                else
                {
                    if (!atomString.StartsWith("\""))
                    {
                        // 如果字符串不是以引号开始，则表示不转义，返回
                        isAtom = true;
                        break;
                    }
                    else if (atomString.StartsWith("\""))
                    {
                        // 如果字符串以引号开始，则表示转义
                        if (containsNumber(atomString, "\"") % 2 == 0)
                        {
                            // 如果含有偶数个引号
                            string temp = atomString;
                            if (temp.EndsWith("\""))
                            {
                                temp = temp.Replace("\"\"", "");
                                if (temp.Equals(""))
                                {
                                    // 如果temp为空
                                    atomString = "";
                                    isAtom = true;
                                    break;
                                }
                                else
                                {
                                    // 如果temp不为空，则去掉前后引号
                                    temp = temp.Substring(1, temp.LastIndexOf("\""));
                                    if (temp.IndexOf("\"") > -1)
                                    {
                                        // 去掉前后引号和相邻引号之后，若temp还包含有引号
                                        // 说明这些引号是单个单个出现的
                                        temp = atomString;
                                        temp = temp.Substring(1);
                                        temp = temp.Substring(0, temp.IndexOf("\""))
                                                + temp.Substring(temp.IndexOf("\"") + 1);
                                        atomString = temp;
                                        isAtom = true;
                                        break;
                                    }
                                    else
                                    {
                                        // 正常的csv文件
                                        temp = atomString;
                                        temp = temp.Substring(1, temp.LastIndexOf("\""));
                                        temp = temp.Replace("\"\"", "\"");
                                        atomString = temp;
                                        isAtom = true;
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                // 如果不是以引号结束，则去掉前两个引号
                                temp = temp.Substring(1, temp.IndexOf('\"', 1))
                                        + temp.Substring(temp.IndexOf('\"', 1) + 1);
                                atomString = temp;
                                isAtom = true;
                                break;
                            }
                        }
                        else
                        {
                            // 如果含有奇数个引号
                            if (!atomString.Equals("\""))
                            {
                                string tempAtomStr = atomString.Substring(1);
                                if (!isQuoteAdjacent(tempAtomStr))
                                {
                                    // 这里做的原因是，如果判断前面的字符串不是原子字符串的时候就读取第一个取到的字符串
                                    // 后面取到的字符串不计入该原子字符串
                                    tempAtomStr = atomString.Substring(1);
                                    int tempQutoIndex = tempAtomStr.IndexOf("\"");
                                    // 这里既然有奇数个quto，所以第二个quto肯定不是最后一个
                                    tempAtomStr = tempAtomStr.Substring(0, tempQutoIndex)
                                            + tempAtomStr.Substring(tempQutoIndex + 1);
                                    atomString = tempAtomStr;
                                    isAtom = true;
                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }
        // 先去掉之前读取的原字符串的母字符串
        if (lineStr.Length > orgString.Length)
        {
            lineStr = lineStr.Substring(orgString.Length);
        }
        else
        {
            lineStr = "";
        }
        // 去掉之后，判断是否以逗号开始，如果以逗号开始则去掉逗号
        if (lineStr.StartsWith(","))
        {
            if (lineStr.Length > 1)
            {
                lineStr = lineStr.Substring(1);
            }
            else
            {
                lineStr = "";
            }
        }
        ret[0] = atomString;
        ret[1] = lineStr;
        return ret;
    }

    private bool readCSVNextRecord()
    {
        // 如果流未被初始化则返回false
        if (inStream == null)
        {
            return false;
        }
        // 如果结果向量未被初始化，则初始化
        if (vContent == null)
        {
            vContent = new List<string>();
        }
        // 移除向量中以前的元素
        vContent.Clear();
        // 声明逻辑行
        string logicLineStr = "";
        // 用于存放读到的行
        StringBuilder strb = new StringBuilder();
        // 声明是否为逻辑行的标志，初始化为false
        bool isLogicLine = false;
        while (!isLogicLine)
        {
            string newLineStr = inStream.ReadLine();
            if (newLineStr == null)
            {
                strb = null;
                vContent = null;
                isLogicLine = true;
                break;
            }
            if (newLineStr.StartsWith("#"))
            {
                // 去掉注释
                continue;
            }
            if (!strb.ToString().Equals(""))
            {
                strb.Append("\r\n");
            }
            strb.Append(newLineStr);
            string oldLineStr = strb.ToString();
            if (oldLineStr.IndexOf(",") == -1)
            {
                // 如果该行未包含逗号
                if (containsNumber(oldLineStr, "\"") % 2 == 0)
                {
                    // 如果包含偶数个引号
                    isLogicLine = true;
                    break;
                }
                else
                {
                    if (oldLineStr.StartsWith("\""))
                    {
                        if (oldLineStr.Equals("\""))
                        {
                            continue;
                        }
                        else
                        {
                            string tempOldStr = oldLineStr.Substring(1);
                            if (isQuoteAdjacent(tempOldStr))
                            {
                                // 如果剩下的引号两两相邻，则不是一行
                                continue;
                            }
                            else
                            {
                                // 否则就是一行
                                isLogicLine = true;
                                break;
                            }
                        }
                    }
                }
            }
            else
            {
                // quotes表示复数的quote
                string tempOldLineStr = oldLineStr.Replace("\"\"", "");
                int lastQuoteIndex = tempOldLineStr.LastIndexOf("\"");
                if (lastQuoteIndex == 0)
                {
                    continue;
                }
                else if (lastQuoteIndex == -1)
                {
                    isLogicLine = true;
                    break;
                }
                else
                {
                    tempOldLineStr = tempOldLineStr.Replace("\",\"", "");
                    lastQuoteIndex = tempOldLineStr.LastIndexOf("\"");
                    if (lastQuoteIndex == 0)
                    {
                        continue;
                    }
                    if (tempOldLineStr[lastQuoteIndex - 1] == ',')
                    {
                        continue;
                    }
                    else
                    {
                        isLogicLine = true;
                        break;
                    }
                }
            }
        }
        if (strb == null)
        {
            // 读到行尾时为返回
            return false;
        }
        // 提取逻辑行
        logicLineStr = strb.ToString();
        if (logicLineStr != null)
        {
            // 拆分逻辑行，把分离出来的原子字符串放入向量中
            while (!logicLineStr.Equals(""))
            {
                string[] ret = readAtomString(logicLineStr);
                string atomString = ret[0];
                logicLineStr = ret[1];
                vContent.Add(atomString);
            }
        }
        return true;
    }

    private List<string> getLineContentVector()
    {
        if (this.readCSVNextRecord())
        {
            return this.vContent;
        }
        return null;
    }

    private List<string> getVContent()
    {
        return this.vContent;
    }

    public int GetRow()
    {
        if (null == table)
        {
            throw new System.Exception("table尚未初始化,请检查是否成功读取");
        }
        return table.Count;
    }

    public int GetCol()
    {
        if (null == table)
        {
            throw new System.Exception("table尚未初始化,请检查是否成功读取");
        }
        if (table.Count == 0)
        {
            throw new System.Exception("table内容为空");
        }
        return table[0].Count;
    }

    public int GetFirstIndexAtCol(string str, int col)
    {
        if (null == table)
        {
            throw new System.Exception("table尚未初始化,请检查是否成功读取");
        }
        if (table.Count == 0)
        {
            throw new System.Exception("table内容为空");
        }
        if (col >= table[0].Count)
        {
            throw new System.Exception("参数错误：col大于最大行");
        }
        for (int i = 0; i < table.Count; ++i)
        {
            if (table[i][col].Equals(str))
            {
                return i;
            }
        }
        return -1;
    }

    public int GetFirstIndexAtRow(string str, int row)
    {
        if (null == table)
        {
            throw new System.Exception("table尚未初始化,请检查是否成功读取");
        }
        if (table.Count == 0)
        {
            throw new System.Exception("table内容为空");
        }
        if (row >= table.Count)
        {
            throw new System.Exception("参数错误：cow大于最大列");
        }
        int tempCount = table[0].Count;
        for (int i = 0; i < tempCount; ++i)
        {
            if (table[row][i].Equals(str))
            {
                return i;
            }
        }
        return -1;
    }

    public int[] GetIndexsAtCol(string str, int col)
    {
        if (null == table)
        {
            throw new System.Exception("table尚未初始化,请检查是否成功读取");
        }
        if (table.Count == 0)
        {
            throw new System.Exception("table内容为空");
        }
        if (col >= table[0].Count)
        {
            throw new System.Exception("参数错误：col大于最大行");
        }
        List<int> tempList = new List<int>();
        for (int i = 0; i < table.Count; ++i)
        {
            if (table[i][col].Equals(str))
            {
                // 增加
                tempList.Add(i);
            }
        }
        return tempList.ToArray();
    }

    public int[] GetIndexsAtRow(string str, int row)
    {
        if (null == table)
        {
            throw new System.Exception("table尚未初始化,请检查是否成功读取");
        }
        if (table.Count == 0)
        {
            throw new System.Exception("table内容为空");
        }
        if (row >= table.Count)
        {
            throw new System.Exception("参数错误：cow大于最大列");
        }
        int tempCount = table[0].Count;
        List<int> tempList = new List<int>();
        for (int i = 0; i < tempCount; ++i)
        {
            if (table[row][i].Equals(str))
            {
                tempList.Add(i);
            }
        }
        return tempList.ToArray();
    }

    public string GetValueAt(int col, int row)
    {
        if (null == table)
        {
            throw new System.Exception("table尚未初始化,请检查是否成功读取");
        }
        if (table.Count == 0)
        {
            throw new System.Exception("table内容为空");
        }
        if (row >= table.Count)
        {
            throw new System.Exception("参数错误：row大于最大列");
        }
        if (col >= table[0].Count)
        {
            throw new System.Exception("参数错误：col大于最大行");
        }
        return table[row][col];
    }

}