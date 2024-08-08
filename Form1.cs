using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace Rename_Project
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        enum enFilesImages { Folder=0,NotePad=1,Vlc=2,None=3,WinRAR=4,Pdf=5};



        void AddImageToFileTreeNode(TreeNode tn , string FilePath)
        {
            
            string FileExtension = Path.GetExtension(FilePath);
            switch(FileExtension)
            {
                case ".txt":
                    tn.ImageIndex=(int)enFilesImages.NotePad; 
                    break;
                case ".mp4":
                case ".mp3":
                    tn.ImageIndex=(int)enFilesImages.Vlc; 
                    break;
                case ".rar":
                case ".zip":
                    tn.ImageIndex = (int)enFilesImages.WinRAR;
                    break;
                case ".pdf":
                    tn.ImageIndex =(int)enFilesImages.Pdf;
                    break;

                    
            }
            tn.SelectedImageIndex = tn.ImageIndex;
        }

        void AddImageToFolderTreeNode(TreeNode tn)
        {
            tn.ImageIndex= (int)enFilesImages.Folder;
            tn.SelectedImageIndex=tn.ImageIndex;
        }

        void AddFileToTreeView(string path)
        {
            string FileName = Path.GetFileNameWithoutExtension(path);
            TreeNode tn = new TreeNode(FileName);
            AddImageToFileTreeNode(tn, path);
            treeView1.Nodes.Add(tn);
        }

        string GetFolderName(string FolderPath)
        {
            string[] arr= FolderPath.Split('\\');
            return arr[arr.Length-1];
        }

        void AddFolderToTreeView(string path)
        {
            string FolderName = GetFolderName(path);
            TreeNode tn = new TreeNode(FolderName);
            AddImageToFolderTreeNode(tn);
            treeView1.Nodes.Add(tn);
        }

        void ShowFilesInTreeView(string FolderDir)
        {
            string[] FilesDir = Directory.GetFiles(FolderDir);
            foreach (string FileDir in FilesDir)
            {
                AddFileToTreeView(FileDir);
            }
        }

        void ShowSubFoldersInTreeView(string MainFolderDir)
        {
            string[] FoldersDir = Directory.GetDirectories(MainFolderDir);
            foreach(string FolderDir in FoldersDir)
            {
                AddFolderToTreeView(FolderDir);
            }
        }

        void ShowFilesAndFoldersInTreeView(string FolderDir)
        {
            treeView1.Nodes.Clear();
            ShowFilesInTreeView(FolderDir);
            ShowSubFoldersInTreeView(FolderDir);
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if(folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                txtOldName.Enabled = true;
                txtNewName.Enabled = true;
                btnRenameFiles.Enabled = true;
                ShowFilesAndFoldersInTreeView(folderBrowserDialog1.SelectedPath);
                treeView1.Visible = true;
            }
        }

        

        void RenameFilesInSpecificFolder(string FolderDir , string ReplacedName , string NameToReplace = "")
        {
            int Counter = 1;
            string[] FilesNames = Directory.GetFiles(FolderDir);
            foreach(string FileName in FilesNames)
            {
                string OldName = Path.GetFileNameWithoutExtension(FileName);
                string NewName = (NameToReplace=="")? ReplacedName+" "+Counter: OldName.Replace(NameToReplace,ReplacedName);
                string NewDir = Path.Combine(FolderDir,NewName+Path.GetExtension(FileName));
                File.Move(FileName, NewDir);
                Counter++;
            }
        }

        bool CheckIfFolderHaveSubFolders(string FolderDir)
        {
            string[] FolderDirs = Directory.GetDirectories(FolderDir);
            return (FolderDirs.Length > 0);
        }

        void RenameFilesInSpecificFolderAndSubFolders(string FolderDir, string ReplacedName, string NameToReplace = "")
        {
            string[] FoldersDir = Directory.GetDirectories(FolderDir);
            string[]FilesDir = Directory.GetFiles(FolderDir);
            RenameFilesInSpecificFolder(FolderDir, ReplacedName, NameToReplace);
            foreach (string FolderName in FoldersDir)
            {
                RenameFilesInSpecificFolderAndSubFolders(FolderName , ReplacedName, NameToReplace);
            }
            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if(string.IsNullOrWhiteSpace(txtNewName.Text))
            {
                errorProvider1.SetError(txtNewName, "Must enter new name");
                return;
            }
            else
            {
                errorProvider1.SetError(txtNewName, "");
            }

            if(CheckIfFolderHaveSubFolders(folderBrowserDialog1.SelectedPath))
            {

                if(MessageBox.Show("Are You Want To Change Files Name In SubFolders Inside This Folder",""
                    ,MessageBoxButtons.YesNo,MessageBoxIcon.Question)==DialogResult.Yes)
                {
                    RenameFilesInSpecificFolderAndSubFolders(folderBrowserDialog1.SelectedPath, txtNewName.Text, txtOldName.Text);
                }
            }
            else
            {
                RenameFilesInSpecificFolder(folderBrowserDialog1.SelectedPath, txtNewName.Text, txtOldName.Text);

            }
            MessageBox.Show("Names replaced Successfully");
            ShowFilesAndFoldersInTreeView(folderBrowserDialog1.SelectedPath);
            txtOldName.Text = "";
            txtNewName.Clear(); 


        }

        private void treeView1_DoubleClick(object sender, EventArgs e)
        {
            txtOldName.Text = treeView1.SelectedNode.Text;
        }

        private void copyTheTextToNewNameTextBoxToolStripMenuItem_Click(object sender, EventArgs e)
        {
            txtNewName.Text=txtOldName.Text;
        }

        private void getFolderNameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            txtNewName.Text = GetFolderName(folderBrowserDialog1.SelectedPath);
        }
    }
}
