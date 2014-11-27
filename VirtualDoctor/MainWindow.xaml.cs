using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace VirtualDoctor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DiseaseDatabaseEntities dbContext;
//        private DiseaseDatabaseServerEntities dbContext;
        private Random random = new Random();


        public MainWindow()
        {
            InitializeComponent();

            dbContext = new DiseaseDatabaseEntities();
//            dbContext = new DiseaseDatabaseServerEntities();
            
//            Label1.Content = (dbContext.SymptomRelationTypes.Any()) ? "yupi" : "damn";

         

        }

 

        private void Button1_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Data.CollectionViewSource symptomCathegoryViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("symptomCathegoryViewSource")));
            // Load data by setting the CollectionViewSource.Source property:
            // symptomCathegoryViewSource.Source = [generic data source]
            dbContext.SymptomCathegories.Load();

            // After the data is loaded call the DbSet<T>.Local property  
            // to use the DbSet<T> as a binding source. 
            symptomCathegoryViewSource.Source = dbContext.SymptomCathegories.Local; 

            string a = "";
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

            System.Windows.Data.CollectionViewSource diseaseDatabaseEntitiesViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("diseaseDatabaseEntitiesViewSource")));
            // Load data by setting the CollectionViewSource.Source property:
            // diseaseDatabaseEntitiesViewSource.Source = [generic data source]
            System.Windows.Data.CollectionViewSource symptomCathegoryViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("symptomCathegoryViewSource")));
            // Load data by setting the CollectionViewSource.Source property:
            // symptomCathegoryViewSource.Source = [generic data source]
        }

    }
}
