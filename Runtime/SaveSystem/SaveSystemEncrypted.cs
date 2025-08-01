using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;
using UnityEngine;

namespace _IM_Code_Tools_Systems.SaveSystem
{
    public enum SaveFormat
    {
        Binary,
        Json
    }

    public class SaveSystemEncrypted
    {
        private readonly string _saveDirectory;
        private readonly byte[] _key;
        private readonly byte[] _iv;
        private readonly SaveFormat _format;

        public SaveSystemEncrypted(string folderName = "Saves", string password = "DefaultStrongPassword123", SaveFormat format = SaveFormat.Binary)
        {
            if (string.IsNullOrWhiteSpace(folderName))
                throw new ArgumentException("Folder name cannot be null or whitespace.", nameof(folderName));

            _saveDirectory = Path.Combine(Application.persistentDataPath, folderName);
            _format = format;

            EnsureDirectoryExists();

            // Derive Key & IV from password
            using var sha = SHA256.Create();
            byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
            _key = sha.ComputeHash(passwordBytes);
            _iv = new byte[16]; Array.Copy(_key, _iv, 16);
        }

        public void Save<T>(string key, T data)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentException("Key cannot be null or whitespace.", nameof(key));

            try
            {
                string path = GetSavePath(key);

                byte[] rawData = _format switch
                {
                    SaveFormat.Binary => SerializeBinary(data),
                    SaveFormat.Json => SerializeJson(data),
                    _ => throw new NotSupportedException("Unsupported save format.")
                };

                byte[] encrypted = Encrypt(rawData);
                File.WriteAllBytes(path, encrypted);
            }
            catch (Exception ex)
            {
                Debug.LogError($"[SaveSystemEncrypted] Failed to save key '{key}': {ex}");
            }
        }

        public T Load<T>(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentException("Key cannot be null or whitespace.", nameof(key));

            string path = GetSavePath(key);
            if (!File.Exists(path))
                return default;

            try
            {
                byte[] encrypted = File.ReadAllBytes(path);
                byte[] decrypted = Decrypt(encrypted);

                return _format switch
                {
                    SaveFormat.Binary => DeserializeBinary<T>(decrypted),
                    SaveFormat.Json => DeserializeJson<T>(decrypted),
                    _ => throw new NotSupportedException("Unsupported load format.")
                };
            }
            catch (Exception ex)
            {
                Debug.LogError($"[SaveSystemEncrypted] Failed to load key '{key}': {ex}");
                return default;
            }
        }

        public bool HasKey(string key)
        {
            return File.Exists(GetSavePath(key));
        }

        public void DeleteKey(string key)
        {
            string path = GetSavePath(key);
            if (File.Exists(path))
            {
                try
                {
                    File.Delete(path);
                }
                catch (Exception ex)
                {
                    Debug.LogError($"[SaveSystemEncrypted] Failed to delete key '{key}': {ex}");
                }
            }
        }

        // ========================= Helper Methods =========================

        private string GetSavePath(string key)
        {
            string safeKey = SanitizeFileName(key);
            string extension = _format == SaveFormat.Json ? ".json" : ".dat";
            return Path.Combine(_saveDirectory, safeKey + extension);
        }

        private void EnsureDirectoryExists()
        {
            if (!Directory.Exists(_saveDirectory))
                Directory.CreateDirectory(_saveDirectory);
        }

        private string SanitizeFileName(string filename)
        {
            foreach (var c in Path.GetInvalidFileNameChars())
                filename = filename.Replace(c, '_');
            return filename;
        }

        // ========================= Serialization =========================

        private byte[] SerializeBinary<T>(T data)
        {
            using var ms = new MemoryStream();
#pragma warning disable SYSLIB0011 // BinaryFormatter is obsolete
            new BinaryFormatter().Serialize(ms, data);
#pragma warning restore SYSLIB0011
            return ms.ToArray();
        }

        private T DeserializeBinary<T>(byte[] bytes)
        {
            using var ms = new MemoryStream(bytes);
#pragma warning disable SYSLIB0011
            return (T)new BinaryFormatter().Deserialize(ms);
#pragma warning restore SYSLIB0011
        }

        private byte[] SerializeJson<T>(T data)
        {
            string json = JsonConvert.SerializeObject(data, Formatting.Indented);
            return Encoding.UTF8.GetBytes(json);
        }

        private T DeserializeJson<T>(byte[] bytes)
        {
            string json = Encoding.UTF8.GetString(bytes);
            return JsonConvert.DeserializeObject<T>(json);
        }

        // ========================= Encryption =========================

        private byte[] Encrypt(byte[] data)
        {
            using var aes = Aes.Create();
            aes.Key = _key;
            aes.IV = _iv;
            using var encryptor = aes.CreateEncryptor();
            return PerformCryptography(data, encryptor);
        }

        private byte[] Decrypt(byte[] data)
        {
            using var aes = Aes.Create();
            aes.Key = _key;
            aes.IV = _iv;
            using var decryptor = aes.CreateDecryptor();
            return PerformCryptography(data, decryptor);
        }

        private byte[] PerformCryptography(byte[] data, ICryptoTransform cryptoTransform)
        {
            using var ms = new MemoryStream();
            using (var cryptoStream = new CryptoStream(ms, cryptoTransform, CryptoStreamMode.Write))
            {
                cryptoStream.Write(data, 0, data.Length);
            }
            return ms.ToArray();
        }
    }
}
