using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using MultiQueueTesting;
using MultiQueueModels;
using System.IO;

namespace MultiQueueSimulation
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            
            SimulationSystem system = new SimulationSystem();
            system.NumberOfServers = 2;
            system.StoppingNumber = 100;
            system.StoppingCriteria = Enums.StoppingCriteria.NumberOfCustomers;
            system.SelectionMethod = Enums.SelectionMethod.HighestPriority;
            TimeDistribution  timed =new TimeDistribution();
            timed.Time = 1;
            timed.Probability =0.25m;
      
            system.InterarrivalDistribution.Add(timed);
            timed = new TimeDistribution();
            timed.Time = 2;
            timed.Probability = 0.40m;
            system.InterarrivalDistribution.Add(timed);
            timed = new TimeDistribution();
            timed.Time = 3;
            timed.Probability = 0.20m;
            system.InterarrivalDistribution.Add(timed);
            timed = new TimeDistribution();
            timed.Time = 4;
            timed.Probability = 0.15m;
            system.InterarrivalDistribution.Add(timed);

            Server s = new Server();
            timed = new TimeDistribution();
            timed.Time = 2;
            timed.Probability = 0.30m;
            s.TimeDistribution.Add(timed);
            timed = new TimeDistribution();
            timed.Time = 3;
            timed.Probability = 0.28m;
            s.TimeDistribution.Add(timed);
            timed = new TimeDistribution();
            timed.Time = 4;
            timed.Probability = 0.25m;
            s.TimeDistribution.Add(timed);
            timed = new TimeDistribution();
            timed.Time = 5;
            timed.Probability = 0.17m;
            s.TimeDistribution.Add(timed);
            system.Servers.Add(s);
            system.Servers[0].ID = 1;
            s = new Server();
            timed = new TimeDistribution();
            timed.Time = 3;
            timed.Probability = 0.35m;
            s.TimeDistribution.Add(timed);
            timed = new TimeDistribution();
            timed.Time = 4;
            timed.Probability = 0.25m;
            s.TimeDistribution.Add(timed);
            timed = new TimeDistribution();
            timed.Time = 5;
            timed.Probability = 0.20m;
            s.TimeDistribution.Add(timed);
            timed = new TimeDistribution();
            timed.Time = 6;
            timed.Probability = 0.20m;
            s.TimeDistribution.Add(timed);
            system.Servers.Add(s);
            system.Servers[1].ID = 2;
            decimal sum = 0.00m;
            for(int i=0;i<system.InterarrivalDistribution.Count;i++)
            {
                system.InterarrivalDistribution[i].MinRange = (int)(sum * 100)+1;
                sum += system.InterarrivalDistribution[i].Probability;
                system.InterarrivalDistribution[i].CummProbability = sum;
                system.InterarrivalDistribution[i].MaxRange = (int)(sum * 100);
                Console.WriteLine(system.InterarrivalDistribution[i].CummProbability);
                Console.WriteLine(system.InterarrivalDistribution[i].MinRange);
                Console.WriteLine(system.InterarrivalDistribution[i].MaxRange);
            }
            for (int i = 0; i < system.Servers.Count; i++)
            {
                sum = 0.00m;
                for (int j = 0; j < system.Servers[i].TimeDistribution.Count; j++)
                {
                    system.Servers[i].TimeDistribution[j].MinRange = (int)(sum * 100) + 1;
                    sum += system.Servers[i].TimeDistribution[j].Probability;
                    system.Servers[i].TimeDistribution[j].CummProbability = sum;
                    system.Servers[i].TimeDistribution[j].MaxRange = (int)(sum * 100);
                    Console.WriteLine(system.Servers[i].TimeDistribution[j].CummProbability);
                    Console.WriteLine(system.Servers[i].TimeDistribution[j].MinRange);
                    Console.WriteLine(system.Servers[i].TimeDistribution[j].MaxRange);
                }
            }
            decimal sumqu = 0.0m, countofq = 0.0m;
            decimal[] numofcustomer = new decimal[system.NumberOfServers];
            Queue<decimal> startcus = new Queue<decimal>(), endcus = new Queue<decimal>();
            int [] endserver = new int[system.StoppingNumber];
            for (int i=0;i< system.StoppingNumber; i++)
            {
                system.SimulationTable.Add(new SimulationCase(0, 0, 0, 0, 0, 0, null, 0, 0, 0));
                system.SimulationTable[i].CustomerNumber = i+1;
                system.SimulationTable[i].RandomInterArrival = new Random().Next(1,100);
                for (int k = 0; k < system.InterarrivalDistribution.Count; k++)
                {
                    if (system.SimulationTable[i].RandomInterArrival >= system.InterarrivalDistribution[k].MinRange && system.SimulationTable[i].RandomInterArrival <= system.InterarrivalDistribution[k].MaxRange)
                        system.SimulationTable[i].InterArrival = system.InterarrivalDistribution[k].Time;
                }
                if (i == 0)
                {
                    system.SimulationTable[i].ArrivalTime = 0;
                }
                else
                {
                    system.SimulationTable[i].ArrivalTime = system.SimulationTable[i - 1].ArrivalTime + system.SimulationTable[i].InterArrival;
                }
                int min=1000;
                    
                for (int l = 0; l < system.Servers.Count; l++)
                {
                    if (system.SimulationTable[i].ArrivalTime >= system.Servers[l].FinishTime)
                    {
                        system.SimulationTable[i].AssignedServer = system.Servers[l];
                        
                        system.SimulationTable[i].StartTime =
                        system.SimulationTable[i].ArrivalTime;
                        system.SimulationTable[i].TimeInQueue = 0;
                        break;
                        
                    }
                  if(system.Servers[l].FinishTime<min)
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
                numofcustomer[system.SimulationTable[i].AssignedServer.ID - 1] = numofcustomer[system.SimulationTable[i].AssignedServer.ID - 1]+1;
                sumqu += system.SimulationTable[i].TimeInQueue;
                if (system.SimulationTable[i].TimeInQueue > 0)
                {
                    countofq++;
                    startcus.Enqueue(system.SimulationTable[i].ArrivalTime);
                    endcus.Enqueue(system.SimulationTable  [i].StartTime);
                }
               
            }
            for (int i = 0; i < system.NumberOfServers; i++)
            {
                system.Servers[i].IdleProbability = (system.SimulationTable[system.StoppingNumber - 1].EndTime - system.Servers[i].TotalWorkingTime) / (decimal)system.SimulationTable[system.StoppingNumber - 1].EndTime;
                system.Servers[i].AverageServiceTime = (system.Servers[i].TotalWorkingTime) / numofcustomer[i];
                system.Servers[i].Utilization = (system.Servers[i].TotalWorkingTime) / (decimal)system.SimulationTable[system.StoppingNumber - 1].EndTime;
            }
            int count=0;
            while(startcus.Count!=0&& endcus.Count != 0)
            {
                
                    if(startcus.First()<endcus.First())
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
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
           
        }
    }
}
