using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
#region CHANGE LOG
/* October,25,2014 - Nathan Hanlan, Added and implemented the File Class
 * November,2,2014 - Nathan Hanlan, Added a GetFirst<T> method to retrieve the first of T type.
 * November,2,2014-  Nathan Hanlan, Added support for adding a collection to the file.
 */
#endregion
namespace EndevGame
{
    /// <summary>
    /// This class contains FileContent data which is custom content the user can put into the file.
    /// The file is virtually stored inside a FileStream which then gets saved to hard disc.
    /// </summary>
    [Serializable]
    public class File : ISerializable
    {
        public File()
        {
            
        }
        public File(string aFilename)
        {
            m_Filename = aFilename;
        }
        /// <summary>
        /// A constructor used by .Net's deserialziation
        /// </summary>
        /// <param name="aInfo"></param>
        /// <param name="aContext"></param>
        public File(SerializationInfo aInfo, StreamingContext aContext)
        {
            m_Filename =  (string)aInfo.GetValue("Filename", typeof(string));
            m_FileContentCount = (int)aInfo.GetValue("FileContentCount", typeof(int));
        }
        /// <summary>
        /// The name of the file within the FileStream
        /// </summary>
        private string m_Filename = string.Empty;
        /// <summary>
        /// The amount of FileContent that the file contains
        /// </summary>
        private int m_FileContentCount = 0;
        /// <summary>
        /// The list of FileContent
        /// </summary>
        private List<FileContent> m_FileContent = new List<FileContent>();

        /// <summary>
        /// Determines the contents existence within the file by name.
        /// </summary>
        /// <param name="aContentName">The content to search for.</param>
        /// <returns>Returns true if it exists.</returns>
        private bool Exists(string aContentName)
        {
            bool result = false;
            bool shouldClean = false;
            List<FileContent>.Enumerator iter = m_FileContent.GetEnumerator();
            while(iter.MoveNext())
            {
                if(iter.Current == null)
                {
                    shouldClean = true;
                    continue;
                }
                if(iter.Current.contentName == aContentName)
                {
                    result = true;
                    break;
                }
            }
            if(shouldClean)
            {
                CleanUp();
            }
            return result;
        }
        /// <summary>
        /// Adds a FileContent into the file. Content with the same name cannot be added
        /// </summary>
        /// <param name="aContent">The content to add.</param>
        /// <returns>Returns true if successful </returns>
        public bool Add(FileContent aContent)
        {
            if (aContent == null)
            {
                return false;
            }
            if (Exists(aContent.contentName))
            {
                return false;
            }
            m_FileContent.Add(aContent);
            m_FileContentCount++;
            return false;
        }
        /// <summary>
        /// Adds a collection of file content into the file.
        /// </summary>
        /// <param name="aContent"></param>
        /// <returns></returns>
        public bool Add(IEnumerable<FileContent>  aContent)
        {
            int errors = 0;
            IEnumerator<FileContent> iter = aContent.GetEnumerator();
            while(iter.MoveNext())
            {
                if(Add(iter.Current) == false)
                {
                    errors++;
                }
            }
            return errors == 0;
        }
        /// <summary>
        /// Removes content from the list by reference
        /// </summary>
        /// <param name="aFileContent">The content to remove</param>
        /// <returns>Returns true if successful</returns>
        public bool Remove(FileContent aFileContent)
        {
            bool result = m_FileContent.Remove(aFileContent);
            if(result == false)
            {
                m_FileContentCount--;
            }
            return result;
        }
        /// <summary>
        /// Removes content from the list by name
        /// </summary>
        /// <param name="aContentName">The name of the content to remove</param>
        /// <returns>Returns the file content removed, returns null for content not found</returns>
        public FileContent Remove(string aContentName)
        {
            bool shouldClean = false;
            FileContent content = null;
            List<FileContent>.Enumerator iter = m_FileContent.GetEnumerator();
            while(iter.MoveNext())
            {
                if(iter.Current == null)
                {
                    shouldClean = true;
                    continue;
                }
                if(iter.Current.contentName == aContentName)
                {
                    content = iter.Current;
                    Remove(content);
                    break;
                }
            }
            if(shouldClean == true)
            {
                CleanUp();
            }
            return content;
        }
        /// <summary>
        /// Gets the content from the list by name
        /// </summary>
        /// <param name="aContentName">The name of the content to search for.</param>
        /// <returns>Returns the content found, null otherwise.</returns>
        public FileContent Get(string aContentName)
        {
            bool shouldClean = false;
            FileContent content = null;
            List<FileContent>.Enumerator iter = m_FileContent.GetEnumerator();
            while (iter.MoveNext())
            {
                if (iter.Current == null)
                {
                    shouldClean = true;
                    continue;
                }
                if (iter.Current.contentName == aContentName)
                {
                    content = iter.Current;
                    break;
                }
            }
            if (shouldClean == true)
            {
                CleanUp();
            }
            return content;
        }
        /// <summary>
        /// Gets the content by name and type
        /// </summary>
        /// <param name="aContentName">The name of the content to search for.</param>
        /// <param name="aType">The type to constrain to.</param>
        /// <returns>Returns the content found, null otherwise.</returns>
        public FileContent Get(string aContentName, Type aType)
        {
            bool shouldClean = false;
            FileContent content = null;
            List<FileContent>.Enumerator iter = m_FileContent.GetEnumerator();
            while (iter.MoveNext())
            {
                if (iter.Current == null)
                {
                    shouldClean = true;
                    continue;
                }
                if (iter.Current.contentName == aContentName && iter.Current.GetType() == aType)
                {
                    content = iter.Current;
                    break;
                }
            }
            if (shouldClean == true)
            {
                CleanUp();
            }
            return content;
        }
        /// <summary>
        /// Gets the content by name and type
        /// </summary>
        /// <typeparam name="T">The type to constrain to</typeparam>
        /// <param name="aContentName">The name of the content to search for</param>
        /// <returns>A casted type of the found content.</returns>
        public T Get<T>(string aContentName) where T : FileContent
        {
            bool shouldClean = false;
            T content = null;
            Type type = typeof(T);
            List<FileContent>.Enumerator iter = m_FileContent.GetEnumerator();
            while (iter.MoveNext())
            {
                if (iter.Current == null)
                {
                    shouldClean = true;
                    continue;
                }
                if (iter.Current.contentName == aContentName && iter.GetType() == type)
                {
                    content = iter.Current as T;
                    break;
                }
            }
            if (shouldClean == true)
            {
                CleanUp();
            }
            return content;
        }
        /// <summary>
        /// Searches for the first of T type and returns it.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetFirst<T>() where T : FileContent
        {
            bool shouldClean = false;
            T content = null;
            Type type = typeof(T);
            IEnumerator<FileContent> iter = m_FileContent.GetEnumerator();
            while(iter.MoveNext())
            {
                if(iter.Current == null)
                {
                    shouldClean = true;
                    continue;
                }
                if(iter.GetType() == type)
                {
                    content = iter.Current as T;
                    break;
                }
            }
            if(shouldClean == true)
            {
                CleanUp();
            }
            return content;
        }
        /// <summary>
        /// Searches for all file content of type and returns a list of the found content.
        /// </summary>
        /// <typeparam name="T">The type of content to search for</typeparam>
        /// <returns></returns>
        public List<T> GetAll<T>() where T : FileContent
        {
            bool shouldClean = false;
            List<T> types = new List<T>();
            List<FileContent>.Enumerator iter = m_FileContent.GetEnumerator();
            

            while(iter.MoveNext())
            {
                if(iter.Current == null)
                {
                    shouldClean = true;
                    continue;
                }
                T current = iter.Current as T;
                if(current != null)
                {
                    types.Add(current);
                }
            }
            if(shouldClean == true)
            {
                CleanUp();
            }
            return types;
        }
        /// <summary>
        /// Renames the content if the content exists and content with the new name does not exist.
        /// </summary>
        /// <param name="aContentName">The current name of the content</param>
        /// <param name="aNewName">The new name of the content.</param>
        /// <returns>Returns true if successful</returns>
        public bool RenameContent(string aContentName, string aNewName)
        {
            FileContent content = Get(aContentName);
            if(content != null && !Exists(aNewName))
            {
                content.contentName = aNewName;
                return true;
            }
            return false;
        }
        /// <summary>
        /// Clears the list of all items.
        /// </summary>
        public void Clear()
        {
            if(m_FileContent != null)
            {
                m_FileContentCount = 0;
                m_FileContent.Clear();
            }
        }
        /// <summary>
        /// Called by the file stream to save the file  and its content to the speciifed filestream 
        /// </summary>
        /// <param name="aFileStream"></param>
        /// <param name="aFormatter"></param>
        public void Save(System.IO.FileStream aFileStream, BinaryFormatter aFormatter)
        {
            if(aFileStream == null || aFormatter == null)
            {
                return;
            }
            aFormatter.Serialize(aFileStream, this);

            List<FileContent>.Enumerator iter = m_FileContent.GetEnumerator();
            while(iter.MoveNext())
            {
                if(iter.Current == null)
                {
                    continue;
                }
                aFormatter.Serialize(aFileStream, iter.Current);
            }
        }
        /// <summary>
        /// Called by the file stream to save the load the file content into the file
        /// </summary>
        /// <param name="aFileStream"></param>
        /// <param name="aFormatter"></param>
        public void Load(System.IO.FileStream aFileStream, BinaryFormatter aFormatter)
        {
            if(aFileStream == null || aFormatter == null)
            {
                return;
            }
            
            for(int i = 0; i < m_FileContentCount; i++)
            {
                m_FileContent.Add((FileContent)aFormatter.Deserialize(aFileStream));
            }
        }
        /// <summary>
        /// Gets called through the ISerializable interface to get the data for saving.
        /// </summary>
        /// <param name="aInfo"></param>
        /// <param name="aContext"></param>
        public void GetObjectData(SerializationInfo aInfo, StreamingContext aContext)
        {
            aInfo.AddValue("Filename", m_Filename);
            aInfo.AddValue("FileContentCount", m_FileContentCount);
        }
        /// <summary>
        /// Removes any null references from the content list.
        /// </summary>
        public void CleanUp()
        {
            for(int i = m_FileContent.Count -1; i >= 0; i--)
            {
                if(m_FileContent[i] == null)
                {
                    m_FileContent.RemoveAt(i);
                }
            }
        }
        /// <summary>
        /// The name of the file
        /// </summary>
        public string filename
        {
            get { return m_Filename;}
            set { m_Filename = value; }
        }
        /// <summary>
        /// The amount of content within the file stream.
        /// </summary>
        public int contentCount
        {
            get { return m_FileContentCount; }
        }
    }
}
