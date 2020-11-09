using System;
using System.Collections.Generic;
using System.Text;

namespace OpGameCalc
{
    interface INamed
    {
        string Name { get; set; }
    }
    interface IFlank : INamed
    {
        double Rate { get; set; }
    }
    interface IDefType : INamed
    {
        public double Efficiency { get; set; }
        public double Count { get; set; }
    }
    interface IGameResultItem
    {
        string FlankName { get; set; }
        string TypeName { get; set; }
        double Result { get; set; }
    }

    interface IGameDataModel
    {
        IEnumerable<IFlank> FlanksCollection { get; }
        IEnumerable<IDefType> TypesCollection { get; }
        IEnumerable<IGameResultItem> ResultCollection { get; }
        double DefensePotential { get; }
    }
    interface IDataLoader
    {
        bool LoadData(out IGameDataModel model);
        bool SaveData(IGameDataModel date);
    }
    interface IGameCalculator
    {
        void Inicialize(IEnumerable<IFlank> flanksCollection, IEnumerable<IDefType> typesCollection);
        IEnumerable<IGameResultItem> Calculate();
        double DefensePotential { get; set; }
        double AssuredResult(double attackCount);
    }
}
