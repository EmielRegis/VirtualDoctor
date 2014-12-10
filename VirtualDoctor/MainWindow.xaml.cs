using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using ValidationResult = System.Windows.Controls.ValidationResult;

namespace VirtualDoctor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private DiseaseDatabaseEntities dbContextSymptomCathegory;
        private DiseaseDatabaseEntities dbContextSymptom;
        private DiseaseDatabaseEntities dbContextDisease;
        private DiseaseDatabaseEntities dbContextDiseaseCorelation;
        private DiseaseDatabaseEntities dbContextConcreteSymptomDiseaseConnection;
        private CollectionViewSource _symptomCathegoryViewSource;
        private CollectionViewSource _symptomCathegoriesList;
        private CollectionViewSource _symptomViewSource;
        private CollectionViewSource _symptomCathegoriesListRemote;
        private CollectionViewSource _diseaseViewSource;
        private CollectionViewSource _diseaseCorelationViewSource;
        private CollectionViewSource _diseasesListRemote;
        private CollectionViewSource _concreteDiseasesListRemote;
        private CollectionViewSource _concreteSymptomsListRemote;
        private CollectionViewSource _concreteSymptomDiseaseConnectionViewSource;


        public MainWindow()
        {
            InitializeComponent();
            dbContextSymptomCathegory = new DiseaseDatabaseEntities();
            dbContextSymptom = new DiseaseDatabaseEntities();
            dbContextDisease = new DiseaseDatabaseEntities();
            dbContextDiseaseCorelation = new DiseaseDatabaseEntities();
            dbContextConcreteSymptomDiseaseConnection = new DiseaseDatabaseEntities();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            dbContextSymptomCathegory.SymptomCathegories.Load();
            dbContextSymptom.Symptoms.Load();
            dbContextSymptom.SymptomCathegories.Load();
            dbContextDisease.Diseases.Load();
            dbContextDiseaseCorelation.DiseaseCorelations.Load();
            dbContextDiseaseCorelation.Diseases.Load();
            dbContextConcreteSymptomDiseaseConnection.ConcreteSymptomDiseaseConnections.Load();
            dbContextConcreteSymptomDiseaseConnection.Diseases.Load();
            dbContextConcreteSymptomDiseaseConnection.Symptoms.Load();


            _symptomCathegoryViewSource = ((CollectionViewSource)(FindResource("symptomCathegoryViewSource")));          
            _symptomCathegoryViewSource.Source = dbContextSymptomCathegory.SymptomCathegories.Local;

            _symptomCathegoriesList = ((CollectionViewSource)(FindResource("symptomCathegoriesList")));
            _symptomCathegoriesList.Source = dbContextSymptomCathegory.SymptomCathegories.Local;

            _symptomCathegoriesListRemote = ((CollectionViewSource)(FindResource("symptomCathegoriesListRemote")));
            _symptomCathegoriesListRemote.Source = dbContextSymptom.SymptomCathegories.Local;

            _symptomViewSource = ((CollectionViewSource)(FindResource("symptomViewSource")));
            _symptomViewSource.Source = dbContextSymptom.Symptoms.Local;

            _diseaseViewSource = ((CollectionViewSource)(FindResource("diseaseViewSource")));
            _diseaseViewSource.Source = dbContextDisease.Diseases.Local;

            _diseaseCorelationViewSource = ((CollectionViewSource)(FindResource("diseaseCorelationViewSource")));
            _diseaseCorelationViewSource.Source = dbContextDiseaseCorelation.DiseaseCorelations.Local;

            _diseasesListRemote = ((CollectionViewSource)(FindResource("diseasesListRemote")));
            _diseasesListRemote.Source = dbContextDiseaseCorelation.Diseases.Local;

            _concreteSymptomDiseaseConnectionViewSource = ((CollectionViewSource)(FindResource("concreteSymptomDiseaseConnectionViewSource")));
            _concreteSymptomDiseaseConnectionViewSource.Source = dbContextConcreteSymptomDiseaseConnection.ConcreteSymptomDiseaseConnections.Local;

            _concreteDiseasesListRemote = ((CollectionViewSource)(FindResource("concreteDiseasesListRemote")));
            _concreteDiseasesListRemote.Source = dbContextConcreteSymptomDiseaseConnection.Diseases.Local;

            _concreteSymptomsListRemote = ((CollectionViewSource)(FindResource("concreteSymptomsListRemote")));
            _concreteSymptomsListRemote.Source = dbContextConcreteSymptomDiseaseConnection.Symptoms.Local;
        }
  
        private void SymptomCathegoryRowUpdate(object sender, DataGridRowEditEndingEventArgs dataGridRowEditEndingEventArgs)
        {   
            if (SymptomCathegoriesDataGrid.SelectedItem != null)
            {                
                SymptomCathegoriesDataGrid.RowEditEnding -= SymptomCathegoryRowUpdate;
                SymptomCathegoriesDataGrid.CommitEdit();               
//                (sender as DataGrid).Items.Refresh();
                SymptomCathegoriesDataGrid.RowEditEnding += SymptomCathegoryRowUpdate;
            }
            else {return;}

            var result = new SymptomCathegoryValidation().Validate(dataGridRowEditEndingEventArgs.Row.BindingGroup, CultureInfo.CurrentCulture);
            
            if (!result.IsValid)
            {
                SymptomCathegoriesSearchBox.IsEnabled = false;
                return;
            }

            SymptomCathegoriesSearchBox.IsEnabled = true;

            dbContextSymptomCathegory.SaveChanges();

            dbContextSymptom = new DiseaseDatabaseEntities();
            dbContextSymptom.SymptomCathegories.Load();
            _symptomCathegoriesListRemote.Source = dbContextSymptom.SymptomCathegories.Local;
            SymptomDataGrid.Items.Refresh();

            SymptomCathegoriesDataGrid.Items.Refresh();
        }



        private void SymptomCathegoryViewSource_Filter(object sender, FilterEventArgs e)
        {
            SymptomCathegory cat = e.Item as SymptomCathegory;
            if (cat != null)
            {
                e.Accepted = (cat.Name.Contains(SymptomCathegoriesSearchBox.Text));
            }
        }

        private void SymptomCathegoriesSearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            _symptomCathegoryViewSource.Filter -= SymptomCathegoryViewSource_Filter;
            _symptomCathegoryViewSource.Filter += SymptomCathegoryViewSource_Filter;
        }

       

        private void SymptomRowUpdate(object sender, DataGridRowEditEndingEventArgs e)
        {
            if (SymptomDataGrid.SelectedItem != null)
            {
                SymptomDataGrid.RowEditEnding -= SymptomRowUpdate;
                SymptomDataGrid.CommitEdit();
                SymptomDataGrid.RowEditEnding += SymptomRowUpdate;
            }
            else { return; }

            var result = new SymptomValidation().Validate(e.Row.BindingGroup, CultureInfo.CurrentCulture);

            if (!result.IsValid)
            {
                SymptomsSearchBox.IsEnabled = false;
                return;
            }

            SymptomsSearchBox.IsEnabled = true;

            dbContextSymptom.SaveChanges();

            dbContextConcreteSymptomDiseaseConnection = new DiseaseDatabaseEntities();
            dbContextConcreteSymptomDiseaseConnection.Symptoms.Load();
            _concreteSymptomsListRemote.Source = dbContextConcreteSymptomDiseaseConnection.Symptoms.Local;
            ConcreteSymptomDiseaseConnectionDataGrid.Items.Refresh();

            SymptomDataGrid.Items.Refresh();

        }

        private void SymptomViewSource_Filter(object sender, FilterEventArgs e)
        {
            Symptom cat = e.Item as Symptom;
            if (cat != null)
            {
                e.Accepted = (cat.Name.Contains(SymptomsSearchBox.Text));
            }
        }

        private void SymptomsSearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            _symptomViewSource.Filter -= SymptomViewSource_Filter;
            _symptomViewSource.Filter += SymptomViewSource_Filter;
        }

        private void DiseaseRowUpdate(object sender, DataGridRowEditEndingEventArgs e)
        {
            if (DiseaseDataGrid.SelectedItem != null)
            {
                DiseaseDataGrid.RowEditEnding -= DiseaseRowUpdate;
                DiseaseDataGrid.CommitEdit();
                DiseaseDataGrid.RowEditEnding += DiseaseRowUpdate;
            }
            else { return; }

            var result = new DiseaseValidation().Validate(e.Row.BindingGroup, CultureInfo.CurrentCulture);

            if (!result.IsValid)
            {
                DiseasesSearchBox.IsEnabled = false;
                return;
            }

            DiseasesSearchBox.IsEnabled = true;

            dbContextDisease.SaveChanges();

            dbContextDiseaseCorelation = new DiseaseDatabaseEntities();
            dbContextDiseaseCorelation.Diseases.Load();
            _diseasesListRemote.Source = dbContextDiseaseCorelation.Diseases.Local;
            DiseaseCorelationDataGrid.Items.Refresh();

            dbContextConcreteSymptomDiseaseConnection = new DiseaseDatabaseEntities();
            dbContextConcreteSymptomDiseaseConnection.Diseases.Load();
            _concreteDiseasesListRemote.Source = dbContextConcreteSymptomDiseaseConnection.Diseases.Local;
            ConcreteSymptomDiseaseConnectionDataGrid.Items.Refresh();

            DiseaseDataGrid.Items.Refresh();

        }

        private void DiseaseViewSource_Filter(object sender, FilterEventArgs e)
        {
            Disease cat = e.Item as Disease;
            if (cat != null)
            {
                e.Accepted = (cat.Name.Contains(DiseasesSearchBox.Text));
            }
        }

        private void DiseasesSearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            _diseaseViewSource.Filter -= DiseaseViewSource_Filter;
            _diseaseViewSource.Filter += DiseaseViewSource_Filter;
        }

        private void DiseaseCorelationRowUpdate(object sender, DataGridRowEditEndingEventArgs e)
        {
            if (DiseaseCorelationDataGrid.SelectedItem != null)
            {
                DiseaseCorelationDataGrid.RowEditEnding -= DiseaseCorelationRowUpdate;
                DiseaseCorelationDataGrid.CommitEdit();
                DiseaseCorelationDataGrid.RowEditEnding += DiseaseCorelationRowUpdate;
            }
            else { return; }

            var result = new DiseaseCorelationValidation().Validate(e.Row.BindingGroup, CultureInfo.CurrentCulture);

            if (!result.IsValid)
            {
                DiseaseCorelationsSearchBox.IsEnabled = false;
                return;
            }

            DiseaseCorelationsSearchBox.IsEnabled = true;

            dbContextDiseaseCorelation.SaveChanges();

            

            DiseaseCorelationDataGrid.Items.Refresh();

        }

        private void DiseaseCorelationViewSource_Filter(object sender, FilterEventArgs e)
        {
            DiseaseCorelation cat = e.Item as DiseaseCorelation;
            if (cat != null)
            {
                e.Accepted =
                    (dbContextDiseaseCorelation.Diseases.First(s => s.Id == cat.DiseaseA)
                        .Name.Contains(DiseaseCorelationsSearchBox.Text)) ||
                    (dbContextDiseaseCorelation.Diseases.First(s => s.Id == cat.DiseaseB)
                        .Name.Contains(DiseaseCorelationsSearchBox.Text));
            }
        }

        private void DiseaseCorelationsSearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            _diseaseCorelationViewSource.Filter -= DiseaseCorelationViewSource_Filter;
            _diseaseCorelationViewSource.Filter += DiseaseCorelationViewSource_Filter;
        }



        private void ConcreteSymptomDiseaseConnectionRowUpdate(object sender, DataGridRowEditEndingEventArgs e)
        {
            if (ConcreteSymptomDiseaseConnectionDataGrid.SelectedItem != null)
            {
                ConcreteSymptomDiseaseConnectionDataGrid.RowEditEnding -= ConcreteSymptomDiseaseConnectionRowUpdate;
                ConcreteSymptomDiseaseConnectionDataGrid.CommitEdit();
                ConcreteSymptomDiseaseConnectionDataGrid.RowEditEnding += ConcreteSymptomDiseaseConnectionRowUpdate;
            }
            else { return; }

            var result = new ConcreteSymptomDiseaseConnectionValidation().Validate(e.Row.BindingGroup, CultureInfo.CurrentCulture);

            if (!result.IsValid)
            {
                ConcreteSymptomDiseaseConnectionsSearchBox.IsEnabled = false;
                return;
            }

            ConcreteSymptomDiseaseConnectionsSearchBox.IsEnabled = true;

            dbContextConcreteSymptomDiseaseConnection.SaveChanges();

            ConcreteSymptomDiseaseConnectionDataGrid.Items.Refresh();

        }

        private void ConcreteSymptomDiseaseConnectionViewSource_Filter(object sender, FilterEventArgs e)
        {
            ConcreteSymptomDiseaseConnection cat = e.Item as ConcreteSymptomDiseaseConnection;
            if (cat != null)
            {
                e.Accepted =
                    (dbContextConcreteSymptomDiseaseConnection.Diseases.First(d => d.Id == cat.Disease)
                        .Name.Contains(DiseaseCorelationsSearchBox.Text)) ||
                    (dbContextConcreteSymptomDiseaseConnection.Symptoms.First(s => s.Id == cat.Symptom)
                        .Name.Contains(DiseaseCorelationsSearchBox.Text));
            }
        }

        private void ConcreteSymptomDiseaseConnectionsSearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            _concreteSymptomDiseaseConnectionViewSource.Filter -= ConcreteSymptomDiseaseConnectionViewSource_Filter;
            _concreteSymptomDiseaseConnectionViewSource.Filter += ConcreteSymptomDiseaseConnectionViewSource_Filter;
        }


    }

    public class SymptomCathegoryValidation : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            SymptomCathegory cathegory = (value as BindingGroup).Items[0] as SymptomCathegory;
            if (cathegory.Name == null || cathegory.Name.Length == 0)
            {
                return new ValidationResult(false, "The name value can't be empty");
            }
            if (cathegory.ParentCathegory != null && cathegory.ParentCathegory.ToString() == cathegory.Id.ToString())
            {
                return new ValidationResult(false, "Cathegory can not be 'parent cathegory' for its self");
            }

            DiseaseDatabaseEntities tempContext = new DiseaseDatabaseEntities();

            if (tempContext.SymptomCathegories.Count(s => (s.Name == cathegory.Name) && (s.Id != cathegory.Id)) > 0)
            {
                return new ValidationResult(false, "Cathegory names have to be unique");
            }
            
            return ValidationResult.ValidResult;
        }
    }

    public class SymptomValidation : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            Symptom symptom = (value as BindingGroup).Items[0] as Symptom;
            if (symptom.Name == null || symptom.Name.Length == 0)
            {
                return new ValidationResult(false, "The name value can't be empty");
            }

            if (symptom.GeneralizationDegree < 0 || symptom.GeneralizationDegree > 100)
            {
                return new ValidationResult(false, "Value of generalization should be in range (0, 100)");
            }

            DiseaseDatabaseEntities tempContext = new DiseaseDatabaseEntities();

            if (tempContext.Symptoms.Count(s => (s.Name == symptom.Name) && (s.Id != symptom.Id)) > 0)
            {
                return new ValidationResult(false, "Symptom names have to be unique");
            }
            
            return ValidationResult.ValidResult;
        }
    }

    public class DiseaseValidation : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            Disease disease = (value as BindingGroup).Items[0] as Disease;
            if (disease.Name == null || disease.Name.Length == 0)
            {
                return new ValidationResult(false, "The name value can't be empty");
            }

            if (disease.OccurencesNumber < 0)
            {
                return new ValidationResult(false, "Value of occurence number shall be greater or equal 0");
            }

            DiseaseDatabaseEntities tempContext = new DiseaseDatabaseEntities();

            if (tempContext.Diseases.Count(s => (s.Name == disease.Name) && (s.Id != disease.Id)) > 0)
            {
                return new ValidationResult(false, "Disease names have to be unique");
            }

            return ValidationResult.ValidResult;
        }
    }

    public class DiseaseCorelationValidation : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            DiseaseCorelation dCor = (value as BindingGroup).Items[0] as DiseaseCorelation;


            if (dCor.DiseaseA == 0)
            {
                return new ValidationResult(false, "You have to choose one disease A option");
            }

            if (dCor.DiseaseB == 0)
            {
                return new ValidationResult(false, "You have to choose one disease B option");
            }

            if (dCor.CommonCases < 0)
            {
                return new ValidationResult(false, "Common case value shall be greater or equal 0");
            }

            if ((dCor.CorelationPower < 0) || (dCor.CorelationPower > 100))
            {
                return new ValidationResult(false, "Corelation power value shall be in range (0, 100)");
            }

            if ((dCor.CorelationDirection < -100) || (dCor.CorelationDirection > 100))
            {
                return new ValidationResult(false, "Corelation direction power value shall be in range (-100, 100)");
            }

            if (dCor.DiseaseA == dCor.DiseaseB)
            {
                return new ValidationResult(false, "Diseases A and B cannot be the same");
            }

            DiseaseDatabaseEntities tempContext = new DiseaseDatabaseEntities();

            if (tempContext.DiseaseCorelations
                .Count(s => (((s.DiseaseA == dCor.DiseaseA) && (s.DiseaseB == dCor.DiseaseB) && (s.Id != dCor.Id)) ||
                             ((s.DiseaseA == dCor.DiseaseB) && (s.DiseaseB == dCor.DiseaseA) && (s.Id != dCor.Id)))) > 0)
            
            {
                return new ValidationResult(false, "Disease corelations have to be unique");
            }

            return ValidationResult.ValidResult;
        }
    }

    public class ConcreteSymptomDiseaseConnectionValidation : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            ConcreteSymptomDiseaseConnection cSDCon = (value as BindingGroup).Items[0] as ConcreteSymptomDiseaseConnection;

            if (cSDCon.Disease == 0)
            {
                return new ValidationResult(false, "You have to choose one disease option");
            }

            if (cSDCon.Symptom == 0)
            {
                return new ValidationResult(false, "You have to choose one symptom option");
            }


            if (cSDCon.OccurencesNumber < 0)
            {
                return new ValidationResult(false, "Occurence number value shall be greater or equal 0");
            }

            if (cSDCon.YesAnswers < 0)
            {
                return new ValidationResult(false, "Yes answers value shall be greater or equal 0");
            }

            if (cSDCon.ProbablyYesAnswers < 0)
            {
                return new ValidationResult(false, "Probably yes value shall be greater or equal 0");
            }

            if (cSDCon.DontKnowAnswers < 0)
            {
                return new ValidationResult(false, "Don't know value shall be greater or equal 0");
            }

            if (cSDCon.ProbablyNotAnswers < 0)
            {
                return new ValidationResult(false, "Probably not value shall be greater or equal 0");
            }

            if (cSDCon.NotAnswers < 0)
            {
                return new ValidationResult(false, "Not answers value shall be greater or equal 0");
            }

            if ((cSDCon.ProbabilisticEvaluation < 0.0) || (cSDCon.ProbabilisticEvaluation > 1.0))
            {
                return new ValidationResult(false, "Probabilisic evaluation value shall be in range (0.0, 1.0)");
            }

            DiseaseDatabaseEntities tempContext = new DiseaseDatabaseEntities();

            if (tempContext.ConcreteSymptomDiseaseConnections
                .Count(s => ((s.Disease == cSDCon.Disease) && (s.Id != cSDCon.Id) && (s.Symptom == cSDCon.Symptom))) > 0)
            {
                return new ValidationResult(false, "Disease Symptom connection have to be unique");
            }


            return ValidationResult.ValidResult;
        }
    }
}
