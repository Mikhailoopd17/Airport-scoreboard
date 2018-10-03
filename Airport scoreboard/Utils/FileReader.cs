using Airport_scoreboard.Entity;
using Microsoft.Win32;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Airport_scoreboard.Utils
{
    public class FileReader
    {
        public ArrayList Read(string path)
        {
            List<string> list = new List<string>();

            //построчно считываем содержимое файла и отправляем в список
            using (StreamReader sr = new StreamReader(path, System.Text.Encoding.Default))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    list.Add(line);
                }
                return ListToArray(list);
            }
        }
        //разделяем строку на элементы
        private ArrayList ListToArray(List<string> list)
        {
            ArrayList array = new ArrayList();
            foreach (string str in list)
            {
                array.Add(str.Split(','));
            }
            return array;
        }

        //получаем объект "Самолет"
        public List<Aircraft> getAircraft(OpenFileDialog openFile)
        {
            List<Aircraft> aircraft = new List<Aircraft>();
            ArrayList listAir = new ArrayList();

            if (openFile.ShowDialog() == true)
            {
                string filename = openFile.FileName;
                listAir = Read(filename);

                foreach (string[] i in listAir)
                {
                    var ts = TimeSpan.Parse(i[2]);
                    aircraft.Add(new Aircraft(i[0], i[1], ts, i[3]));
                }

            }
            return sortListByTime(aircraft);
        }

        public List<Aircraft> sortListByTime(List<Aircraft> list)  //Метод сортировки списка самолетов в зависимости от 
        {                                                          //времени вылета/прилета с привязкой к текущему времени
            //клонируем list
            List<Aircraft> temp = new List<Aircraft>();
            foreach (Aircraft clone in list)
                temp.Add((Aircraft)clone.Clone());
            //приводим время к интервалу до вылета/прилета
            var timeNow = TimeSpan.Parse(DateTime.Now.ToString("T"));
            foreach (Aircraft aircraft in temp)
            {
                if (aircraft.Date < timeNow)
                    aircraft.Date = aircraft.Date - timeNow + new TimeSpan(24, 0, 0);
                else
                    aircraft.Date -= timeNow;
            }
            //сортируем по интервалу времени до вылета/прилета
            var sortedlist = from l in temp
                             orderby l.Date
                             select l;

            //заполняем новый список элементами из отсортированного var
            List<Aircraft> tempN = new List<Aircraft>();
            foreach (Aircraft a in sortedlist)
                tempN.Add(a);

            //возвращаем время вылета/прилета (взамен интервала)
            for (int i = 0; i < tempN.Count(); i++)
            {
                for (int j = 0; j < list.Count(); j++)
                {
                    if (tempN[i].Type == list[j].Type)
                        tempN[i].Date = list[j].Date;
                }
            }

            return tempN;
        }
    }
}