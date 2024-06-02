using System;
using System.Collections.Generic;
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
using System.Windows.Threading;

namespace PermDynamics.Pages
{
    /// <summary>
    /// Логика взаимодействия для Chart.xaml
    /// </summary>
    public partial class Chart : Page
    {
        MainWindow mainWindow;

        public double actualHeightCanvas = 0;
        public double maxValue = 0;
        double averageValue = 0;

        public DispatcherTimer dispatcherTimer = new DispatcherTimer();

        public Chart(MainWindow mainWindow)
        {
            InitializeComponent();
            this.mainWindow = mainWindow;
            actualHeightCanvas = mainWindow.Height - 50d;

            dispatcherTimer.Interval = new TimeSpan(0, 0, 2);
            dispatcherTimer.Tick += CreateNewValue;
            dispatcherTimer.Start();

            CreateChart();
            ColorChart();
        }

        private void CreateNewValue(object sender, EventArgs e)
        {
            Random random = new Random();
            double value = mainWindow.pointsInfo[mainWindow.pointsInfo.Count - 1].value;
            double newValue = value * (random.NextDouble() + 0.5d);
            mainWindow.pointsInfo.Add(new Classes.PointInfo(newValue));
            ControlCreateChart();
        }

        private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            actualHeightCanvas = mainWindow.Height - 50d;
            CreateChart();
            ColorChart();
        }

        public void CreateChart()
        {
            canvas.Children.Clear();
            for (int i = 0; i < mainWindow.pointsInfo.Count; i++) 
                if (mainWindow.pointsInfo[i].value > maxValue)
                    maxValue = mainWindow.pointsInfo[i].value;
            
            for (int i = 0; i < mainWindow.pointsInfo.Count; i++)
            {
                Line line = new Line();
                line.X1 = i * 20;
                line.X2 = (i + 1) * 20;
                if (i == 0)
                    line.Y1 = actualHeightCanvas;
                else
                    line.Y1 = actualHeightCanvas - ((mainWindow.pointsInfo[(i - 1)].value / maxValue) * actualHeightCanvas);
                line.Y2 = actualHeightCanvas - ((mainWindow.pointsInfo[i].value / maxValue) * actualHeightCanvas);
                line.StrokeThickness = 2;
                mainWindow.pointsInfo[i].line = line;
                canvas.Children.Add(line);
            }
        }

        public void CreatePoint()
        {
            Line line = new Line();
            line.X1 = (mainWindow.pointsInfo.Count - 1) * 20;
            line.X2 = mainWindow.pointsInfo.Count * 20;
            line.Y1 = actualHeightCanvas - ((mainWindow.pointsInfo[(mainWindow.pointsInfo.Count - 2)].value / maxValue) * actualHeightCanvas);
            line.Y2 = actualHeightCanvas - ((mainWindow.pointsInfo[(mainWindow.pointsInfo.Count - 1)].value / maxValue) * actualHeightCanvas);
            line.StrokeThickness = 2;
            mainWindow.pointsInfo[(mainWindow.pointsInfo.Count - 1)].line = line;
            canvas.Children.Add(line);
        }
        public void ControlCreateChart()
        {
            double value = mainWindow.pointsInfo[mainWindow.pointsInfo.Count - 1].value;
            if (value < maxValue)
                CreatePoint();
            else
                CreateChart();

            ColorChart();
        }

        public void ColorChart()
        {
            double value = mainWindow.pointsInfo[mainWindow.pointsInfo.Count - 1].value;
            for (int i = 0; i < mainWindow.pointsInfo.Count; i++)
                averageValue += mainWindow.pointsInfo[i].value;
            averageValue /= mainWindow.pointsInfo.Count;

            for (int i = 0; i < mainWindow.pointsInfo.Count; i++)
                if (value < averageValue)
                    mainWindow.pointsInfo[i].line.Stroke = Brushes.Red;
                else
                    mainWindow.pointsInfo[i].line.Stroke = Brushes.Green;

            canvas.Width = mainWindow.pointsInfo.Count * 20 + 300;
            scroll.ScrollToHorizontalOffset(canvas.Width);
            current_value.Content = "Тек. знач: " + Math.Round(value, 2);
            average_value.Content = "Сред. знач: " + Math.Round(averageValue, 2);
        }
    }
}
