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

        private void button1_Click(object sender, EventArgs e)
        {
            if(folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                txtOldName.Enabled = true;
                txtNewName.Enabled = true;
                btnRenameFiles.Enabled = true;
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






            txtOldName.Enabled = false;
            txtNewName.Enabled = false;
            btnRenameFiles.Enabled = false;
            txtOldName.Text = "";
            txtNewName.Clear(); 

        }
    }
}
