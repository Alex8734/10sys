using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using System.Data.OleDb;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Windows;
using System.Windows.Input;
using KeyEventHandler = System.Windows.Forms.KeyEventHandler;
using Point = System.Drawing.Point;

namespace _10sys
{
    public partial class Form1 : Form
    {
        
        public Form1()
        {
            InitializeComponent();

        }
     
        private void Form1_Load(object sender, EventArgs e)
        {
/*
            var nr = new LektionNR(1, 1, 0);
            var Lektion = GetLektion(nr.ToString());
            int i = 1;
            do
            {
                int j = 1;
                nr.Categorie = i;
                nr.SubLektion = 0;
                do
                {
                    nr.Lektion = j;
                    nr.SubLektion = 0;
                    if(nr.Categorie == 15)
                    {
                        int k = 1;
                        nr.SubLektion = k;
                        do
                        {
                            Lektion = GetLektion(nr.ToString());
                            if(Lektion != null)
                            {
                                CreateCategories(Lektion);
                            }
                            nr.SubLektion = ++k;
                            Lektion = GetLektion(nr.ToString());
                        }while (Lektion != null);
                    }
                    
                    Lektion = GetLektion(nr.ToString());
                    if (Lektion != null)
                    {
                        CreateCategories(Lektion);
                    }

                    nr.Lektion = ++j;
                    nr.SubLektion = nr.Categorie == 15 ? 1 : 0;
                    Lektion = GetLektion(nr.ToString());
                }while (Lektion != null);

                nr.Categorie = ++i;
                nr.Lektion = 1;
                nr.SubLektion = nr.Categorie == 15 ? 1 : 0;
                Lektion = GetLektion(nr.ToString());
            } while (Lektion != null);
            */

            var nr = new LektionNR(1, 1, 0);
            var Lektion = GetLektionItems(nr.ToString());
            for (int i = 1; i <= 16; i++)
            {
                var lekNr = $"{i:00}";
                if(i == 15)
                {
                    for (int j = 1; j <= 3; j++)
                    {
                        var items15 = GetLektionItems($"{lekNr}01{j:00}");
                        foreach (var artikel in items15)
                        {
                            CreateCategories(artikel);
                        }
                    }
                    for (int j = 1; j <= 3; j++)
                    {
                        var items15 = GetLektionItems($"{lekNr}02{j:00}");
                        foreach (var artikel in items15)
                        {
                            CreateCategories(artikel);
                        }
                    }
                    continue;
                }
                var items = GetLektionItems(lekNr);
                foreach (var artikel in items)
                {
                    CreateCategories(artikel);
                }
            }
        }
        private static void write(string Lektion, int typesPerMin)
        {
            
            
            List<string> lines = new List<string>();
            Random rnd = new Random();
            Thread.Sleep(5000);

            string doc = Lektion;

            lines = doc.Split('\n').ToList();
            foreach (string line in lines)
            {
                foreach (char letter in line)
                {
                    if (letter == 32)
                    {
                        SendKeys.Send(" ");
                    }
                    else if (letter == '\r')
                    {
                        SendKeys.Send("{ENTER}");
                        SendKeys.Send("{ENTER}");
                    }
                    else
                    {
                        SendKeys.Send(Convert.ToString(letter));
                    }
                    double sleep = (60 / (double)typesPerMin) *1000;
                    //int time = rnd.Next((int)sleep - 10, (int)sleep + 10);
                    //for (int i = 0; i<= time; i++)
                    //{
                      //  string Key = ReadInput();
                      var rndSleep = rnd.Next((int) sleep - 10, (int) sleep + 10);
                      Thread.Sleep(rndSleep);
                      if(KeyboardHook.Closing)
                      {
                          return;
                      }
                      //}
                }
            }
        }

        private void CreateCategories(Artikel lek)
        {
            var bz3 = "";
            if(lek.Lektionnr.Substring(4,2) != "00")
            {
                bz3 = lek.Bz2;
                lek.Bz2 = lek.Bz1.Split('-')[1].Trim();
                lek.Bz1 = lek.Bz1.Split('-')[0].Trim();
            }
            var menuItem = new ToolStripMenuItem()
            {
                Text = lek.Bz2,
                Name = bz3 == "" ? lek.Lektionnr : lek.Lektionnr.Substring(0,4)+"00"
            };
            if(bz3 != "")
            {
                var found2 = false;
                foreach (var lektion in lektionen.DropDownItems)
                {
                    if (lektion is ToolStripMenuItem i && i.Text == lek.Bz1)
                    {
                        if(i.DropDownItems.Find(menuItem.Name, true).Length == 0)
                        {
                            i.DropDownItems.Add(menuItem);
                        }
                        found2 = true;
                    }
                }
                if(!found2)
                {
                    lektionen.DropDownItems.Add(new ToolStripMenuItem
                    {
                        Name = lek.Lektionnr.Substring(0,2)+"0000",
                        Text = lek.Bz1,
                        DropDownItems = { menuItem }
                    });
            
                }
                
                var subItem = new ToolStripMenuItem()
                {
                    Text = bz3,
                    Name = lek.Lektionnr
                };
                subItem.Click += SelectLektion;
                ((ToolStripMenuItem)lektionen.DropDownItems.Find(lek.Lektionnr.Substring(0,4)+"00",true).First()).DropDownItems.Add(subItem);
                
            }
            else
            {
                menuItem.Click += SelectLektion;
            
                // cathegory lection 1st level
                var found = false;
                foreach (var lektion in lektionen.DropDownItems)
                {
                    if (lektion is ToolStripMenuItem i && i.Text == lek.Bz1)
                    {
                        i.DropDownItems.Add(menuItem);
                        found = true;
                    }
                }
                if(!found)
                {
                    
                    lektionen.DropDownItems.Add(new ToolStripMenuItem
                    {
                        Name = lek.Lektionnr.Substring(0,2)+"0000",
                        Text = lek.Bz1,
                        DropDownItems = { menuItem }
                    });
                
            }
            }
        }
        private static void SelectLektion(Object sender, EventArgs e)
        {   
            
            TextBox textBox=new TextBox();
            Button button = new Button();
            NumericUpDown typesPSec = new NumericUpDown();
            Button speedButton = new Button();
            button.Text = "Start";
            speedButton.Text = "Speed";
            button.Location = new Point(387, 223);
            textBox.Location = new Point(55, 61);
            speedButton.Location = new Point(166, 223);
            textBox.Name = "Lektion";
            textBox.Multiline = true;
            textBox.Size = new System.Drawing.Size(376, 134);
            textBox.Parent = button;   
            typesPSec.Location = new Point(11, 226);
            typesPSec.Maximum = 250;
            typesPSec.Minimum = 50;
            typesPSec.Name = "typesPSec";
            speedButton.Click += SpeedButton_Click;
            button.Click += StartButton_Click;

            var templatedParent = ((ToolStripMenuItem)sender).Name;
            
            string Lektion = GetLektion(templatedParent);
            textBox.Text = Lektion;
            for (int i = 0; i < ActiveForm.Controls.Count; i++)
            {
                Console.WriteLine(ActiveForm.Controls[i]);   
            }

            Console.WriteLine("---------------------------------------");
            if(ActiveForm.Controls.Count > 2)
            {
                var lektion = (TextBox) ActiveForm.Controls.Find("Lektion", true).FirstOrDefault();
                lektion!.Text = Lektion;
            }else
            {
                ActiveForm.Controls.Add(speedButton);
                ActiveForm.Controls.Add(textBox);
                ActiveForm.Controls.Add(button);
                ActiveForm.Controls.Add(typesPSec);
            }

        }

        private static void SpeedButton_Click(object sender, EventArgs e)
        {
            if (sender == null)
            {
                // Handle the case where the sender is not a Button
                return;
            }

            if (sender is not Button {Parent: Form parentForm})
            {
                return;
            }

            var numericUpDown = parentForm.Controls.OfType<NumericUpDown>().FirstOrDefault();

            if (numericUpDown == null) return;
            // Set the Maximum and Minimum properties
            numericUpDown.Maximum = 5454;
            numericUpDown.Minimum = 1;


        }

        private static void StartButton_Click(object sender, EventArgs e)
        {
            int typesPerSec = Convert.ToInt32(((NumericUpDown)ActiveForm.Controls.Find("typesPSec", true).First()).Value);


            using (var lektionTextBox = (TextBox) ActiveForm.Controls.Find("Lektion", true).FirstOrDefault())
            {
                if(lektionTextBox is null )
                {
                    MessageBox.Show("Error in starting the Lektion");
                    return;
                }

                
                if(!Windows.FocusWindow("easytyping2"))
                {
                    return;
                }
                
                write(lektionTextBox!.Text, typesPerSec);
            }
            
            //write(Form1.ActiveForm.Controls["Lektion"].Text);
        }

        private static string GetLektion(string Nr)
        {
            string strSql = "SELECT Lektion FROM T_EasyTypingLektionen WHERE Lektionnr = @Nr";

            using (OleDbConnection conn = new OleDbConnection(@"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=U:\etlektionen.mdb"))
            {
                conn.Open();
                using (OleDbCommand command = new OleDbCommand(strSql, conn))
                {
                    command.Parameters.AddWithValue("@Nr", Nr);

                    using (OleDbDataReader reader = command.ExecuteReader())
                    {
                        
                        while (reader.Read())
                        {
                            return reader["Lektion"].ToString();
                        }
                    }
                }
            }
            return null; // Handle the case when the record is not found.
        }
        private static Artikel[] GetLektionItems(string Nr)
        {
            string strSql = "SELECT Bezeichnung1, Bezeichnung2, Lektion, Lektionnr FROM T_EasyTypingLektionen WHERE LektionNr LIKE @Nr";

            using (OleDbConnection conn = new OleDbConnection(@"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=U:\etlektionen.mdb"))
            {
                conn.Open();
                using (OleDbCommand command = new OleDbCommand(strSql, conn))
                {
                    command.Parameters.AddWithValue("@Nr", $"{Nr}%");

                    using (OleDbDataReader reader = command.ExecuteReader())
                    {
                        var artikels = new List<Artikel>();
                        while (reader.Read())
                        {
                            artikels.Add( new Artikel
                            {
                                Bz1 = reader["Bezeichnung1"].ToString(),
                                Bz2 = reader["Bezeichnung2"].ToString(),
                                Lektion = reader["Lektion"].ToString(),
                                Lektionnr = reader["Lektionnr"].ToString(),
                            });
                        }
                        return artikels.ToArray();
                    }
                }
            }
        }
        
    }  
}

