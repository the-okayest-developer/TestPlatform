using System.Windows;
using TestProject_2.windows;

namespace TestProject_2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>S
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void teacherLoginButton_Click(object sender, RoutedEventArgs e)
        {
            TeacherWindow teacherWindow = new TeacherWindow();
            teacherWindow.Show();
            this.Close();
        }

        private void studentLoginButton_Click(object sender, RoutedEventArgs e)
        {
            StudentWindow teacherWindow = new StudentWindow();
            teacherWindow.Show();
            this.Close();

        }
    }
}