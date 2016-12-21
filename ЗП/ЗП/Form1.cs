using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace ЗП
{
    public partial class Form1 : Form
    {
        //Parent
        [Serializable]
        [XmlInclude(typeof(EmpTime))]
        [XmlInclude(typeof(EmpFix))]
        public abstract class Employee 
        {
            public Employee() { }

            private string _id;
            [XmlElement("id")]
            public string id
            {
                get { return _id; }
                set { _id = value; }
            }

            private string _fullname;
            [XmlElement("fullname")]
            public string fullname
            {
                get { return _fullname; }
                set { _fullname = value; }
            }

            private string _wage;
            [XmlElement("wage")]
            public string wage
            {
                get { return _wage; }
                set { _wage = value; }
            }

            private string _method;
            [XmlElement("method")]
            public string method
            {
                get { return _method; }
                set { _method = value; }
            }
            public Employee(string id, string fullname, string wage, string method) { string _id = id; string _fullname = fullname; string _wage = wage; string _method = method; }
           
            [XmlIgnore]
            public double insum;
            public Employee(double msal)
            {
                insum = msal;
            }
            // abstract method
            public abstract double Salary
            {
                get;
                set;
            }
            public Employee(Employee other)//Deep copy constructor                                 
            {
                this.id = other.id;
                this.fullname = other.fullname;
                this.wage = other.wage;
                this.method = other.method;
            }
        }
        public class MyEmployeeCollection
        {
            public MyEmployeeCollection() { }
            public MyEmployeeCollection(List<Employee> collection)
            {
                Collection = new List<Employee>();
                if (collection == null)
                {
                    return;
                }
                foreach (Employee element in collection)
                {
                    if (element is EmpFix)
                    {
                        Collection.Add(new EmpFix(0) { id = element.id, fullname = element.fullname, wage = element.wage, method = element.method });
                    } else
                    {
                        Collection.Add(new EmpTime(0) { id = element.id, fullname = element.fullname, wage = element.wage, method = element.method });
                    }
                }
            }
            [XmlArray("employees"), XmlArrayItem("employee")]
            public List<Employee> Collection { get; set; }
        }

        public MyEmployeeCollection newemp = new MyEmployeeCollection(null);
        public MyEmployeeCollection copyemp = new MyEmployeeCollection(null);

        //Child1
        [Serializable]
        public class EmpTime : Employee 
        {
            public EmpTime()
            {
            }
            public EmpTime(double msal) : base(msal)
            {
            }
            public override double Salary
            {
                get
                {
                    return (20.8 * 8 * insum);
                }
                set
                {
                    insum = value;
                }                   
            }
        }

        //Child2
        [Serializable]
        public class EmpFix : Employee 
        {
            public EmpFix()
            {
            }
            public EmpFix(double msal) : base(msal)
            {
            }
            public override double Salary
            {
                get
                {
                    return insum;
                }
                set
                {
                    insum = value;
                }
            }
        }           

        //Сортировка первым способом
        public List<Employee> Sort1(int crow, int ccol, List<Employee> employeemas ) {                   
            string[] savetab = new string[ccol];
            //выполнение сортировки
            for (int i = 0; i < crow; i++)
            {
                //запомним i-ые элементы
                savetab[0] = employeemas[i].id;
                savetab[1] = employeemas[i].fullname; 
                savetab[2] = employeemas[i].wage;
                savetab[3] = employeemas[i].method;

                int j = i - 1;//будем идти, начиная с i-1 элемента 
                while (j >= 0 && Convert.ToDouble(employeemas[j].wage) < Convert.ToDouble(savetab[2]))
                // пока не достигли начала массива 
                // или не нашли элемент больше i-1-го
                // который храниться в строке 
                {
                    //проталкиваем элемент вверх
                    employeemas[j + 1].id = employeemas[j].id;
                    employeemas[j + 1].fullname = employeemas[j].fullname;
                    employeemas[j + 1].wage = employeemas[j].wage;
                    employeemas[j + 1].method = employeemas[j].method;
                    j--;
                }
                // возвращаем i-1 элемент
                employeemas[j + 1].id = savetab[0];
                employeemas[j + 1].fullname = savetab[1];
                employeemas[j + 1].wage = savetab[2];
                employeemas[j + 1].method = savetab[3];
            }

            //Сортировка ФИО
            string[,] sortname = new string[crow, ccol];
            for (int i = 0; i < crow; i++)
            {
                sortname[i, 0] = employeemas[i].id;
                sortname[i, 1] = employeemas[i].fullname;
                sortname[i, 2] = employeemas[i].wage;
                sortname[i, 3] = employeemas[i].method;
            }
            string[] savename = new string[ccol];
            for (int i = 0; i < crow - 1; i++)
            {
                //Поиск одинаковых сумм
                if (employeemas[i].wage == employeemas[i+1].wage)
                {
                    int rowind = i;
                    int z = i + 1;
                    while ((z < crow) && (employeemas[i].wage == employeemas[z].wage))
                    {
                        z += 1;
                    }
                    //Копирование сотрудников с одинаковыми суммами 
                    for (int m = rowind; m < z; m++)
                    {
                        sortname[m, 0] = employeemas[m].id;
                        sortname[m, 1] = employeemas[m].fullname;
                        sortname[m, 2] = employeemas[m].wage;
                        sortname[m, 3] = employeemas[m].method;
                    }
                    //Сортировка по алфавиту
                    bool flag = true;
                    while (flag)
                    {
                        flag = false;
                        for (int y = rowind; y < z - 1; ++y)
                            if (sortname[y, 1].CompareTo(sortname[y + 1, 1]) > 0)
                            {
                                for (int k = 0; k < ccol; k++)
                                {
                                    savename[k] = sortname[y, k];
                                    sortname[y, k] = sortname[y + 1, k];
                                    sortname[y + 1, k] = savename[k];
                                }
                                flag = true;
                            }
                    }
                    //Вставка сортированных строк
                    for (int m = rowind; m < z; m++)
                    {
                        employeemas[m].id = sortname[m, 0];
                        employeemas[m].fullname = sortname[m, 1];
                        employeemas[m].wage = sortname[m, 2];
                        employeemas[m].method = sortname[m, 3];
                    }
                    i = z - 1;
                }

            }
            return employeemas;            
        }

        //Сортировка вторым способом
        public List<Employee> Sort2(int crow, int ccol, List<Employee> employeemas)
        {            
            List<Employee> employeemas5 = new List<Employee>(Sort1(crow, ccol, employeemas));
            employeemas5.RemoveRange(5, crow-5);            
            return employeemas5;
        }

        //Сортировка третьим способом
        public List<Employee> Sort3(int crow, int ccol, List<Employee> employeemas)
        {
            List<Employee> employeemas3 = new List<Employee>(Sort1(crow, ccol, employeemas));
            employeemas3.RemoveRange(0, crow-3);
            return employeemas3;
        }

        public Form1()
        {
            InitializeComponent();
        }       

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            label3.Text = "Почасовая ставка, руб.";
            textBox3.Text = "0";
            textBox4.Text = "0";
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            label3.Text = "Фиксированная месячная оплата, руб.";
            textBox3.Text = "0";
            textBox4.Text = "0";
        }

        private void button1_Click_1(object sender, EventArgs e)
        {            
            if ((textBox1.Text == "") || (textBox2.Text == ""))
            {
                MessageBox.Show("Необходимые входные данные не были заполнены!", "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                if (newemp.Collection.Count == 0)
                {
                    dataGridView1.Columns.Clear();
                }
                if (radioButton1.Checked) {                    
                    newemp.Collection.Add(new EmpTime(0) { id = textBox1.Text, fullname = textBox2.Text, wage = textBox4.Text, method = "Повременная ОТ" });
                } else if (radioButton2.Checked)
                {                    
                    newemp.Collection.Add(new EmpFix(0) { id = textBox1.Text, fullname = textBox2.Text, wage = textBox4.Text, method = "Фиксированная ЗП" });
                }                
                BindingSource biSour = new BindingSource();
                biSour.DataSource = newemp.Collection;
                dataGridView1.DataSource = biSour;
                Controls.Add(dataGridView1);
                dataGridView1.Columns.RemoveAt(dataGridView1.Columns.Count - 1);
                button6.Enabled = true;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewCell cell in dataGridView1.SelectedCells)
            {
                if ((cell.RowIndex <= dataGridView1.RowCount-1) && (newemp.Collection[cell.RowIndex].fullname !="") && (newemp.Collection[cell.RowIndex].wage != "") && (newemp.Collection[cell.RowIndex].id != "") && (newemp.Collection[cell.RowIndex].method != ""))
                {
                    dataGridView1.Rows.RemoveAt(cell.RowIndex);
                }
                else
                {
                    return;
                }
            }       
        }

        private void button3_Click(object sender, EventArgs e)
        {            
            dataGridView1.Rows.Clear();            
        }

        private void button4_Click(object sender, EventArgs e)
        {
            dataGridView1.Columns.Clear();
            dataGridView1.Rows.Clear();
            newemp.Collection.Add(new EmpFix(0) { id = "1", fullname = "Иванов Иван Иванович", wage = "15000", method = "Фиксированная ЗП" });
            newemp.Collection.Add(new EmpTime(0) { id = "2", fullname = "Кондратьев Ким Антонович", wage = "15000", method = "Повременная ОТ" });
            newemp.Collection.Add(new EmpFix(0) { id = "3", fullname = "Капустин Ефим Германнович", wage = "15000", method = "Фиксированная ЗП" });
            newemp.Collection.Add(new EmpFix(0) { id = "4", fullname = "Бирюков Егор Якунович", wage = "11111", method = "Фиксированная ЗП" });
            newemp.Collection.Add(new EmpTime(0) { id = "5", fullname = "Белов Арсений Улебович", wage = "11300", method = "Повременная ОТ" });
            newemp.Collection.Add(new EmpTime(0) { id = "6", fullname = "Агафонов Парфений Евгеньевич", wage = "22010", method = "Повременная ОТ" });
            newemp.Collection.Add(new EmpFix(0) { id = "7", fullname = "Фёдоров Созон Артёмович", wage = "11111", method = "Фиксированная ЗП" });
            newemp.Collection.Add(new EmpTime(0) { id = "8", fullname = "Фёдоров Абрам Артёмович", wage = "12222", method = "Повременная ОТ" });
            newemp.Collection.Add(new EmpFix(0) { id = "9", fullname = "Симонов Авдей Алексеевич", wage = "12222", method = "Фиксированная ЗП" });
            newemp.Collection.Add(new EmpFix(0) { id = "10", fullname = "Капко Иван Петрович", wage = "15000", method = "Фиксированная ЗП" });
            BindingSource biSour = new BindingSource();
            biSour.DataSource = newemp.Collection;
            dataGridView1.DataSource = biSour;
            Controls.Add(dataGridView1);
            dataGridView1.Columns.RemoveAt(dataGridView1.Columns.Count - 1);
            button6.Enabled = true;
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {            
            double inmoney;
            try {
                inmoney = Convert.ToDouble(textBox3.Text);
            }
            catch {
                textBox3.Text = "0";
                MessageBox.Show("Должно быть введено числовое значение!", "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            //Расчет среднемесячной заработной платы
            if (radioButton1.Checked)
            {
                EmpTime emptime = new EmpTime(inmoney);
                textBox4.Text = Convert.ToString(emptime.Salary);
            }
            else if (radioButton2.Checked)
            {
                EmpFix empfix = new EmpFix(inmoney);
                textBox4.Text = Convert.ToString(empfix.Salary);
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            textBox1.Text = "";
            textBox2.Text = "";
            textBox3.Text = "0";
            textBox4.Text = "0";
        }

        private void button6_Click(object sender, EventArgs e)
        {
            dataGridView2.Columns.Clear();
            List<Employee> newempcopy = new List<Employee>(newemp.Collection);
            List<Employee> newemp53 = new List<Employee>(newempcopy);
            //Первый способ сортировки
            if (radioButton3.Checked)
            {
                newemp53 = new List<Employee>(Sort1(newemp.Collection.Count, 4, newempcopy));                
                //newemp = Sort1(newemp.Count, 4, newemp);
                BindingSource biSour = new BindingSource();
                biSour.DataSource = newemp53;
                dataGridView2.DataSource = biSour;
                Controls.Add(dataGridView2);
                dataGridView2.Columns.RemoveAt(dataGridView1.Columns.Count);
            }
            //Второй способ сортировки
            if (radioButton4.Checked) {
                if (newemp.Collection.Count < 5)
                {
                    MessageBox.Show("Введите минимально необходимое сотрудников (5) для выполнения данной сортировки!", "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    newemp53 = new List<Employee>(Sort2(newemp.Collection.Count, 4, newempcopy));
                    BindingSource biSour = new BindingSource();
                    biSour.DataSource = newemp53;
                    dataGridView2.DataSource = biSour;
                    Controls.Add(dataGridView2);
                    dataGridView2.Columns.RemoveAt(dataGridView1.Columns.Count);
                }
            }
            //Третий способ сортировки
            if (radioButton5.Checked)
            {
                if (newemp.Collection.Count < 3)
                {
                    MessageBox.Show("Введите минимально необходимое сотрудников (3) для выполнения данной сортировки!", "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    newemp53 = new List<Employee>(Sort3(newemp.Collection.Count, 4, newempcopy));
                    BindingSource biSour = new BindingSource();
                    biSour.DataSource = newemp53;
                    dataGridView2.DataSource = biSour;
                    Controls.Add(dataGridView2);
                    dataGridView2.Columns.RemoveAt(dataGridView1.Columns.Count);
                }
            }
            copyemp.Collection = newemp53;
        }       

        private void button7_Click(object sender, EventArgs e)
        {
            BindingSource biSour = new BindingSource();
            newemp.Collection.Clear();
            biSour.DataSource = newemp.Collection;
            dataGridView1.DataSource = biSour;
            dataGridView2.DataSource = biSour;
            Controls.Add(dataGridView1);
            Controls.Add(dataGridView2);
            dataGridView1.Columns.Clear();
            dataGridView2.Columns.Clear();
            dataGridView2.DataSource = null;
            try
            {
                string str = "";
                OpenFileDialog dialog = new OpenFileDialog();
                dialog.Filter = "XML-файлы (*.xml)|*.xml";
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    str = dialog.FileName;
                }
                else
                {
                    return;
                }
                XmlSerializer xs = new XmlSerializer(typeof(MyEmployeeCollection));
                using (FileStream fs = new FileStream(str, FileMode.OpenOrCreate))
                {
                    newemp = (MyEmployeeCollection)xs.Deserialize(fs);
                }
            
            button6.Enabled = true;
            biSour.DataSource = newemp.Collection;
            dataGridView1.DataSource = biSour;
            dataGridView1.Columns.RemoveAt(dataGridView1.Columns.Count - 1);
            Controls.Add(dataGridView1);
            }
            catch
            {
                MessageBox.Show("Ошибка при чтении из файла!", "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            MyEmployeeCollection newe = null;
            DataGridView dbg = dataGridView1;
            if (radioButton6.Checked)
            {
                newe = new MyEmployeeCollection(newemp.Collection);
            }
            else if ((radioButton7.Checked))
            {
                newe = new MyEmployeeCollection(copyemp.Collection);
            }            
            string str = "";
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "XML-файлы (*.xml)|*.xml";
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                str = saveFileDialog1.FileName;
            }
            else
            {
                return;
            }
            XmlSerializer xs = new XmlSerializer(typeof(MyEmployeeCollection));
            //StringWriter stringWriter = new StringWriter();
            //xs.Serialize(stringWriter, newe);;
            using (FileStream fs = new FileStream(str, FileMode.OpenOrCreate))
            {
                xs.Serialize(fs, newe);

                Console.WriteLine("Объект сериализован");
            }
        }       

        private void Form1_Load(object sender, EventArgs e)
        {
            BindingSource biSour = new BindingSource();
            biSour.DataSource = newemp.Collection;
            dataGridView2.DataSource = biSour;
            dataGridView1.DataSource = biSour;
            Controls.Add(dataGridView1);
            Controls.Add(dataGridView2);
            dataGridView1.Columns.Clear();
            dataGridView2.Columns.Clear();
        }        
        
    }
}
