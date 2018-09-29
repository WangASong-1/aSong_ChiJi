using UnityEngine;
using UnityEditor;
using System.Data;
using System.Runtime.InteropServices;
using System.IO;
using Excel;
using LitJson;
using System.Text;

public class Editor_ExcelToJson : EditorWindow
{
    [MenuItem("GameObject/aSongEditor/ExcelToJson")]
    static void AddWindow()
    {
        //创建窗口
        Rect wr = new Rect(0, 0, 500, 500);
        Editor_ExcelToJson window = (Editor_ExcelToJson)EditorWindow.GetWindowWithRect(typeof(Editor_ExcelToJson), wr, true, "ExcelToJson");
        window.Show();
    }

    private void OnGUI()
    {
        if (GUILayout.Button("excel转换为json", GUILayout.Height(200), GUILayout.Width(200)))
        {
            OpenFileWin();
        }
    }

    /// <summary>
    /// 只读Excel方法
    /// </summary>
    /// <param name="ExcelPath"></param>
    /// <returns></returns>
    public void GameReadExcel(string ExcelPath)
    {
        var file = new FileInfo(ExcelPath);
        using (var stream = File.Open(ExcelPath, FileMode.Open, FileAccess.Read))
        {
            IExcelDataReader reader = null;
            if (file.Extension == ".xls")
            {
                reader = ExcelReaderFactory.CreateBinaryReader(stream);

            }
            else if (file.Extension == ".xlsx")
            {
                reader = ExcelReaderFactory.CreateOpenXmlReader(stream);
            }
            if (reader == null)
                return;

            DataSet result = reader.AsDataSet();



            //读完excel数据后,转换为json
            if (result != null)
            {
                Debug.Log("列 = " + result.Tables[0].Columns.Count);
                Debug.Log("行 = " + result.Tables[0].Rows.Count);
            }
            int columns = result.Tables[0].Columns.Count;//获取列数
            int rows = result.Tables[0].Rows.Count;//获取行数

            StringBuilder mStringBuilder = new StringBuilder();
            JsonWriter mJsonWriter = new JsonWriter(mStringBuilder);
            mJsonWriter.WriteObjectStart();
            var table = result.Tables[0];
            //从第二行开始读
            for (int i = 2; i < rows; i++)
            {
                mJsonWriter.WritePropertyName(table.Rows[i][0].ToString());

                mJsonWriter.WriteObjectStart();
                for (int j = 1; j < columns; j++)
                {
                    mJsonWriter.WritePropertyName(table.Rows[1][j].ToString());
                    mJsonWriter.Write(table.Rows[i][j].ToString());
                    string nvalue = table.Rows[i][j].ToString();
                }
                mJsonWriter.WriteObjectEnd();
            }
            mJsonWriter.WriteObjectEnd();
            SaveFileWin(mStringBuilder.ToString());
            //Debug.Log(mJsonWriter.ToString());
            //Debug.Log(mStringBuilder.ToString());

        }
    }

    public void OpenFileWin()
    {
        WindowsOpenFile wof = new WindowsOpenFile();

        wof.structSize = Marshal.SizeOf(wof);

        //wof.filter = "All Files\0*.*\0\0";
        wof.filter = "excel文件 (*.xlsx; *.xls)\0 *.xlsx; *.xls\0\0";

        wof.file = new string(new char[256]);

        wof.maxFile = wof.file.Length;

        wof.fileTitle = new string(new char[64]);

        wof.maxFileTitle = wof.fileTitle.Length;
        string path = Application.streamingAssetsPath;
        path = path.Replace('/', '\\');
        //默认路径  
        wof.initialDir = path;
        //ofn.initialDir = "D:\\MyProject\\UnityOpenCV\\Assets\\StreamingAssets";  
        wof.title = "Open Project";

        wof.defExt = "xls";//显示文件的类型  
        //注意 一下项目不一定要全选 但是0x00000008项不要缺少  
        wof.flags = 0x00080000 | 0x00001000 | 0x00000800 | 0x00000200 | 0x00000008;//OFN_EXPLORER|OFN_FILEMUSTEXIST|OFN_PATHMUSTEXIST| OFN_ALLOWMULTISELECT|OFN_NOCHANGEDIR  

        if (WindowDll.GetOpenFileName(wof))
        {
            Debug.Log("Selected file with full path: {0} = " + wof.file);
            GameReadExcel(wof.file);
        }
    }

    public void SaveFileWin(string fileContent)
    {
        WindowsOpenFile wof = new WindowsOpenFile();

        wof.structSize = Marshal.SizeOf(wof);

        wof.filter = "All Files\0*.*\0\0";

        wof.file = new string(new char[256]);

        wof.maxFile = wof.file.Length;

        wof.fileTitle = new string(new char[64]);

        wof.maxFileTitle = wof.fileTitle.Length;
        string path = Application.streamingAssetsPath;
        path = path.Replace('/', '\\');
        //默认路径  
        wof.initialDir = path;
        //ofn.initialDir = "D:\\MyProject\\UnityOpenCV\\Assets\\StreamingAssets";  
        wof.title = "Open Project";

        wof.defExt = "json";//显示文件的类型  
        //注意 一下项目不一定要全选 但是0x00000008项不要缺少  
        wof.flags = 0x00080000 | 0x00001000 | 0x00000800 | 0x00000200 | 0x00000008;
        //OFN_EXPLORER|OFN_FILEMUSTEXIST|OFN_PATHMUSTEXIST| OFN_ALLOWMULTISELECT|OFN_NOCHANGEDIR  
       
        if (WindowDll.GetSaveFileName(wof))
        {
            Debug.Log("SaveFileWin = " + wof.file);
            FileStream fileStream = new FileStream(wof.file, FileMode.Create);
            byte[] bts = new UTF8Encoding().GetBytes(fileContent);
            fileStream.Write(bts, 0, bts.Length);
            fileStream.Close();
            //GameReadExcel(wof.file);
        }
    }
}
