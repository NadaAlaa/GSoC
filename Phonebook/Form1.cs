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
        public static Person person;
        public static Contact contact;
        public static string Record;
        public static int MAX = 200;

        public byte[] ConvertToByteArray(string Record)
        {
            byte[] tmp = new byte[Record.Length];

            for (int i = 0; i < Record.Length; i++)
                tmp[i] = (byte)Record[i];

            return tmp;
        }

        public string ConvertToString(char[] arr)
        {
            string str = "";
            for (int i = 0; i < arr.Length; i++)
                str += arr[i];

            return str;
        }

        public string ConvertToString(byte[] arr)
        {
            string str = "";
            for (int i = 0; i < arr.Length; i++)
                str += (char)arr[i];

            return str;
        }

        bool AddInFirstFit(FileStream FS, Person person)
        {
            Record = person.ID + person.Name + person.Address;
            int RequiredLen = Record.Length + 1;
            byte[] Buffer;
            FS.Seek(0, SeekOrigin.Begin);

            int Header = FS.ReadByte();
            int FirstPos = 0, SecondPos = Header, Offset;

            while (SecondPos != 0)
            {
                FS.Seek(SecondPos, SeekOrigin.Begin);
                int Len = FS.ReadByte();

                if (RequiredLen <= Len)
                {
                    FS.ReadByte();
                    Offset = FS.ReadByte();

                    Buffer = new Byte[RequiredLen];
                    ConvertToByteArray(Record).CopyTo(Buffer, 0);

                    FS.Seek(SecondPos + 1, SeekOrigin.Begin);
                    FS.Write(Buffer, 0, RequiredLen);

                    for (int i = 0; i < Len - RequiredLen; i++)
                        FS.WriteByte((byte)0);

                    FS.Seek(FirstPos, SeekOrigin.Begin);
                    FS.WriteByte((byte)Offset);

                    return true;
                }
                else
                {
                    FS.ReadByte();
                    FirstPos = SecondPos + 2;
                    SecondPos = FS.ReadByte();
                }
            }
            return false;
        }

        public bool DeletePerson(string ID)
        {
            FileStream FS = new FileStream("Person.txt", FileMode.Open, FileAccess.ReadWrite);
            if (FS.Length <= 0)
                return false;

            int Header = FS.ReadByte();
            int RecordLength = FS.ReadByte(), Pos;

            while (RecordLength != -1)
            {
                if (RecordLength == 0) continue;

                Pos = (int)FS.Position;
                byte[] Temp = new Byte[RecordLength];
                FS.Read(Temp, 0, RecordLength);

                if ((char)Temp[0] != '*')
                {
                    string str = ConvertToString(Temp);
                    string[] RecordTokens = str.Split('|');

                    if (RecordTokens[0].CompareTo(ID) == 0)
                    {
                        FS.Seek(Pos, SeekOrigin.Begin);
                        FS.WriteByte((byte)'*');
                        FS.WriteByte((byte)Header);

                        FS.Seek(0, SeekOrigin.Begin);
                        FS.WriteByte((byte)(Pos - 1));

                        return true;
                    }
                }

                RecordLength = FS.ReadByte();
            }
            return false;
        }

        public bool DeleteContact(string ID)
        {
            FileStream FS = new FileStream("Contact.txt", FileMode.Open, FileAccess.ReadWrite);
            StreamReader SR = new StreamReader(FS);
            StreamWriter SW;
            string[] FileRecords = new string[MAX];
            bool Found = false;
            int SIZE = 0;

            while (SR.Peek() != -1)
            {
                Record = SR.ReadLine();
                if (Record[0] != '*')
                {
                    string[] Tokens = Record.Split('*');
                    if (Tokens[0] == ID)
                    {
                        contact = new Contact("*", Tokens[1]);
                        Record = contact.ID + contact.PhoneNumber;

                        Found = true;
                    }
                }
                FileRecords[SIZE++] = Record;
            }

            FS.Seek(0, SeekOrigin.Begin);
            SW = new StreamWriter(FS);

            for (int i = 0; i < SIZE; i++)
            {
                SW.WriteLine(FileRecords[i]);
            }

            SW.Close();
            SR.Close();
            return Found;
        }

        public void AddPerson(string id, string name, string add)
        {
            int Header = 0;
            FileStream FS = new FileStream("Person.txt", FileMode.OpenOrCreate, FileAccess.ReadWrite);
            person = new Person(id, name, add);

            if (FS.Length != 0)
                Header = FS.ReadByte();
            else FS.WriteByte((byte)0);

            if (Header == 0 || !AddInFirstFit(FS, person))
            {
                FS.Seek(0, SeekOrigin.End);

                string Record = person.ID + person.Name + person.Address;
                byte[] Buffer = new byte[Record.Length];

                ConvertToByteArray(Record).CopyTo(Buffer, 0);

                FS.WriteByte((byte)person.RecordLength);
                FS.Write(Buffer, 0, person.RecordLength);
            }
            FS.Close();

            MessageBox.Show("Person Added!");
        }

        public void UpdateFile(FileStream FS, string[] FileRecords, int SIZE, int RRN)
        {
            if(SIZE!=0) FS.Seek(0, SeekOrigin.Begin);
            
            StreamWriter SW = new StreamWriter(FS);

            for (int i = 0; i < SIZE; i++)
            {
                if(RRN!=i) SW.WriteLine(FileRecords[i]);
            }

            SW.WriteLine(Record);
            SW.Close();

            MessageBox.Show("Contact Added!");
        }

        public void AddContact(string ID, string PhoneNumber, string ContactType)
        {
            FileStream FS = new FileStream("Contact.txt", FileMode.OpenOrCreate, FileAccess.ReadWrite);
            StreamReader SR = new StreamReader(FS);
            
            contact = new Contact();

            if (contact.CanAddContact(ContactType, PhoneNumber))
            {
                int RRN = -1, NoOfRecords = 0;
                string[] FileRecords= new string[MAX];
                string TempRecord;

                while (SR.Peek() != -1)
                {
                    TempRecord = SR.ReadLine();

                    string[] Tokens = TempRecord.Split('*');

                    if (Tokens[0] == ID)
                    {
                        contact = new Contact(ID, Tokens[1]);
                        contact.AddNumber(PhoneNumber, ContactType);

                        Record = contact.ID + contact.PhoneNumber;

                        RRN = NoOfRecords;
                    }
                    else FileRecords[NoOfRecords++] = TempRecord; 
                }

                if (RRN != -1)
                {
                    UpdateFile(FS, FileRecords, NoOfRecords, -1);
                    SR.Close();
                    return;
                }

                RRN = 0;
                
                FS.Seek(0, SeekOrigin.Begin);
                SR = new StreamReader(FS);

                while (SR.Peek() != -1)
                {
                    Record = SR.ReadLine();
                    if (Record[0] == '*')
                    {
                        contact = new Contact(ID, ContactType[0] + PhoneNumber);
                        Record = contact.ID + contact.PhoneNumber;

                        UpdateFile(FS, FileRecords, NoOfRecords, RRN);

                        SR.Close();
                        return;
                    }
                    RRN++;
                }
                
                contact = new Contact(ID, ContactType[0] + PhoneNumber);
                Record = contact.ID + contact.PhoneNumber;

                UpdateFile(FS, FileRecords, 0, -1);
                SR.Close();

            }
            else
            {
                MessageBox.Show("Invalid Number!");
            }

        }
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

            AddPerson(ID, Name, Address);
        }

        private void AddNewContact_Button_Click(object sender, EventArgs e)
        {
            string ID = AddNewContactID_TextBox.Text;
            string Number = AddNewContact_TextBox.Text;
            string ContactType = PhoneType_ComboBox.Text;

            AddContact(ID, Number, ContactType);
        }

        private void DeletePerson_Button_Click(object sender, EventArgs e)
        {
            if (DeletePerson(DeleteContactID_TextBox.Text) || DeleteContact(DeleteContactID_TextBox.Text))
            {
                MessageBox.Show("Person Deleted!");
            }
        }
    }
}
