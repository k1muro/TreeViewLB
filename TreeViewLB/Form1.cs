using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Management;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TreeViewLB
{
    public partial class Form1 : Form
    {
        Dictionary<int, int> Elements = new Dictionary<int, int>();

        [DllImport("user32.dll")]
        public static extern IntPtr SendMessage(IntPtr hwnd, uint Msg, int wParam, [MarshalAs(UnmanagedType.LPStr)] string lParam);
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            LoadAvailible();
        }
        private void LoadAvailible()
        {
            foreach (Process proc in Process.GetProcesses())
            {
                Elements.Add(proc.Id, GetParentprocessId(proc.Id));
            }

            foreach (KeyValuePair<int, int> item in Elements)
            {
                if (item.Key == item.Value)
                {
                    treeView1.Nodes.Add(item.Key.ToString());
                }
                else
                {
                    bool flag = false;
                    foreach (KeyValuePair<int, int> item2 in Elements)
                    {
                        if (item.Value == item2.Value)
                        {
                            flag = true;
                        }

                    }
                    foreach (TreeNode node in treeView1.Nodes)
                    {
                        if (node.Text == item.Value.ToString())
                        {
                            flag = true;
                        }
                    }
                    if (!flag)
                    {
                        treeView1.Nodes.Add(item.Value.ToString());
                    }

                }
            }
            NewMethod();
        }
        private void NewMethod()
        {
            FindChildren(treeView1.Nodes);
        }
        private int GetParentprocessId(int Id)
        {
            int parentId = 0;

            try
            {
                using (ManagementObject obj = new ManagementObject($"Win32_Process.Handle={Id}"))
                {
                    obj.Get();
                    parentId = Int32.Parse(obj["ParentProcessId"].ToString());
                }
            }
            catch (Exception e)
            {

                MessageBox.Show(e.Message);
            }
            return parentId;
        }
        private void FindChildren(TreeNodeCollection collection)
        {
            foreach (KeyValuePair<int, int> item2 in Elements)
            {
                foreach (TreeNode node in collection)
                {
                    if (node.Text == item2.Value.ToString() && node.Text != item2.Key.ToString())
                    {
                        try
                        {
                            node.Nodes.Add(item2.Key.ToString());
                        }
                        catch (Exception ex)
                        {

                            MessageBox.Show(ex.Message);
                        }
                    }
                }
            }
            foreach (TreeNode item in collection)
            {
                if (item.Nodes.Count != 0)
                {
                    FindChildren(item.Nodes);
                }
            }

        }
        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            try
            {
                Process process = Process.GetProcessById(Int32.Parse(treeView1.SelectedNode.Text));
                label1.Text = process.Id.ToString();
                label2.Text = process.ProcessName.ToString();
                label3.Text = process.BasePriority.ToString();
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }
    }
}
