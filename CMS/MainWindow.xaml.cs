using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using MySql.Data.MySqlClient;

namespace CMS
{

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>


    public partial class MainWindow : Window
    {
        private MySqlConnection Conn;
        public string EmpName = "";
        private string UserType;

        public string Createdirectory = @"C:\CMS\Logs";
        public string Path_ = @"C:\CMS\Logs\" + DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString().PadLeft(2, '0') + ".txt";

        public void Log(string logMessage)
        {
            File.AppendAllText(Path_, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " : " + logMessage + Environment.NewLine);
        }

        public void Directory_()
        {
            if (File.Exists(Path_) == false)
            {
                Directory.CreateDirectory(Createdirectory);
                File.Create(Path_);
            }
        }

        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnlogin_Click(object sender, RoutedEventArgs e)
        {
            LoginIN(txt_emp.Text, txt_pass.Password);
        }

        private void txt_emp_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                txt_emp.Text = txt_emp.Text.ToUpper();
                Log("Employee Code:" + txt_emp.Text);
                txt_pass.Focus();
            }
        }

        private void txt_pass_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                LoginIN(txt_emp.Text, txt_pass.Password);
            }
        }

        public void LoginIN(string Emp, string Pass)
        {
            if (!string.IsNullOrEmpty(Emp) || !string.IsNullOrEmpty(Pass))
            {
                try
                {
                    MySqlCommand CheckCredentials = new MySqlCommand("SELECT user_name,user_type FROM ionics_user where user_id ='" + txt_emp.Text.Trim() + "' and user_pass = md5('"+ txt_pass.Password.Trim() +"')", Conn);
                    MySqlDataReader reader = CheckCredentials.ExecuteReader();                  
                    while (reader.Read())
                    {
                        EmpName = reader[0].ToString();
                        UserType = reader[1].ToString();
                    }
                    if (EmpName != "")
                    {
                        Log("Username:" + EmpName);                      
                        Menu menus = new Menu();
                        menus.lblname.Text = EmpName;
                        menus.lblid.Text = txt_emp.Text.ToUpper();
                        if (UserType == "ADMINISTRATOR")
                        {
                            menus.manage.Visibility = Visibility.Visible;
                        }
                        else
                        {
                            menus.manage.Visibility = Visibility.Hidden;
                        }
                        menus.Show();
                        Close();
                    }
                    else
                    {
                        MessageBox.Show("Not Registered, Please Contact MSD");
                        txt_emp.Text = "";
                        txt_pass.Password ="";
                        txt_emp.Focus();
                    }
                    reader.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }
            else
            {
                MessageBox.Show("Required All Fields");
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Conn = DBModule.DBConnect();
            Directory_();
            txt_emp.Focus();
        }

        

        private void DialogHost_DialogClosing(object sender, MaterialDesignThemes.Wpf.DialogClosingEventArgs eventArgs)
        {

            if (!Equals(eventArgs.Parameter, false)) { Application.Current.Shutdown(); };
           
        }

    }
  
}

