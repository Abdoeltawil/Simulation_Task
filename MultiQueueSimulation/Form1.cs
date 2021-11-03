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
        SimulationSystem system = new SimulationSystem();
        TimeDistribution timed;
        public Form1()
        {
            InitializeComponent();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            system.NumberOfServers = Convert.ToInt32(textBox1.Text);
            system.StoppingNumber = Convert.ToInt32(textBox2.Text);
            if(textBox3.Text == "1")
                system.StoppingCriteria = Enums.StoppingCriteria.NumberOfCustomers;
            else if (textBox3.Text == "2")
                system.StoppingCriteria = Enums.StoppingCriteria.SimulationEndTime;
            if (textBox4.Text == "1")
                system.SelectionMethod = Enums.SelectionMethod.HighestPriority;
            else if (textBox4.Text == "2")
                system.SelectionMethod = Enums.SelectionMethod.Random;
            if (textBox4.Text == "3")
                system.SelectionMethod = Enums.SelectionMethod.LeastUtilization;

            MessageBox.Show("Done ♥");

        }

        private void button2_Click(object sender, EventArgs e)
        {
            //dataGrid.Rows[rows].Cells[col].Value.ToString()      
            //
            for (int i =0;i<dataGridView1.Rows.Count-1;++i)
            {
                    timed = new TimeDistribution();
                    timed.Time = Convert.ToInt32(dataGridView1.Rows[i].Cells[0].Value);
                    timed.Probability = Convert.ToDecimal(dataGridView1.Rows[i].Cells[1].Value);
                    system.InterarrivalDistribution.Add(timed);
                
            }
            decimal sum = 0.00m;
            string s2;
            for (int i = 0; i < system.InterarrivalDistribution.Count; i++)
            {
                s2 = " - ";
                system.InterarrivalDistribution[i].MinRange = (int)(sum * 100) + 1;
                s2 = system.InterarrivalDistribution[i].MinRange.ToString()+ s2;
                sum += system.InterarrivalDistribution[i].Probability;
                system.InterarrivalDistribution[i].CummProbability = sum;
                dataGridView1.Rows[i].Cells[2].Value = sum.ToString() ;
                system.InterarrivalDistribution[i].MaxRange = (int)(sum * 100);
                s2 = s2 + system.InterarrivalDistribution[i].MaxRange.ToString();
                dataGridView1.Rows[i].Cells[3].Value = s2;
                Console.WriteLine(system.InterarrivalDistribution[i].CummProbability);
                Console.WriteLine(system.InterarrivalDistribution[i].MinRange);
                Console.WriteLine(system.InterarrivalDistribution[i].MaxRange);
            }

            MessageBox.Show("Done ♥");
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Server s = new Server();
            for(int i=0;i<dataGridView2.Rows.Count-1;++i)
            {
                    timed = new TimeDistribution();
                    timed.Time = Convert.ToInt32(dataGridView2.Rows[i].Cells[0].Value.ToString());
                    timed.Probability = Convert.ToDecimal(dataGridView2.Rows[i].Cells[1].Value.ToString());
                    s.TimeDistribution.Add(timed);                
            }
            system.Servers.Add(s);
            int siz = system.Servers.Count;
            system.Servers[siz-1].ID =siz ;
            decimal sum = 0.00m;
          
            string s1 ;
            for (int j = 0; j < system.Servers[siz - 1].TimeDistribution.Count; j++)
            {
                s1 = " - ";
                system.Servers[siz-1].TimeDistribution[j].MinRange = (int)(sum * 100) + 1;
                s1 = system.Servers[siz - 1].TimeDistribution[j].MinRange.ToString() + s1;
                sum += system.Servers[siz - 1].TimeDistribution[j].Probability;
                dataGridView2.Rows[j].Cells[2].Value = sum.ToString();
                system.Servers[siz - 1].TimeDistribution[j].CummProbability = sum;
                system.Servers[siz - 1].TimeDistribution[j].MaxRange = (int)(sum * 100);
                s1 = s1 + system.Servers[siz - 1].TimeDistribution[j].MaxRange.ToString();
                dataGridView2.Rows[j].Cells[3].Value = s1;
                Console.WriteLine(system.Servers[siz - 1].TimeDistribution[j].CummProbability);
                Console.WriteLine(system.Servers[siz - 1].TimeDistribution[j].MinRange);
                Console.WriteLine(system.Servers[siz - 1].TimeDistribution[j].MaxRange);
            }
            MessageBox.Show("Done ♥");
        }

        private void button4_Click(object sender, EventArgs e)
        {
            decimal sumqu = 0.0m, countofq = 0.0m;
            decimal[] numofcustomer = new decimal[system.NumberOfServers];
            Queue<decimal> startcus = new Queue<decimal>(), endcus = new Queue<decimal>();
            int[] endserver = new int[system.StoppingNumber];
            for (int i = 0; i < system.StoppingNumber; i++)
            {
                system.SimulationTable.Add(new SimulationCase(0, 0, 0, 0, 0, 0, null, 0, 0, 0));
                system.SimulationTable[i].CustomerNumber = i + 1;
                system.SimulationTable[i].RandomInterArrival = new Random().Next(1, 100);
                for (int k = 0; k < system.InterarrivalDistribution.Count; k++)
                {
                    if (system.SimulationTable[i].RandomInterArrival >= system.InterarrivalDistribution[k].MinRange && system.SimulationTable[i].RandomInterArrival <= system.InterarrivalDistribution[k].MaxRange)
                        system.SimulationTable[i].InterArrival = system.InterarrivalDistribution[k].Time;
                }
                if (i != 0)
                {
                    system.SimulationTable[i].ArrivalTime = system.SimulationTable[i - 1].ArrivalTime + system.SimulationTable[i].InterArrival;
                }


                int min = 99999999;
                for (int l = 0; l < system.Servers.Count; l++)
                {
                    if (system.SimulationTable[i].ArrivalTime >= system.Servers[l].FinishTime)
                    {
                        system.SimulationTable[i].AssignedServer = system.Servers[l];
                        system.SimulationTable[i].StartTime = system.SimulationTable[i].ArrivalTime;
                        system.SimulationTable[i].TimeInQueue = 0;
                        break;

                    }
                    if (system.Servers[l].FinishTime < min)
                    {
                        min = system.Servers[l].FinishTime;
                        system.SimulationTable[i].AssignedServer = system.Servers[l];
                        system.SimulationTable[i].StartTime = system.SimulationTable[i].AssignedServer.FinishTime;
                        system.SimulationTable[i].TimeInQueue = system.SimulationTable[i].AssignedServer.FinishTime - system.SimulationTable[i].ArrivalTime;

                    }
                }

                system.SimulationTable[i].RandomService = new Random().Next(1, 100);
                for (int k = 0; k < system.InterarrivalDistribution.Count; k++)
                {
                    if (system.SimulationTable[i].RandomService >= system.SimulationTable[i].AssignedServer.TimeDistribution[k].MinRange && system.SimulationTable[i].RandomInterArrival <= system.SimulationTable[i].AssignedServer.TimeDistribution[k].MaxRange)
                        system.SimulationTable[i].ServiceTime = system.SimulationTable[i].AssignedServer.TimeDistribution[k].Time;
                }

                system.SimulationTable[i].EndTime = system.SimulationTable[i].StartTime + system.SimulationTable[i].ServiceTime;
                system.SimulationTable[i].AssignedServer.FinishTime = system.SimulationTable[i].EndTime;
                system.SimulationTable[i].AssignedServer.TotalWorkingTime += system.SimulationTable[i].ServiceTime;
                numofcustomer[system.SimulationTable[i].AssignedServer.ID - 1] = numofcustomer[system.SimulationTable[i].AssignedServer.ID - 1] + 1;
                sumqu += system.SimulationTable[i].TimeInQueue;
                if (system.SimulationTable[i].TimeInQueue > 0)
                {
                    countofq++;
                    startcus.Enqueue(system.SimulationTable[i].ArrivalTime);
                    endcus.Enqueue(system.SimulationTable[i].StartTime);
                }
                Console.WriteLine(system.SimulationTable[i].CustomerNumber + " " + system.SimulationTable[i].AssignedServer.ID);

            }
            for (int i = 0; i < system.NumberOfServers; i++)
            {
                system.Servers[i].IdleProbability = (system.SimulationTable[system.StoppingNumber - 1].EndTime - system.Servers[i].TotalWorkingTime) / (decimal)system.SimulationTable[system.StoppingNumber - 1].EndTime;
                system.Servers[i].AverageServiceTime = (system.Servers[i].TotalWorkingTime) / numofcustomer[i];
                system.Servers[i].Utilization = (system.Servers[i].TotalWorkingTime) / (decimal)system.SimulationTable[system.StoppingNumber - 1].EndTime;
            }
            int count = 0;
            while (startcus.Count != 0 && endcus.Count != 0)
            {

                if (startcus.First() < endcus.First())
                {
                    count++;
                    startcus.Dequeue();
                }
                else
                {

                    endcus.Dequeue();
                    count--;
                }


            }

            system.PerformanceMeasures.AverageWaitingTime = (sumqu / system.StoppingNumber);
            system.PerformanceMeasures.WaitingProbability = countofq / system.StoppingNumber;
            system.PerformanceMeasures.MaxQueueLength = count;
            string result = TestingManager.Test(system, Constants.FileNames.TestCase1);
            MessageBox.Show(result);


        }

        private void button5_Click(object sender, EventArgs e)
        {
            dataGridView2.Rows.Clear();
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
