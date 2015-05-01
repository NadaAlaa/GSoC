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
using System.Dynamic;

namespace Phonebook
{
    public partial class Form1 : Form
    {
        
        public Form1()
        {
            InitializeComponent();
        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {

        }

        private void AddNewPerson_Button_Click(object sender, EventArgs e)
        {
            string ID = AddNewPersonID_TextBox.Text;
            string Name = AddNewPersonName_TextBox.Text;
            string Address = AddNewPersonAddress_TextBox.Text;

            Program.RemoveLeadingSpaces(ref ID);
            Program.RemoveLeadingSpaces(ref Name);
            Program.RemoveLeadingSpaces(ref Address);

            if (ID.Length == 0)
            {
                MessageBox.Show("Please Enter a Valid ID!");
                return;
            }

            if (Name.Length == 0)
            {
                MessageBox.Show("Please Enter a Valid Name!");
                return;
            }

            if (Address.Length == 0)
            {
                MessageBox.Show("Please Enter a Valid Address!");
                return;
            }

            Program.AddPerson(ID, Name, Address);
        }

        private void AddNewContact_Button_Click(object sender, EventArgs e)
        {
            string ID = AddNewContactID_TextBox.Text;
            string Number = AddNewContact_TextBox.Text;
            string ContactType = PhoneType_ComboBox.Text;

            Program.RemoveLeadingSpaces(ref ID);
            Program.RemoveLeadingSpaces(ref Number);
            Program.RemoveLeadingSpaces(ref ContactType);

            if (ID.Length == 0)
            {
                MessageBox.Show("Please Enter a Valid ID!");
                return;
            }

            if (ContactType.Length == 0)
            {
                MessageBox.Show("Please Choose a Valid Contact Type!");
                return;
            }

            Program.AddContact(ID, Number, ContactType);
        }

        private void DeletePerson_Button_Click(object sender, EventArgs e)
        {
            if (Program.DeletePerson(DeleteContactID_TextBox.Text))
            {
                Program.DeleteContact(DeleteContactID_TextBox.Text);
                MessageBox.Show("Person Deleted.");
            }
            else
            {
                MessageBox.Show("Person Not Found.");
            }
        }

        private void DisplayHomeNumbers_Click(object sender, EventArgs e)
        {
            ListView.Clear();

            ListView.Columns.Add("Contact Name", 150);
            ListView.Columns.Add("Home Numbers", 430);
            ListView.GridLines = true;
            ListView.View = View.Details;

            if (!Program.FindNumbersOfAllContacts("Home"))
            {
                MessageBox.Show("No Contacts Found.");
            }
        }

        private void ContactsMissingWorkNumbers_Click(object sender, EventArgs e)
        {
            ListView.Clear();

            ListView.Columns.Add("Person Name", 200);
            ListView.Columns.Add("Person Address", 383);
            ListView.GridLines = true;
            ListView.View = View.Details;

            if (!Program.FindNumbersOfAllContacts("Work"))
            {
                MessageBox.Show("No Missing Work Numbers.");
            }
        }
    }
}
