using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Phonebook
{
    public class Contact
    {
        public string ID;
        public string PhoneNumber;

        public Contact()
        { }

        public Contact(string id, string phoneNumber = "0000000000")
        {
            this.ID = id + '*';
            this.PhoneNumber = phoneNumber;
        }

        public bool CanAddContact(string ContactType, string PhoneNumber)
        {
            if (ContactType == "Mobile" && PhoneNumber.Length != 12)
            {
                return false;
            }

            if (ContactType != "Mobile" && PhoneNumber.Length != 10)
            {
                return false;
            }

            return true;
        }

        public void AddNumber(string PhoneNumber, string ContactType)
        {
            this.PhoneNumber += ContactType[0] + PhoneNumber;
        }

        public bool DisplayHomeNumbers(string Name)
        {
            ListViewItem _Name = new ListViewItem(Name);
            bool FoundHomeNumber = false;
            string AllNumbers = "";
            for (int i = 0; i < this.PhoneNumber.Length; i++)
            {
                if (this.PhoneNumber[i] == 'H')
                {
                    if(AllNumbers.Length==0) AllNumbers=PhoneNumber.Substring(i+1,10);
                    else AllNumbers += " / " +PhoneNumber.Substring(i + 1, 10);
                    FoundHomeNumber = true;
                }
            }
            if (FoundHomeNumber) 
            {
                _Name.SubItems.Add(AllNumbers);
                Form1.ListView.Items.Add(_Name);
            }
            return FoundHomeNumber;
        }

        public bool FoundWorkNumber()
        {
            for (int i = 0; i < this.PhoneNumber.Length; i++)
            {
                if (this.PhoneNumber[i] == 'W')
                {
                    return true;
                }
            }
            return false;
        }
    }
}
