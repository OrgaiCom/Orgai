/*
    The MIT License

    Copyright(c) 2024 OrgaiCom

    以下に定める条件に従い、本ソフトウェアおよび関連文書のファイル（以下「ソフトウェア」）
    の複製を取得するすべての人に対し、ソフトウェアを無制限に扱うことを無償で許可します。
    これには、ソフトウェアの複製を使用、複写、変更、結合、掲載、頒布、サブライセンス、
    および/または販売する権利、およびソフトウェアを提供する相手に同じことを許可する権利も
    無制限に含まれます。

    上記の著作権表示および本許諾表示を、ソフトウェアのすべての複製または重要な部分に
    記載するものとします。

    ソフトウェアは「現状のまま」で、明示であるか暗黙であるかを問わず、何らの保証もなく
    提供されます。ここでいう保証とは、商品性、特定の目的への適合性、および権利非侵害に
    ついての保証も含みますが、それに限定されるものではありません。 作者または著作権者は、
    契約行為、不法行為、またはそれ以外であろうと、ソフトウェアに起因または関連し、
    あるいはソフトウェアの使用またはその他の扱いによって生じる一切の請求、損害、
    その他の義務について何らの責任も負わないものとします。
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualBasic.FileIO;
using System.Windows.Forms;

namespace CommonClass
{
    public class LibCsv
    {
        /// <summary>
        /// CSVを読み込みDataTableとして返す
        /// </summary>
        /// <param name="csvFilePath">CSVファイルのパス</param>
        /// <param name="isHeader">true:ヘッダあり   false:なし</param>
        /// <param name="limit">行数の限界値（デフォルト：long.MaxValue）</param>
        /// <param name="enccodeName">エンコード（デフォルト："shift-jis"）</param>
        /// <param name="delimiter">区切り文字（デフォルト：自動判定）</param>
        /// <returns>DataTable | CSVの内容を読み込んだDataTable   失敗：null</returns>
        public DataTable Read(string csvFilePath, bool isHeader, long limit = long.MaxValue, string encodeName = "shift-jis", string delimiter = "")
        {
            if (File.Exists(csvFilePath) == false)
            {
                MessageBox.Show(csvFilePath + " ファイルが存在しないため、読み込みに失敗しました。");

                return null;
            }

            limit += (isHeader == true && limit < long.MaxValue) ? 1 : 0;
            long count = 0;

            var enc = Encoding.GetEncoding(encodeName);
            delimiter = (delimiter == "") ? GetDelimiter(csvFilePath, enc) : delimiter;

            DataTable dt = new DataTable();

            using (TextFieldParser parser = new TextFieldParser(csvFilePath, enc) { TextFieldType = FieldType.Delimited })
            {
                parser.Delimiters = new string[] { delimiter };

                while (!parser.EndOfData)
                {
                    var fields = parser.ReadFields();

                    if (count++ == 0)
                    {
                        //ヘッダがある場合、1行目のデータで列を追加
                        dt.Columns.AddRange(fields.Select(i => (isHeader) ? new DataColumn(i) : new DataColumn()).ToArray());
                        if (isHeader)
                        {
                            continue;
                        }
                    }

                    if (fields.Length > dt.Columns.Count)
                    {
                        dt.Columns.AddRange(Enumerable.Range(0, fields.Length - dt.Columns.Count).Select(i => new DataColumn()).ToArray());
                    }

                    if (count > limit)
                    {
                        break;
                    }

                    DataRow dr = dt.NewRow();
                    Enumerable.Range(0, fields.Length).Select(i => dr[i] = fields[i]).ToArray();
                    dt.Rows.Add(dr);
                }
            }

            return dt;
        }

        /// <summary>
        /// DataTableの内容をCSV形式でファイルに出力する
        /// </summary>
        /// <param name="dt">DataTable</param>
        /// <param name="csvFilePath">CSVファイルのパス</param>
        /// <param name="writeHeader">true:ヘッダあり   false:なし</param>
        /// <param name="delimiter">区切り文字（デフォルト：","）</param>
        /// <param name="encodeName">エンコード（デフォルト："shift-jis"）</param>
        /// <param name="isAppend">true:追記する   false:上書きする（デフォルト）</param>
        public void Write(DataTable dt, string csvFilePath, bool writeHeader, string delimiter = ",", string encodeName = "shift-jis", bool isAppend = false)
        {
            bool header = (writeHeader && (isAppend == false || (isAppend == true && File.Exists(csvFilePath) == false)));
            //書き込むファイルを開く
            using (StreamWriter sw = new StreamWriter(csvFilePath, isAppend, Encoding.GetEncoding(encodeName)))
            {
                //ヘッダを書き込む
                if (header)
                {
                    string[] headers = dt.Columns.Cast<DataColumn>().Select(i => enclose_ifneed(i.ColumnName)).ToArray();
                    sw.WriteLine(String.Join(delimiter, headers));
                }

                //レコードを書き込む
                foreach (DataRow dr in dt.Rows)
                {
                    string[] fields = Enumerable.Range(0, dt.Columns.Count).Select(i => enclose_ifneed(dr[i].ToString())).ToArray();
                    sw.WriteLine(String.Join(delimiter, fields));
                }
            }
        }

        /// 必要ならば、文字列をダブルクォートで囲む
        private string enclose_ifneed(string p_field)
        {
            //ダブルクォートで括る必要があるかを確認
            if (p_field.Contains('"') || p_field.Contains(',') || p_field.Contains('\r') || p_field.Contains('\n') ||
                 p_field.StartsWith(" ") || p_field.StartsWith("\t") || p_field.EndsWith(" ") || p_field.EndsWith("\t"))
            {
                //ダブルクォートが含まれていたら２つ重ねて、前後にダブルクォートを付加
                return (p_field.Contains('"')) ? ("\"" + p_field.Replace("\"", "\"\"") + "\"") : ("\"" + p_field + "\"");
            }
            else
            {
                //何もせずそのまま返す
                return p_field;
            }
        }

        /// <summary>
        /// 区切り文字の自動判定
        /// </summary>
        /// <param name="filePath">判定するファイルのパス</param>
        /// <param name="encodeName">エンコード</param>
        /// <returns>string | 区切り文字（"," or "\t"）</returns>
        public string GetDelimiter(string filePath, Encoding encodeName)
        {
            using (StreamReader sr = new StreamReader(filePath, encodeName))
            {
                string line = sr.ReadLine();
                return (line.Split(',').Length > line.Split('\t').Length) ? "," : "\t";
            }
        }
    }
}
