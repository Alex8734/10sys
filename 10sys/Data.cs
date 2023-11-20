using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.OleDb;
using System.Windows.Forms;
using System.Diagnostics;

namespace _10sys
{
    public class Artikel
    {
        public int Nr {  get; set; }
        public string Bz1 {  get; set; }
        public string Bz2 {  get; set; }
        public string Lektionnr { get; set; }
        public string Lektion { get; set; }

        public override string ToString()
        {
            return Lektion;
        }
    }
    public class LektionNR
    {
        public LektionNR(int Categorie, int Lektion, int SubLektion)
        {
            this.Categorie = Categorie;
            this.Lektion = Lektion;
            this.SubLektion = SubLektion;
        }

        public override string ToString()
        {
            return $"{Categorie:00}{Lektion:00}{SubLektion:00}";
        }

        public int Categorie { get; set; }
        public int Lektion { get; set; }
        public int SubLektion { get; set; }
    }
}
