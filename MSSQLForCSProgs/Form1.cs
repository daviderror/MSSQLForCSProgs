using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using System.Linq;
namespace MSSQLForCSProgs
{
    public partial class Form1 : Form
    {
        private SqlConnection sqlConnection = null;
        private SqlConnection nrthwindConnection = null;
        private List<string[]> rows = new List<string[]>();
        private List<string[]> filteredList = null;
        public Form1()
        {
            InitializeComponent();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["TestDB"].ConnectionString);
            sqlConnection.Open();
            nrthwindConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["NorthwindDB"].ConnectionString);
            nrthwindConnection.Open();
            SqlDataAdapter dataAdapter1 = new SqlDataAdapter("SELECT * FROM Products", nrthwindConnection);
            DataSet workDtGrV = new DataSet();
            dataAdapter1.Fill(workDtGrV);
            dataGridView2.DataSource = workDtGrV.Tables[0];

            listView1.Items.Clear();
            SqlDataReader sqlDataReader = null;
            rows = new List<string[]>();
            string[] row = null;
            try
            {
                SqlCommand sqlCommand = new SqlCommand("SELECT ProductName , QuantityPerUnit , UnitPrice FROM Products", nrthwindConnection);
                sqlDataReader = sqlCommand.ExecuteReader();
                ListViewItem item = null;
                while (sqlDataReader.Read())
                {
                    row = new string[]
                    {
                        Convert.ToString(sqlDataReader["ProductName"]),
                        Convert.ToString(sqlDataReader["QuantityPerUnit"]),
                        Convert.ToString(sqlDataReader["UnitPrice"])
                    };
                    rows.Add(row);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                if (sqlDataReader != null && !sqlDataReader.IsClosed)
                {
                    sqlDataReader.Close();
                }
            }
            RefreshList(rows);
        }
        private void RefreshList(List <string[]> list)
        {
            listView2.Items.Clear();
            foreach (string[] s in list)
            {
                listView2.Items.Add(new ListViewItem(s));
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            SqlCommand command = new SqlCommand(
                $"INSERT INTO [Students] (Name, Surname, Birthday, City, Phone, Email) VALUES (@Name, @Surname, @Birthday, @City, @Phone, @Email)", 
                sqlConnection);
            DateTime date = DateTime.Parse(textBox3.Text);

            command.Parameters.AddWithValue("Name", textBox1.Text);
            command.Parameters.AddWithValue("Surname", textBox2.Text);
            command.Parameters.AddWithValue("Birthday", $"{date.Month}/{date.Day}/{date.Year}");
            command.Parameters.AddWithValue("City", textBox4.Text);
            command.Parameters.AddWithValue("Phone", textBox5.Text);
            command.Parameters.AddWithValue("Email", textBox6.Text);

            MessageBox.Show(command.ExecuteNonQuery().ToString());
        }
        private void button2_Click(object sender, EventArgs e)
        {
            SqlDataAdapter dataAdapter = new SqlDataAdapter(textBox7.Text,nrthwindConnection);
            DataSet dataSet = new DataSet();
            dataAdapter.Fill(dataSet);
            dataGridView1.DataSource = dataSet.Tables[0];
        }
        private void button3_Click(object sender, EventArgs e)
        {
            
        }
        private void textBox8_TextChanged(object sender, EventArgs e)
        {
            (dataGridView2.DataSource as DataTable).DefaultView.RowFilter = $"ProductName LIKE '%{textBox8.Text}%'";
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (comboBox1.SelectedIndex)
            {
                case 0:
                    (dataGridView2.DataSource as DataTable).DefaultView.RowFilter = $"UnitsInStock <= 10";
                    break;
                case 1:
                    (dataGridView2.DataSource as DataTable).DefaultView.RowFilter = $"UnitsInStock >= 10 AND UnitsInStock <= 50";
                    break;
                case 2:
                    (dataGridView2.DataSource as DataTable).DefaultView.RowFilter = $"UnitsInStock >= 50";
                    break;
                case 3:
                    (dataGridView2.DataSource as DataTable).DefaultView.RowFilter = "";
                    break;
            }
        }

        private void textBox9_TextChanged(object sender, EventArgs e)
        {
            filteredList = rows.Where((x) =>
            x[0].ToLower().Contains(textBox9.Text.ToLower())).ToList();
            RefreshList(filteredList);
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (comboBox2.SelectedIndex)
            {
                case 0:
                    filteredList = rows.Where((x) =>
                    Double.Parse(x[2]) <= 10).ToList();
                    RefreshList(filteredList);
                    break;
                case 1:
                    filteredList = rows.Where((x) =>
                    Double.Parse(x[2]) > 10 && Double.Parse(x[2])<=100).ToList();
                    RefreshList(filteredList);
                    break;
                case 2:
                    filteredList = rows.Where((x) =>
                    Double.Parse(x[2]) > 100).ToList();
                    RefreshList(filteredList);
                    break;
                case 3:
                    RefreshList(rows);
                    break;
            }
        }
    }
}