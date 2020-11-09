using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace OpGameCalc
{
    class CppOpGameCalcInct : IGameCalculator
    {
        public double DefensePotential { get; set; } = 0;
        double AllChance = 0;
        IEnumerable<IDefType> types;
        IEnumerable<IFlank> flanks;
        public double AssuredResult(double attackCount)
        {
            //Thread.Sleep(5000);
            return Math.Max(0, attackCount - DefensePotential);
        }

        public IEnumerable<IGameResultItem> Calculate()
        {
            //Thread.Sleep(5000);
            var result = new List<SaveResultInstance>();
            foreach(var f in flanks)
            {
                foreach(var t in types)
                {
                    double res = f.Rate * t.Count * AllChance;
                    result.Add(new SaveResultInstance() { FlankName = f.Name, TypeName = t.Name, Result = res });
                }
            }
            return result;
        }

        public void Inicialize(IEnumerable<IFlank> flanksCollection, IEnumerable<IDefType> typesCollection)
        {
            var tempFlanks = new List<IFlank>();
            double tempChance = 0;
            foreach(var flank in flanksCollection)
            {
                double temp = 1 / flank.Rate;
                tempChance += temp;
                tempFlanks.Add(new SaveFlankInstance() { Name = flank.Name, Rate = temp });
            }
            double tempMaxDef = 0;
            foreach(var ty in typesCollection)
            {
                tempMaxDef += ty.Efficiency * ty.Count;
            }
            AllChance = 1 / tempChance;
            DefensePotential = tempMaxDef / tempChance;
            types = typesCollection;
            flanks = tempFlanks;
        }
    }
}
