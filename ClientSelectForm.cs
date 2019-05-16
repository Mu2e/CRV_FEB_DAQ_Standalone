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

namespace TB_mu2e
{
    public partial class ClientSelectForm : Form
    {
        private string clientMemoryPath;
        private string clientDefaultPath;
        public ClientSelectForm()
        {
            clientMemoryPath = Path.Combine(Environment.CurrentDirectory, "ClientMemoryList.txt"); //the client memory list (a simple text file) will live in the same directory as the program
            clientDefaultPath = Path.Combine(Environment.CurrentDirectory, "ClientDefaultList.txt"); //the list of clients that should be loaded by default
            InitializeComponent();

            try
            {
                using (StreamReader sr = new StreamReader(clientDefaultPath, true))
                {
                    while (!sr.EndOfStream)
                        clientListBox.Items.Add(sr.ReadLine());
                }
            }
            catch
            {
                System.Console.WriteLine("Trouble reading default client list!");
            }


            try
            {
                using (StreamReader sr = new StreamReader(clientMemoryPath, true))
                {
                    while(!sr.EndOfStream)
                        clientMemoryBox.Items.Add(sr.ReadLine());
                }
            }
            catch
            {
                System.Console.WriteLine("Trouble reading client list! Attempting to make a new one.");
                try
                {
                    using (StreamWriter sw = new StreamWriter(clientMemoryPath, false))
                    {
                        System.Console.WriteLine("\t Success! New file @ " + clientMemoryPath);
                    }
                }
                catch { System.Console.WriteLine("Unable to make @ " + clientMemoryPath); }
            }
        }

        private void ClientMemoryAddBtn_Click(object sender, EventArgs e)
        {
            try
            {
                clientMemoryBox.Items.Add(clientIPAddBox.Text);

                using (StreamWriter sw = new StreamWriter(clientMemoryPath, true))
                {
                    sw.WriteLine(clientIPAddBox.Text);
                }
            }
            catch { System.Console.WriteLine("Trouble saving new IP to client list!"); }
        }

        private void ClientAddBtn_Click(object sender, EventArgs e)
        {
            foreach (var clientAddress in clientMemoryBox.SelectedItems)
                if(!clientListBox.Items.Contains(clientAddress))
                    clientListBox.Items.Add(clientAddress);
        }

        private void ClientMemoryRemoveBtn_Click(object sender, EventArgs e)
        {
            while (clientMemoryBox.SelectedItems.Count > 0) //while there are selected items to remove
                clientMemoryBox.Items.Remove(clientMemoryBox.SelectedItems[0]); //remove them in order

            try
            {
                using (StreamWriter sw = new StreamWriter(clientMemoryPath, false))
                {
                    foreach (var clientAddress in clientMemoryBox.Items)
                        sw.WriteLine(clientAddress);
                }
            }
            catch { System.Console.WriteLine("Trouble writing to IP client list!"); }
        }

        private void ClientRemoveBtn_Click(object sender, EventArgs e)
        {
            while (clientListBox.SelectedItems.Count > 0) //while there are selected items to remove
                clientListBox.Items.Remove(clientListBox.SelectedItems[0]); //remove them in order
        }


        private void ClientIPAddBox_Enter(object sender, EventArgs e)
        {
            this.AcceptButton = clientMemoryAddBtn;
        }

        private void ClientIPAddBox_Leave(object sender, EventArgs e)
        {
            this.AcceptButton = null;
        }

        public string[] GetClientAddresses()
        {
            int numAddresses = clientListBox.Items.Count;
            string[] addresses = new string[numAddresses];
            for (int iaddr = 0; iaddr < numAddresses; iaddr++)
                addresses[iaddr] = (string) clientListBox.Items[iaddr];

            return addresses;
        }

        public void PopulateCurrentAddresses(string[] addresses)
        {
            if (clientListBox.Items.Count > 0)
                clientListBox.Items.Clear();
            foreach (string address in addresses)
                clientListBox.Items.Add(address);
        }

        private void ClientDefaultSave_Click(object sender, EventArgs e)
        {
            try
            {
                using (StreamWriter sw = new StreamWriter(clientDefaultPath, false))
                {
                    foreach (var clientAddress in clientListBox.Items)
                        sw.WriteLine(clientAddress);
                }
            }
            catch { System.Console.WriteLine("Trouble writing to default client list!"); }

        }

        private void ClientListMoveUpBtn_Click(object sender, EventArgs e)
        {
            int selectedIndex = clientListBox.SelectedIndex;
            if (selectedIndex > 0)
            {
                var addr = clientListBox.Items[selectedIndex];
                clientListBox.Items.RemoveAt(selectedIndex--);
                clientListBox.Items.Insert(selectedIndex, addr);
                clientListBox.SelectedIndex = selectedIndex;
            }
        }

        private void ClientListMoveDownBtn_Click(object sender, EventArgs e)
        {
            int selectedIndex = clientListBox.SelectedIndex;
            if (selectedIndex < clientListBox.Items.Count-1)
            {
                var addr = clientListBox.Items[selectedIndex];
                clientListBox.Items.RemoveAt(selectedIndex++);
                clientListBox.Items.Insert(selectedIndex, addr);
                clientListBox.SelectedIndex = selectedIndex;
            }

        }
    }
}
