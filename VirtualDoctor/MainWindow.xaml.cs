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
        private CollectionViewSource _symptomCathegoryViewSource;
        private CollectionViewSource _symptomCathegoriesList;
        private CollectionViewSource _symptomViewSource;
        private CollectionViewSource _symptomCathegoriesListRemote;
        private CollectionViewSource _diseaseViewSource;


        public MainWindow()
        {
            InitializeComponent();
            dbContextSymptomCathegory = new DiseaseDatabaseEntities();
            dbContextSymptom = new DiseaseDatabaseEntities();
            dbContextDisease = new DiseaseDatabaseEntities();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            dbContextSymptomCathegory.SymptomCathegories.Load();
            dbContextSymptom.Symptoms.Load();
            dbContextSymptom.SymptomCathegories.Load();
            dbContextDisease.Diseases.Load();

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
}
