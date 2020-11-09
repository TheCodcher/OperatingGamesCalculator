using System;
using System.CodeDom;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Markup;
using Microsoft.Win32;
using Newtonsoft.Json;

namespace OpGameCalc
{
    class DataLoader : IDataLoader
    {
        public const string Extention = "opgame";
        public string FileFormat { get; private set; } = $"*.{Extention}";
        public string FilePath { get; private set; } = null;
        public bool LoadData(out IGameDataModel model)
        {
            model = null;
            var open = new OpenFileDialog() { Filter = $"Game Data File (*.opgame)|*.{Extention}|Data File(*.json)|*.json|All File(*.*) | *.*" };
            if (!(bool)open.ShowDialog())
                return false;
            FileFormat = "*" + open.FileName.Substring(open.FileName.LastIndexOf('.'));
            FilePath = open.FileName;
            var Loadtext = "";
            using (var s = open.OpenFile())
            {
                Loadtext = new StreamReader(s).ReadToEnd();
            }
            var load = JsonConvert.DeserializeObject<SaveFile>(Loadtext);
            model = load;
            if (model == null)
                return false;
            else
                return true;
        }

        public bool SaveData(IGameDataModel date)
        {
            var res = JsonConvert.SerializeObject(date);
            if (FilePath == null)
            {
                var save = new SaveFileDialog() { Filter = "Сохранить как |" + FileFormat };
                if ((bool)save.ShowDialog())
                {
                    FilePath = save.FileName;
                }
                else
                {
                    return false;
                }
            }
            using (var s = new StreamWriter(File.Open(FilePath, FileMode.Create)))
            {
                s.Write(res);
            }
            return true;
        }
    }
    class SaveFile : IGameDataModel
    {

        public List<SaveFlankInstance> FlanksCollection { get; set; }

        public List<SaveTypeInstance> TypesCollection { get; set; }

        public List<SaveResultInstance> ResultCollection { get; set; }

        IEnumerable<IFlank> IGameDataModel.FlanksCollection => FlanksCollection;

        IEnumerable<IDefType> IGameDataModel.TypesCollection => TypesCollection;

        IEnumerable<IGameResultItem> IGameDataModel.ResultCollection => ResultCollection;

        public double DefensePotential { get; set; }

        public SaveFile() { }
    }
    class SaveFlankInstance : IFlank
    {
        public double Rate { get; set; }
        public string Name { get; set; }
    }
    class SaveTypeInstance : IDefType
    {
        public double Efficiency { get; set; }
        public double Count { get; set; }
        public string Name { get; set; }
    }
    class SaveResultInstance : IGameResultItem
    {
        public string FlankName { get; set; }
        public string TypeName { get; set; }
        public double Result { get; set; }
    }
}
