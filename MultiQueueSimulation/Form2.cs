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
using MultiQueueModels;
using MultiQueueTesting;

namespace MultiQueueSimulation
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string path = @"../../TestCases/TestCase1.txt";
            string test = Constants.FileNames.TestCase1;
            if (comboBox1.Text == "TestCase2")
            { path = @"../../TestCases/TestCase2.txt"; test = Constants.FileNames.TestCase2; }
            else if (comboBox1.Text == "TestCase3")
            { path = @"../../TestCases/TestCase3.txt"; test = Constants.FileNames.TestCase3; }

            string[] Lines = File.ReadAllLines(path);

            Run run = new Run();
            string NumberOfServers="", StoppingNumber="", StoppingCriteria="", SelectionMethod="";
            string time="",prob="";
            int i = 0;
            int count = 0;
            for ( ; ;++i)
            {
                if (Lines[i] == "NumberOfServers")
                    NumberOfServers = Lines[i+1];
                else if (Lines[i] == "StoppingNumber")
                    StoppingNumber = Lines[i+1];
                else if (Lines[i] == "StoppingCriteria")
                    StoppingCriteria = Lines[i+1];
                else if (Lines[i] == "SelectionMethod")
                     SelectionMethod = Lines[i+1];
                else if (Lines[i] == "InterarrivalDistribution")
                {
                    i++;
                    while(Lines[i].Length!=0)
                    {
                        time += Lines[i][0]+"*";
                        prob += Lines[i].Substring(3) + "*";
                        count++;
                        i++;
                    }
                    i++;
                    break;
                }

            }
            run.Enter_Data(NumberOfServers, StoppingNumber, StoppingCriteria, SelectionMethod);
            Dictionary<string,string>d = run.Enter_InterarrivalDistribution(count,time.Split('*'),prob.Split('*'));
            i++;
            int temp = i;
            count = 0;
            // MessageBox.Show(Lines[i]);
            while (Lines[i].Length != 0)
            {
                count++;
                i++;
            }

            i = temp;
            for (;i<Lines.Length;)
            {
                time = "";
                prob = "";
                for (int m =0;m<count;i++,++m)
                {
                    time += Lines[i][0] + "*";
                    prob += Lines[i].Substring(3) + "*";
                }
                i+=2;
                d = run.Add_ServiceDistribution(count,time.Split('*'),prob.Split('*'));
            }
            
            SimulationSystem system = run.run();
            string result = TestingManager.Test(system, test);
            MessageBox.Show(result);
            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Hide();          
            Form1 f = new Form1();
            f.Show();
        }
    }
}
