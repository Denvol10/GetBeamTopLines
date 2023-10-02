using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using Autodesk.Revit.DB.Architecture;
using System.Collections.ObjectModel;
using GetBeamTopLines.Models;
using System.IO;

namespace GetBeamTopLines
{
    public class RevitModelForfard
    {
        private UIApplication Uiapp { get; set; } = null;
        private Application App { get; set; } = null;
        private UIDocument Uidoc { get; set; } = null;
        private Document Doc { get; set; } = null;

        public RevitModelForfard(UIApplication uiapp)
        {
            Uiapp = uiapp;
            App = uiapp.Application;
            Uidoc = uiapp.ActiveUIDocument;
            Doc = uiapp.ActiveUIDocument.Document;
        }

        private List<FamilyInstance> BeamInstances { get; set; }

        private int _selectedFaceId;
        private int SelectedFaceId
        {
            get => _selectedFaceId;
            set => _selectedFaceId = value;
        }

        #region Получение списка названий типоразмеров семейств
        public ObservableCollection<FamilySymbolSelector> GetFamilySymbolNames()
        {
            var familySymbolNames = new ObservableCollection<FamilySymbolSelector>();
            var allFamilies = new FilteredElementCollector(Doc).OfClass(typeof(Family)).OfType<Family>();
            var structuralFramingFamilies = allFamilies.Where(f => f.FamilyCategory.Id.IntegerValue
                                                              == (int)BuiltInCategory.OST_StructuralFraming);
            if (structuralFramingFamilies.Count() == 0)
                return familySymbolNames;

            foreach (var family in structuralFramingFamilies)
            {
                foreach (var symbolId in family.GetFamilySymbolIds())
                {
                    var familySymbol = Doc.GetElement(symbolId);
                    familySymbolNames.Add(new FamilySymbolSelector(family.Name, familySymbol.Name));
                }
            }

            return familySymbolNames;
        }
        #endregion

        #region Получение всех экземпляров семейств выбранного типоразмера
        public void GetFamilyInstanceByFamilySymbol(FamilySymbolSelector familySymbolName)
        {
            FamilySymbol familySymbol = GetFamilySymbolByName(familySymbolName);
            var structuralFramingElements = new FilteredElementCollector(Doc).OfCategory(BuiltInCategory.OST_StructuralFraming)
                                                                             .ToElements()
                                                                             .OfType<FamilyInstance>();

            var selectBeamElements = structuralFramingElements.Where(e => e.Symbol.Id.IntegerValue == familySymbol.Id.IntegerValue);

            BeamInstances = selectBeamElements.ToList();
        }
        #endregion



        #region Получение типоразмера по имени
        private FamilySymbol GetFamilySymbolByName(FamilySymbolSelector familyAndSymbolName)
        {
            var familyName = familyAndSymbolName.FamilyName;
            var symbolName = familyAndSymbolName.SymbolName;

            Family family = new FilteredElementCollector(Doc).OfClass(typeof(Family)).Where(f => f.Name == familyName).First() as Family;
            var symbolIds = family.GetFamilySymbolIds();
            foreach (var symbolId in symbolIds)
            {
                FamilySymbol fSymbol = (FamilySymbol)Doc.GetElement(symbolId);
                if (fSymbol.get_Parameter(BuiltInParameter.SYMBOL_NAME_PARAM).AsString() == symbolName)
                {
                    return fSymbol;
                }
            }
            return null;
        }
        #endregion

        #region Получение Id выбранной грани
        public void GetFaceIdBySelection()
        {
            Selection sel = Uiapp.ActiveUIDocument.Selection;
            Reference selectedFace = sel.PickObject(ObjectType.Face, "Выберете верх балки");
            string stableRepresentation = selectedFace.ConvertToStableRepresentation(Doc);
            string[] representInfo = stableRepresentation.Split(':');
            SelectedFaceId = int.Parse(representInfo.ElementAt(representInfo.Length - 2));

            string resultPath = @"O:\Revit Infrastructure Tools\GetBeamTopLines\GetBeamTopLines\result.txt";
            using(StreamWriter sw = new StreamWriter(resultPath, false, Encoding.Default))
            {
                sw.WriteLine(SelectedFaceId);
            }
        }
        #endregion
    }
}
