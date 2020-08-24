using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using WindowsFormsApp1.ServiceReference1;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        string connectionString = "Data Source=(localdb)\\MSSQLLocalDB;Initial" +
                " Catalog=sp;Integrated Security=True"; //string of database source we are connecting to

        WebService1SoapClient obj; //initializing web service client object

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int a = Convert.ToInt32(textBox1.Text);
            int b = Convert.ToInt32(textBox2.Text);
            int ans = obj.add(a, b); //call to the web service add method
            MessageBox.Show("Addition is :" + ans);
        }

        /// <summary>
        ///  implemented when subtract button on the UI is clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            int a = Convert.ToInt32(textBox1.Text);
            int b = Convert.ToInt32(textBox2.Text);
            int ans = obj.subtract(a, b); //call to the web service subtract method
            MessageBox.Show("Subtraction is :" + ans);
        }

        /// <summary>
        /// Impelemented when form UI is loaded
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_Load(object sender, EventArgs e)
        {
            obj = new WebService1SoapClient(); //initializing webservice class object
        }

        /// <summary>
        /// implemented when multiply button of the UI is clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button3_Click(object sender, EventArgs e)
        {
            int a = Convert.ToInt32(textBox1.Text);
            int b = Convert.ToInt32(textBox2.Text);
            int ans = obj.multiply(a, b); //call to the web service multiply function
            MessageBox.Show("Multiplication is :" + ans);

        }

        /// <summary>
        /// Impelements divide functionality
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button4_Click(object sender, EventArgs e)
        {
            int a = Convert.ToInt32(textBox1.Text);
            int b = Convert.ToInt32(textBox2.Text);
            int ans = obj.divide(a, b); //call to the web service divide function
            MessageBox.Show("Division is :" + ans);
        }


        /// <summary>
        /// Implemented when SAVE button is clicked after the user has entered relevant information
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button5_Click(object sender, EventArgs e)
        {

            using (SqlConnection conn = new SqlConnection(connectionString))//usng directive helps close 
            //the connection in case an error occurs 
            {
                conn.Open(); //open connection
                SqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = "Execute spSaveDetails @name,@type"; //stored procedure to save details

                cmd.Parameters.Add("@name", SqlDbType.VarChar, 10).Value = name.Text.ToString();

                cmd.Parameters.Add("@type", SqlDbType.VarChar, 10).Value = type.Text.ToString();

                int i = cmd.ExecuteNonQuery();
                if (i != 0)
                {
                    MessageBox.Show("Data saved"); //executed when 'spSaveDetails' stored procedure is successful
                }
                else
                {
                    MessageBox.Show("Query didn't work you fuckin' IDIOT!!!");//executed when 'spSaveDetails' stored procedure is unsuccessful
                }
            }
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void clientsButton_Click(object sender, EventArgs e)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd2 = conn.CreateCommand();
                var x = cmd2.CommandText = "Execute spGetAllClients"; //executes spGetAllClients stored procedure
                SqlDataAdapter da = new SqlDataAdapter(x, conn);
                DataTable dt = new DataTable();
                da.Fill(dt);

                dgv1.DataSource = dt;
            }
             
        }

        private void adminsButton_Click(object sender, EventArgs e)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd3 = conn.CreateCommand();
                var y = cmd3.CommandText = "Execute spGetAllAdmins"; //executes spGetAllClients stored procedure
                SqlDataAdapter da2 = new SqlDataAdapter(y, conn);
                DataTable dt1 = new DataTable();
                da2.Fill(dt1);

                dgv1.DataSource = dt1;
            }
        }
    }
}
