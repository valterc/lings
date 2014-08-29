using LiNGS.Server.State.Persistent.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace LiNGS.Server.State.Persistent
{
    internal class StorageEngine<T>
    {
        private string sessionDirectoryPath;

        internal StorageEngine(string sessionGuid) : this(Directory.GetCurrentDirectory(), sessionGuid)
        {
        }

        internal StorageEngine(string sessionBaseFolder, string sessionGuid)
        {
            sessionDirectoryPath = Path.Combine(sessionBaseFolder, sessionGuid);
        }

        private String GetUserStatePath(String userId)
        {
            return Path.Combine(sessionDirectoryPath, userId);
        }

        internal void SaveState(String userId, T state)
        {
            FileStream f = null;

            try
            {
                Directory.CreateDirectory(sessionDirectoryPath);

                f = File.Open(GetUserStatePath(userId), FileMode.Create);

                using (GZipStream gZipStream = new GZipStream(f, CompressionMode.Compress))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(T));
                    serializer.Serialize(gZipStream, state);
                }                
                  
            }
            catch (Exception)
            {
                //Ignore exceptions
            }
            finally
            {
                if (f != null)
                {
                    f.Dispose();
                }
            }
        }

        internal T RestoreState(String userId)
        {
            FileStream f = null;
            T t = default(T);

            try
            {
                f = File.Open(GetUserStatePath(userId), FileMode.Open);

                using (GZipStream gZipStream = new GZipStream(f, CompressionMode.Decompress))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(T));
                    t = (T)serializer.Deserialize(gZipStream);
                }
            }
            catch (Exception)
            {
                //Ignore exceptions
            }
            finally
            {
                if (f != null)
                {
                    f.Dispose();
                }
            }
            
            return t;
        }

        internal bool StateFileExists(String userId)
        {
            return File.Exists(GetUserStatePath(userId));
        }

        internal void ClearStates()
        {
            Directory.Delete(sessionDirectoryPath, true);
        }

    }
}
