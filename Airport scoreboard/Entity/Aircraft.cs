using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Airport_scoreboard.Entity
{
    public class Aircraft : ICloneable
    {
        public string Type { get; set; }
        public string Action { get; set; }
        public TimeSpan Date { get; set; }
        public string Sity { get; set; }
        public int Capasity { get; set; } //максимальное количество мест определяемое типом самолета
        public int Passenger { get; set; } // количество пассажжиров, определяется рандомом в процессе работы

        public Aircraft()
        {
        }

        public Aircraft(string type, string action, TimeSpan date, string sity)
        {
            Type = type;
            Action = action;
            Date = date;
            Sity = sity;
            Capasity = setCapasity(type);
            Passenger = setPassenger();
        }

        public override string ToString() => "Самолет -" + Type.ToString() + ", вылет/прилет - " + Action.ToString() + ", время - " + Date.ToString() + ", город - " + Sity.ToString()+"\nСвободных мест:" + (Capasity-Passenger).ToString()+"/"+Capasity;

        private int setCapasity(string type)
        {
            int capasity = 0;
            //пока в словарь, затем можно сделать выгрузку из базы данных
            Dictionary<String, int> keys = new Dictionary<string, int>
            {
                {"Ту-134", 80}, 
                {"Ту-154", 152},
                {"Ту-204", 196},
                {"SS-100", 98},
                {"Airbus-A310", 183},
                {"Boeing-747", 310}
            };

            foreach (string s in keys.Keys)
                if (s==type)
                    capasity = keys[s];

            return capasity;   
        }

        private int setPassenger()
        {
            Random rand = new Random();
            return rand.Next(this.Capasity*3/4, this.Capasity);
        }

        public object Clone() //клонирование объекта
        {
            return new Aircraft {
                Type = this.Type, Action = this.Action, Date=this.Date, Sity = this.Sity,
                Capasity = this.Capasity, Passenger = this.Passenger };
        }

        
    }
}
