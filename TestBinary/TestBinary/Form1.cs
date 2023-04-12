using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace TestBinary
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        List<short> znamky = new List<short>();
        List<short> vahy = new List<short>();

        private void button1_Click(object sender, EventArgs e)
        {
            znamky.Add((short)numericUpDown1.Value);
            vahy.Add((short)numericUpDown2.Value);

            listBox1.Items.Add((short)numericUpDown1.Value + " s váhou " + (short)numericUpDown2.Value);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (znamky.Count > 0 && textBox1.Text.Length > 0 && textBox2.Text.Length > 0)
            {
                FileStream fs = new FileStream("seznam.dat", FileMode.OpenOrCreate, FileAccess.ReadWrite);
                BinaryWriter bw = new BinaryWriter(fs);
                bw.BaseStream.Position = bw.BaseStream.Length;
                bw.Write(znamky.Count);
                for(int i = 0; i<znamky.Count;i++)
                {
                    bw.Write(znamky[i]);
                    bw.Write(vahy[i]);
                }
                bw.Write(textBox1.Text);
                bw.Write(textBox2.Text);
                bw.Close();
                textBox1.Text = "";
                textBox2.Text = "";
                znamky.Clear();
                vahy.Clear();
                listBox1.Items.Clear();
            } else
            {
                MessageBox.Show("Zkontrolujte že jste zadali jméno příjmení i známky!!");
            }
        }

        List<long> pozice = new List<long>();
        List<string> jmeno = new List<string>();

        private void button3_Click(object sender, EventArgs e)
        {
            pozice.Clear();
            jmeno.Clear();
            comboBox1.Items.Clear();
            FileStream fs = new FileStream("seznam.dat", FileMode.OpenOrCreate, FileAccess.ReadWrite);
            BinaryReader br = new BinaryReader(fs);
            br.BaseStream.Position = 0;
            while (br.BaseStream.Position < br.BaseStream.Length)
            {
                pozice.Add(br.BaseStream.Position);
                int count = br.ReadInt32();
                br.BaseStream.Seek(count * 4, SeekOrigin.Current);
                string jmApr = br.ReadString() + " " + br.ReadString();
                jmeno.Add(jmApr);
                comboBox1.Items.Add(jmApr);
            }
            br.Close();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            int soucet = 0, pocet = 0;
            label6.Text = "";
            FileStream fs = new FileStream("seznam.dat", FileMode.OpenOrCreate, FileAccess.ReadWrite);
            BinaryReader br = new BinaryReader(fs);
            br.BaseStream.Position = pozice[jmeno.IndexOf(comboBox1.Text)];
            int count = br.ReadInt32();
            listBox2.Items.Clear();
            for(int i = 0; i < count; i++)
            {
                short znamka = br.ReadInt16();
                short vaha = br.ReadInt16();
                listBox2.Items.Add(znamka + " s váhou " + vaha);
                soucet += znamka * vaha;
                pocet += vaha;
            }
            int vyslednaZnamka = (int)Math.Round((double)soucet / pocet, 0);
            label5.Text = "Průměr: " + Math.Round((double)soucet / pocet, 2) + "\nZnámka: " + vyslednaZnamka;

            if (vyslednaZnamka == 1)
                label6.Text = "Skvělé tomuto žákovy vychází 1 na vysvědčení!!";
            else if(vyslednaZnamka==4)
                label6.Text = "Pozor průměr žáka je 4 bylo by potřeba zlepšit průměr!!!";
            else if(vyslednaZnamka==5)
            {
                BinaryWriter bw = new BinaryWriter(fs);
                bw.BaseStream.Position = br.BaseStream.Position;
                /*bw.Write("John");
                bw.Write("Doe");*/
                label6.Text = "Pozor průměr žáka je 5 bylo by potřeba zlepšit průměr!!!";
            }
            br.Close();
        }
    }
}
