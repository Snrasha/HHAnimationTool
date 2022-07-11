
using AnimatedGif;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace HHAnimationTool
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public ObservableCollection<ComboBoxItem> GifscaleItems { get; set; }
        public ComboBoxItem SelectedcbItem { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;


            GifscaleItems = new ObservableCollection<ComboBoxItem>();
            for (int i = 1; i < 10; i++)
            {
                var cbItem = new ComboBoxItem { Content = "" + i };

                if (i == 6)
                {
                    SelectedcbItem = cbItem;
                }
                GifscaleItems.Add(cbItem);
            }


        }

        string openDialog(string title)
        {

            var dialog = new Microsoft.Win32.OpenFileDialog();
            dialog.Title = title;
            dialog.FileName = "Image"; // Default file name
            dialog.DefaultExt = ".png"; // Default file extension
            dialog.Filter = "PNG Files (.png)|*.png"; // Filter files by extension

            // Show open file dialog box
           
            bool? result = dialog.ShowDialog();

            // Process open file dialog box results
            if (result == true)
            {
                if(dialog.FileName.Trim().Length == 0)
                {
                    return null;
                }
                // Open document
                return dialog.FileName;
            }

            return null;
        }
        string saveDialog(string loadedImagePath)
        {
            string[] inifilesplit = loadedImagePath.Split('/');
            string inifile = inifilesplit[inifilesplit.Length - 1];
            int idx=inifile.LastIndexOf(".");
            if (idx != -1)
            {
                inifile = inifile.Substring(0, idx);
            }
            if (!inifile.EndsWith(" Animation"))
            {
                inifile += " Animation";
            }
            if (!inifile.EndsWith(".gif"))
            {
                inifile += ".gif";
            }

            var dialog = new Microsoft.Win32.SaveFileDialog();
            dialog.Title = "Save GIF";
            dialog.FileName = inifile; // Default file name
            dialog.Filter = "GIF Files (.gif)|*.gif"; // Filter files by extension

            // Show open file dialog box
            bool? result = dialog.ShowDialog();

            // Process open file dialog box results
            if (result == true)
            {
                string resultpath = dialog.FileName;
                if (resultpath.Trim().Length == 0)
                {
                    return null;
                }
                else if (!resultpath.EndsWith(".gif"))
                {
                    resultpath += ".gif";
                }
                // Open document
                return resultpath;
            }

            return null;
        }
        void createGif(string title,bool isHero)
        {
            string filename = openDialog(title);
            if (filename == null)
            {
                return;
            }



            if (!double_bg_checkbox.IsChecked.HasValue) //check for a value  
            {
                // Assume that IsInitialized  
                // returns either true or false.  
                double_bg_checkbox.IsChecked = false;
            }
            if (!sized24_checkbox.IsChecked.HasValue) //check for a value  
            {
                // Assume that IsInitialized  
                // returns either true or false.  
                sized24_checkbox.IsChecked = false;
            }
            if (!death_checkbox.IsChecked.HasValue) //check for a value  
            {
                // Assume that IsInitialized  
                // returns either true or false.  
                death_checkbox.IsChecked = false;
            }
            bool double_bg = (bool)double_bg_checkbox.IsChecked;
            bool sized24 = (bool)sized24_checkbox.IsChecked;
            bool deathOnly = (bool)death_checkbox.IsChecked;



            try
            {
                string tempfilename = Path.GetTempFileName();

                GifMaker gifmaker = new GifMaker();

                List<Bitmap> listToAdd=gifmaker.Gif(filename, double_bg, sized24, deathOnly, gifScaleComboBox.SelectedIndex + 1,isHero);

                // 33ms delay (~30fps)
                using (var gif = AnimatedGif.AnimatedGif.Create(tempfilename, 100))
                {
                    foreach (Bitmap item in listToAdd)
                    {
                        gif.AddFrame(item, delay: 100, quality: GifQuality.Bit8);
                    }
                }
                string filenamegif = saveDialog(filename);
                if (filenamegif == null)
                {
                    return;
                }
                System.IO.File.Copy(tempfilename, filenamegif,true);
                System.IO.File.Delete(tempfilename);
                gifmaker.clear();

            }
            catch (IOException e)
            {
                System.Diagnostics.Debug.WriteLine(e);
                // Temp folder is full, need to be cleanup
            }
          


        }


        void OnClickHero(object sender, RoutedEventArgs e)
        {
            //hero_btn.Foreground = new SolidColorBrush(Colors.Blue);
            createGif("Select a Hero spritesheet",true);
        }

        void OnClickUnit(object sender, RoutedEventArgs e)
        {
            //unit_btn.Foreground = new SolidColorBrush(Colors.Green);
            createGif("Select a Unit spritesheet",false);
        }
        void OnClickDismiss(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            //Console.WriteLine(e.Key);


            if (Keyboard.Modifiers == ModifierKeys.None && e.Key == Key.U)
            {
                OnClickHero(null, null);
            }
            else if (Keyboard.Modifiers == ModifierKeys.None && e.Key == Key.D)
            {
                OnClickUnit(null, null);
            }
            else if (Keyboard.Modifiers == ModifierKeys.None && e.Key == Key.V)
            {
                double_bg_checkbox.IsChecked = !double_bg_checkbox.IsChecked;
            }
            else if (Keyboard.Modifiers == ModifierKeys.None && e.Key == Key.F)
            {
                sized24_checkbox.IsChecked = !sized24_checkbox.IsChecked;
            }
            else if (Keyboard.Modifiers == ModifierKeys.None && e.Key == Key.G)
            {
                death_checkbox.IsChecked = !death_checkbox.IsChecked;
            }
            else if (Keyboard.Modifiers == ModifierKeys.None && e.Key == Key.Escape)
            {
                this.Close();
            }
            else if (Keyboard.Modifiers == ModifierKeys.None && e.Key == Key.Add)
            {

                int idx = gifScaleComboBox.SelectedIndex;
                if (idx < 9)
                {
                    gifScaleComboBox.SelectedIndex = idx + 1;
                }

            }
            else if (Keyboard.Modifiers == ModifierKeys.None && e.Key == Key.Subtract)
            {
                int idx = gifScaleComboBox.SelectedIndex;
                if (idx >0)
                {
                    gifScaleComboBox.SelectedIndex = idx - 1;
                }
            }
        }


    }

}
