using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;

namespace CMS
{
    /// <summary>
    /// Interaction logic for Menu.xaml
    /// </summary>
    public partial class Menu : Window
    {
        public MySqlConnection conn;
        private int Total_Receive, Total_Stocks,Total_Return;
        public Menu()
        {
            InitializeComponent();
        }

        // load database connection
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            conn = DBModule.DBConnect();
        }

        #region navbutton click
        private void inventory_Click(object sender, RoutedEventArgs e)
        {
            tabs.SelectedIndex = 0;
            ClearTextbox_Issue();
            ClearTextbox_Issue_selected();
            ClearTextbox_Return();
            ClearTextbox_Receive();
            Disable_M_Input_Details();
        }

        private void register_Click(object sender, RoutedEventArgs e)
        {
            tabs.SelectedIndex = 1;
            ClearTextbox_Issue();
            ClearTextbox_Issue_selected();
            ClearTextbox_Return();
            Disable_M_Input_Details();
        }

        private void transact_Click(object sender, RoutedEventArgs e)
        {
            tabs.SelectedIndex = 2;
            ClearTextbox_Issue();
            ClearTextbox_Issue_selected();
            ClearTextbox_Return();
            ClearTextbox_Receive();
            Disable_M_Input_Details();
        }

        private void manage_Click(object sender, RoutedEventArgs e)
        {
            tabs.SelectedIndex = 3;
            Load_Material_Details();
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
                cmd.CommandText = "INSERT INTO `ionics_issuance` (`did`, `partnumber`, `timestamp`, `pic`) VALUES ('" + i_did.Text + "','" + i_ipn.Text + "', NOW(),'" + lblname.Text + "')";
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
            string[] LineString;
            MySqlCommand cmd = new MySqlCommand
            {
                Connection = conn,
                CommandText = "select `value` from `ionics_settings` where `name` = 'reason'"
            };
            LineString = cmd.ExecuteScalar().ToString().Split(',');
            re_reason.Items.Clear();

            for (int i = 0; i < LineString.Length - 1; i++)
            {
                re_reason.Items.Add(LineString[i]);
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

                     
                        m_cpn.Text = dr[1].ToString();
                        m_ipn.Text = dr[0].ToString();
                        m_maker.Text = dr[2].ToString();
                        m_maker_code.Text = dr[3].ToString();
                        m_desc.Text = dr[4].ToString();
                        m_comp_type.Text = dr[5].ToString();
                        m_remarks.Text = dr[6].ToString();

                        Enable_M_Input_Details();
                    }

                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }

        }
        private void Load_Material_Details()
        {
            grid_material.ItemsSource = null;
            MySqlCommand cmd = new MySqlCommand
            {
                Connection = conn
            };
            cmd.CommandText = "SELECT ionics_partnumber as 'IONICS PN',customer_partnumber as 'CUSTOMER PN',maker as 'MAKER',maker_code as 'MAKER CODE',description as 'DESCRIPTION',component_type as 'COMPONENT TYPE',remarks as 'REMARKS' FROM ionics_material_details";
                MySqlDataAdapter da = new MySqlDataAdapter(cmd); 
              DataTable dt = new DataTable("dt");
              da.Fill(dt);
               grid_material.ItemsSource = dt.DefaultView;
        }

        private void btn_save_m_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MySqlCommand cmd = new MySqlCommand
                {
                    Connection = conn
                };
                cmd.CommandText = "SELECT Count(*) FROM ionics_material_details WHERE ionics_partnumber = '" + m_ipn.Text + "' AND customer_partnumber = '" + m_cpn.Text + "' ";
                if (Convert.ToInt32(cmd.ExecuteScalar()) == 0)
                {
                    cmd.CommandText = "INSERT INTO ionics_material_details (ionics_partnumber,customer_partnumber,maker,maker_code,description,component_type,remarks) VALUES ('" + m_cpn.Text +"','" + m_ipn.Text +"','" + m_maker.Text + "','" + m_maker_code.Text +"','" + m_desc.Text + "','" + m_comp_type.Text +"','" + m_remarks.Text +"')";
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Added Successfully.");
                    ClearTextbox_Material();
                }
                else
                {
                    cmd.CommandText = "UPDATE ionics_material_details SET maker = '" + m_maker.Text + "',maker_code= '" + m_maker_code.Text + "',description = '" + m_desc.Text + "',component_type = '" + m_comp_type.Text + "',remarks ='" + m_remarks.Text + "'  WHERE ionics_partnumber = '" + m_ipn.Text + "' AND customer_partnumber = '" + m_cpn.Text + "' ";
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Updated Successfully.");
                    ClearTextbox_Material();
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        private void btn_reset_save_m_Click(object sender, RoutedEventArgs e)
        {
            ClearTextbox_Material();
        }

        private void btn_add_m_Click(object sender, RoutedEventArgs e)
        {
            ClearTextbox_Material();
            Enable_M_Input_Details();

        }

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
        #endregion
    }
}
