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

        // Актуальная высота Canvas
        public double actualHeightCanvas = 0;
        // Максимальное значение, используется для расчётов высоты графика
        public double maxValue = 0;
        // Среднее арифметическое графика
        double averageValue = 0;

        // Таймер для обновления графика
        public DispatcherTimer dispatcherTimer = new DispatcherTimer();

        public Chart(MainWindow mainWindow)
        {
            InitializeComponent();
            this.mainWindow = mainWindow;
            actualHeightCanvas = mainWindow.Height - 50d;

            // Настройка таймера
            dispatcherTimer.Interval = new TimeSpan(0, 0, 2);
            dispatcherTimer.Tick += CreateNewValue;
            dispatcherTimer.Start();

            // Инициализация графика
            CreateChart();
            ColorChart();
        }

        // Метод для создания нового значения
        private void CreateNewValue(object sender, EventArgs e)
        {
            Random random = new Random();

            // Проверка, что в pointsInfo есть хотя бы один элемент
            if (mainWindow.pointsInfo.Count > 0)
            {
                // Генерация нового значения для первого графика
                double value = mainWindow.pointsInfo[mainWindow.pointsInfo.Count - 1].value;
                double newValue = value * (random.NextDouble() + 0.5d);
                mainWindow.pointsInfo.Add(new Classes.PointInfo(newValue));

                // Проверка и обновление максимального значения
                if (newValue > maxValue)
                {
                    maxValue = newValue;
                }
            }

            // Проверка, что в pointsInfoSecond есть хотя бы один элемент
            if (mainWindow.pointsInfoSecond.Count > 0)
            {
                // Генерация нового значения для второго графика
                double valueSecond = mainWindow.pointsInfoSecond[mainWindow.pointsInfoSecond.Count - 1].value;
                double newValueSecond = valueSecond * (random.NextDouble() + 0.5d);
                mainWindow.pointsInfoSecond.Add(new Classes.PointInfo(newValueSecond));

                // Проверка и обновление максимального значения
                if (newValueSecond > maxValue)
                {
                    maxValue = newValueSecond;
                }
            }

            // Обновление графика после добавления новых точек
            ControlCreateChart();
        }

        // Метод для обработки изменения размера страницы
        private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            actualHeightCanvas = mainWindow.Height - 50d;
            CreateChart();
            ColorChart();
        }

        // Метод для создания графика
        public void CreateChart()
        {
            // Очищаем все элементы с canvas
            canvas.Children.Clear();

            // Обнуляем maxValue перед пересчетом
            maxValue = 0;

            // Определяем максимальное значение среди pointsInfo для масштабирования
            foreach (var point in mainWindow.pointsInfo)
            {
                // Если текущее значение больше maxValue, обновляем maxValue
                if (point.value > maxValue)
                {
                    maxValue = point.value;
                }
            }

            // Определяем максимальное значение среди pointsInfoSecond для масштабирования
            foreach (var point in mainWindow.pointsInfoSecond)
            {
                // Если текущее значение больше maxValue, обновляем maxValue
                if (point.value > maxValue)
                {
                    maxValue = point.value;
                }
            }

            // Создаем линии графика для первого набора данных (pointsInfo)
            for (int i = 0; i < mainWindow.pointsInfo.Count; i++)
            {
                // Создаем новую линию
                Line line = new Line();
                // Устанавливаем начальную координату X
                line.X1 = i * 20;
                // Устанавливаем конечную координату X
                line.X2 = (i + 1) * 20;

                // Устанавливаем начальную координату Y
                if (i == 0)
                {
                    // Для первой точки берем значение текущей точки
                    line.Y1 = actualHeightCanvas - (mainWindow.pointsInfo[i].value / maxValue * actualHeightCanvas);
                }
                else
                {
                    // Для остальных точек берем значение предыдущей точки
                    line.Y1 = actualHeightCanvas - (mainWindow.pointsInfo[i - 1].value / maxValue * actualHeightCanvas);
                }

                // Устанавливаем конечную координату Y
                line.Y2 = actualHeightCanvas - (mainWindow.pointsInfo[i].value / maxValue * actualHeightCanvas);
                // Устанавливаем толщину линии
                line.StrokeThickness = 2;
                // Присваиваем линию текущей точке
                mainWindow.pointsInfo[i].line = line;
                // Добавляем линию на canvas
                canvas.Children.Add(line);

                // Создаем линии для второго набора данных (pointsInfoSecond)
                if (i < mainWindow.pointsInfoSecond.Count)
                {
                    // Создаем новую линию для второго графика
                    Line lineSecond = new Line();
                    // Устанавливаем начальную координату X
                    lineSecond.X1 = i * 20;
                    // Устанавливаем конечную координату X
                    lineSecond.X2 = (i + 1) * 20;

                    // Устанавливаем начальную координату Y
                    if (i == 0)
                    {
                        // Для первой точки берем значение текущей точки
                        lineSecond.Y1 = actualHeightCanvas - (mainWindow.pointsInfoSecond[i].value / maxValue * actualHeightCanvas);
                    }
                    else
                    {
                        // Для остальных точек берем значение предыдущей точки
                        lineSecond.Y1 = actualHeightCanvas - (mainWindow.pointsInfoSecond[i - 1].value / maxValue * actualHeightCanvas);
                    }

                    // Устанавливаем конечную координату Y
                    lineSecond.Y2 = actualHeightCanvas - (mainWindow.pointsInfoSecond[i].value / maxValue * actualHeightCanvas);
                    // Устанавливаем толщину линии
                    lineSecond.StrokeThickness = 2;
                    // Устанавливаем цвет линии
                    lineSecond.Stroke = Brushes.Blue;
                    // Присваиваем линию текущей точке второго графика
                    mainWindow.pointsInfoSecond[i].line = lineSecond;
                    // Добавляем линию на canvas
                    canvas.Children.Add(lineSecond);
                }
            }
        }

        // Метод для создания новой точки на графике
        public void CreatePoint()
        {
            // Проверяем, что в pointsInfo есть больше одной точки
            if (mainWindow.pointsInfo.Count > 1)
            {
                // Создаем новую линию
                Line line = new Line();
                // Устанавливаем начальную координату X
                line.X1 = (mainWindow.pointsInfo.Count - 2) * 20;
                // Устанавливаем конечную координату X
                line.X2 = (mainWindow.pointsInfo.Count - 1) * 20;
                // Устанавливаем начальную координату Y
                line.Y1 = actualHeightCanvas - (mainWindow.pointsInfo[mainWindow.pointsInfo.Count - 2].value / maxValue * actualHeightCanvas);
                // Устанавливаем конечную координату Y
                line.Y2 = actualHeightCanvas - (mainWindow.pointsInfo[mainWindow.pointsInfo.Count - 1].value / maxValue * actualHeightCanvas);
                // Устанавливаем толщину линии
                line.StrokeThickness = 2;
                // Присваиваем линию текущей точке
                mainWindow.pointsInfo[mainWindow.pointsInfo.Count - 1].line = line;
                // Добавляем линию на canvas
                canvas.Children.Add(line);
            }

            // Проверяем, что в pointsInfoSecond есть больше одной точки
            if (mainWindow.pointsInfoSecond.Count > 1)
            {
                // Создаем новую линию для второго графика
                Line lineSecond = new Line();
                // Устанавливаем начальную координату X
                lineSecond.X1 = (mainWindow.pointsInfoSecond.Count - 2) * 20;
                // Устанавливаем конечную координату X
                lineSecond.X2 = (mainWindow.pointsInfoSecond.Count - 1) * 20;
                // Устанавливаем начальную координату Y
                lineSecond.Y1 = actualHeightCanvas - (mainWindow.pointsInfoSecond[mainWindow.pointsInfoSecond.Count - 2].value / maxValue * actualHeightCanvas);
                // Устанавливаем конечную координату Y
                lineSecond.Y2 = actualHeightCanvas - (mainWindow.pointsInfoSecond[mainWindow.pointsInfoSecond.Count - 1].value / maxValue * actualHeightCanvas);
                // Устанавливаем толщину линии
                lineSecond.StrokeThickness = 2;
                // Устанавливаем цвет линии
                lineSecond.Stroke = Brushes.Blue;
                // Присваиваем линию текущей точке второго графика
                mainWindow.pointsInfoSecond[mainWindow.pointsInfoSecond.Count - 1].line = lineSecond;
                // Добавляем линию на canvas
                canvas.Children.Add(lineSecond);
            }
        }

        // Контроль создания графика
        public void ControlCreateChart()
        {
            // Обновление графика при изменении данных
            CreateChart();

            // Обновление цветов и размеров графика
            ColorChart();
        }

        // Метод для раскрашивания графика
        public void ColorChart()
        {
            double value = mainWindow.pointsInfo[mainWindow.pointsInfo.Count - 1].value;
            averageValue = 0;
            // Вычисление среднего значения
            for (int i = 0; i < mainWindow.pointsInfo.Count; i++)
            {
                averageValue += mainWindow.pointsInfo[i].value;
            }
            averageValue /= mainWindow.pointsInfo.Count;

            // Раскраска линий в зависимости от среднего значения
            for (int i = 0; i < mainWindow.pointsInfo.Count; i++)
            {
                if (mainWindow.pointsInfo[i].value < averageValue)
                {
                    mainWindow.pointsInfo[i].line.Stroke = Brushes.Red;
                }
                else
                {
                    mainWindow.pointsInfo[i].line.Stroke = Brushes.Green;
                }
            }

            // Обновление ширины canvas и прокрутки
            canvas.Width = mainWindow.pointsInfo.Count * 20 + 300;
            scroll.ScrollToHorizontalOffset(canvas.Width);

            // Обновление меток текущего и среднего значений
            current_value.Content = "Тек. знач: " + Math.Round(value, 2);
            average_value.Content = "Сред. знач: " + Math.Round(averageValue, 2);
        }
    }
}
