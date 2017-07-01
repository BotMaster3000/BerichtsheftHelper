using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;

namespace BerichtsheftHelper
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = this;
        }

        private string nameVorlage;
        public string NameVorlage
        {
            get
            {
                
                if(nameVorlage == null)
                {
                    if(Properties.Settings.Default.NameVorlage != "")
                    {
                        nameVorlage = Properties.Settings.Default.NameVorlage;
                    }
                    else
                    {
                        nameVorlage = "Vorlage.docx";
                    }                    
                }
                return nameVorlage;
            }
            set
            {
                if(nameVorlage != value)
                {
                    nameVorlage = value;
                }
            }
        }
        public string datumBeginn;
        public string DatumBeginn
        {
            get
            {
                if(datumBeginn == null)
                {
                    if (Properties.Settings.Default.DatumBeginn != "")
                    {                        
                        datumBeginn = Properties.Settings.Default.DatumBeginn;
                    }
                    else
                    {
                        datumBeginn = DateTime.Now.ToString("yyyy-MM-dd");
                    }                    
                }
                return datumBeginn;
            }
            set
            {
                if(datumBeginn != value)
                {
                    datumBeginn = value;
                }
            }
        }
        private string datumEnde;
        public string DatumEnde
        {
            get
            {
                if(datumEnde == null)
                {
                    if(Properties.Settings.Default.DatumEnde != "")
                    {
                        datumEnde = Properties.Settings.Default.DatumEnde;
                    }
                    else
                    {
                        datumEnde = DateTime.Now.ToString("yyyy-MM-dd");
                    }
                    
                }
                return datumEnde;
            }
            set
            {
                if(datumEnde != value)
                {
                    datumEnde = value;
                }
            }
        }
        private void ErstenTagDerWocheAuswaehlen(DateTime date, bool anfangDerWoche = true)
        {
            string wochentagName;
            int incrementor;
            if(anfangDerWoche == true)
            {
                wochentagName = "Monday";
                incrementor = -1;
            }
            else
            {
                wochentagName = "Friday";
                incrementor = 1;
            }

            while(date.DayOfWeek.ToString() != wochentagName)
            {                
                date = date.AddDays(incrementor);
            }
        }                
        private void VerzeichnisseErstellen(object sender, RoutedEventArgs e)
        {
            DateTime beginDate = DateTime.Parse(DatumBeginn);
            DateTime endDate = DateTime.Parse(DatumEnde);

            if(beginDate > endDate)
            {
                MessageBox.Show("Das erste Datum muss kleiner sein als das zweite Datum.");
                return;
            }

            ErstenTagDerWocheAuswaehlen(beginDate);
            ErstenTagDerWocheAuswaehlen(endDate, false);

            string path = Environment.CurrentDirectory + @"\Berichtshefte";

            List<string> createdYearDirs = new List<string>();
            try
            {
                for (int year = beginDate.Year; year <= endDate.Year; ++year)
                {
                    string directoryPath = path + @"\" + year.ToString();
                    Directory.CreateDirectory(directoryPath);
                    createdYearDirs.Add(directoryPath);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }


            List<string> createdMonthDirs = new List<string>();
            try
            {
                foreach (string createdDir in createdYearDirs)
                {
                    for (int month = 1; month <= 12; ++month)
                    {
                        
                            string directoryPath = createdDir + @"\" + month + "_" + (new DateTime(2017, month, 1)).ToString("MMMM");
                            Directory.CreateDirectory(directoryPath);
                            createdMonthDirs.Add(directoryPath);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

            string message = "";
            foreach(string dir in createdMonthDirs)
            {
                message += dir + "\n";
            }       
            DateTime tmpDate = new DateTime(beginDate.Year, 1, 1);
            while (tmpDate.DayOfWeek.ToString() != "Monday")
            {
                tmpDate = tmpDate.AddDays(1);
            }
            foreach (string createdDir in createdMonthDirs)
            {
                int currentMonth = tmpDate.Month;
                int currentYear = tmpDate.Year;
                int nextYear = currentYear + 1;
                int nextMonth = currentMonth + 1;
                if (currentMonth + 1 > 12)
                {
                    nextMonth = 1;
                }
                while(tmpDate.Month != nextMonth)
                {                    
                    string filePath = createdDir + @"\" + tmpDate.ToString("dd.MM.-") + (tmpDate.AddDays(4).ToString("dd.MM.yyyy"));
                    string[] splits = NameVorlage.Split('.');
                    string fileExtension = "." + splits[splits.Count() - 1];
                    try
                    {
                        File.Copy(Environment.CurrentDirectory + @"\" + NameVorlage, filePath + fileExtension);                        
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.ToString());
                        return;
                    }

                    if(!(tmpDate.AddDays(7).Month == nextMonth))
                    {
                        tmpDate = tmpDate.AddDays(7);
                    }
                    else
                    {
                        break;
                    }                    
                }
                while(tmpDate.Day > 1)
                {
                    tmpDate = tmpDate.AddDays(1);
                }
                while(tmpDate.DayOfWeek.ToString() != "Monday")
                {
                    tmpDate = tmpDate.AddDays(1);
                }
            }
        }
        private void Window_Closing(object sender, CancelEventArgs e)
        {
            try
            {
                Properties.Settings.Default.DatumBeginn = DatumBeginn;
                Properties.Settings.Default.DatumEnde = DatumEnde;
                Properties.Settings.Default.NameVorlage = NameVorlage;

                Properties.Settings.Default.Save();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
    }
}
