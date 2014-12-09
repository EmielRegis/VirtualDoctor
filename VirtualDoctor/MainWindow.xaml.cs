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
        DiseaseDatabaseEntities dbContextSymptomCathegory;
        private DiseaseDatabaseEntities dbContextSymptom;
        private CollectionViewSource _symptomCathegoryViewSource;
        private CollectionViewSource _symptomCathegoriesList;
        private CollectionViewSource _symptomViewSource;
        private CollectionViewSource _symptomCathegoriesListRemote;


        public MainWindow()
        {
            InitializeComponent();
            dbContextSymptomCathegory = new DiseaseDatabaseEntities();
            dbContextSymptom = new DiseaseDatabaseEntities();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            dbContextSymptomCathegory.SymptomCathegories.Load();
            dbContextSymptom.Symptoms.Load();
            dbContextSymptom.SymptomCathegories.Load();

            _symptomCathegoryViewSource = ((CollectionViewSource)(FindResource("symptomCathegoryViewSource")));          
            _symptomCathegoryViewSource.Source = dbContextSymptomCathegory.SymptomCathegories.Local;

            _symptomCathegoriesList = ((CollectionViewSource)(FindResource("symptomCathegoriesList")));
            _symptomCathegoriesList.Source = dbContextSymptomCathegory.SymptomCathegories.Local;

            _symptomCathegoriesListRemote = ((CollectionViewSource)(FindResource("symptomCathegoriesListRemote")));
            _symptomCathegoriesListRemote.Source = dbContextSymptom.SymptomCathegories.Local;

            _symptomViewSource = ((CollectionViewSource)(FindResource("symptomViewSource")));
            _symptomViewSource.Source = dbContextSymptom.Symptoms.Local;
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
                return new ValidationResult(false, "Cathegory names have to be unique");
            }
            
            return ValidationResult.ValidResult;
        }
    }
}
