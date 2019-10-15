/* Copyright (C) 2017-2019 ROM Knowledgeware. All rights reserved.
 * 
 * You can redistribute this program and/or modify it under the terms of
 * the GNU Lesser Public License as published by the Free Software Foundation,
 * either version 3 of the License, or (at your option) any later version.
 * 
 * Maintainer: Tal Aloni <tal@kmrom.com>
 */
using ICSharpCode.SharpZipLib.Zip;
using IPALibrary.CodeSignature;
using IPALibrary.MachO;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.X509;
using System;
using System.Collections.Generic;
using System.IO;
using Utilities;

namespace IPALibrary
{
    public class IPAFile
    {
        public const string InfoFileName = "Info.plist";
        public const string MobileProvisionFileName = "embedded.mobileprovision";
        public const string CodeResourcesFilePath = "_CodeSignature/CodeResources";
        public const char ZipDirectorySeparator = '/';

        private string m_appDirectoryPath;
        private MemoryStream m_stream = new MemoryStream();
        private ZipFile m_zipFile;

        private Dictionary<string, byte[]> m_updateFile;

        public IPAFile(Stream stream)
        {
            ByteUtils.CopyStream(stream, m_stream);
            m_stream.Position = 0;

            m_zipFile = new ZipFile(m_stream);
            m_appDirectoryPath = GetAppDirectoryPath();

            if (string.IsNullOrEmpty(m_appDirectoryPath))
            {
                throw new InvalidDataException("Invalid directory structure for IPA file");
            }
        }

        public void Save(string path)
        {
            using (var outStream = new ZipOutputStream(File.Create(path)))
            {
                foreach (ZipEntry entry in m_zipFile)
                {
                    if (entry.IsFile)
                    {
                        outStream.PutNextEntry(new ZipEntry(entry.Name));
                        if (m_updateFile.TryGetValue(entry.Name, out byte[] data))
                        {
                            outStream.Write(data, 0, data.Length);
                        }
                        else
                        {
                            Stream sourceStream = m_zipFile.GetInputStream(entry);
                            sourceStream.CopyTo(outStream);
                        }
                    }
                }
                m_updateFile.Clear();
            }
        }

        private void Close()
        {
            m_zipFile.Close();
        }

        private string GetAppDirectoryPath()
        {
            foreach (ZipEntry entry in m_zipFile)
            {
                var name = entry.Name;
                if (name.EndsWith(ZipDirectorySeparator + MobileProvisionFileName) && name.StartsWith("Payload" + ZipDirectorySeparator))
                {
                    string subPath = name.Substring(0, name.Length - MobileProvisionFileName.Length - 1).Substring(8);
                    if (subPath.IndexOf(ZipDirectorySeparator) == -1)
                    {
                        return "Payload" + ZipDirectorySeparator + subPath + ZipDirectorySeparator;
                    }
                }
            }
            throw new InvalidDataException("Invalid directory structure for IPA file");
        }

        public MemoryStream GetFileStream(string path)
        {
            ZipEntry fileEntry = m_zipFile.GetEntry(m_appDirectoryPath + path);
            if (fileEntry != null && fileEntry.IsFile)
            {
                // We can't seek using the decompressed stream so we create a copy
                Stream sourceStream = m_zipFile.GetInputStream(fileEntry);
                MemoryStream fileStream = new MemoryStream();
                ByteUtils.CopyStream(sourceStream, fileStream);
                fileStream.Position = 0;
                return fileStream;
            }
            return null;
        }

        public byte[] GetFileBytes(string path)
        {
            ZipEntry fileEntry = m_zipFile.GetEntry(m_appDirectoryPath + path);
            if (fileEntry != null && fileEntry.IsFile)
            {
                Stream sourceStream = m_zipFile.GetInputStream(fileEntry);
                return ByteReader.ReadBytes(sourceStream, (int)fileEntry.Size);
            }
            return null;
        }

        public MobileProvisionFile GetMobileProvision()
        {
            byte[] fileBytes = GetFileBytes(MobileProvisionFileName);
            return new MobileProvisionFile(fileBytes);
        }

        public string GetExecutableName()
        {
            InfoFile infoFile = GetInfoFile();
            if (infoFile != null)
            {
                return infoFile.ExecutableName;
            }
            return null;
        }

        public string GetBundleIdentifier()
        {
            InfoFile infoFile = GetInfoFile();
            if (infoFile != null)
            {
                return infoFile.BundleIdentifier;
            }
            return null;
        }

        public byte[] GetInfoFileBytes()
        {
            return GetFileBytes(InfoFileName);
        }

        public MemoryStream GetInfoFileStream()
        {
            return GetFileStream(InfoFileName);
        }

        public InfoFile GetInfoFile()
        {
            byte[] infoBytes = GetInfoFileBytes();
            return new InfoFile(infoBytes);
        }

        public byte[] GetExecutableBytes()
        {
            string executableName = GetExecutableName();
            return GetFileBytes(executableName);
        }

        public byte[] GetCodeResourcesBytes()
        {
            return GetFileBytes(CodeResourcesFilePath);
        }

        public CodeResourcesFile GetCodeResourcesFile()
        {
            byte[] codeResourcesBytes = GetCodeResourcesBytes();
            return new CodeResourcesFile(codeResourcesBytes);
        }

        public bool ValidateExecutableSignature(X509Certificate certificate)
        {
            byte[] buffer = GetExecutableBytes();
            byte[] infoFileBytes = GetInfoFileBytes();
            byte[] codeResourcesBytes = GetFileBytes(CodeResourcesFilePath);
            List<MachObjectFile> files = MachObjectHelper.ReadMachObjects(buffer);
            foreach(MachObjectFile file in files)
            {
                if (!CodeSignatureHelper.ValidateExecutableHash(file))
                {
                    return false;
                }

                if (!CodeSignatureHelper.ValidateSpecialHashes(file, infoFileBytes, codeResourcesBytes))
                {
                    return false;
                }

                if (!CodeSignatureHelper.ValidateExecutableSignature(file, certificate))
                {
                    return false;
                }
            }

            return true;
        }

        public void ReplaceMobileProvision(byte[] mobileProvisionBytes)
        {
            this.m_updateFile[m_appDirectoryPath + MobileProvisionFileName] = mobileProvisionBytes;
            
            CodeResourcesFile codeResources = GetCodeResourcesFile();
            codeResources.UpdateFileHash(MobileProvisionFileName, mobileProvisionBytes);

            MobileProvisionFile mobileProvision = new MobileProvisionFile(mobileProvisionBytes);
            string bundleId = mobileProvision.PList.Entitlements.BundleIdentifier;
            if (bundleId != GetBundleIdentifier())
            {
                // We must update the info.plist's CFBundleIdentifier to match the one from the mobile provision
                InfoFile infoFile = GetInfoFile();
                infoFile.BundleIdentifier = bundleId;
                byte[] infoBytes = infoFile.GetBytes();
                this.m_updateFile[m_appDirectoryPath + InfoFileName] = infoBytes;
                codeResources.UpdateFileHash(InfoFileName, infoBytes);
            }

            byte[] codeResourcesBytes = codeResources.GetBytes();
            this.m_updateFile[m_appDirectoryPath + CodeResourcesFilePath] = codeResourcesBytes;
        }


        public void ResignIPA(List<X509Certificate> certificateChain, AsymmetricKeyEntry privateKey, MobileProvisionFile mobileProvision)
        {
            //MobileProvisionFile mobileProvision = GetMobileProvision();
            string bundleId = mobileProvision.PList.Entitlements.BundleIdentifier;
            byte[] infoFileBytes;
            if (m_updateFile.TryGetValue(m_appDirectoryPath + InfoFileName, out infoFileBytes) == false)
            {
                infoFileBytes = GetInfoFileBytes();
            }
            byte[] codeResourcesBytes;
            if (m_updateFile.TryGetValue(m_appDirectoryPath + CodeResourcesFilePath, out codeResourcesBytes) == false)
            {
                codeResourcesBytes = GetCodeResourcesBytes();
            }
            byte[] buffer;
            string executableName = GetExecutableName();
            if (m_updateFile.TryGetValue(m_appDirectoryPath + executableName, out buffer) == false)
            {
                buffer = GetFileBytes(executableName);
            }
            List<MachObjectFile> files = MachObjectHelper.ReadMachObjects(buffer);
            foreach (MachObjectFile file in files)
            {
                CodeSignatureHelper.ResignExecutable(file, bundleId, certificateChain, privateKey, infoFileBytes, codeResourcesBytes, mobileProvision.PList.Entitlements);
            }
            byte[] executableBytes = MachObjectHelper.PackMachObjects(files);
            this.m_updateFile[m_appDirectoryPath + executableName] = executableBytes;
        }

        public bool ContainsFolder(string folderPath)
        {
            foreach (ZipEntry entry in m_zipFile)
            {
                if (entry.IsDirectory)
                {
                    if (entry.Name.Equals(m_appDirectoryPath + folderPath + ZipDirectorySeparator, StringComparison.OrdinalIgnoreCase))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public bool HasFrameworksFolder
        {
            get
            {
                return ContainsFolder("Frameworks");
            }
        }

        public string ResignIPA(byte[] mobileProvisionBytes, byte[] signingCertificateBytes, string certificatePassword, string outputIPAPath)
        {
            this.m_updateFile = new Dictionary<string, byte[]>();

            // Validate that the mobileprovision match the given certificate
            MobileProvisionFile mobileProvision;
            if (mobileProvisionBytes == null)
            {
                mobileProvision = this.GetMobileProvision();
            }
            else
            {
                mobileProvision = new MobileProvisionFile(mobileProvisionBytes);
            }

            List<byte[]> developerCertificates = mobileProvision.PList.DeveloperCertificates;
            if (developerCertificates.Count == 0)
            {
                return "Mobile Provision does not contain developer certificate information";
            }

            AsymmetricKeyEntry privateKey;
            X509Certificate signingCertificate = CertificateHelper.GetCertificateAndKeyFromBytes(signingCertificateBytes, certificatePassword, out privateKey);
            if (signingCertificate == null)
            {
                return "Failed to parse the given signing certificate";
            }

            bool foundMatchingCertificate = false;
            for (int index = 0; index < developerCertificates.Count; index++)
            {
                X509Certificate provisionedCertificate = CertificateHelper.GetCertificatesFromBytes(developerCertificates[index]);
                if (provisionedCertificate.Equals(signingCertificate))
                {
                    foundMatchingCertificate = true;
                }
            }

            if (!foundMatchingCertificate)
            {
                return "The signing certificate given does not match any specified in the Mobile Provision file";
            }

            List<X509Certificate> certificateStore;
            try
            {
                certificateStore = ReadCertificatesDirectory();
            }
            catch
            {
                return "Failed to read certificate directory";
            }

            List<X509Certificate> certificateChain = CertificateHelper.BuildCertificateChain(signingCertificate, certificateStore);

            if (mobileProvisionBytes != null)
            {
                this.ReplaceMobileProvision(mobileProvisionBytes);
            }

            if (this.HasFrameworksFolder)
            {
                return "Signing an IPA containing a framework is not supported";
            }

            this.ResignIPA(certificateChain, privateKey, mobileProvision);
            this.Save(outputIPAPath);
            return string.Empty;
        }

        public string ValidateIPA(byte[] signingCertificateBytes, string certificatePassword)
        {
            AsymmetricKeyEntry privateKey;
            X509Certificate signingCertificate = CertificateHelper.GetCertificateAndKeyFromBytes(signingCertificateBytes, certificatePassword, out privateKey);
            if (signingCertificate == null)
            {
                return "Failed to parse the given signing certificate";
            }

            bool isValid;
            try
            {
                isValid = this.ValidateExecutableSignature(signingCertificate);
            }
            catch (Org.BouncyCastle.Security.Certificates.CertificateExpiredException)
            {
                return "Certificate is outdated";
            }
            catch (Org.BouncyCastle.Security.Certificates.CertificateNotYetValidException)
            {
                return "Certificate is outdated";
            }

            if (isValid)
            {
                return "success";
            }
            else
            {
                return "Signature is invalid";
            }
        }

        private static string GetCertificatesPath()
        {
            string assemblyPath = System.Reflection.Assembly.GetExecutingAssembly().Location;
            string assemblyDirectory = Path.GetDirectoryName(assemblyPath);
            if (!assemblyDirectory.EndsWith(@"\"))
            {
                assemblyDirectory += @"\";
            }
            return assemblyDirectory + @"Certificates\";
        }

        private static List<X509Certificate> ReadCertificatesDirectory()
        {
            List<X509Certificate> certificates = new List<X509Certificate>();
            string certificatesPath = GetCertificatesPath();
            string[] files = Directory.GetFiles(certificatesPath, "*.cer");
            foreach (string filePath in files)
            {
                byte[] fileBytes = File.ReadAllBytes(filePath);
                X509Certificate certificate = CertificateHelper.GetCertificatesFromBytes(fileBytes);
                certificates.Add(certificate);
            }
            return certificates;
        }
    }
}
