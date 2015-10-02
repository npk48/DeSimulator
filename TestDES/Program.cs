using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using React;
using React.Distribution;
using React.Tasking;
using React.Monitoring;

namespace TestDES
{
    using DeSimulator;
    public class Program
    {
        static void Main(string[] args)
        {
            Simulator Sim = new Simulator();
            Sim.Run(new Process(Sim, Sim.Generator));
            Console.ReadKey();
        }
    }
}
