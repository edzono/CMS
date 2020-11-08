using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using Xceed.Wpf.Toolkit.Primitives;
using Telerik.Windows.Controls.GridView.Cells;

namespace CMS
{
    /// <summary>
    /// Interaction logic for Menu.xaml
    /// </summary>
    public partial class Menu : Window
    {
        public MySqlConnection conn;
        private int Total_Receive, Total_Stocks,Total_Return;
        private int acc_id,m_id,model_id,rack_id,line_id;
        private string Stats ="";
        
        DataTable dt_oi = new DataTable("dt_oi");
        DataTable dt_ii = new DataTable("dt_ii");

        private string WithTable, WithColumn;
        private string receive_select_query = "SELECT a.did 'DID',a.partnumber 'PART NUMBER', b.timestamp 'TIMESTAMP',lot_number 'LOT NUMBER',quantity 'QUANTITY',invoice_number 'INVOICE NUMBER', pic 'REGISTERED BY',remarks 'REMARKS' FROM ionics_parts a INNER JOIN";
        private string issuance_select_query = "select  a.did 'DID',a.partnumber 'PART NUMBER',b.timestamp 'TIMESTAMP',lot_number 'LOT NUMBER',issued_qty 'QUANTITY',invoice_number 'INVOICE NUMBER',pic 'REGISTERED BY',line 'LINE ISSUED',model 'MODEL',a.date 'DATE PLAN',plan_quantity 'PLAN QUANTITY' from ionics_parts a inner join";
        private string return_select_query = "select  a.did 'DID',a.partnumber 'PART NUMBER',b.timestamp 'TIMESTAMP',lot_number 'LOT NUMBER',return_quantity 'QUANTITY',invoice_number 'INVOICE NUMBER',pic 'REGISTERED BY',line 'FROM LINE',model 'FROM MODEL',a.date 'FROM DATE PLAN' from ionics_parts a inner join";


        public Menu()
        {
            InitializeComponent();
        }

        // load database connection
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            conn = DBModule.DBConnect();
            dt_from.Value = DateTime.Today;
            dt_to.Value = DateTime.Today.AddDays(1);
         
            Load_Overall_Inventory();
        }

      
        #region navbutton click
        private void inventory_Click(object sender, RoutedEventArgs e)
        {
            tabs.SelectedIndex = 0;
            ClearTextbox_Issue();
            ClearTextbox_Issue_selected();
            ClearTextbox_Return();
            ClearTextbox_Receive();
            CALL_CLEARTEXTBOX_AND_DISABLE();
        }

        private void register_Click(object sender, RoutedEventArgs e)
        {
            tabs.SelectedIndex = 1;
            ClearTextbox_Issue();
            ClearTextbox_Issue_selected();
            ClearTextbox_Return();
            CALL_CLEARTEXTBOX_AND_DISABLE();
        }

        private void transact_Click(object sender, RoutedEventArgs e)
        {
            tabs.SelectedIndex = 2;
            ClearTextbox_Issue();
            ClearTextbox_Issue_selected();
            ClearTextbox_Return();
            ClearTextbox_Receive();
            CALL_CLEARTEXTBOX_AND_DISABLE();
        }

        private void manage_Click(object sender, RoutedEventArgs e)
        {
            tabs.SelectedIndex = 3;
            CALL_LOAD_MAINTENANCE();
            CALL_CLEARTEXTBOX_AND_DISABLE();
            ClearTextbox_Issue();
            ClearTextbox_Issue_selected();
            ClearTextbox_Return();
            ClearTextbox_Receive();

        }

        private void btn_exit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void logout_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start(Application.ResourceAssembly.Location);
            Application.Current.Shutdown();
        }
        #endregion
        //window closed
        private void Window_Closed(object sender, System.EventArgs e)
        {
            Application.Current.Shutdown();
        }


        #region parts receive
        private void r_did_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                r_did.Text = r_did.Text.Substring(0, 15).ToUpper().Trim();
                r_ipn.Focus();
            }
        }

        private void r_ipn_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Total_Receive = 0;
                Total_Stocks = 0;
                try {
                    MySqlCommand cmd = new MySqlCommand
                    {
                        Connection = conn,
                        CommandText = "SELECT a.`maker`, a.`maker_code`, a.`description`, a.`component_type`, b.`total_receive` ,  b.`total_stocks` FROM `ionics_material_details` a LEFT JOIN `ionics_inventory` b ON a.`ionics_partnumber` = b.`partnumber` WHERE a.`ionics_partnumber` = '" + r_ipn.Text.ToUpper() + "'"
                    };
                    MySqlDataReader rd = cmd.ExecuteReader();
                    while (rd.Read())
                    {
                        r_supplier.Text = rd.GetString(0);
                        r_makercode.Text = rd.GetString(1);
                        r_desc.Text = rd.GetString(2);
                        r_comptype.Text = rd.GetString(3);
                        Total_Receive = rd.GetInt32(4);
                        Total_Stocks= rd.GetInt32(5);
                    }
                    rd.Close();
                    r_lotnum.Focus();
                }
                catch (Exception message)
                {
                    MessageBox.Show(message.ToString());
                }
            }
        }

        private void r_qty_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void r_qty_KeyDown(object sender, KeyEventArgs e)
        {
            if (r_qty.Text != "")
                r_invonum.Focus();
        }

        private void r_invonum_KeyDown(object sender, KeyEventArgs e)
        {
            if (r_invonum.Text != "")
                r_remarks.Focus();
        }

        private void btn_reg_Click(object sender, RoutedEventArgs e)
        {
            if (r_did.Text != "" || r_ipn.Text != "" || r_lotnum.Text != ""|| r_qty.Text != ""|| r_invonum.Text != "")
            {
                if(r_remarks.Text == "")
                {
                    r_remarks.Text = null;
                }
                MySqlCommand cmd = new MySqlCommand
                {
                    Connection = conn,
                };
                cmd.CommandText = "INSERT INTO `ionics_parts` (`did`, `partnumber`, `lot_number`, `quantity`,`current_qty`, `invoice_number`, `status`, `remarks`,`processtoken`) VALUES ('" + r_did.Text + "','" + r_ipn.Text + "','" + r_lotnum.Text + "','" + r_qty.Text + "','" + r_qty.Text + "','" + r_invonum.Text + "','WHS','" + r_remarks.Text + "','received')";
                cmd.ExecuteNonQuery();
                cmd.CommandText = "INSERT INTO `ionics_receive` (`did`, `partnumber`, `timestamp`, `pic`) VALUES ('" + r_did.Text + "','" + r_ipn.Text + "', NOW(),'" + lblname.Text + "')";
                cmd.ExecuteNonQuery();

                Total_Receive += Convert.ToInt32(r_qty.Text);
                Total_Stocks += Convert.ToInt32(r_qty.Text);

                cmd.CommandText = "UPDATE `ionics_inventory` SET `total_receive` = '"+ Total_Receive + "' , `total_stocks` = '" + Total_Stocks + "' WHERE  `partnumber` = '"+ r_ipn.Text +"'";
                cmd.ExecuteNonQuery();


                MessageBox.Show("Registered Successfully");
                ClearTextbox_Receive();
                r_did.Focus();
            }
            else
            {
                MessageBox.Show("Please complete the details to proceed");
            }
        }

        private void ClearTextbox_Receive()
        {
            r_did.Text = "";
            r_ipn.Text = "";
            r_lotnum.Text = "";
            r_qty.Text = "";
            r_invonum.Text = "";
            r_remarks.Text = "";

            r_supplier.Text = "";
            r_makercode.Text = "";
            r_desc.Text = "";
            r_comptype.Text = "";

            
        }

        private void btn_reset_reg_Click(object sender, RoutedEventArgs e)
        {
            ClearTextbox_Receive();
            r_did.Focus();
        }

        #endregion

        #region parts issuance
        private void i_line_DropDownOpened(object sender, System.EventArgs e)
        {
            MySqlCommand cmd = new MySqlCommand
            {
                Connection = conn,
                CommandText = "select distinct `line` from `ionics_line`"
            };
            MySqlDataAdapter da = new MySqlDataAdapter(cmd);
            DataTable dt = new DataTable("dt");
            da.Fill(dt);
            i_line.Items.Clear();

            foreach (DataRow dr in dt.Rows)
            {
                i_line.Items.Add(dr[0].ToString());
            }
        }

        private void i_model_DropDownOpened(object sender, System.EventArgs e)
        {
            if (i_ipn.Text == "")
            {
                MessageBox.Show("Need to scan DID first to select model.");
            }
            else
            {
                DataTable dt = new DataTable("dt");
                MySqlCommand cmd = new MySqlCommand
                {
                    Connection = conn,
                    CommandText = "select distinct `model` from `ionics_model` a left join `ionics_line` b on a.`ID` = b.`model_id` WHERE b.`line` = '" + i_line.Text + "'"
                };
                MySqlDataAdapter da = new MySqlDataAdapter(cmd);
                da.Fill(dt);
                i_model.Items.Clear();

                foreach (DataRow dr in dt.Rows)
                {
                    i_model.Items.Add(dr[0].ToString());
                }
            }
        }

        private void i_plan_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void i_did_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Total_Receive = 0;
                Total_Stocks = 0;
                try
                {
                    MySqlCommand cmd = new MySqlCommand
                    {
                        Connection = conn,
                     };

                    cmd.CommandText = "SELECT `processtoken` FROM `ionics_parts` WHERE `did` = '" + i_did.Text + "'";
                    if (cmd.ExecuteScalar().ToString() == "received")
                    {
                        cmd.CommandText = "SELECT a.`partnumber`, c.`description`, c.`maker`, a.`lot_number`, a.`quantity`, b.`pic`, a.`status`, a.`invoice_number`, d.`total_receive`, d.`total_stocks` FROM `ionics_parts` a LEFT JOIN `ionics_receive` b ON a.`did` = b.`did` LEFT JOIN  `ionics_material_details` c ON a.`partnumber` = c.`ionics_partnumber` LEFT JOIN `ionics_inventory` d on a.`partnumber` = d.`partnumber` WHERE a.`did` = '" + i_did.Text + "'";
                    }
                    else if (cmd.ExecuteScalar().ToString() == "returned")
                    {
                        cmd.CommandText = "SELECT a.`partnumber`, c.`description`, c.`maker`, a.`lot_number`, a.`current_quantity`, b.`pic`, a.`status`, a.`invoice_number`, d.`total_receive`, d.`total_stocks` FROM `ionics_parts` a LEFT JOIN `ionics_receive` b ON a.`did` = b.`did` LEFT JOIN  `ionics_material_details` c ON a.`partnumber` = c.`ionics_partnumber` LEFT JOIN `ionics_inventory` d on a.`partnumber` = d.`partnumber` WHERE a.`did` = '" + i_did.Text + "'";
                    }
                    MySqlDataReader rd = cmd.ExecuteReader();
                        while (rd.Read())
                        {
                            i_pn.Text = rd.GetString(0);
                            i_desc.Text = rd.GetString(1);
                            i_sup.Text = rd.GetString(2);
                            i_lotnum.Text = rd.GetString(3);
                            i_qty.Text = rd.GetString(4);
                            i_pic.Text = rd.GetString(5);
                            i_status.Text = rd.GetString(6);
                            i_invonum.Text = rd.GetString(7);
                            Total_Receive = rd.GetInt32(8);
                            Total_Stocks = rd.GetInt32(9);
                        }
                        rd.Close();
                        i_ipn.Focus();
                    
                }
                catch (Exception message)
                {
                    MessageBox.Show(message.ToString());
                }
            }
        }

        private void i_ipn_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            { 
                if(i_ipn.Text == i_pn.Text)
                {
                    btn_issue.IsEnabled = true;
                    btn_issue.Focus();
                }
                else
                {
                    MessageBox.Show("Scanned Ionics Partnumber mismatch with the receive Partnumber of DID.");
                    btn_issue.IsEnabled = false;
                }
            }
        }

        private void btn_issue_Click(object sender, RoutedEventArgs e)
        {
            if(i_did.Text == "" || i_ipn.Text =="" || i_line.Text =="" || i_model.Text == "" || i_plan.Text == "" || i_date.Text == "")
            {
                MySqlCommand cmd = new MySqlCommand
                {
                    Connection = conn,
                };

                cmd.CommandText = "SELECT `processtoken` FROM `ionics_parts` WHERE `did` = '"+ i_did.Text +"'";
                if (cmd.ExecuteScalar().ToString() == "received")
                {                
                  cmd.CommandText = "UPDATE `ionics_parts` SET `status`= 'PRD', `line` = '" + i_line.Text + "', `model` = '" + i_model.Text + "', `plan_quantity` = '" + i_plan.Text + "', `date` = '" + i_date.Text + "',`processtoken`= 'issued' WHERE did = '"+ i_did.Text +"'";
                cmd.ExecuteNonQuery();
                cmd.CommandText = "INSERT INTO `ionics_issuance` (`did`, `partnumber`, `timestamp`, `pic`,`issued_qty`) VALUES ('" + i_did.Text + "','" + i_ipn.Text + "', NOW(),'" + lblname.Text + "','" + i_qty.Text + "')";
                cmd.ExecuteNonQuery();

                Total_Receive -= Convert.ToInt32(i_qty.Text);
                Total_Stocks -= Convert.ToInt32(i_qty.Text);

                cmd.CommandText = "UPDATE `ionics_inventory` SET `total_receive` = '" + Total_Receive + "' , `total_stocks` = '" + Total_Stocks + "' WHERE  `partnumber` = '" + r_ipn.Text + "'";
                cmd.ExecuteNonQuery();

                MessageBox.Show("Issued Successfully");
                ClearTextbox_Issue();
                i_did.Focus();
                }
                else
                {
                    MessageBox.Show("Part cannot be issued.");
                }
            }
            else
            {
                MessageBox.Show("Please Complete necessary details");
            }
        }

        private void ClearTextbox_Issue()
        {
            i_did.Text = "";
            i_ipn.Text = "";

            i_lotnum.Text = "";
            i_qty.Text = "";
            i_invonum.Text = "";
            i_sup.Text = "";
            i_desc.Text = "";
            i_pic.Text = "";
            i_status.Text = "";
            i_pn.Text = "";

        }

        private void ClearTextbox_Issue_selected()
        {
            i_line.Text = "";
            i_model.Text = "";
            i_plan.Text = "";
            i_date.Text = "";
            btn_issue.IsEnabled = false;
        }

        private void btn_reset_issue_Click(object sender, RoutedEventArgs e)
        {
            ClearTextbox_Issue();
            ClearTextbox_Issue_selected();
            i_did.Focus();
        }

        #endregion

        #region parts return

        private void re_return_qty_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void btn_return_Click(object sender, RoutedEventArgs e)
        {
            if(re_pn.Text == "" || re_return_qty.Text == "" || re_reason.Text == "")
            {
                if (Convert.ToInt32(re_return_qty.Text) <= Convert.ToInt32(re_qty.Text))
                {                
                MySqlCommand cmd = new MySqlCommand
                {
                    Connection = conn,
                };
                cmd.CommandText = "UPDATE `ionics_parts` SET `status`= 'WHS', `current_quantity` = '"+ re_return_qty.Text +"' , `processtoken`= 'returned' WHERE did = '" + i_did.Text + "'";
                cmd.ExecuteNonQuery();
                cmd.CommandText = "INSERT INTO `ionics_return` (`did`, `partnumber`, `timestamp`, `pic` , `return_quantity` , `reason`) VALUES ('" + i_did.Text + "','" + i_ipn.Text + "', NOW(),'" + lblname.Text + "' ,'" + re_return_qty.Text + "' ,'" + re_reason.Text + "' )";
                cmd.ExecuteNonQuery();

                Total_Return += Convert.ToInt32(i_qty.Text);
                Total_Stocks += Convert.ToInt32(i_qty.Text);


                cmd.CommandText = "UPDATE `ionics_inventory` SET `total_return` = '" + Total_Receive + "' , `total_stocks` = '" + Total_Stocks + "' WHERE  `partnumber` = '" + r_ipn.Text + "'";
                cmd.ExecuteNonQuery();

                MessageBox.Show("Return Successfully");
                ClearTextbox_Return();
                re_did.Focus();
                }
                else
                {
                    MessageBox.Show("Invalid Return Quantity. Cannot return larger quantity than the recorded.");
                }
            }
            else
            {
                MessageBox.Show("Please Complete necessary details");
            }
        }
              
        private void re_reason_DropDownOpened(object sender, System.EventArgs e)
        {
            MySqlCommand cmd = new MySqlCommand
            {
                Connection = conn,
                CommandText = "select distinct `reason` from `ionics_reason`"
            };
            MySqlDataAdapter da = new MySqlDataAdapter(cmd);
            DataTable dt = new DataTable("dt");
            da.Fill(dt);
            re_reason.Items.Clear();

            foreach (DataRow dr in dt.Rows)
            {
                re_reason.Items.Add(dr[0].ToString());
            }
        }

        private void re_did_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Total_Receive = 0;
                Total_Stocks = 0;
                Total_Return = 0;
                try
                {
                    MySqlCommand cmd = new MySqlCommand
                    {
                        Connection = conn,
                        CommandText = "SELECT a.`partnumber`, c.`description`, c.`maker`, a.`lot_number`, a.`current_quantity`, b.`pic`, a.`status`, a.`invoice_number`, d.`total_receive`, d.`total_stocks`, d.`total_return`, a.`line`, a.`model`, a.`plan_quantity` , a.`date` FROM `ionics_parts` a LEFT JOIN `ionics_issuance` b ON a.`did` = b.`did` LEFT JOIN  `ionics_material_details` c ON a.`partnumber` = c.`ionics_partnumber` LEFT JOIN `ionics_inventory` d on a.`partnumber` = d.`partnumber` WHERE a.`did` = '" + i_did.Text + "';"
                    };
                    MySqlDataReader rd = cmd.ExecuteReader();
                    while (rd.Read())
                    {
                        re_pn.Text = rd.GetString(0);
                        re_desc.Text = rd.GetString(1);
                        re_sup.Text = rd.GetString(2);
                        re_lotnum.Text = rd.GetString(3);
                        re_qty.Text = rd.GetString(4);
                        re_pic.Text = rd.GetString(5);
                        re_status.Text = rd.GetString(6);
                        re_invonum.Text = rd.GetString(7);
                        Total_Receive = rd.GetInt32(8);
                        Total_Stocks = rd.GetInt32(9);
                        Total_Return = rd.GetInt32(10);
                        re_line.Text = rd.GetString(11);
                        re_model.Text = rd.GetString(12);
                        re_plan.Text = rd.GetString(13);
                        re_date.Text = rd.GetDateTime(14).ToString("MM/dd/yyyy");
                    }
                    rd.Close();
                    re_return_qty.Focus();
                }
                catch (Exception message)
                {
                    MessageBox.Show(message.ToString());
                }
            }
        }

        private void btn_reset_return_Click(object sender, RoutedEventArgs e)
        {
            ClearTextbox_Return();
            re_did.Focus();
        }

        private void ClearTextbox_Return()
        {
            re_did.Text = "";
            re_pn.Text = "";
            re_desc.Text ="";
            re_sup.Text = "";
            re_lotnum.Text ="";
            re_qty.Text ="";
            re_pic.Text ="";
            re_status.Text = "";
            re_invonum.Text = "";
         
            re_line.Text = "";
            re_model.Text = "";
            re_plan.Text = "";
            re_date.Text = "";
            re_reason.Text = "";
            re_return_qty.Text = "";                    

        }

        #endregion

        #region parts inquiry
     

        private void btn_search_Click(object sender, RoutedEventArgs e)
        {
            try
            {           
                MySqlCommand cmd = new MySqlCommand
                {
                    Connection = conn,
                    CommandText = "SELECT a.`did`, a.`partnumber`, c.`description`, c.`maker`, a.`lot_number`,a.`quantity`, a.`current_quantity`, b.`pic`, a.`status`, a.`invoice_number` FROM `ionics_parts` a LEFT JOIN `ionics_receive` b ON a.`did` = b.`did` LEFT JOIN  `ionics_material_details` c ON a.`partnumber` = c.`ionics_partnumber`  WHERE a.`did` = '" + i_did.Text + "';"
                };
                MySqlDataReader rd = cmd.ExecuteReader();
                while (rd.Read())
                {
                    d_scan_did.Text = rd.GetString(0);
                    d_pn.Text = rd.GetString(1);
                    d_desc.Text = rd.GetString(2);
                    d_sup.Text = rd.GetString(3);
                    d_lotnum.Text = rd.GetString(4);
                    d_qty.Text = rd.GetString(5);
                    d_remaining_qty.Text = rd.GetString(6);
                    d_pic.Text = rd.GetString(7);
                    d_status.Text = rd.GetString(8);
                    d_invonum.Text = rd.GetString(9);
                }
                rd.Close();

                did_transaction_grid.ItemsSource = null;
    
                cmd.CommandText = " SELECT 'DID','TIMESTAMP','PIC','TRANSACTION','QUANTITY','REASON' FROM (SELECT a.did, timestamp, pic, 'RECEIVED', quantity, 'NA' FROM ionics_receive a LEFT JOIN ionics_parts b ON a.did = b.did UNION ALL SELECT did, timestamp, pic, 'ISSUED', '0', 'NA' FROM ionics_issuance UNION ALL SELECT did, timestamp, pic, 'RETURNED', return_quantity, reason FROM ionics_return WHERE did = '" + i_did.Text + "') AS A";
                MySqlDataAdapter da = new MySqlDataAdapter(cmd);
                DataTable dt = new DataTable("dt");
                da.Fill(dt);
                did_transaction_grid.ItemsSource = dt.DefaultView;

                s_did.Text = "";
                s_did.Focus();
            }
            catch (Exception message)
            {
                MessageBox.Show(message.ToString());
            }
        }
        #endregion

        #region parts maintenance

        private void CALL_LOAD_MAINTENANCE()
        {
            Load_Material_Details();
            Load_Account_List();
            Load_Model_Details();
            Load_Line_Details();
            Load_Rack_Details();
        }

        private void CALL_CLEARTEXTBOX_AND_DISABLE()
        {
            ClearTextbox_Account();
            ClearTextbox_Line();
            ClearTextbox_Material();
            ClearTextbox_Model();
            ClearTextbox_Rack();

            Disable_Account_Input_Details();
            Disable_Line_Input_Details();
            Disable_Model_Input_Details();
            Disable_M_Input_Details();
            Disable_Rack_Input_Details();
        }


        //Load Grid View
        private void Load_Material_Details()
        {
            grid_material.ItemsSource = null;
            MySqlCommand cmd = new MySqlCommand
            {
                Connection = conn
            };
            cmd.CommandText = "SELECT id as 'ID',ionics_partnumber as 'IONICS PN',customer_partnumber as 'CUSTOMER PN',maker as 'MAKER',maker_code as 'MAKER CODE',description as 'DESCRIPTION',component_type as 'COMPONENT TYPE',remarks as 'REMARKS' FROM ionics_material_details";
            MySqlDataAdapter da = new MySqlDataAdapter(cmd);
            DataTable dt = new DataTable("dt");
            da.Fill(dt);
            grid_material.ItemsSource = dt.DefaultView;
        }

        private void Load_Model_Details()
        {
            grid_model.ItemsSource = null;
            MySqlCommand cmd = new MySqlCommand
            {
                Connection = conn
            };
            cmd.CommandText = "SELECT id as 'ID',petname as 'FAMILY NAME',model as 'MODEL' FROM ionics_model";
            MySqlDataAdapter da = new MySqlDataAdapter(cmd);
            DataTable dt = new DataTable("dt");
            da.Fill(dt);
            grid_model.ItemsSource = dt.DefaultView;
        }

        private void Load_Account_List()
        {
            grid_acc.ItemsSource = null;
            MySqlCommand cmd = new MySqlCommand
            {
                Connection = conn
            };
            cmd.CommandText = "SELECT id as 'ID',user_id as 'EMPLOYEE CODE',user_name as 'EMPLOYEE NAME',user_type as 'USER TYPE' FROM ionics_user";
            MySqlDataAdapter da = new MySqlDataAdapter(cmd);
            DataTable dt = new DataTable("dt");
            da.Fill(dt);
            grid_acc.ItemsSource = dt.DefaultView;
        }

        private void Load_Line_Details()
        {
            grid_line.ItemsSource = null;
            MySqlCommand cmd = new MySqlCommand
            {
                Connection = conn
            };
            cmd.CommandText = "SELECT a.id as 'ID', a.line as 'LINE' , b.petname as 'FAMILY NAME', b.model as 'MODEL' FROM ionics_line a LEFT JOIN ionics_model b on a.model_id = b.id";
            MySqlDataAdapter da = new MySqlDataAdapter(cmd);
            DataTable dt = new DataTable("dt");
            da.Fill(dt);
            grid_line.ItemsSource = dt.DefaultView;
        }

        private void Load_Rack_Details()
        {
            grid_rack.ItemsSource = null;
            MySqlCommand cmd = new MySqlCommand
            {
                Connection = conn
            };
            cmd.CommandText = "SELECT id as 'ID', rack as 'RACK',location as 'SLOT' , partnumber as 'PART NUMBER' FROM ionics_rack";
            MySqlDataAdapter da = new MySqlDataAdapter(cmd);
            DataTable dt = new DataTable("dt");
            da.Fill(dt);
            grid_rack.ItemsSource = dt.DefaultView;
        }

        // Mouse Double Click in  DATA GRID
        private void grid_material_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (sender != null)
                {
                    DataGrid grid = sender as DataGrid;
                    if (grid != null && grid.SelectedItems != null && grid.SelectedItems.Count == 1)
                    {
                        //This is the code which helps to show the data when the row is double clicked.
                        DataGridRow dgr = grid.ItemContainerGenerator.ContainerFromItem(grid.SelectedItem) as DataGridRow;
                        DataRowView dr = (DataRowView)dgr.Item;

                        m_id = Convert.ToInt32(dr[0]);
                        m_ID.Text = m_id.ToString();
                        m_ipn.Text = dr[1].ToString();
                        m_cpn.Text = dr[2].ToString();                       
                        m_maker.Text = dr[3].ToString();
                        m_maker_code.Text = dr[4].ToString();
                        m_desc.Text = dr[5].ToString();
                        m_comp_type.Text = dr[6].ToString();
                        m_remarks.Text = dr[7].ToString();

                        Enable_M_Input_Details();
                    }

                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }

        }

        private void grid_model_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (sender != null)
                {
                    DataGrid grid = sender as DataGrid;
                    if (grid != null && grid.SelectedItems != null && grid.SelectedItems.Count == 1)
                    {
                        //This is the code which helps to show the data when the row is double clicked.
                        DataGridRow dgr = grid.ItemContainerGenerator.ContainerFromItem(grid.SelectedItem) as DataGridRow;
                        DataRowView dr = (DataRowView)dgr.Item;

                        Enable_Model_Input_Details();

                        model_id = Convert.ToInt32(dr[0]);
                        model_ID.Text = model_id.ToString();
                        model_fam.Text = dr[1].ToString();
                        model_model.Text = dr[2].ToString();
                    }

                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }

        private void grid_acc_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (sender != null)
                {
                    DataGrid grid = sender as DataGrid;
                    if (grid != null && grid.SelectedItems != null && grid.SelectedItems.Count == 1)
                    {
                        //This is the code which helps to show the data when the row is double clicked.
                        DataGridRow dgr = grid.ItemContainerGenerator.ContainerFromItem(grid.SelectedItem) as DataGridRow;
                        DataRowView dr = (DataRowView)dgr.Item;

                        acc_id = Convert.ToInt32(dr[0]);
                        acc_emp_code.Text = dr[1].ToString();
                        acc_emp_name.Text = dr[2].ToString();
                        acc_type.Text = dr[3].ToString();
                    }

                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }

        private void grid_line_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (sender != null)
                {
                    DataGrid grid = sender as DataGrid;
                    if (grid != null && grid.SelectedItems != null && grid.SelectedItems.Count == 1)
                    {
                        //This is the code which helps to show the data when the row is double clicked.
                        DataGridRow dgr = grid.ItemContainerGenerator.ContainerFromItem(grid.SelectedItem) as DataGridRow;
                        DataRowView dr = (DataRowView)dgr.Item;

                        Enable_Line_Input_Details();

                        line_id = Convert.ToInt32(dr[0]);
                        line_ID.Text = line_id.ToString();
                        line_family.Text = dr[2].ToString();
                        line_model.Text = dr[3].ToString();
                        line_line.Text = dr[1].ToString();


                    }

                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }

        private void grid_rack_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (sender != null)
                {
                    DataGrid grid = sender as DataGrid;
                    if (grid != null && grid.SelectedItems != null && grid.SelectedItems.Count == 1)
                    {
                        //This is the code which helps to show the data when the row is double clicked.
                        DataGridRow dgr = grid.ItemContainerGenerator.ContainerFromItem(grid.SelectedItem) as DataGridRow;
                        DataRowView dr = (DataRowView)dgr.Item;

                        Enable_Rack_Input_Details();

                        rack_id = Convert.ToInt32(dr[0]);
                        rack_ID.Text = rack_id.ToString();
                        rack_pn.Text = dr[3].ToString();
                        rack_rack.Text = dr[1].ToString();
                        rack_slot.Text = dr[2].ToString();
                    }

                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }

        // Save Button 
        private void btn_save_m_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MySqlCommand cmd = new MySqlCommand
                {
                    Connection = conn
                };
                if (m_ipn.Text == "" || m_cpn.Text == "" || m_maker.Text == "" || m_maker_code.Text == "" || m_desc.Text == "" || m_comp_type.Text == "" || m_remarks.Text == "")
                {
                    MessageBox.Show("Please input required details");
                }
                else
                {
               
                if (m_id == 0)
                {
                    cmd.CommandText = "SELECT Count(*) FROM ionics_material_details WHERE ionics_partnumber = '" + m_ipn.Text + "' AND customer_partnumber = '" + m_cpn.Text + "' ";
                    if (Convert.ToInt32(cmd.ExecuteScalar()) == 0)
                    {
                        cmd.CommandText = "INSERT INTO ionics_material_details (ionics_partnumber,customer_partnumber,maker,maker_code,description,component_type,remarks) VALUES ('" + m_cpn.Text + "','" + m_ipn.Text + "','" + m_maker.Text + "','" + m_maker_code.Text + "','" + m_desc.Text + "','" + m_comp_type.Text + "','" + m_remarks.Text + "')";
                        cmd.ExecuteNonQuery();
                        MessageBox.Show("Added Successfully.");
                            Load_Material_Details();
                        ClearTextbox_Material();
                    }
                    else
                    {
                        MessageBox.Show("Existing record found. Kindly verify on the table beside.");
                    }
                }
                else
                {
                    cmd.CommandText = "UPDATE ionics_material_details SET maker = '" + m_maker.Text + "',maker_code= '" + m_maker_code.Text + "',description = '" + m_desc.Text + "',component_type = '" + m_comp_type.Text + "',remarks ='" + m_remarks.Text + "'  WHERE ionics_partnumber = '" + m_ipn.Text + "' AND customer_partnumber = '" + m_cpn.Text + "' ";
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Updated Successfully.");
                        Load_Material_Details();
                        ClearTextbox_Material();
                }
                }

            }
            catch (Exception)
            {

                throw;
            }
        }

        private void btn_save_model_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MySqlCommand cmd = new MySqlCommand
                {
                    Connection = conn
                };
                if (model_fam.Text == "" || model_model.Text == "")
                {
                    MessageBox.Show("Please input required details");
                }
                else
                {                    
                    if (model_id == 0)
                    {
                        cmd.CommandText = "SELECT COUNT(*) FROM ionics_model WHERE petname = '" + model_fam.Text + "' and model = '" + model_model.Text + "'";
                        var count = Convert.ToInt32(cmd.ExecuteScalar());
                        if (count == 0)
                        {
                            cmd.CommandText = "INSERT INTO ionics_model (petname,model) VALUES ('" + model_fam.Text + "', '" + model_model.Text + "')";
                            cmd.ExecuteNonQuery();
                            MessageBox.Show("Added Successfully.");
                            Load_Model_Details();
                            ClearTextbox_Model();
                        }
                        else
                        {
                            MessageBox.Show("Existing record found. Kindly verify on the table beside.");
                        }
                    }
                    else
                    {
                        cmd.CommandText = "UPDATE ionics_model SET petname = '" + model_fam.Text + "',model = '" + model_model.Text + "' WHERE id = '" + model_id + "'";
                        cmd.ExecuteNonQuery();
                        MessageBox.Show("Updated Successfully.");
                        Load_Model_Details();
                        ClearTextbox_Model();
                    }

                }
            }
            catch (Exception message)
            {
                MessageBox.Show(message.ToString());
            }
        }

        private void btn_save_rack_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MySqlCommand cmd = new MySqlCommand
                {
                    Connection = conn
                };

                if (rack_pn.Text == "" || rack_rack.Text == "" || rack_slot.Text == "")
                {
                    MessageBox.Show("Please input required details");
                }
                else
                {
                    if (rack_id == 0)
                    {

                        cmd.CommandText = "SELECT COUNT(*) FROM ionics_rack WHERE partnumber = '" + rack_pn.Text + "' and rack = '" + rack_rack.Text + "' and location = '" + rack_slot.Text + "'";
                        var count = Convert.ToInt32(cmd.ExecuteScalar());
                        if (count == 0)
                        {
                            cmd.CommandText = "INSERT INTO ionics_rack (partnumber,rack,location) VALUES ('" + rack_pn.Text + "', '" + rack_rack + "','" + rack_slot.Text + "')";
                            cmd.ExecuteNonQuery();
                            MessageBox.Show("Added Successfully.");
                            Load_Rack_Details();
                            ClearTextbox_Rack();
                        }
                        else
                        {
                            MessageBox.Show("Existing record found. Kindly verify on the table beside.");
                        }
                    }

                    else
                    {
                        cmd.CommandText = "UPDATE ionics_rack SET partnumber = '" + rack_pn.Text + "',rack = '" + rack_rack + "',location = '" + rack_slot.Text + "' WHERE id= '" + rack_id + "'";
                        cmd.ExecuteNonQuery();
                        MessageBox.Show("Updated Successfully.");
                        Load_Rack_Details();
                        ClearTextbox_Rack();
                    }
                }
            }
            catch (Exception message)
            {
                MessageBox.Show(message.ToString());
            }
        }

        private void btn_save_line_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MySqlCommand cmd = new MySqlCommand
                {
                    Connection = conn
                };
                if (line_family.Text == "" || line_line.Text == "" || line_model.Text == "")
                {
                    MessageBox.Show("Please input required details");
                }
                else
                {                
                    cmd.CommandText = " SELECT id from ionics_model where petname = '" + line_family.Text + "' and model = '" + line_model.Text + "'";
                    var id = cmd.ExecuteScalar().ToString();
                    if (line_id == 0)
                    {
                        cmd.CommandText = "SELECT COUNT(*) FROM ionics_line WHERE line = '" + line_line.Text + "' and model_id = '" + id + "'";
                        var count = Convert.ToInt32(cmd.ExecuteScalar());
                        if (count == 0)
                        {
                            cmd.CommandText = "INSERT INTO ionics_line (line,model_id) VALUES ('" + line_line.Text + "', '" + id + "')";
                            cmd.ExecuteNonQuery();
                            MessageBox.Show("Added Successfully.");
                            Load_Line_Details();
                            ClearTextbox_Line();
                        }
                        else
                        {
                            MessageBox.Show("Existing record found. Kindly verify on the table beside.");
                        }
                    }
                    else
                    {
                        cmd.CommandText = "UPDATE ionics_line SET line = '" + line_line.Text + "',model_id = '" + id + "' WHERE id = '" + line_id + "')";
                        cmd.ExecuteNonQuery();
                        MessageBox.Show("Updated Successfully.");
                        Load_Line_Details();
                        ClearTextbox_Line();
                    }
                }
            }
            catch (Exception message)
            {
                MessageBox.Show(message.ToString());
            }
        }

        private void btn_save_acc_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MySqlCommand cmd = new MySqlCommand
                {
                    Connection = conn
                };

                switch (Stats)
                {
                    case "ANU":
                        if (acc_emp_code.Text == "" || acc_emp_name.Text == "" || acc_type.Text == "" || acc_pass.Text =="" || acc_pass.Text != acc_cpass.Text)
                        {
                            MessageBox.Show("Please input required details");
                        }
                        else
                        {
                            cmd.CommandText = "INSERT INTO ionics_user (user_id,user_pass,user_name,user_type) VALUES ('" + acc_emp_code.Text + "', md5('" + acc_cpass.Text + "'), '"+ acc_emp_name.Text + "','"+ acc_type .Text+ "')";
                            cmd.ExecuteNonQuery();
                            MessageBox.Show("Added Successfully.");
                            Load_Account_List();
                            ClearTextbox_Account();
                        }
                        break;
                    case "EU":
                        if (acc_emp_code.Text == "" || acc_emp_name.Text == "" || acc_type.Text == "" )
                        {
                            MessageBox.Show("Please input required details");
                        }
                        else
                        {
                            cmd.CommandText = "UPDATE ionics_user SET user_id = '" + acc_emp_code.Text + "',user_name = '" + acc_emp_name + "' , user_type = '" + acc_type.Text + "' WHERE id = '" + acc_id + "'";
                            cmd.ExecuteNonQuery();
                            MessageBox.Show("Updated Successfully.");
                            Load_Account_List();
                            ClearTextbox_Account();
                        }
                        break;
                    case "CP":
                        if (acc_emp_code.Text == "" || acc_emp_name.Text == "" || acc_type.Text == "" || acc_pass.Text == "" || acc_pass.Text != acc_cpass.Text)
                        {
                            MessageBox.Show("Please input required details");
                        }
                        else
                        {
                            cmd.CommandText = "UPDATE ionics_user SET user_pass =md5('" + acc_cpass.Text + "') WHERE id = '" + acc_id + "' and user_id = '" + acc_emp_code.Text + "' and user_type ='" + acc_type.Text + "' ";
                            cmd.ExecuteNonQuery();
                            MessageBox.Show("Changed Password Successfully.");
                            Load_Account_List();
                            ClearTextbox_Account();
                        }
                        break;
                }                              
            }
            catch (Exception message)
            {
                MessageBox.Show(message.ToString());
            }

        }

        // Add Button
        private void btn_add_m_Click(object sender, RoutedEventArgs e)
        {
            ClearTextbox_Material();
            Enable_M_Input_Details();
            m_id = 0;
            m_ID.Text = m_id.ToString();

        }

        private void btn_add_model_Click(object sender, RoutedEventArgs e)
        {
            ClearTextbox_Model();
            Enable_Model_Input_Details();
            model_id = 0;
            model_ID.Text = model_id.ToString();
        }

        private void btn_add_rack_Click(object sender, RoutedEventArgs e)
        {
            ClearTextbox_Rack();
            Enable_Rack_Input_Details();
            rack_id = 0;
            rack_ID.Text = rack_id.ToString();
        }

        private void btn_add_line_Click(object sender, RoutedEventArgs e)
        {
            ClearTextbox_Line();
            Enable_Line_Input_Details();
            line_id = 0;
            line_ID.Text = line_id.ToString();
        }

        // Disable input fields

        private void Disable_M_Input_Details()
        {
            m_cpn.IsEnabled = false;
            m_ipn.IsEnabled = false;
            m_maker.IsEnabled = false;
            m_maker_code.IsEnabled = false;
            m_desc.IsEnabled = false;
            m_comp_type.IsEnabled = false;
            m_remarks.IsEnabled = false;
            btn_save_m.IsEnabled = false;
            btn_reset_save_m.IsEnabled = false;
        }

        private void Disable_Model_Input_Details()
        {
            model_fam.IsEnabled = false;
            model_model.IsEnabled = false;
            btn_save_m.IsEnabled = false;
            btn_reset_save_m.IsEnabled = false;
        }

        private void Disable_Account_Input_Details()
        {
            acc_grid.IsEnabled = false;
        }

        private void Disable_Line_Input_Details()
        {
            line_family.IsEnabled = false;
            line_model.IsEnabled = false;
            line_line.IsEnabled = false;
            btn_save_line.IsEnabled = false;
            btn_reset_save_line.IsEnabled = false;
        }

        private void Disable_Rack_Input_Details()
        {
            rack_pn.IsEnabled = false;
            rack_rack.IsEnabled = false;
            rack_slot.IsEnabled = false;
            btn_save_rack.IsEnabled = false;
            btn_reset_save_rack.IsEnabled = false;
        }

        // Enable input fields
        private void Enable_M_Input_Details()
        {
            m_cpn.IsEnabled = true;
            m_ipn.IsEnabled = true;
            m_maker.IsEnabled = true;
            m_maker_code.IsEnabled = true;
            m_desc.IsEnabled = true;
            m_comp_type.IsEnabled = true;
            m_remarks.IsEnabled = true;
            btn_save_m.IsEnabled = true;
            btn_reset_save_m.IsEnabled = true;
        }

        private void Enable_Model_Input_Details()
        {
            model_fam.IsEnabled = true;
            model_model.IsEnabled = true;
            btn_save_model.IsEnabled = true;
            btn_reset_save_model.IsEnabled = true;
        }

        private void Enable_Account_Input_Details()
        {
            acc_grid.IsEnabled = true;
        }

        private void Enable_Line_Input_Details()
        {
            line_family.IsEnabled = true;
            line_model.IsEnabled = true;
            line_line.IsEnabled = true;
            btn_save_line.IsEnabled = true;
            btn_reset_save_line.IsEnabled = true;
        }

        private void Enable_Rack_Input_Details()
        {
            rack_pn.IsEnabled = true;
            rack_rack.IsEnabled = true;
            rack_slot.IsEnabled = true;
            btn_save_rack.IsEnabled = true;
            btn_reset_save_rack.IsEnabled = true;
        }

        // Clear Input fields
        private void ClearTextbox_Material()
        {
            m_cpn.Text = "";
            m_ipn.Text = "";
            m_maker.Text = "";
            m_maker_code.Text = "";
            m_desc.Text = "";
            m_comp_type.Text = "";
            m_remarks.Text = "";
        }

        private void ClearTextbox_Model()
        {
            model_fam.Text = "";
            model_model.Text = "";
        }

        private void ClearTextbox_Line()
        {
            line_family.Text = "";
            line_model.Text = "";
            line_line.Text = "";
        }

        private void ClearTextbox_Rack()
        {
            rack_pn.Text = "";
            rack_rack.Text = "";
            rack_slot.Text = "";
        }

        private void ClearTextbox_Account()
        {
            acc_id = 0;
            acc_emp_code.Text = "";
            acc_emp_name.Text = "";
            acc_type.Text = "";
            acc_pass.Text = "";
            acc_cpass.Text = "";
        }

        // Drop down fields

        private void line_family_DropDownOpened(object sender, EventArgs e)
        {
            MySqlCommand cmd = new MySqlCommand
            {
                Connection = conn,
                CommandText = "select distinct `petname` from `ionics_model`"
            };
            MySqlDataAdapter da = new MySqlDataAdapter(cmd);
            DataTable dt = new DataTable("dt");
            da.Fill(dt);
            line_family.Items.Clear();

            foreach (DataRow dr in dt.Rows)
            {
                line_family.Items.Add(dr[0].ToString());
            }
        }

        private void line_model_DropDownOpened(object sender, EventArgs e)
        {
            MySqlCommand cmd = new MySqlCommand
            {
                Connection = conn,
                CommandText = "select distinct `model` from `ionics_model` where `petname` ='"+ line_family.Text + "'"
            };
            MySqlDataAdapter da = new MySqlDataAdapter(cmd);
            DataTable dt = new DataTable("dt");
            da.Fill(dt);
            line_model.Items.Clear();

            foreach (DataRow dr in dt.Rows)
            {
                line_model.Items.Add(dr[0].ToString());
            }
        }


      
        // Reset input fields
        private void btn_reset_save_model_Click(object sender, RoutedEventArgs e)
        {
            ClearTextbox_Model();
        }

        private void btn_reset_save_rack_Click(object sender, RoutedEventArgs e)
        {
            ClearTextbox_Rack();
        }

        private void btn_reset_save_line_Click(object sender, RoutedEventArgs e)
        {
            ClearTextbox_Line();
        }

        private void btn_reset_save_acc_Click(object sender, RoutedEventArgs e)
        {
            ClearTextbox_Account();
        }

        private void btn_reset_save_m_Click(object sender, RoutedEventArgs e)
        {
            ClearTextbox_Material();
        }

       


        // ACCOUNTS BUTTON
        private void btn_ANU_Click(object sender, RoutedEventArgs e)
        {
            acc_grid.IsEnabled = true;
            Enable_Account_Input_Details();
            acc_id = 0;
            Stats = "ANU";
        }


        private void btn_ENU_Click(object sender, RoutedEventArgs e)
        {
            acc_grid.IsEnabled = true;
            acc_pass.IsEnabled = false;
            acc_pass.IsEnabled = false;
            Stats = "EU";
        }

        private void btn_CP_Click(object sender, RoutedEventArgs e)
        {
            acc_grid.IsEnabled = true;
            acc_emp_code.IsEnabled = false;
            acc_emp_name.IsEnabled = false;
            acc_type.IsEnabled = false;
            Stats = "CP";
        }

        #endregion

        #region inventory
        private void dt_to_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (dt_to.Value < dt_from.Value)
            {
                MessageBox.Show("Invalid Date Time. Please pick greater than the Time Range From .");
            }
        }

   
        private void Load_Overall_Inventory()
        {
            grid_rad_oi.ItemsSource = null;
            MySqlCommand cmd = new MySqlCommand
            {
                Connection = conn
            };
            cmd.CommandText = "SELECT c.customer_partnumber as 'EPSON PN',a.partnumber as 'IONICS PN',c.description as 'DESCRIPTION',c.remarks as 'REMARKS',b.rack as 'RACK',b.location as 'SLOT',a.total_receive as 'TOTAL RECEIVE'," +
                "a.total_return as 'TOTAL RETURN',a.total_stocks as 'TOTAL STOCKS' FROM admdams.ionics_inventory a  inner join ionics_material_details c on a.partnumber = c.ionics_partnumber inner join ionics_rack b on c.customer_partnumber = b.partnumber";
            MySqlDataAdapter da = new MySqlDataAdapter(cmd);
            
            da.Fill(dt_oi);
        
            grid_rad_oi.ItemsSource = dt_oi.DefaultView;
        }

        private void Load_Individual_Inventory(string did)
        {
            grid_rad_ii.ItemsSource = null;
            MySqlCommand cmd = new MySqlCommand
            {
                Connection = conn
            };
            cmd.CommandText = "SELECT did as 'DID',partnumber as 'PART NUMBER',lot_number as 'LOT NUMBER',quantity as 'QUANTITY',invoice_number as 'INVOICE NUMBER' ,pic as 'REGISTERED BY', remarks as 'REMARKS',stats as 'STATUS' FROM " +
                "(select a.did,a.partnumber,lot_number,quantity,invoice_number,pic,remarks,'RECEIVED' as 'stats' from ionics_receive a inner join ionics_parts b on a.partnumber=b.partnumber union all " +
                "select a.did,a.partnumber,lot_number,issued_qty,invoice_number,pic,remarks,'ISSUED' as 'stats' from ionics_issuance a inner join ionics_parts b on a.partnumber=b.partnumber  union all " +
                "select a.did,a.partnumber,lot_number,return_quantity,invoice_number,pic,remarks,'RETURNED' as 'stats' from ionics_return a inner join ionics_parts b on a.partnumber=b.partnumber) query WHERE did = '" + did + "'";
            MySqlDataAdapter da = new MySqlDataAdapter(cmd);

            da.Fill(dt_ii);

            grid_rad_ii.ItemsSource = dt_ii.DefaultView;
        }

        private void io_trans_DropDownClosed(object sender, EventArgs e)
        {
            switch (io_trans.Text)
            {
                case "Receiving":                    
                    WithTable = receive_select_query + " ionics_receive";
                    ii_cate.IsEnabled = true;
                    ii_search.Text = "";
                    ii_cate.SelectedIndex = -1;
                    break;
                case "Issuance":                   
                    WithTable = issuance_select_query + " ionics_issuance";
                    ii_cate.IsEnabled = true;
                    ii_search.Text = "";
                    ii_cate.SelectedIndex = -1;
                    break;
                case "Return":                   
                    WithTable = return_select_query + " ionics_return";
                    ii_cate.IsEnabled = true;
                    ii_search.Text = "";
                    ii_cate.SelectedIndex = -1;
                    break;
                case "All":
                    ii_cate.SelectedIndex = 0;
                    ii_search.Text = "";
                    ii_cate.IsEnabled = false;
                    ii_search.Focus();
                    break;
            }
           
        }

        private void ii_cate_DropDownClosed(object sender, EventArgs e)
        {
            switch (ii_cate.Text)
            {
                case "DID":                    
                    WithColumn = "a.did";
                    ii_search.IsEnabled = true;
                    break;
                case "Ionics PN":
                    WithColumn = "a.partnumber";
                    ii_search.IsEnabled = true;
                    break;
                case "Rack":
                    WithColumn = "a.rack";
                    ii_search.IsEnabled = true;
                    break;
                case "Slot":
                    WithColumn = "a.location";
                    ii_search.IsEnabled = true;
                    break;
                case "Invoice Number":
                    WithColumn = "a.invoice_number";
                    ii_search.IsEnabled = true;
                    break;
                case "Lot Number":
                    WithColumn = "a.lot_number";
                    ii_search.IsEnabled = true;
                    break;
                case "Registered By":
                    WithColumn = "b.pic";
                    ii_search.IsEnabled = true;
                    break;
                case "Time Range Only":
                    ii_search.IsEnabled = false;
                    ii_search.Text = "";
                    break;
            }
        }

        private void btn_serach_individual_Click(object sender, RoutedEventArgs e)
        {
            try
            {                           
            MySqlCommand cmd = new MySqlCommand
            {
                Connection = conn
            };
            if (io_trans.Text != "" && ii_cate.Text != "" && ii_search.Text == "")
            {
                grid_rad_ii.ItemsSource = null;
                    grid_rad_ii.Items.Refresh();

                if (ii_cate.Text == "Time Range Only")
                {
                    cmd.CommandText = WithTable + " b ON a.partnumber = b.partnumber WHERE (b.timestamp BETWEEN '"+ dt_from.Value + "' AND '" + dt_to.Value + "')";
                }               
                MySqlDataAdapter da = new MySqlDataAdapter(cmd);
                da.Fill(dt_ii);
                grid_rad_ii.ItemsSource = dt_ii.DefaultView;
            }
            else if (io_trans.Text != "" && ii_cate.Text != "" && ii_search.Text != "")
            {
                if (io_trans.Text == "All")
                {
                    Load_Individual_Inventory(ii_search.Text.ToUpper());
                }
                else
                {
                    grid_rad_ii.ItemsSource = null;
                        grid_rad_ii.Items.Refresh();
                        cmd.CommandText = WithTable + " b ON a.partnumber = b.partnumber WHERE (b.timestamp BETWEEN '" + dt_from.Value + "' AND '" + dt_to.Value + "') AND "+ WithColumn +" = '"+ ii_search.Text.ToUpper() +"'";
                    MySqlDataAdapter da = new MySqlDataAdapter(cmd);
                    da.Fill(dt_ii);
                    grid_rad_ii.ItemsSource = dt_ii.DefaultView;
                }
            }
            else
            {
                MessageBox.Show("Please complete the details.");
            }
            }
            catch (Exception message)
            {

                MessageBox.Show(message.ToString());
            }
        }

        #endregion
    }
}
