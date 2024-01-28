using System.Globalization;

namespace GPS_data_reader
{
    class Program {     

       
        static void Main(string[] args)
        {
            Console.WriteLine("Odczyt pomiarów GPS");
            GPSRaeder reader=new GPSRaeder();
            if(reader.possibleReading)
            {
                bool exit=false;
                while(!exit)
                {
                    Console.WriteLine("Aby zakonczyc dzialanie programu wcisnij esc, jesli chcesz uzyskac kolejny odczyt, wcisnij enter");          
                    if(Console.ReadKey().Key==ConsoleKey.Escape)
                        exit=true;
                    else if(Console.ReadKey().Key==ConsoleKey.Enter)
                    {
                        reader.odczytGPS();
                    }      
                }
            }
            else Console.WriteLine("Nie odnaleziono pliku. Aby zakonczyc wcisnij esc");
            while(Console.ReadKey().Key!=ConsoleKey.Escape);
        }

    }

    class GPSRaeder
    {
        string [] answersFromReader; 
        static string filePath= @"dane.txt";  

        public bool possibleReading;
        private int przesuniecieCzasowe;
        public GPSRaeder()
        {
            possibleReading=readFromFile(filePath);
        }

        private bool readFromFile(string path)
        {
            if(File.Exists(path))
            {
                answersFromReader = File.ReadAllLines(path);
                return true;
            }
            else return false;
        }

        public void odczytGPS()
        {
            Random rand=new Random();
            int k=rand.Next(answersFromReader.Length);
            while(!decodeMessage(answersFromReader[k]))
            {
                k=rand.Next(answersFromReader.Length);
            }

        }

        private bool decodeMessage(string line)
        {
            string [] splittedLine=line.Split(',');
            if(splittedLine[0].Equals("$GPGGA"))
            {
                for(int i=0; i<splittedLine.Length; i++)
                {
                    Console.WriteLine(i+" "+splittedLine[i]);
                }
                string lat=szerokosc(splittedLine[2]);
                string latInfo="szerokosc: "+lat+" "+splittedLine[3];
                
                string longi=dlugosc(splittedLine[4]);
                string longiInfo="dlugosc: "+longi+" "+splittedLine[5];

                string czas="czas odczytu: "+czasOdczytu(splittedLine[1], splittedLine[5]=="E");
                string wysokosc="wysokosc: "+wys(splittedLine[9], splittedLine[11])+" m";
                if(int.Parse(splittedLine[7])>=4)
                    Console.WriteLine("Odczytana lokalizacja ma szanse byc poprawna");
                else 
                    Console.WriteLine("Pomiaru dokonala niewystarczajaca licba satelitow, aby moc uznac go za poprawny, jednak oto on:");
                Console.WriteLine("Odczytano nastepujace informacje: \n"+czas+"\n"+latInfo+"\n"+longiInfo+"\n"+wysokosc+"\n");
                Console.WriteLine("Link do mapy: "+link(lat, splittedLine[3], longi, splittedLine[5]));
                return true;
            }
            else return false;
        }

        private string czasOdczytu(string line, bool kierunekPrzesuniecia)
        {
            int godzina= int.Parse(line.Substring(0,2))+ (kierunekPrzesuniecia ? 1 : (-1))*przesuniecieCzasowe;
            return godzina+":"+line.Substring(2,2)+":"+line.Substring(4,2);
        }

        private string szerokosc (string line)
        {
            return line.Substring(0,2)+","+line.Substring(2,2);
        }

        private string dlugosc (string line)
        {
            int d=int.Parse(line.Substring(1,2));
            przesuniecieCzasowe=d/15;
            return line.Substring(1,2)+","+line.Substring(3,2);
        }

        public string link(string lat, string sn, string longi, string we)
        {
            string link = "https://www.google.com/maps/@";
            if ( sn == "S")
                link += "-";
            link += lat + ",";
            if (we == "W")
                link += "-";
            link += longi;

            return link;
        }

        public string wys(string wysokosc1, string wysokosc2)
        {
            float h1=float.Parse(wysokosc1, CultureInfo.InvariantCulture.NumberFormat);
            float h2=float.Parse(wysokosc2, CultureInfo.InvariantCulture.NumberFormat);
            return ""+(h1-h2);
        }

    }

   

}
