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
        DiseaseDatabaseEntities dbContext;
        private CollectionViewSource _symptomCathegoryViewSource;
        private CollectionViewSource _symptomCathegoriesList;


        public MainWindow()
        {
            InitializeComponent();
            dbContext = new DiseaseDatabaseEntities();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _symptomCathegoryViewSource = ((CollectionViewSource)(FindResource("symptomCathegoryViewSource")));

            dbContext.SymptomCathegories.Load();
            _symptomCathegoryViewSource.Source = dbContext.SymptomCathegories.Local;

            _symptomCathegoriesList = ((CollectionViewSource)(FindResource("symptomCathegoriesList")));
            _symptomCathegoriesList.Source = dbContext.SymptomCathegories.Local;          
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

            dbContext.SaveChanges();
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
}
