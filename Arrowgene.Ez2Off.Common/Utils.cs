﻿/*
 * This file is part of Arrowgene.Ez2Off
 *
 * Arrowgene.Ez2Off is a server implementation for the game "Ez2On".
 * Copyright (C) 2017-2018 Sebastian Heinz
 * Copyright (C) 2017-2018 Halgulaea
 * Copyright (C) 2017-2018 David Via
 *
 * Github: https://github.com/Arrowgene/Arrowgene.Ez2Off
 *
 * Arrowgene.Ez2Off is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * Arrowgene.Ez2Off is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with Arrowgene.Ez2Off. If not, see <https://www.gnu.org/licenses/>.
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;

namespace Arrowgene.Ez2Off.Common
{
    public class Utils
    {
        public const string DefaultVersion = "0.0.0.0/unofficial-debug";
        public const byte KeyFirstParameter = 0x24;
        public const byte KeySecondParameter = 0x2A;

        public static readonly Encoding KoreanEncoding = CodePagesEncodingProvider.Instance.GetEncoding("EUC-KR");

        public static byte[] DecryptParameter(byte[] parameter, byte key)
        {
            int lenght = parameter.Length;
            byte[] result = new byte[lenght];
            for (int i = 0; i < lenght; i++)
            {
                result[i] = (byte) (parameter[i] - key);
            }

            return result;
        }

        public static string ParameterToString(byte[] parameter)
        {
            string response = string.Empty;
            foreach (byte b in parameter)
            {
                if (b == 0)
                {
                    break;
                }

                response += (Char) b;
            }

            return response;
        }

        public static string PathDifference(string directoryInfo1, string directoryInfo2, bool unRoot)
        {
            return PathDifference(new DirectoryInfo(directoryInfo1), new DirectoryInfo(directoryInfo2), unRoot);
        }

        public static string PathDifference(FileSystemInfo directoryInfo1, FileSystemInfo directoryInfo2, bool unRoot)
        {
            string result;
            if (directoryInfo1.FullName == directoryInfo2.FullName)
            {
                result = "";
            }
            else if (directoryInfo1.FullName.StartsWith(directoryInfo2.FullName))
            {
                result = directoryInfo1.FullName.Split(new[] {directoryInfo2.FullName},
                    StringSplitOptions.RemoveEmptyEntries)[0];
            }
            else if (directoryInfo2.FullName.StartsWith(directoryInfo1.FullName))
            {
                result = directoryInfo2.FullName.Split(new[] {directoryInfo1.FullName},
                    StringSplitOptions.RemoveEmptyEntries)[0];
            }
            else
            {
                result = "";
            }

            if (unRoot)
            {
                result = UnrootPath(result);
            }

            return result;
        }

        public static string UnrootPath(string path)
        {
            // https://stackoverflow.com/questions/53102/why-does-path-combine-not-properly-concatenate-filenames-that-start-with-path-di
            if (Path.IsPathRooted(path))
            {
                path = path.TrimStart(Path.DirectorySeparatorChar);
                path = path.TrimStart(Path.AltDirectorySeparatorChar);
            }

            return path;
        }

        public static AssemblyName GetAssemblyName(string name)
        {
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var assembly in assemblies)
            {
                AssemblyName assemblyName = assembly.GetName();
                if (assemblyName.Name == name)
                {
                    return assemblyName;
                }
            }

            return null;
        }

        public static Version GetAssemblyVersion(string name)
        {
            AssemblyName assemblyName = GetAssemblyName(name);
            if (assemblyName != null)
            {
                return assemblyName.Version;
            }

            return null;
        }

        public static string GetAssemblyVersionString(string name)
        {
            Version version = GetAssemblyVersion(name);
            if (version != null)
            {
                return version.ToString();
            }

            return null;
        }

        public static byte[] ReadFile(string source)
        {
            if (!File.Exists(source))
            {
                throw new Exception(string.Format("'{0}' does not exist or is not a file", source));
            }

            return File.ReadAllBytes(source);
        }

        public static string ReadFileText(string source)
        {
            if (!File.Exists(source))
            {
                throw new Exception(string.Format("'{0}' does not exist or is not a file", source));
            }

            return File.ReadAllText(source);
        }

        public static void WriteFile(byte[] content, string destination)
        {
            if (content != null)
            {
                File.WriteAllBytes(destination, content);
            }
            else
            {
                throw new Exception(string.Format("Content of '{0}' is null", destination));
            }
        }

        public static List<FileInfo> GetFiles(DirectoryInfo directoryInfo, string[] extensions, bool recursive)
        {
            if (recursive)
            {
                List<FileInfo> filteredFiles = GetFiles(directoryInfo, extensions);
                DirectoryInfo[] directoryInfos = directoryInfo.GetDirectories("*", SearchOption.TopDirectoryOnly);
                foreach (DirectoryInfo dInfo in directoryInfos)
                {
                    List<FileInfo> files = GetFiles(dInfo, extensions, true);
                    filteredFiles.AddRange(files);
                }

                return filteredFiles;
            }

            return GetFiles(directoryInfo, extensions);
        }

        public static List<FileInfo> GetFiles(DirectoryInfo directoryInfo, string[] extensions)
        {
            List<FileInfo> filteredFiles = new List<FileInfo>();
            FileInfo[] files = directoryInfo.GetFiles("*.*", SearchOption.TopDirectoryOnly);
            foreach (FileInfo file in files)
            {
                if (extensions != null)
                {
                    foreach (string extension in extensions)
                    {
                        if (file.Extension.EndsWith(extension, StringComparison.OrdinalIgnoreCase))
                        {
                            filteredFiles.Add(file);
                            break;
                        }
                    }
                }
                else
                {
                    filteredFiles.Add(file);
                }
            }

            return filteredFiles;
        }

        public static DirectoryInfo EnsureDirectory(string directory)
        {
            return Directory.CreateDirectory(directory);
        }

        public static string ApplicationDirectory()
        {
            string path = Assembly.GetEntryAssembly().CodeBase;
            Uri uri = new Uri(path);
            string directory = Path.GetDirectoryName(uri.LocalPath);
            return directory;
        }

        public static string CreateMD5(string input)
        {
            MD5 md5 = MD5.Create();
            byte[] inputBytes = Encoding.ASCII.GetBytes(input);
            byte[] hashBytes = md5.ComputeHash(inputBytes);

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hashBytes.Length; i++)
            {
                sb.Append(hashBytes[i].ToString("X2"));
            }

            return sb.ToString().ToLower();
        }


        public static string RelativeDirectory(string fromDirectory, string toDirectory)
        {
            return RelativeDirectory(fromDirectory, toDirectory, toDirectory, Path.DirectorySeparatorChar);
        }

        public static string RelativeDirectory(string fromDirectory, string toDirectory, string defaultDirectory)
        {
            return RelativeDirectory(fromDirectory, toDirectory, defaultDirectory, Path.DirectorySeparatorChar);
        }

        /// <summary>
        /// Returns a directory that is relative.
        /// </summary>
        /// <param name="fromDirectory">The directory to navigate from.</param>
        /// <param name="toDirectory">The directory to reach.</param>
        /// <param name="defaultDirectory">A directory to return on failure.</param>
        /// <param name="directorySeparator"></param>
        /// <returns>The relative directory or the defaultDirectory on failure.</returns>
        public static string RelativeDirectory(string fromDirectory, string toDirectory, string defaultDirectory,
            char directorySeparator)
        {
            string result;

            if (fromDirectory.EndsWith("\\") || fromDirectory.EndsWith("/"))
            {
                fromDirectory = fromDirectory.Remove(fromDirectory.Length - 1);
            }

            if (toDirectory.EndsWith("\\") || toDirectory.EndsWith("/"))
            {
                toDirectory = toDirectory.Remove(toDirectory.Length - 1);
            }

            if (toDirectory.StartsWith(fromDirectory))
            {
                result = toDirectory.Substring(fromDirectory.Length);
                if (result.StartsWith("\\") || result.StartsWith("/"))
                {
                    result = result.Substring(1, result.Length - 1);
                }

                if (result != "")
                {
                    result += directorySeparator;
                }
            }
            else
            {
                string[] fromDirs = fromDirectory.Split(':', '\\', '/');
                string[] toDirs = toDirectory.Split(':', '\\', '/');
                if (fromDirs.Length <= 0 || toDirs.Length <= 0 || fromDirs[0] != toDirs[0])
                {
                    return defaultDirectory;
                }

                int offset = 1;
                for (; offset < fromDirs.Length; offset++)
                {
                    if (toDirs.Length <= offset)
                    {
                        break;
                    }

                    if (fromDirs[offset] != toDirs[offset])
                    {
                        break;
                    }
                }

                StringBuilder relativeBuilder = new StringBuilder();
                for (int i = 0; i < fromDirs.Length - offset; i++)
                {
                    relativeBuilder.Append("..");
                    relativeBuilder.Append(directorySeparator);
                }

                for (int i = offset; i < toDirs.Length - 1; i++)
                {
                    relativeBuilder.Append(toDirs[i]);
                    relativeBuilder.Append(directorySeparator);
                }

                result = relativeBuilder.ToString();
            }

            result = DirectorySeparator(result, directorySeparator);
            return result;
        }

        public static string DirectorySeparator(string path)
        {
            return DirectorySeparator(path, Path.DirectorySeparatorChar);
        }

        public static string DirectorySeparator(string path, char directorySeparator)
        {
            if (directorySeparator != '\\')
            {
                path = path.Replace('\\', directorySeparator);
            }

            if (directorySeparator != '/')
            {
                path = path.Replace('/', directorySeparator);
            }

            return path;
        }

        public static string RelativeApplicationDirectory()
        {
            return RelativeDirectory(Environment.CurrentDirectory, ApplicationDirectory());
        }

        public static string GenerateSessionKey(int desiredLength)
        {
            StringBuilder sessionKey = new StringBuilder();
            using (RNGCryptoServiceProvider cryptoProvider = new RNGCryptoServiceProvider())
            {
                byte[] random = new byte[1];
                int length = 0;
                while (length < desiredLength)
                {
                    cryptoProvider.GetBytes(random);
                    char c = (char) random[0];
                    if ((Char.IsDigit(c) || Char.IsLetter(c)) && random[0] < 127)
                    {
                        length++;
                        sessionKey.Append(c);
                    }
                }
            }

            return sessionKey.ToString();
        }
    }
}