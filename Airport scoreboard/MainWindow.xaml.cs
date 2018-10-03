using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Media;
using Airport_scoreboard.Entity;
using Airport_scoreboard.Utils;
using Microsoft.Win32;

namespace Airport_scoreboard
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // создаем таймер для постоянного обновления UI
        System.Windows.Threading.DispatcherTimer timer = new System.Windows.Threading.DispatcherTimer();
        //список рейсов 
        List<Aircraft> aircrafts = new List<Aircraft>();

        // переменные для хранения, накопления и вывода информации о рейсах и пассажирах на UI
        int K, PassIn, PassOut;
        private static List<int> AllIn = new List<int>();
        private static List<int> AllOut = new List<int>();
        private static Dictionary<DateTime, int> InLast24Hours= new Dictionary<DateTime, int>();
        private static Dictionary<DateTime, int> OutLast24Hours = new Dictionary<DateTime, int>();



        public MainWindow()
        {
            InitializeComponent();

            timer.Tick += new EventHandler(timerTick);
            timer.Interval = new TimeSpan(0, 0, 1);
            timer.Start();

           
        }

        private void timerTick(object sender, EventArgs e)
        {

            txblastprilet.Text = PassIn.ToString();  //пассажиры последнего рейса(прилет)
            txblastvilet.Text = PassOut.ToString();      //пассажиры последнего рейса(вылет)

            TxbInLast24.Text = InLast24Hours.Values.Sum().ToString();   //пассажиры за поселдние 24 часа (прилет)

            foreach (KeyValuePair<DateTime, int> p in InLast24Hours.ToArray())      //моделируем отображение информации 
            {                                                                       //за последние 24 часа
                DateTime timeNow = DateTime.Now;
                if ((timeNow - p.Key).TotalSeconds > 86400/K)
                    InLast24Hours.Remove(p.Key);
            }

            TxbOutLast24.Text = OutLast24Hours.Values.Sum().ToString(); //пассажиры за поселдние 24 часа (вылет)

            foreach (KeyValuePair<DateTime, int> p in OutLast24Hours.ToArray())     //моделируем отображение информации 
            {                                                                       //за последние 24 часа
                DateTime timeNow = DateTime.Now;
                if ((timeNow - p.Key).TotalSeconds > 86400 / K)
                    OutLast24Hours.Remove(p.Key);
            }
       
            txbAllIn.Text = AllIn.Sum().ToString();     //пассажиры за все вермя работы (прилет)
            txbAllOut.Text = AllOut.Sum().ToString();   //пассажиры за все вермя работы (вылет)      
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            // читаем файл и выводим путь в текстбокс (для отображения что файл загружен)
            OpenFileDialog openFile = new OpenFileDialog();
            openFile.Filter = "Text files(*.txt)|*.txt|All files(*.*)|*.*";

            FileReader reader = new FileReader();
            aircrafts = reader.getAircraft(openFile);
            txbFile.Text = openFile.FileName;

            if (aircrafts.Count != 0)
                lastAircrafttxb.Text = "Последний рейс\n" + aircrafts.Last().ToString();
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            foreach (Aircraft aircraft in aircrafts)  // запускаем эмуляцию работы аэропорта 
            {                                         //в виде фоновых ассинхронных потоков, где 1 поток - 1 рейс
                Emulator(aircraft);
            }
        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            K = (int)slider1.Value;                 //слайдер для задания скорости эмуляции
            txbSpeed.Text = "x" + K.ToString();

        }

        public void Emulator(object obj) //метод создания рейсов (поток в фоне)
        {
            Aircraft aircraft = (Aircraft)obj;
            Thread thread = new Thread(Count) { Name = aircraft.Type };
            thread.IsBackground = true;
            thread.Start(aircraft);
        }

        private void TxbInLast24_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {

        }

        private void Count(object num) // метод выполняемый потоком
        {
                Aircraft aircraft = (Aircraft)num; //в метод передается объект "Самолет"
                for (int i = 0; i < Convert.ToInt32(getPeriod(K, aircraft) * 1000); i++) 
                {
                    Thread.Sleep(1); // усыпляем на 1 сек поток для эмуляции во времени
                }
                //начинаем считать, накапливать и корректировать количество пассажиров прилет/вылет
                if (aircraft.Action == "прилет")
                {
                    DateTime timeNow = DateTime.Now;
                    PassIn = aircraft.Passenger;
                    AllIn.Add(aircraft.Passenger);
                    InLast24Hours.Add(timeNow, aircraft.Passenger);

                }

                else if (aircraft.Action == "вылет")
                {
                    DateTime timeNow = DateTime.Now;
                    PassOut = aircraft.Passenger;
                    AllOut.Add( aircraft.Passenger);
                    OutLast24Hours.Add(timeNow, aircraft.Passenger);

                }
                //MessageBox.Show("Поток выполнен " + aircraft.Type + " " + aircraft.Action + "ел"); //Test
        }

        public double getPeriod(object K, Aircraft aircraft) //метод для получения количества 
        {                                                    //секунд до прилета/вылета самолета
            var count = new TimeSpan(0, 0, 0);
            var timeNow = TimeSpan.Parse(DateTime.Now.ToString("T"));
            if (timeNow > aircraft.Date)
                count = aircraft.Date - timeNow + new TimeSpan(24, 0, 0);
            else
                count = aircraft.Date - timeNow;
            return (count.TotalSeconds) / Convert.ToInt32(K);
        }
    }
}
