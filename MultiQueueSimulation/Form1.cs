using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MultiQueueModels;
using MultiQueueTesting;

namespace MultiQueueSimulation
{
    public partial class Form1 : Form
    {
        
        Run run = new Run();
        public Form1()
        {
           
            InitializeComponent();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            run.Enter_Data(textBox1.Text, textBox2.Text, textBox3.Text, textBox4.Text);
            MessageBox.Show("Done ♥");

        }

        private void button2_Click(object sender, EventArgs e)
        {
            //dataGrid.Rows[rows].Cells[col].Value.ToString()      
            //
            string[] time = new string[dataGridView1.Rows.Count];
            string [] prob = new string [dataGridView1.Rows.Count];
            for (int i =0;i<dataGridView1.Rows.Count-1;++i)
            {
                    time[i]=dataGridView1.Rows[i].Cells[0].Value.ToString();
                    prob[i] = dataGridView1.Rows[i].Cells[1].Value.ToString();
                    
            }
            Dictionary<string, string> d = run.Enter_InterarrivalDistribution(dataGridView1.Rows.Count-1,time,prob);           
            for (int i = 0; i < d.Count; i++)
            {
                dataGridView1.Rows[i].Cells[2].Value = d.ElementAt(i).Key;
                dataGridView1.Rows[i].Cells[3].Value = d.ElementAt(i).Value;
            }

            MessageBox.Show("Done ♥");
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Server s = new Server();
            string[] time = new string[dataGridView1.Rows.Count];
            string[] prob = new string[dataGridView1.Rows.Count];
            for (int i = 0; i < dataGridView1.Rows.Count - 1; ++i)
            {
                time[i] = dataGridView2.Rows[i].Cells[0].Value.ToString();
                prob[i] = dataGridView2.Rows[i].Cells[1].Value.ToString();

            }
            Dictionary<string, string> d = run.Add_ServiceDistribution(dataGridView1.Rows.Count-1, time, prob);
            for (int i = 0; i < d.Count; i++)
            {
                dataGridView2.Rows[i].Cells[2].Value = d.ElementAt(i).Key;
                dataGridView2.Rows[i].Cells[3].Value = d.ElementAt(i).Value;
            }
            MessageBox.Show("Done ♥");
        }

        private void button4_Click(object sender, EventArgs e)
        {
            SimulationSystem system = run.run();
            string test = Constants.FileNames.TestCase1;
            if (comboBox1.Text=="TestCase2")
                 test = Constants.FileNames.TestCase2;
            else if (comboBox1.Text == "TestCase3")
                test = Constants.FileNames.TestCase3;
            string result = TestingManager.Test(system, test);
            MessageBox.Show(result);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            dataGridView2.Rows.Clear();
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
