using Downloader;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Security.Policy;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Forms;

namespace MP_Mod_Mng
{
    public partial class Form1 : Form
    {

        public string LocalLowPath = "";
        public int nodeInt = 0;

        DownloadConfiguration downloadOpt = new DownloadConfiguration() {};
        DownloadService downloader = null;

        public Boolean onlineActive = false;
        public Boolean onlineFileSync = false;
        public Boolean resetable = false;

        public string[] modList = null;
        public string[] modValues = null;

        public Form1()
        {
            InitializeComponent();

            Boolean appDataLinked = true;
            LocalLowPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData).Replace("Roaming", "LocalLow");

            LocalLowPath = Path.Combine(new string[] { LocalLowPath, "Ludeon Studios", "RimWorld by Ludeon Studios", "Config" });

            if (Directory.Exists(LocalLowPath))
            {
                textBox2.Text = LocalLowPath;

                string[] files = Directory.GetFiles(LocalLowPath);
                foreach (var file in files)
                {
                    treeView1.Nodes.Add(Path.GetFileName(file));  
                }
            }
            else {
                textBox2.Text = "[ERROR]";

                logLabel.Text = "Could Not find path to %appdata%.";
            }

            button3_Click(null, null);

            downloader = new DownloadService(downloadOpt);
            
        }

        private async void button1_ClickAsync(object sender, EventArgs e)
        {
            Stream destinationStream = await downloader.DownloadFileTaskAsync(textBox1.Text);

            StreamReader sr = new StreamReader(destinationStream, Encoding.UTF8);

            string content = sr.ReadToEnd();

            logLabel.Text = content;

            sr.Dispose();
            destinationStream.Dispose();
            /*downloader.Dispose();*/

            content = content.Substring(1,content.Length - 2);

            string[] strs = content.Split(',');

            logLabel.Text = "";

            for (int x = 0; x < strs.Length; x++) {
                strs[x] = strs[x].Substring(1, strs[x].Length - 2);
                logLabel.Text += strs[x] + " ";
            }

            onlineActive = true;

            modList = strs;
            modValues = new string[strs.Length];

            button3_Click(null, null);

            
            for (int i = 0; i < strs.Length - 1; i++) {
                Boolean clear = true;

                foreach (TreeNode tn in treeView1.Nodes)
                {
                    if (tn.Text.Equals(strs[i]) && tn.ForeColor == Color.Gray) {
                        clear = false;
                    }
                }

                if (clear) {

                    /*try
                    {*/
                        //downloader = new DownloadService(downloadOpt);
                        Stream destinationStream2 = await downloader.DownloadFileTaskAsync(textBox1.Text + "/" + strs[i]);

                        /*logLabel.Text = textBox1.Text + "/" + strs[i];*/




                        StreamReader sr2 = new StreamReader(destinationStream2, Encoding.UTF8);

                        modValues[i] = sr2.ReadToEnd();

                        //downloader.Dispose();
                        destinationStream2.Dispose();
                        sr2.Dispose();
                    /*}
                    catch (Exception error)
                    {
                        modValues[i] = "[Could-Not-Load] "+ error;
                    }*/
                }

            }

            for (int i = 0; i < treeView1.Nodes.Count; i++) {
                if (treeView1.Nodes[i].Text.Contains("🌐")) {
                    treeView1.Nodes[i].Remove();
                    i--;
                }
            }


            for(int z = 0; z < modList.Length; z++) { 
                if (modList[z] != null) {
                    Boolean clear = true;

                    for (int i = 0; i < treeView1.Nodes.Count; i++)
                    {
                        if (treeView1.Nodes[i].Text.Contains(modList[z]))
                        {
                           clear = false;
                        }
                    }

                    if (clear) {
                        treeView1.Nodes.Add(modList[z] + " 🌐");
                    }
                }
            }

            button2.Enabled = true;
            button4.Enabled = true;

            onlineFileSync = true;
            logLabel.Text = "RecevedFiles";
        }

        private void button2_Click(object sender, EventArgs e)
        {
         
        }

        private void logLabel_Click(object sender, EventArgs e)
        {

        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {

            nodeInt = e.Node.Index;
            if (!onlineActive || e.Node.ForeColor == Color.Gray)
            {
                richTextBox1.Text = File.ReadAllText(Path.Combine(LocalLowPath, e.Node.Text), Encoding.UTF8);

                
                
            }
            label5.Text = e.Node.Text;
            if (!e.Node.Text.Contains("🌐"))
            {
                label5.Text += " (Local)";
            }

            if (onlineFileSync && e.Node.ForeColor != Color.Gray)
            {
                for (int z = 0; z < modList.Length; z++)
                {
                    if (modList[z] != null)
                    {
                        if (e.Node.Text.Contains(modList[z])) {
                            richTextBox1.Text = modValues[z];
                            
                        }
                    }
                }

                logLabel.Text = e.Node.Text;
            }    
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            button3.Enabled = true;

            if (treeView1.Nodes[nodeInt].ForeColor != Color.Gray)
            {
                treeView1.Nodes[nodeInt].ForeColor = Color.Gray;

                if (onlineActive)
                {
                    if (treeView1.Nodes[nodeInt].Text.Contains("🌐"))
                    {
                        treeView1.Nodes[nodeInt].Text = treeView1.Nodes[nodeInt].Text.Substring(0, treeView1.Nodes[nodeInt].Text.Length - 3);
                    }
                }
            }
            else {
                treeView1.Nodes[nodeInt].ForeColor = Color.Black;

                if (onlineActive)
                {
                    if (!treeView1.Nodes[nodeInt].Text.Contains("🌐"))
                    {
                        treeView1.Nodes[nodeInt].Text += " 🌐";
                    }
                }
            }

            treeView1_AfterSelect(null, new TreeViewEventArgs(treeView1.Nodes[nodeInt]));
        }

        private void button3_Click(object sender, EventArgs e)
        {
            foreach (TreeNode tn in treeView1.Nodes)
            {
                if (tn.Text.ToLower().Contains("mod"))
                {
                    if (tn.Text.ToLower().Contains("2010777010") || tn.Text.ToLower().Contains("2606448745")){
                        
                        tn.ForeColor = Color.Gray;

                        if (onlineActive) {
                            if (tn.Text.Contains("🌐"))
                            {
                                tn.Text = tn.Text.Substring(0, tn.Text.Length - 3);
                            }
                        }
                    }
                    else {
                        tn.ForeColor = Color.Black;

                        if (onlineActive)
                        {
                            if (!tn.Text.Contains("🌐"))
                            {
                                tn.Text += " 🌐";
                            }
                        }
                    }
                }
                else {
                    tn.ForeColor = Color.Gray;

                    if (onlineActive)
                    {
                        if (tn.Text.Contains("🌐"))
                        {
                            tn.Text = tn.Text.Substring(0, tn.Text.Length - 3);
                        }
                    }
                }
            }

        }

        private async void button4_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < treeView1.Nodes.Count; i++)
            {
                if (treeView1.Nodes[i].Text.Contains("🌐"))
                {
                    string mod = treeView1.Nodes[i].Text.Substring(0, treeView1.Nodes[i].Text.Length - 2);

                    for (int z = 0; z < modList.Length; z++)
                    {
                        if (modList[z] != null)
                        {
                            if (treeView1.Nodes[i].Text.Contains(modList[z]))
                            {
                                richTextBox1.Text = modValues[z];
                                File.WriteAllText(Path.Combine(LocalLowPath, mod), modValues[z]);

                            }
                        }
                    }

                    if (File.Exists(Path.Combine(LocalLowPath, mod))){

                        logLabel.Text = mod + " has been overwriten";
                    }
                }
            }

            logLabel.Text = "Done!";
            button4.Text = "Done!";

            await Task.Delay(2000);

            button4.Text = "Save Changes";
        }
    }
}
