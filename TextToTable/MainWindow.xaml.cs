using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;
using TextBox = System.Windows.Controls.TextBox;

namespace TextToTable
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }


        private DataTable GenerateDataTableFromFolder(string folder)
        {
            DataTable ret = new DataTable();

            ret.Columns.Add("Date",typeof(string));
            ret.Columns.Add("Type", typeof(string));
            ret.Columns.Add("Description", typeof(string));
            ret.Columns.Add("Amount", typeof(string));
            ret.Columns.Add("Purpose", typeof(string));

            var files = Directory.GetFiles(folder).Where(k => k.EndsWith("txt"));
            foreach (var i in files)
            {
                String text = File.ReadAllText(i);

                var lines = text.Split('\n');
                var trim = lines.Select(k => k.Replace('\r', ' '));
                foreach (var l in trim)
                {
                    String padString = l.PadRight(150, ' ');
                    bool firstCharIsTabOrSpace = padString[0] == '\t' || padString[0] == ' ';

                    if (!firstCharIsTabOrSpace)
                    {
                        DateTime date;
                        String dt = padString.Substring(0, 15).Trim();

                        bool parsedasDate = DateTime.TryParse(dt, out date);
                        if (parsedasDate)
                        {
                            var nr = ret.NewRow();

                            String desc = padString.Substring(15, 115);
                            String amt = padString.Substring(131);
                            nr.SetField(0, dt.Trim());
                            nr.SetField(2, desc.Trim());
                            nr.SetField(3, amt.Trim());
                            ret.Rows.Add(nr);
                        }
                    }
                }
            }


            return ret;
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            var ofd = new FolderBrowserDialog();
            
            ofd.ShowDialog();


            DataTable dt = GenerateDataTableFromFolder(ofd.SelectedPath);

            Preview.ItemsSource = dt.AsDataView();

        }
    }
}
