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
using System.Data;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CommonClass
{
    public class LibData
    {
        string newLine = System.Environment.NewLine;

        /// <summary>
        /// DataSetをファイルに保存する。
        /// </summary>
        /// <param name="ds">DataSet</param>
        /// <param name="filePath">保存先のパス</param>
        /// <returns>bool | true:成功   false:失敗</returns>
        public bool DataSetSave(DataSet ds, string filePath)
        {
            try
            {
                // DataSetをXML形式で保存します。
                ds.WriteXml(filePath);
                return true;
            }
            catch (Exception)
            {
                // 何らかのエラーが発生した場合は、falseを返します。
                return false;
            }
        }

        /// <summary>
        /// DataSetをXMLファイルから読み込む。
        /// </summary>
        /// <param name="filePath">読み込むXMLファイルのパス</param>
        /// <returns>DataSet | XMLファイルから読み込んだDataSet   null;失敗</returns>
        public DataSet DataSetLoad(string filePath)
        {
            DataSet ds = new DataSet();

            try
            {
                ds.ReadXml(filePath);
            }
            catch (System.IO.FileNotFoundException)
            {
                ds = null;

                MessageBox.Show("指定されたファイルが見つかりません。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (System.Xml.XmlException)
            {
                ds = null;

                MessageBox.Show("XMLの形式が正しくありません。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                ds = null;

                MessageBox.Show("予期しないエラーが発生しました: " + ex.Message, "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return ds;
        }

        /// <summary>
        /// 指定されたDataSetに 指定されたDataTableを挿入したDataSetを返します。
        /// 挿入する位置はidxで指定します。
        /// </summary>
        /// <param name="ds">DataSet [in]</param>
        /// <param name="dt">DataTable [in]</param>
        /// <param name="idx">挿入する位置 [in]</param>
        /// <returns>DataSet | DataTable挿入後のDataSet   失敗：null</returns>
        public DataSet DataSetInsertAt(DataSet ds, DataTable dt, int idx)
        {
            try
            {
                // 新しいDataSetを作成します。
                DataSet newDs = new DataSet();

                // 指定された位置までのテーブルを新しいDataSetに追加します。
                for (int i = 0; i < idx; i++)
                {
                    newDs.Tables.Add(ds.Tables[i].Copy());
                }

                // 指定されたDataTableを新しいDataSetに追加します。
                newDs.Tables.Add(dt.Copy());

                // 残りのテーブルを新しいDataSetに追加します。
                for (int i = idx; i < ds.Tables.Count; i++)
                {
                    newDs.Tables.Add(ds.Tables[i].Copy());
                }

                // 新しいDataSetを返します。
                return newDs;
            }
            catch(Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// 指定されたDataSetの 指定されたインデックスのテーブルを １つ上のインデックスに移動する。
        /// </summary>
        /// <param name="ds">DataSet</param>
        /// <param name="idx">移動するテーブルのインデックス</param>
        public void DataSetTableUp(DataSet ds, int idx)
        {
            if (ds != null 
                && ds.Tables.Count > idx 
                && idx > 0)
            {
                // テーブルを一時的なリストにコピーします。
                List<DataTable> tables = new List<DataTable>();

                foreach (DataTable dt in ds.Tables)
                {
                    tables.Add(dt);
                }

                // リスト内でテーブルの位置を変更します。
                DataTable temp = tables[idx];
                tables[idx] = tables[idx - 1];
                tables[idx - 1] = temp;

                // DataSetのテーブルをクリアします。
                ds.Tables.Clear();

                // リストのテーブルをDataSetに戻します。
                foreach (DataTable dt in tables)
                {
                    ds.Tables.Add(dt);
                }
            }
        }

        /// <summary>
        /// 指定されたDataSetの 指定されたインデックスのテーブルを １つ下のインデックスに移動する。
        /// </summary>
        /// <param name="ds">DataSet</param>
        /// <param name="idx">移動するテーブルのインデックス</param>
        public void DataSetTableDown(DataSet ds, int idx)
        {
            if (ds != null
                && 0 <= idx
                && idx < ds.Tables.Count - 1)
            {
                // テーブルを一時的なリストにコピーします。
                List<DataTable> tables = new List<DataTable>();

                foreach (DataTable dt in ds.Tables)
                {
                    tables.Add(dt);
                }

                // リスト内でテーブルの位置を変更します。
                DataTable temp = tables[idx];
                tables[idx] = tables[idx + 1];
                tables[idx + 1] = temp;

                // DataSetのテーブルをクリアします。
                ds.Tables.Clear();

                // リストのテーブルをDataSetに戻します。
                foreach (DataTable dt in tables)
                {
                    ds.Tables.Add(dt);
                }
            }
        }

        /// <summary>
        /// 2つのDataTableの内容を足したものを返す。
        /// ※dt1とdt2の列は同じものとする。
        /// ※dt1の最後の行にdt2の全行を足したものを生成して返す。
        /// </summary>
        /// <param name="dt1">DataTable1</param>
        /// <param name="dt2">DataTable2</param>
        /// <returns>DataTable | 足した後のDataTable</returns>
        public DataTable DataTablePlus(DataTable dt1, DataTable dt2)
        {
            // dt1のクローンを作成します。これにより、スキーマがコピーされますが、行はコピーされません。
            DataTable result = dt1.Clone();

            // dt1の全ての行を新しいDataTableに追加します。
            foreach (DataRow row in dt1.Rows)
            {
                result.ImportRow(row);
            }

            // dt2の全ての行を新しいDataTableに追加します。
            foreach (DataRow row in dt2.Rows)
            {
                result.ImportRow(row);
            }

            // 結果のDataTableを返します。
            return result;
        }

        /// <summary>
        /// DataTableの内容を文字列に変換する。
        /// </summary>
        /// <param name="dt">文字列に変換するDataTable</param>
        /// <returns>string | 変換後の文字列</returns>
        public string DataTableToString(DataTable dt)
        {
            string ret = "";

            string cols = "";  // 列名の文字列

            // 列名の文字列作成
            for (int c = 0; c < dt.Columns.Count; c++)
            {
                cols += dt.Columns[c].ColumnName;

                if (c < dt.Columns.Count - 1)
                {
                    cols += ", ";
                }
            }

            // 列名表示
            ret += cols + newLine;

            // 全行の内容表示
            for (int r = 0; r < dt.Rows.Count; r++)
            {
                string row = "";  // 1行の文字列

                // 1行の文字列作成
                for (int c = 0; c < dt.Columns.Count; c++)
                {
                    row += "" + dt.Rows[r][c];

                    if (c < dt.Columns.Count - 1)
                    {
                        row += ", ";
                    }
                }

                // 1行の内容表示
                ret += row + newLine;
            }

            return ret;
        }

        /// <summary>
        /// 指定されたDataTableの 指定された列から searchWordを検索して 最初に見つかったrowIndexを返す。
        /// </summary>
        /// <param name="dt">検索するDataTable</param>
        /// <param name="colName">検索する列名</param>
        /// <param name="searchWord">検索ワード</param>
        /// <returns>int | 最初に見つかったrowIndex   -1:見つからなかった</returns>
        public int DataTableGetRowIndex(DataTable dt, string colName, string searchWord)
        {
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                if (dt.Rows[i][colName].ToString() == searchWord)
                {
                    return i;
                }
            }

            return -1; // 見つからなかった場合
        }

        /// <summary>
        /// 指定されたDataTableの 指定された位置に 新しい行を挿入する。
        /// </summary>
        /// <param name="dt">DataTable</param>
        /// <param name="idx">行を挿入する位置（0の場合は、先頭に追加）</param>
        public void DataTableInsertAtNewRow(DataTable dt, int idx)
        {
            // 新しい行を作成
            DataRow newRow = dt.NewRow();

            // 指定された位置に新しい行を挿入
            dt.Rows.InsertAt(newRow, idx);
        }

        /// <summary>
        /// 指定されたDataTableの 指定された行を １つ上に移動する。
        /// </summary>
        /// <param name="dt">DataTable</param>
        /// <param name="rowIdx">行のインデックス</param>
        public void DataTableRowUp(DataTable dt, int rowIdx)
        {
            if (rowIdx <= 0) // If it's already the first row, return
            {
                return;
            }

            DataRow row = dt.Rows[rowIdx];
            DataRow newRow = dt.NewRow();
            newRow.ItemArray = row.ItemArray; // Copy data

            dt.Rows.Remove(row);
            dt.Rows.InsertAt(newRow, rowIdx - 1);
        }

        /// <summary>
        /// 指定されたDataTableの 指定された行を １つ下に移動する。
        /// </summary>
        /// <param name="dt">DataTable</param>
        /// <param name="rowIdx">行のインデックス</param>
        public void DataTableRowDown(DataTable dt, int rowIdx)
        {
            if (rowIdx >= dt.Rows.Count - 1)
            {
                return;
            }

            DataRow row = dt.Rows[rowIdx];
            DataRow newRow = dt.NewRow();
            newRow.ItemArray = row.ItemArray; // Copy data

            dt.Rows.Remove(row);
            dt.Rows.InsertAt(newRow, rowIdx + 1);
        }

        /// <summary>
        /// 指定されたDataGridViewの前列をソート不可にする。
        /// </summary>
        /// <param name="dataGridView">DataGridView</param>
        public void DataGridViewNotSortable(DataGridView dataGridView)
        {
            foreach (DataGridViewColumn column in dataGridView.Columns)
            {
                column.SortMode = DataGridViewColumnSortMode.NotSortable;
            }
        }

        /// <summary>
        /// 指定されたDataGridViewに DataTableをセットします。
        /// DataGridViewは前列ソート不可にします。
        /// セルの大きさはAutoResizeします。
        /// </summary>
        /// <param name="dataGridView">DataGridView</param>
        /// <param name="dt">DataTable</param>
        public void DataGridViewSetDataTable(DataGridView dataGridView, DataTable dt)
        {
            dataGridView.DefaultCellStyle.WrapMode = DataGridViewTriState.False;

            dataGridView.DataSource = dt;

            DataGridViewNotSortable(dataGridView);

            dataGridView.AutoResizeColumns();

            dataGridView.DefaultCellStyle.WrapMode = DataGridViewTriState.True;

            dataGridView.AutoResizeRows();
        }
    }
}
