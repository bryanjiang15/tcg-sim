using System;
using System.IO;
using UnityEngine;

namespace CardLibrary
{
    public class ArtLibraryManager : Singleton<ArtLibraryManager>
    {
        public static ArtLibraryManager Instance { get; private set; }

        private string artDirectory;

        private void Awake()
        {
            InitializeArtLibrary();
        }

        private void InitializeArtLibrary()
        {
            artDirectory = Path.Combine(Application.persistentDataPath, "CardArt");
            if (!Directory.Exists(artDirectory))
            {
                Directory.CreateDirectory(artDirectory);
            }
        }

        /// <summary>
        /// Saves card art to the art library and returns the file path
        /// </summary>
        /// <param name="cardId">The ID of the card</param>
        /// <param name="cardArt">The texture to save</param>
        /// <returns>The file path where the art was saved</returns>
        public string SaveCardArt(int cardId, Texture2D cardArt)
        {
            if (cardArt == null)
            {
                Debug.LogError($"Cannot save null texture for card ID: {cardId}");
                return null;
            }

            string artPath = Path.Combine(artDirectory, $"cardArt_{cardId}.png");
            
            try
            {
                byte[] bytes = cardArt.EncodeToPNG();
                File.WriteAllBytes(artPath, bytes);
                Debug.Log($"Card art saved successfully for card ID {cardId} at: {artPath}");
                return artPath;
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to save card art for card ID {cardId}: {e.Message}");
                return null;
            }
        }

        /// <summary>
        /// Loads card art from the specified file path
        /// </summary>
        /// <param name="artPath">The file path to load the art from</param>
        /// <returns>The loaded sprite, or null if loading failed</returns>
        public Sprite LoadCardArt(string artPath)
        {
            if (string.IsNullOrEmpty(artPath))
            {
                Debug.LogError("Art path is null or empty");
                return null;
            }

            if (!File.Exists(artPath))
            {
                Debug.LogWarning($"Art file not found at path: {artPath}");
                return null;
            }

            try
            {
                byte[] bytes = File.ReadAllBytes(artPath);
                Texture2D texture = new Texture2D(2, 2);
                texture.LoadImage(bytes);
                return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to load card art from {artPath}: {e.Message}");
                return null;
            }
        }

        /// <summary>
        /// Loads card art by card ID using the default naming convention
        /// </summary>
        /// <param name="cardId">The ID of the card</param>
        /// <returns>The loaded sprite, or null if loading failed</returns>
        public Sprite LoadCardArtById(int cardId)
        {
            string artPath = Path.Combine(artDirectory, $"cardArt_{cardId}.png");
            return LoadCardArt(artPath);
        }

        /// <summary>
        /// Checks if art exists for a specific card ID
        /// </summary>
        /// <param name="cardId">The ID of the card</param>
        /// <returns>True if art exists, false otherwise</returns>
        public bool HasCardArt(int cardId)
        {
            string artPath = Path.Combine(artDirectory, $"cardArt_{cardId}.png");
            return File.Exists(artPath);
        }

        /// <summary>
        /// Deletes card art for a specific card ID
        /// </summary>
        /// <param name="cardId">The ID of the card</param>
        /// <returns>True if deletion was successful, false otherwise</returns>
        public bool DeleteCardArt(int cardId)
        {
            string artPath = Path.Combine(artDirectory, $"cardArt_{cardId}.png");
            
            if (File.Exists(artPath))
            {
                try
                {
                    File.Delete(artPath);
                    Debug.Log($"Card art deleted successfully for card ID {cardId}");
                    return true;
                }
                catch (Exception e)
                {
                    Debug.LogError($"Failed to delete card art for card ID {cardId}: {e.Message}");
                    return false;
                }
            }
            
            Debug.LogWarning($"No art file found to delete for card ID {cardId}");
            return false;
        }

        /// <summary>
        /// Gets the total size of the art library in bytes
        /// </summary>
        /// <returns>The total size in bytes</returns>
        public long GetArtLibrarySize()
        {
            if (!Directory.Exists(artDirectory))
                return 0;

            long totalSize = 0;
            try
            {
                string[] files = Directory.GetFiles(artDirectory, "*.png");
                foreach (string file in files)
                {
                    FileInfo fileInfo = new FileInfo(file);
                    totalSize += fileInfo.Length;
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to calculate art library size: {e.Message}");
            }
            
            return totalSize;
        }

        /// <summary>
        /// Clears all art from the art library
        /// </summary>
        public void ClearArtLibrary()
        {
            if (Directory.Exists(artDirectory))
            {
                try
                {
                    string[] files = Directory.GetFiles(artDirectory, "*.png");
                    foreach (string file in files)
                    {
                        File.Delete(file);
                    }
                    Debug.Log("Art library cleared successfully");
                }
                catch (Exception e)
                {
                    Debug.LogError($"Failed to clear art library: {e.Message}");
                }
            }
        }
    }
} 