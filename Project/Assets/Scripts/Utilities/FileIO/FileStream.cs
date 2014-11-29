using System; //For the Serializable Attribute
using System.Collections.Generic; //For Lists
using System.IO; //For Saving / Loading
using System.Runtime.Serialization.Formatters.Binary; //For Serialization/Deserialization
using Gem;

#region CHANGE LOG
/* October,25,2014 - Nathan Hanlan, Added and implemented the class FileStream.
 * 
 */
#endregion

namespace EndevGame
{
    /// <summary>
    /// This class contains a bunch of data known as Files which contain FileContent.
    /// This class enables easy saving / loading using the Save and Load methods.
    /// Users can set the filename and extension of the file directly through properties.
    /// Users can get data use the Get methods as well as add and remove data using the Add and Remove methods.
    /// </summary>
    [Serializable]
    public class FileStream
    {
        #region ERROR_CONSTANTS
        public const int ERROR_MISSING_FILENAME = 32; 
        public const int ERROR_MISSING_EXTENSION = 16;
        public const int ERROR_LOAD_FILE_FAILED = 8;
        public const int ERROR_SAVE_FILE_FAILED = 4;
        public const int ERROR_LOAD_FAILED = 2;
        public const int ERROR_SAVE_FAILED = 1;
        public const int ERROR_NONE = 0;
        /// <summary>
        /// Represents a default error or false statement.
        /// </summary>
        public const int ERROR = -1;
        #endregion

        public FileStream()
        {

        }
        public FileStream(string aFilename, string aFileExtension)
        {
            m_Filename = aFilename;
            m_Extension = aFileExtension;
        }

        /// <summary>
        /// The name of the file saved on the hard disc
        /// </summary>
        private string m_Filename = string.Empty;
        /// <summary>
        /// The extension of the file saved on the hard disc
        /// </summary>
        private string m_Extension = string.Empty;
        /// <summary>
        /// The amount of files currently held within the list
        /// </summary>
        private int m_FileCount = 0;
        /// <summary>
        /// The list of files
        /// </summary>
        private List<File> m_Files = new List<File>();
        /// <summary>
        /// A masked bit field containing any errors present in the filestream.
        /// </summary>
        private int m_ErrorFlag = 0;

        /// <summary>
        /// Determines the files existence within the list by name.
        /// </summary>
        /// <param name="aName"></param>
        /// <returns>Returns true if the file exists.</returns>
        private bool Exists(string aName)
        {
            bool result = false;
            bool shouldClean = false;
            List<File>.Enumerator iter = m_Files.GetEnumerator();
            while (iter.MoveNext())
            {
                if (iter.Current == null)
                {
                    shouldClean = true;
                    continue;
                }
                if (iter.Current.filename == aName)
                {
                    result = true;
                    break;
                }
            }
            if(shouldClean == true)
            {
                CleanUp();
            }
            return result;
        }
        /// <summary>
        /// Adds a file to the filestream. Does not add duplicate files. (Files with same names).
        /// </summary>
        /// <param name="aFile">The file to add.</param>
        /// <returns>True - where the file was added successfully, False - where the file was not added successfully</returns>
        public bool Add(File aFile)
        {
            if(aFile == null)
            {
                return false;
            }
            if(Exists(aFile.filename))
            {
                return false;
            }
            m_Files.Add(aFile);
            m_FileCount++;
            return true;
        }
        /// <summary>
        /// Creates a new file and adds it to the filestream. A file with the same name cannot be added.
        /// </summary>
        /// <param name="aName">The name of the file to create</param>
        /// <returns>A reference to the file created. Null if the file was not added successfully.</returns>
        public File Add(string aName)
        {
            if(Exists(aName))
            {
                return null;
            }
            File file = new File(aName);
            m_Files.Add(file);
            m_FileCount++;
            return file;
        }
        /// <summary>
        /// Removes a file from the list by reference
        /// </summary>
        /// <param name="aFile">The file to remove</param>
        /// <returns>Returns true if the remove was successful. </returns>
        public bool Remove(File aFile)
        {
            bool result = m_Files.Remove(aFile);
            if(result == true)
            {
                m_FileCount--;
            }
            return result;
        }

        /// <summary>
        /// Removes a file from the list by name
        /// </summary>
        /// <param name="aFilename">The name of the file to remove</param>
        /// <returns>Returns the file if the removal was successful, returns null otherwise.</returns>
        public File Remove(string aFilename)
        {
            bool shouldClean = false;
            File file = null;
            List<File>.Enumerator iter = m_Files.GetEnumerator();
            while (iter.MoveNext())
            {
                if (iter.Current == null)
                {
                    shouldClean = true;
                    continue;
                }
                if (iter.Current.filename == aFilename)
                {
                    
                    file = iter.Current;
                    Remove(file);
                    break;
                }
            }
            if(shouldClean == true)
            {
                CleanUp();
            }
            return file;
        }
        /// <summary>
        /// Retrieves a file from the stream by name.
        /// </summary>
        /// <param name="aFilename">The name of the file to get.</param>
        /// <returns>Returns the file found, or null if not found.</returns>
        public File Get(string aFilename)
        {
            bool shouldClean = false;
            File file = null;
            List<File>.Enumerator iter = m_Files.GetEnumerator();
            while(iter.MoveNext())
            {
                if(iter.Current == null)
                {
                    shouldClean = true;
                    continue;
                }
                if(iter.Current.filename == aFilename)
                {
                    file = iter.Current;
                    break;
                }
            }
            if(shouldClean == true)
            {
                CleanUp();
            }
            return file;
        }
        /// <summary>
        /// Attempts to rename a file.
        /// </summary>
        /// <param name="aFilename">The current file name.</param>
        /// <param name="aNewName">The new file name</param>
        /// <returns>Returns true if successful, returns false if it doesnt exists or the new name already exists.</returns>
        public bool RenameFile(string aFilename, string aNewName)
        {
            File file = Get(aFilename);

            if(file != null && !Exists(aNewName))
            {
                file.filename = aNewName;
                return true;
            }

            return false;
        }
        /// <summary>
        /// Clears the file list.
        /// </summary>
        public void Clear()
        {
            if(m_Files !=  null)
            {
                m_FileCount = 0;
                m_Files.Clear();
            }
        }
        /// <summary>
        /// Saves all the files and their content to the specified filename and extension.
        /// </summary>
        public void Save()
        {
            string filePath = GetPath(m_Filename,m_Extension);
            string directoryPath = GetDirectoryPath();

            //If missing file path - Create Directory
            if(!Directory.Exists(directoryPath))
            {
                try
                {
                    Directory.CreateDirectory(directoryPath);
                }
                catch(IOException aException)
                {
                    DebugUtils.Log(aException.Message);
                    m_ErrorFlag &= ERROR_SAVE_FAILED;
                    return;
                }
            }
            //Setup
            BinaryFormatter formatter = new BinaryFormatter();
            System.IO.FileStream fileStream = System.IO.File.Open(filePath, FileMode.Create, FileAccess.Write);
            FileStreamHeader header = new FileStreamHeader();
            header.filename = filename;
            header.extension = extension;
            header.fileCount = fileCount;


            formatter.Serialize(fileStream, header);
            //Write from each file
            bool shouldClean = false;
            List<File>.Enumerator iter = m_Files.GetEnumerator();
            while(iter.MoveNext())
            {
                if(iter.Current == null)
                {
                    shouldClean = true;
                    continue;
                }
                iter.Current.Save(fileStream, formatter);
            }
            //Close the Stream
            fileStream.Close();
            
            if(shouldClean == true)
            {
                CleanUp();
            }
        }
        /// <summary>
        /// Loads all the files but clears the list first if clear is true.
        /// </summary>
        /// <param name="aClear">Whether or not to clear the list</param>
        public void Load(bool aClear)
        {
            if(aClear == true)
            {
                Clear();
            }
            Load();
        }
        /// <summary>
        /// Loads alll the files from disc if the file exists.
        /// </summary>
        public void Load()
        {
            string filePath = GetPath(m_Filename, m_Extension);
            //string directoryPath = GetDirectoryPath();


            if(!System.IO.File.Exists(filePath))
            {
                return;
            }

            //Setup
            BinaryFormatter formatter = new BinaryFormatter();
            System.IO.FileStream fileStream = System.IO.File.Open(filePath, FileMode.Open, FileAccess.Read);
            FileStreamHeader header = (FileStreamHeader)formatter.Deserialize(fileStream);
            m_Filename = header.filename;
            m_Extension = header.extension;
            m_FileCount = header.fileCount;

            for(int i = 0; i < m_FileCount; i++)
            {
                File file = (File)formatter.Deserialize(fileStream);
                file.Load(fileStream, formatter);
                m_Files.Add(file);
            }
            //Close Stream
            fileStream.Close();
        }
        /// <summary>
        /// Checks the list for any null references and removes them.
        /// </summary>
        public void CleanUp()
        {
            for(int i = m_Files.Count - 1; i >= 0; i--)
            {
                if(m_Files[i] == null)
                {
                    m_Files.RemoveAt(i);
                }
            }
        }

        /// <summary>
        /// Returns the path to the directory of where to save.
        /// </summary>
        /// <returns></returns>
        private string GetDirectoryPath()
        {
#if UNITY_IPHONE
                return Application.dataPath + "/Raw/";
#elif UNITY_ANDROID
                return "jar:file://" + Application.dataPath + "!/assets/;
#else
                return UnityEngine.Application.dataPath + "/StreamingAssets/";
#endif
            //return Directory.GetCurrentDirectory() + "\\StreamingAssets\\";
        }
        /// <summary>
        /// Returns the full path of where the file should be.
        /// </summary>
        /// <param name="aFilePath">The filename to append to the directory</param>
        /// <param name="aExtension">The extension to append to the filename</param>
        /// <returns></returns>
        private string GetPath(string aFilePath, string aExtension)
        {
            return GetDirectoryPath() + aFilePath + aExtension;
        }

        /// <summary>
        /// An accessor to the filename to save to/ load from
        /// </summary>
        public string filename
        {
            get { return m_Filename; }
            set { m_Filename = value; }
        }
        /// <summary>
        /// An accessor to the extension to append to the filename
        /// </summary>
        public string extension
        {
            get { return m_Extension; }
            set { m_Extension = value; }
        }
        /// <summary>
        /// How many files are currently in the list.
        /// </summary>
        public int fileCount
        {
            get { return m_FileCount; }
        }
    }
}
