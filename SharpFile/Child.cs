using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using Common;

namespace SharpFile
{
    public partial class Child : Form
    {
        enum UnitDisplay
        {
            Bytes,
            KiloBytes,
            MegaBytes
        }

        private UnitDisplay unitDisplay = UnitDisplay.Bytes;

        public Child()
        {
            InitializeComponent();
            setup();
        }

        private void setup()
        {
            // Attach to some events.
            this.ClientSizeChanged += new EventHandler(listView_ClientSizeChanged);

            // Set some options ont he listview.
            listView.View = View.Details;

            List<string> columns = new List<string>();
            columns.Add("Filename");
            columns.Add("Size");
            columns.Add("Date");
            columns.Add("Time");

            foreach (string column in columns)
            {
                listView.Columns.Add(column);
            }
        }

        void listView_ClientSizeChanged(object sender, EventArgs e)
        {
            listView.Size = new Size(this.Size.Width-50, this.Size.Height-100);
        }

        public void UpdateDriveListing()
        {
            comboBox.Items.Clear();
            List<DriveInfo> drives = Infrastructure.GetDrives();

            foreach (DriveInfo driveInfo in drives)
            {
                comboBox.Items.Add(driveInfo.Name);
            }

            comboBox.SelectedIndex = 0;
        }

        public void UpdateFileListing(string path)
        {
            List<DataInfo> dataInfos = Infrastructure.GetFiles(path);

            try
            {
                foreach (DataInfo dataInfo in dataInfos)
                {
                    double size;
                    ListViewItem item = new ListViewItem(dataInfo.Name);
                    //item.ImageIndex = Data.GetImageIndex(theFile, imageList, imageHash);

                    switch (unitDisplay)
                    {
                        case UnitDisplay.KiloBytes:
                            size = Math.Round(Convert.ToDouble(dataInfo.Size), 2) / 1024;
                            break;
                        case UnitDisplay.MegaBytes:
                            size = Math.Round(Convert.ToDouble(dataInfo.Size), 2) / 1024 / 1024;
                            break;
                        default:
                            size = dataInfo.Size;
                            break;
                    }

                    item.SubItems.Add(General.GetHumanReadableSize(size));
                    item.SubItems.Add(dataInfo.LastWriteTime.ToShortDateString());
                    item.SubItems.Add(dataInfo.LastWriteTime.ToShortTimeString());

                    listView.BeginUpdate();
                    listView.Items.Add(item);
                    listView.EndUpdate();
                }
            }
            catch (System.UnauthorizedAccessException)
            {
                listView.BeginUpdate();
                listView.Items.Add("Unauthorized Access");
                listView.EndUpdate();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + ex.StackTrace);
            }
        }
    }
}