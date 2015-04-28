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
        static Person person;
        static Contact contact;
        static string record;


        static byte[] ConvertToByteArray(string Record)
        {
            byte[] tmp = new byte[Record.Length];

            for (int i = 0; i < Record.Length; i++)
                tmp[i] = (byte)Record[i];

            return tmp;
        }

        static string ConvertToString(char[] arr)
        {
            string str = "";
            for (int i = 0; i < arr.Length; i++)
                str += arr[i];

            return str;
        }

        static string ConvertToString(byte[] arr)
        {
            string str = "";
            for (int i = 0; i < arr.Length; i++)
                str += (char)arr[i];

            return str;
        }

        static bool AddInFirstFit(FileStream FS, string Record)
        {
            int RequiredLen = Record.Length;
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

        static bool DeleteRecord(FileStream FS, string ID)
        {
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

        static void AddPerson(string id, string name, string add)
        {
            person = new Person(id, name, add);
            FileStream FS = new FileStream("Persons.txt", FileMode.Append);
            Byte[] B = new Byte[person.RecordLength];

            FS.WriteByte((byte)person.RecordLength);
            record = person.ID + person.Name + person.Address;

            for (int i = 0; i < person.RecordLength; i++)
                B[i] = (byte)record[i];

            FS.Write(B, 0, person.RecordLength);

            FS.Close();
        }

        static void AddContact(string id, string PhoneNumber)
        {
            FileStream FS = new FileStream("Contacts.txt", FileMode.Open);
            StreamReader SR = new StreamReader(FS);
            StreamWriter SW = new StreamWriter(FS);
            while (SR.Peek() != -1)
            {
                long position = FS.Position;
                string[] fields;

                record = SR.ReadLine();
                fields = record.Split('|');

                if (fields[0] == id)
                {
                    char Type = ' '; //ContactType
                    contact = new Contact(fields[0], fields[1]);
                    contact.AddContact(Type, PhoneNumber);
                    FS.Seek(position, SeekOrigin.Begin);
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
