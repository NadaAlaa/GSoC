using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Phonebook
{
    public class Person
    {
        public int RecordLength;
        public string ID, Name, Address;

        public Person()
        {
            this.RecordLength = 0;
        }

        public Person(string id, string name, string address)
        {
            this.ID = id + '|';
            this.Name = name + '|';
            this.Address = address;
            RecordLength = this.ID.Length + this.Name.Length + this.Address.Length;
        }
        
        public void DisplayData()
        {
            ListViewItem data = new ListViewItem(Name);
            data.SubItems.Add(Address);

            Form1.ListView.Items.Add(data);
        }
    }

}
