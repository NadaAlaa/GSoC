using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.IO;

namespace Phonebook
{
    static class Program
    {
        Person person;
        Contact contact;
        string record;

        void AddPerson(string id, string name, string add)
        {
            person = new Person(id, name, add);
            FileStream FS= new FileStream("Persons.txt", FileMode.Append);
            Byte []B = new Byte[person.RecordLength];

            FS.WriteByte((byte)person.RecordLength);
            record = person.ID + person.Name + person.Address;

            for (int i = 0; i < person.RecordLength; i++)
                B[i] = (byte)record[i];

            FS.Write(B, 0, person.RecordLength);

            FS.Close();
        }

        void AddContact(string id, string PhoneNumber)
        {
            FileStream FS = new FileStream("Contacts.txt",FileMode.Open);
            StreamReader SR =  new StreamReader(FS);
            StreamWriter SW = new StreamWriter(FS);
            while(SR.Peek()!=-1)
            {
                long position = FS.Position;
                string[] fields;

                record = SR.ReadLine();
                fields = record.Split('|');

                if (fields[0] == id)
                {
                    char Type = ''; //ContactType
                    contact = new Contact(fields[0], fields[1]);
                    contact.AddContact(Type,PhoneNumber);
                    FS.Seek(position,SeekOrigin.Begin);
                    record = contact.ID + contact.PhoneNumber;
                    SW.WriteLine(record);
                    SW.Close();
                    SR.Close();
                    return;
                }
            }
        }

        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}
