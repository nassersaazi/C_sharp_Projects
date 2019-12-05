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
        WebService1SoapClient obj;
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int a = Convert.ToInt32(textBox1.Text);
            int b = Convert.ToInt32(textBox2.Text);
            int ans = obj.add(a, b);
            MessageBox.Show("Addition is :" + ans);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            int a = Convert.ToInt32(textBox1.Text);
            int b = Convert.ToInt32(textBox2.Text);
            int ans = obj.subtract(a, b);
            MessageBox.Show("Subtraction is :" + ans);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            obj = new WebService1SoapClient();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            int a = Convert.ToInt32(textBox1.Text);
            int b = Convert.ToInt32(textBox2.Text);
            int ans = obj.multiply(a, b);
            MessageBox.Show("Multiplication is :" + ans);

        }

        private void button4_Click(object sender, EventArgs e)
        {
            int a = Convert.ToInt32(textBox1.Text);
            int b = Convert.ToInt32(textBox2.Text);
            int ans = obj.divide(a, b);
            MessageBox.Show("Division is :" + ans);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            SqlConnection conn = new SqlConnection("Data Source=(localdb)\\MSSQLLocalDB;Initial" +
                " Catalog=sp;Integrated Security=True");
            conn.Open();
            //SqlCommand cmd = new SqlCommand("insert into users(name,type) values('"+name.Text+ "','"+type.Text+"')",conn);
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = "Execute spSaveDetails @name,@type";

            cmd.Parameters.Add("@name", SqlDbType.VarChar, 10).Value = name.Text.ToString();
           
            cmd.Parameters.Add("@type", SqlDbType.VarChar, 10).Value = type.Text.ToString();

            int i = cmd.ExecuteNonQuery();
            if (i != 0 )
            {
                MessageBox.Show("Data saved");
            }
            else
            {
                MessageBox.Show("Query didn't work you fuckin' IDIOT!!!");
            }
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }
    }
}
