/* Copyright (C) 2017-2019 ROM Knowledgeware. All rights reserved.
 * 
 * You can redistribute this program and/or modify it under the terms of
 * the GNU Lesser Public License as published by the Free Software Foundation,
 * either version 3 of the License, or (at your option) any later version.
 * 
 * Maintainer: Tal Aloni <tal@kmrom.com>
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using IPALibrary;

namespace IPASign
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void btnSelectInputIPA_Click(object sender, EventArgs e)
        {
            DialogResult result = openIPADialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                txtInputIPA.Text = openIPADialog.FileName;
            }
        }

        private void btnSelectMobileProvision_Click(object sender, EventArgs e)
        {
            DialogResult result = openMobileProvisionDialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                txtMobileProvision.Text = openMobileProvisionDialog.FileName;
            }
        }

        private void btnSelectSigningCertificate_Click(object sender, EventArgs e)
        {
            DialogResult result = openCertificateDialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                txtSigningCertificate.Text = openCertificateDialog.FileName;
            }
        }

        private void btnTestIPA_Click(object sender, EventArgs e)
        {
            string inputIPAPath = txtInputIPA.Text;
            string signingCertificatePath = txtSigningCertificate.Text;
            string certificatePassword = txtCertificatePassword.Text;
            FileStream ipaStream;
            byte[] signingCertificateBytes;

            try
            {
                ipaStream = new FileStream(inputIPAPath, FileMode.Open, FileAccess.Read);
            }
            catch (IOException)
            {
                MessageBox.Show("Failed to read Input IPA file", "Error");
                return;
            }

            try
            {
                signingCertificateBytes = File.ReadAllBytes(signingCertificatePath);
            }
            catch (ArgumentException)
            {
                MessageBox.Show("Failed to read signing certificate file", "Error");
                return;
            }
            catch (IOException)
            {
                MessageBox.Show("Failed to read signing certificate file", "Error");
                return;
            }

            IPAFile ipaFile = new IPAFile(ipaStream);
            var msg = ipaFile.ValidateIPA(signingCertificateBytes, certificatePassword);
            if (msg == "Success")
            {
                MessageBox.Show("Signature is valid", "Success");
            }
            else
            {
                MessageBox.Show(msg, "Error");
            }
        }

        private void btnSaveIPA_Click(object sender, EventArgs e)
        {
            string inputIPAPath = txtInputIPA.Text;
            string mobileProvisionPath = txtMobileProvision.Text;
            string signingCertificatePath = txtSigningCertificate.Text;
            string certificatePassword = txtCertificatePassword.Text;

            FileStream ipaStream;
            byte[] mobileProvisionBytes = null;
            byte[] signingCertificateBytes;
            try
            {
                ipaStream = new FileStream(inputIPAPath, FileMode.Open, FileAccess.Read);
            }
            catch (IOException)
            {
                MessageBox.Show("Failed to read Input IPA file", "Error");
                return;
            }

            if (mobileProvisionPath != String.Empty)
            {
                try
                {
                    mobileProvisionBytes = File.ReadAllBytes(mobileProvisionPath);
                }
                catch (IOException)
                {
                    MessageBox.Show("Failed to read mobile provision file", "Error");
                    return;
                }
            }

            try
            {
                signingCertificateBytes = File.ReadAllBytes(signingCertificatePath);
            }
            catch (ArgumentException)
            {
                MessageBox.Show("Failed to read signing certificate file", "Error");
                return;
            }
            catch (IOException)
            {
                MessageBox.Show("Failed to read signing certificate file", "Error");
                return;
            }

            DialogResult result = saveIPADialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                string outputIPAPath = saveIPADialog.FileName;
                IPAFile ipaFile = new IPAFile(ipaStream);
                try
                {
                    var msg = ipaFile.ResignIPA(mobileProvisionBytes, signingCertificateBytes, certificatePassword, outputIPAPath);
                    if (!string.IsNullOrEmpty(msg))
                    {
                        MessageBox.Show(msg, "Error");
                    }
                    else
                    {

                        MessageBox.Show("Done!");
                    }
                }
                catch (IOException ex)
                {
                    MessageBox.Show("Failed to save output IPA: " + ex.Message, "Error");
                    return;
                }

            }
        }
      
    }
}