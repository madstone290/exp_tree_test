using Flee.PublicTypes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ExpTree
{
    public partial class Form1 : Form
    {
        private readonly Dictionary<string, string> param = new Dictionary<string, string>()
        {
            { "가", "a" },
            { "나", "b" },
            { "다", "c" },
            { "라", "d" },
        };
        public Form1()
        {
            InitializeComponent();
        }

        public double Value1
        {
            get => double.TryParse(textBox1.Text, out double r) ? r : 0;
            set => textBox1.Text = value.ToString();
        }
        public double Value2
        {
            get => double.TryParse(textBox2.Text, out double r) ? r : 0;
            set => textBox2.Text = value.ToString();
        }
        public double Value3
        {
            get => double.TryParse(textBox3.Text, out double r) ? r : 0;
            set => textBox3.Text = value.ToString();
        }
        public double Value4
        {
            get => double.TryParse(textBox4.Text, out double r) ? r : 0;
            set => textBox4.Text = value.ToString();
        }
        public string ExpressionText
        {
            get => textBox5.Text;
            set => textBox5.Text = value;
        }

        public string Result
        {
            get => textBox6.Text;
            set => textBox6.Text = value;
        }

        public double GetValue(string param)
        {
            switch (param)
            {
                case "a":
                    return Value1;
                case "b":
                    return Value2;
                case "c":
                    return Value3;
                case "d":
                    return Value4;
                default:
                    return 0;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {

                var context = new ExpressionContext();
                context.Imports.AddType(typeof(Math));

                string exp = ExpressionText;
                foreach (var pair in param)
                {
                    if (exp.Contains(pair.Key))
                    {
                        exp = exp.Replace(pair.Key, pair.Value);
                        context.Variables.Add(pair.Value, GetValue(pair.Value));
                    }
                }

                var compiledExp = context.CompileGeneric<double>(exp);
                string r = Convert.ToString(compiledExp.Evaluate());
                Result = Convert.ToString(r);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }
    }
}
