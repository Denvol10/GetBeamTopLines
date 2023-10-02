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
using System.Windows.Input;
using GetBeamTopLines.Infrastructure;
using GetBeamTopLines.Models;

namespace GetBeamTopLines.ViewModels
{
    internal class MainWindowViewModel : Base.ViewModel
    {
        private RevitModelForfard _revitModel;

        internal RevitModelForfard RevitModel
        {
            get => _revitModel;
            set => _revitModel = value;
        }

        #region Заголовок
        private string _title = "Линии верха балок";

        public string Title
        {
            get => _title;
            set => Set(ref _title, value);
        }
        #endregion

        #region Список семейств и их типоразмеров
        private ObservableCollection<FamilySymbolSelector> _structuralFramingFamilySymbols = new ObservableCollection<FamilySymbolSelector>();
        public ObservableCollection<FamilySymbolSelector> StructuralFramingFamilySymbols
        {
            get => _structuralFramingFamilySymbols;
            set => Set(ref _structuralFramingFamilySymbols, value);
        }
        #endregion

        #region Выбранный типоразмер семейства
        private FamilySymbolSelector _familySymbolName;
        public FamilySymbolSelector FamilySymbolName
        {
            get => _familySymbolName;
            set => Set(ref _familySymbolName, value);
        }
        #endregion

        #region Команды

        #region Сохранить линии верха балок в файл
        public ICommand SaveBeamLinesCommand { get; }

        private void OnSaveBeamLinesCommandExecuted(object parameter)
        {
            RevitModel.GetFamilyInstanceByFamilySymbol(FamilySymbolName);
        }

        private bool CanSaveBeamLinesCommandExecute(object parameter)
        {
            return true;
        }
        #endregion

        #endregion


        #region Конструктор класса MainWindowViewModel
        public MainWindowViewModel(RevitModelForfard revitModel)
        {
            RevitModel = revitModel;

            StructuralFramingFamilySymbols = RevitModel.GetFamilySymbolNames();

            #region Команды

            SaveBeamLinesCommand = new LambdaCommand(OnSaveBeamLinesCommandExecuted, CanSaveBeamLinesCommandExecute);

            #endregion
        }

        public MainWindowViewModel() { }
        #endregion
    }
}
