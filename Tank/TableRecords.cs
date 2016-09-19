using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
using System.Drawing;

namespace Tank
{
   public struct Record: IComparable<Record>
    {
        public string name;
        public int score;
        public Record(string name, int score)
        { 
            this.name = name; 
            this.score = score; 
        }

        public int CompareTo(Record record)
        {
            if (this.score > record.score)
                return -1;
            if (this.score < record.score)
                return 1;
            else
                return 0;
        }
    }


    public static class TableRecords
    {
        
        public static List<Record> records = new List<Record>();


        static TableRecords()
        {
            
            string line;
            StreamReader sr = new StreamReader("Records.txt");
            while ((line = sr.ReadLine()) != null)
            {
                records.Add(new Record(line,Int32.Parse(sr.ReadLine())));
            }
            sr.Close();
            sr.Dispose();
            
            records.Sort();
        }

     

        public static Boolean Order_To_Add(int score)
         {
             if (records[9].score < score)
                 return true;
             else
                 return false;
         }
        
        public static void Add(string name,int score)
         {
             if (name == "")
                 name = "Unknown";
             records.Add(new Record(name, score));
             records.Sort();
             if(records.Count>10)
             records.RemoveAt(10);
             Submit();    
         }

        public static void Submit()
        {
            StreamWriter sw = new StreamWriter("Records.txt", false, Encoding.Default, 10);

           foreach( var player in records)
            {
               sw.WriteLine(player.name); 
               sw.WriteLine(player.score);
            }
            sw.Close();
            sw.Dispose();
  
        }
    
        public static void Clear()
        {
            records = new List<Record>();
            for (int i = 0; i < 10; i++)
            {
                records.Add(new Record("", 1));
            }
                Submit();
        }
    }
}
