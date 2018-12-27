using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace Phone_boob
{
    public partial class Form1 : Form
    {
        static AppData db;

        protected static AppData App
        {
            get
            {
                if (db == null)
                    db = new AppData();
                return db;
            }
        }

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                string fileName = string.Format("{0}//data.dat", Application.StartupPath);
                if (File.Exists(fileName))
                    App.PhoneBook.ReadXml(fileName);
                phoneBookBindingSource.DataSource = App.PhoneBook;
                btnSave.Enabled = false;
                panel1.Enabled = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                App.PhoneBook.RejectChanges();
            }
        }

        private void dataGridView_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                if (MessageBox.Show("Вы действительно хотите удалить эту запись?", "Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    phoneBookBindingSource.RemoveCurrent();
                    App.PhoneBook.WriteXml(string.Format("{0}//data.dat", Application.StartupPath));
                }
            }
        }

        private void txtSearch_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)13)
                if (!string.IsNullOrEmpty(txtSearch.Text))
                {
                    var query = from o in App.PhoneBook
                                where o.PhoneNumber == txtSearch.Text || o.Name.Contains(txtSearch.Text) || o.Email.Contains(txtSearch.Text)
                                select o;
                    dataGridView.DataSource = query.ToList();
                }
                else
                    dataGridView.DataSource = phoneBookBindingSource;
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            try
            {
                btnNew.Enabled = false;
                btnSave.Enabled = true;
                panel1.Enabled = true;
                App.PhoneBook.AddPhoneBookRow(App.PhoneBook.NewPhoneBookRow());
                phoneBookBindingSource.MoveLast();
                txtPhoneNumber.Focus();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                App.PhoneBook.RejectChanges();
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            btnSave.Enabled = true;
            panel1.Enabled = true;
            txtPhoneNumber.Focus();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            try
            {
                if (btnNew.Enabled == false)
                {
                    App.PhoneBook.RemovePhoneBookRow(App.PhoneBook[App.PhoneBook.Count() - 1]);
                    btnNew.Enabled = true;
                    btnSave.Enabled = false;
                }
                phoneBookBindingSource.ResetBindings(false);
                panel1.Enabled = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                App.PhoneBook.RejectChanges();
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                phoneBookBindingSource.EndEdit();
                App.PhoneBook.AcceptChanges();
                App.PhoneBook.WriteXml(string.Format("{0}//data.dat", Application.StartupPath));
                panel1.Enabled = false;
                btnNew.Enabled = true;
                btnSave.Enabled = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                App.PhoneBook.RejectChanges();
            }
        }

        private void txtEmail_Validating(object sender, CancelEventArgs e)
        {
            Regex regex = new Regex(@"^([a-z0-9_-]+\.)*[a-z0-9_-]+@[a-z0-9_-]+(\.[a-z0-9_-]+)*\.[a-z]{2,6}$");
            if (!regex.IsMatch(txtEmail.Text) && txtEmail.Text.Length != 0)
            {
                txtEmail.ForeColor = Color.Red;
                MessageBox.Show("Invalid mail address!");
            }
            else
            {
                btnSave.Enabled = true;
                txtEmail.ForeColor = Color.Black;
            }
        }

        private void txtPhoneNumber_Validating(object sender, CancelEventArgs e)
        {
            Regex regex = new Regex(@"^\+*[0-9]+$");
            if (!regex.IsMatch(txtPhoneNumber.Text) && txtEmail.Text.Length == 0)
            {
                txtPhoneNumber.ForeColor = Color.Red;
                MessageBox.Show("Write or correct phone number!");
            }
            else
            {
                txtPhoneNumber.ForeColor = Color.Black;
            }
        }

        private void txtName_Validating(object sender, CancelEventArgs e)
        {

        }
    }
}
