using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
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
    }
}
