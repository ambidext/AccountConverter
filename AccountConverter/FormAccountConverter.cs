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

namespace AccountConverter
{
    public partial class FormAccountConverter : Form
    {
        public string m_CurDir = ".";
        Dictionary<string, string> m_dicItemContents = new Dictionary<string, string>();
        Dictionary<string, string> m_dicItemStore = new Dictionary<string, string>();

        public FormAccountConverter()
        {
            InitializeComponent();
        }

        public void loadItemFiles()
        {
            // load item file
            FileStream fs = new FileStream(m_CurDir+"\\item_contents.csv", FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            StreamReader sr = new StreamReader(fs, Encoding.Default);
            string line;
            while ((line = sr.ReadLine()) != null)
            {
                string[] words = line.Split(',');
                m_dicItemContents.Add(words[0], words[1]);
            }
            sr.Close();
            fs.Close();

            fs = new FileStream(m_CurDir+"\\item_store.csv", FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            sr = new StreamReader(fs, Encoding.Default);
            while ((line = sr.ReadLine()) != null)
            {
                string[] words = line.Split(',');
                m_dicItemStore.Add(words[0], words[1]);
            }
            sr.Close();
            fs.Close();
        }

        private void buttonOpen_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "All File(*.*)|*.*";
            ofd.InitialDirectory = Environment.CurrentDirectory;
            ofd.Title = "Select a file";
            if (ofd.ShowDialog() != DialogResult.OK)
            {
                return;
            }
            textBoxPath.Text = ofd.FileName;
            m_CurDir = ofd.FileName.Replace(Path.GetFileName(ofd.FileName), "");
            loadItemFiles();
        }

        private void buttonConvert_Click(object sender, EventArgs e)
        {
            if (textBoxUser.Text == "")
            {
                MessageBox.Show("Fill User");
                return;
            }

            string line;
            FileStream fs = new FileStream(textBoxPath.Text, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            StreamReader sr = new StreamReader(fs, Encoding.Default);
            FileStream fw = new FileStream(m_CurDir+"\\out.csv", FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
            StreamWriter sw = new StreamWriter(fw, Encoding.Default);
            while ((line = sr.ReadLine()) != null)
            {
                if (line == "")
                    break;
                string[] words = line.Split('\t');
                accountInfo objInfo = new accountInfo(words, textBoxUser.Text, m_dicItemContents, m_dicItemStore);

                sw.Write(objInfo.date); sw.Write(",");
                sw.Write(objInfo.price + ""); sw.Write(",");
                sw.Write(objInfo.item); sw.Write(",");
                sw.Write(objInfo.contents); sw.Write(",");
                sw.Write(objInfo.store); sw.Write(",");
                sw.Write(objInfo.user);
                sw.WriteLine();
            }
            sr.Close();
            fs.Close();
            sw.Close();
            fw.Close();

            MessageBox.Show("Done!");
        }
    }

    public class accountInfo
    {
        public string date;
        public int price;
        public string item;
        public string contents;
        public string store;
        public string user;
        Dictionary<string, string> dicContents;
        Dictionary<string, string> dicStore;
        public accountInfo()
        {

        }

        public accountInfo(string[] str, string username, Dictionary<string, string> dic1, Dictionary<string, string> dic2)
        {
            date = str[0];
            date = date.Replace("년", "-");
            date = date.Replace("월", "-");
            date = date.Replace("일", "");

            store = str[1];
            contents = str[2];
            price = int.Parse(str[3]);
            user = username;

            dicContents = dic1;
            dicStore = dic2;

            item = extractItem();
        }

        public string extractItem()
        {
            foreach (var v in dicContents)
            {
                if (contents.Contains(v.Key))
                {
                    return v.Value;
                }
            }

            foreach (var v in dicStore)
            {
                if (store.Contains(v.Key))
                {
                    return v.Value;
                }
            }

            return "미정";
        }
    }
}
