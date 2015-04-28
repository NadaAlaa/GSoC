using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Phonebook
{
    public class Contact
    {
        public string ID;
        public string PhoneNumber;

        public Contact() 
        {}

       public Contact(string id, string phoneNumber = "0000000000")
        {
            this.ID = id;
            this.PhoneNumber = phoneNumber;
        }


        public bool AddContact(char ContactType, string PhoneNumber)
        {
            if(ContactType == 'M' && PhoneNumber.Length!=12){
                return false;
            }

            if (ContactType != 'M' && PhoneNumber.Length != 10)
            {
                return false;
            }

            this.PhoneNumber += ContactType + PhoneNumber;

            return true;
        }

        public bool DisplayHomeNumbers()
        {
            bool FoundHomeNumber = false;
            for (int i = 0; i < this.PhoneNumber.Length; i++)
            {
                if (this.PhoneNumber[i] == 'H')
                {
                    /*
                     * Output: ID this.PhoneNumber.substr(i+1,10);
                     * 
                     * */
                    FoundHomeNumber = true;
                }
            }
            return FoundHomeNumber;
        }

        bool FoundWorkNumber()
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
