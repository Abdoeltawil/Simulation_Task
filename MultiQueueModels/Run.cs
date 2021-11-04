using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiQueueModels
{
    public class Run
    {
        SimulationSystem system = new SimulationSystem();
        TimeDistribution timed;
        public void Enter_Data(string NumberOfServers,string StoppingNumber , string StoppingCriteria,string SelectionMethod)
        {
            system.NumberOfServers = Convert.ToInt32(NumberOfServers);
            system.StoppingNumber = Convert.ToInt32(StoppingNumber);
            if (StoppingCriteria == "1")
                system.StoppingCriteria = Enums.StoppingCriteria.NumberOfCustomers;
            else if (StoppingCriteria == "2")
                system.StoppingCriteria = Enums.StoppingCriteria.SimulationEndTime;
            if (SelectionMethod == "1")
                system.SelectionMethod = Enums.SelectionMethod.HighestPriority;
            else if (SelectionMethod == "2")
                system.SelectionMethod = Enums.SelectionMethod.Random;
            if (SelectionMethod == "3")
                system.SelectionMethod = Enums.SelectionMethod.LeastUtilization;
        }
        public Dictionary<string,string> Enter_InterarrivalDistribution(int count , string [] time ,string[] prob )
        {
            for (int i = 0; i < count; ++i)
            {
                timed = new TimeDistribution();
                timed.Time = Convert.ToInt32(time[i]);
                timed.Probability = Convert.ToDecimal(prob[i]);
                system.InterarrivalDistribution.Add(timed);

            }
            decimal sum = 0.00m;
            string s2;
            Dictionary<string, string> d = new Dictionary<string,string>();
            for (int i = 0; i < system.InterarrivalDistribution.Count; i++)
            {
                s2 = " - ";
                system.InterarrivalDistribution[i].MinRange = (int)(sum * 100) + 1;
                s2 = system.InterarrivalDistribution[i].MinRange.ToString() + s2;
                sum += system.InterarrivalDistribution[i].Probability;
                system.InterarrivalDistribution[i].CummProbability = sum;
                system.InterarrivalDistribution[i].MaxRange = (int)(sum * 100);
                s2 = s2 + system.InterarrivalDistribution[i].MaxRange.ToString();
                d[sum.ToString()]= s2;
                Console.WriteLine(system.InterarrivalDistribution[i].CummProbability);
                Console.WriteLine(system.InterarrivalDistribution[i].MinRange);
                Console.WriteLine(system.InterarrivalDistribution[i].MaxRange);
            }
            return d; 
        }
        public Dictionary<string, string> Add_ServiceDistribution(int count , string [] time, string []prob)
        {
            Server s = new Server();
            for (int i = 0; i < count ; ++i)
            {
                timed = new TimeDistribution();
                timed.Time = Convert.ToInt32(time[i]);
                timed.Probability = Convert.ToDecimal(prob[i]);
                s.TimeDistribution.Add(timed);
            }
            system.Servers.Add(s);
            int siz = system.Servers.Count;
            system.Servers[siz - 1].ID = siz;
            decimal sum = 0.00m;
            Dictionary<string, string> d = new Dictionary<string, string>();
            string s1;
            for (int j = 0; j < system.Servers[siz - 1].TimeDistribution.Count; j++)
            {
                s1 = " - ";
                system.Servers[siz - 1].TimeDistribution[j].MinRange = (int)(sum * 100) + 1;
                s1 = system.Servers[siz - 1].TimeDistribution[j].MinRange.ToString() + s1;
                sum += system.Servers[siz - 1].TimeDistribution[j].Probability;                
                system.Servers[siz - 1].TimeDistribution[j].CummProbability = sum;
                system.Servers[siz - 1].TimeDistribution[j].MaxRange = (int)(sum * 100);
                s1 = s1 + system.Servers[siz - 1].TimeDistribution[j].MaxRange.ToString();
                d[sum.ToString()] = s1;
                Console.WriteLine(system.Servers[siz - 1].TimeDistribution[j].CummProbability);
                Console.WriteLine(system.Servers[siz - 1].TimeDistribution[j].MinRange);
                Console.WriteLine(system.Servers[siz - 1].TimeDistribution[j].MaxRange);
            }
            return d;
        }
        public SimulationSystem run ()
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
                try
                {
                    system.Servers[i].IdleProbability = (system.SimulationTable[system.StoppingNumber - 1].EndTime - system.Servers[i].TotalWorkingTime) / (decimal)system.SimulationTable[system.StoppingNumber - 1].EndTime;
                }
                catch (Exception ee) { system.Servers[i].IdleProbability = 0; }
                try
                {
                    system.Servers[i].AverageServiceTime = (system.Servers[i].TotalWorkingTime) / numofcustomer[i];
                }
                catch (Exception eee) { system.Servers[i].AverageServiceTime = 0; }
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
            return system;
        }
    }

}
