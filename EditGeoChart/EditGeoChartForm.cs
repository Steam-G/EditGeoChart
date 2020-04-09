using LAS_file;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ZedGraph;

namespace EditGeoChart
{
    public partial class EditGeoChartForm : Form
    {
        LAS_reader lAS_Reader = new LAS_reader();

        public EditGeoChartForm()
        {
            InitializeComponent();
            LoadDefaultLas();
        }
        #region События меню

        private void оПрограммеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutForm about = new AboutForm();
            about.ShowDialog();
        }

        private void btOpenFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Filter = "LAS files|*.las";
            if (fileDialog.ShowDialog() != DialogResult.OK)
                return;

            lAS_Reader.LoadFile(fileDialog.FileName);
            DrawGraph();
            PaintTable();
        }

        #endregion

        #region Работа с таблицей
        BindingSource bindingSource;

        private void PaintTable()
        {
            bindingSource = new BindingSource();
            bindingSource.DataSource = lAS_Reader.dataValue;
            dataGridView2.AutoGenerateColumns = true;
            dataGridView2.DataSource = bindingSource;
            paramTableLoad(dataGridView1, lAS_Reader);
            dgvload(dataGridView2, lAS_Reader);
        }

        //Таблица со списком парамтеров и галочками
        private void paramTableLoad(DataGridView dataGrid, LAS_reader reader)
        {
            BindingSource bindSource = new BindingSource();
            DataGridView dgv = new DataGridView();
            DataTable table = new DataTable();
            //table.Columns.Add("qew", typeof(DataGridViewCheckBoxColumn));

            DataGridViewCheckBoxColumn checkColumn = new DataGridViewCheckBoxColumn();
            checkColumn.Name = "X";
            checkColumn.HeaderText = "X";
            checkColumn.Width = 50;
            checkColumn.ReadOnly = false;
            checkColumn.FillWeight = 10; //if the datagridview is resized (on form resize) the checkbox won't take up too much; value is relative to the other columns' fill values

            dgv.Columns.Add(checkColumn);

            // Создаем ячейку типа CheckBox
            //DataGridViewCheckBoxCell checkCell = new DataGridViewCheckBoxCell();
            //checkCell.Value = true;

            dgv.Columns.Add("param", "parameters");
            //c.AutoIncrement = true;
            //c.AutoIncrementSeed = 0;
            //c.AutoIncrementStep = 1;

            table.Columns.Add(new DataColumn("Selected", typeof(bool)));
            table.Columns.Add(new DataColumn("Text", typeof(string)));
            for (int i = 0; i < reader.CurveInfo_Count; i++)
            {
                table.Rows.Add(table.NewRow());

                //table.Columns.Add(reader.CurveInfo[i]._name, typeof(string));
                //dgv.Rows[i].Cells[1].Value = (string)reader.CurveInfo[i]._name;
                table.Rows[i][1] = (string)reader.CurveInfo[i]._name;
            }

            bindSource.DataSource = dgv;
            bindSource.DataSource = table;
            dataGrid.DataSource = bindSource;
        }

        //работа с таблицей
        private void dgvload(DataGridView dataGrid, LAS_reader reader)
        {
            DataTable table = new DataTable();
            BindingSource bindingSource = new BindingSource();

            DataColumn c = table.Columns.Add("Ключ", typeof(String));
            c.AutoIncrement = true;
            c.AutoIncrementSeed = 0;
            c.AutoIncrementStep = 1;

            for (int i = 0; i < reader.CurveInfo_Count; i++)
            {
                table.Columns.Add(reader.CurveInfo[i]._name, typeof(string));
            }

            table.PrimaryKey = new DataColumn[] { table.Columns[0] };

            int length1 = reader.CurveInfo_Count;
            int length2 = reader.dataValue.Length;
            //for (int i = 0; i < length1; i++)
            //    table.Columns.Add();

            // заполняем таблицу
            for (int i = 0; i < length2; i++)
            {
                table.Rows.Add(table.NewRow());
                for (int j = 0; j < length1; j++)
                {
                    if (j == 1)
                    {
                        //перевожу Unix Timestamp в представление типа DateTime
                        DateTime dateTime = ConvertFromUnixTimestamp(double.Parse(reader.dataValue[i]._dataValue[j], CultureInfo.InvariantCulture));
                        //теперь привожу полученную переменную даты и времени в текст, вида "дд.мм.гггг чч:мм:сс"
                        string sDT = dateTime.ToString("G", CultureInfo.CreateSpecificCulture("de-DE"));
                        //кладу полученное значение в таблицу
                        table.Rows[i][j + 1] = sDT;
                        continue;
                    }
                    table.Rows[i][j + 1] = (string)reader.dataValue[i]._dataValue[j];
                }
            }

            bindingSource.DataSource = table;
            dataGrid.DataSource = bindingSource;
        }

        //Конвертирование Unix Timestamp в DateTime
        static DateTime ConvertFromUnixTimestamp(double timestamp)
        {
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return origin.AddSeconds(timestamp);
        }

        //Обратное конвертирование DateTime в Unix Timestamp
        static double ConvertToUnixTimestamp(DateTime date)
        {
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            TimeSpan diff = date - origin;
            return Math.Floor(diff.TotalSeconds);
        }
        #endregion


        #region Рисование графика (ZedGraph)

        /// <summary>
        /// Бралось как пример для рисование в стандартном компоненте Chart
        /// </summary>
        private void PaintChart()
        {
            PointPairList list = new PointPairList();
            int length2 = lAS_Reader.dataValue.Length;
            for (int i = 0; i < length2; i++)
            {
                //chart1.Series[0].Points.AddXY(double.Parse(lAS_Reader.dataValue[i]._dataValue[3], CultureInfo.InvariantCulture), i); // 
                //chart1.Series[1].Points.AddXY(double.Parse(lAS_Reader.dataValue[i]._dataValue[2], CultureInfo.InvariantCulture), i); //
                //chart1.Series[2].Points.AddXY(double.Parse(lAS_Reader.dataValue[i]._dataValue[4], CultureInfo.InvariantCulture), i); //

                list.Add(double.Parse(lAS_Reader.dataValue[i]._dataValue[3], CultureInfo.InvariantCulture), i);

            }
            //chart1.Series[0].Points.AddXY(0, 5);
            //chart1.Series[0].Points.AddXY(1, 7);
            //chart1.Series[0].Points.AddXY(2, 2);
            //chart1.Series[0].Points.AddXY(3, 6);
        }


        private PointPairList PointPair(LAS_reader las_doc, int n_las_param)
        {
            PointPairList _pointPair = new PointPairList();

            int length2 = las_doc.dataValue.Length;
            for (int i = 0; i < length2; i++)
            {
                _pointPair.Add(double.Parse(lAS_Reader.dataValue[i]._dataValue[n_las_param], CultureInfo.InvariantCulture), i);
            }

            return _pointPair;
        }

        private void DrawGraph()
        {
            GraphPane pane = zedGraph_1.GraphPane;
            pane.CurveList.Clear();

            // !!! С помощью этого свойства указываем, что шрифты не надо масштабировать
            // при изменении размера компонента.
            pane.IsFontsScaled = false;

            PointPairList list = new PointPairList();

            //double xmin = -50;
            //double xmax = 50;
            //
            // Заполняем список точек
            //for (double x = xmin; x <= xmax; x += 0.01)
            //{
            //    // добавим в список точку, но меняем местами координаты
            //    list.Add(f(x), x);
            //}
            list = PointPair(lAS_Reader, 3);
            LineItem myCurve = pane.AddCurve("Sinc", list, Color.Blue, SymbolType.None);

            list = PointPair(lAS_Reader, 4);
            LineItem myCurve2 = pane.AddCurve("Sinc", list, Color.Red, SymbolType.None);

            // !!!
            // Теперь линия по нулевому уровню должна быть перпендикулярна оси X
            pane.XAxis.MajorGrid.IsZeroLine = true;

            // !!!
            // Линию по нулевому уровню, перпендикулярную оси Y отключаем
            pane.YAxis.MajorGrid.IsZeroLine = false;


            // !!!
            // Поменяем названия осей, чтобы еще больше запутать противника :)
            pane.XAxis.Title.Text = "YAxis";
            pane.YAxis.Title.Text = "XAxis";

            // Вызываем метод AxisChange (), чтобы обновить данные об осях. 
            // В противном случае на рисунке будет показана только часть графика, 
            // которая умещается в интервалы по осям, установленные по умолчанию
            zedGraph_1.AxisChange();

            zedGraph_1.Invalidate();
        }
        #endregion

        #region Для отладки
        private void LoadDefaultLas()
        {
            var appDir = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);// + "\\logTelLeuza.txt";

            //var baseFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments); // путь к Моим Документах
            // или
            //var baseFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData); // путь к папке AppData
            //var appStorageFolder = Path.Combine(baseFolder, "Имя Вашей Программы");

            var relativePath = @"..\..\assets\las\gti.las";
            var fullPath = Path.Combine(appDir, relativePath);

            lAS_Reader.LoadFile(fullPath);
            DrawGraph();
            PaintTable();
        }


        #endregion

        private void button1_Click(object sender, EventArgs e)
        {

        }
    }
}
