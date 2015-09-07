using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using Aria;

public class ItemDatabaseEditor : EditorWindow
{
    static ItemDB db;

    // EditorGUI stuff.
    Vector2 scrollOffset = Vector2.zero;

    [MenuItem("Window/Database/Items")]
    public static void ShowWindow ()
    {
        ItemDatabaseEditor dbEditor = (ItemDatabaseEditor)EditorWindow.GetWindow(typeof(ItemDatabaseEditor));
        dbEditor.Show();
    }

    void OnGUI()
    {
        if (db == null) db = new ItemDB();

        Item item;
        Rect iconPos;

        GUILayout.Label("New Item");
        GUILayout.BeginHorizontal ();
        if (GUILayout.Button("Create New")) db.CreateItem();
        GUILayout.EndHorizontal ();

        GUILayout.Label("// TODO: Add Sort and Filter options.");

        GUILayout.Label("Items", EditorStyles.boldLabel);

        scrollOffset = GUILayout.BeginScrollView(scrollOffset);
        for (int i = 0, len = db.size; i < len; i++)
        {
            item = db.items[i];

            /*
             * Begin GUILayout formatting
             */
            EditorGUILayout.BeginHorizontal(GUILayout.Height(48f));

                EditorGUILayout.BeginVertical();
                    if (i != 0)
                    {
                        if (GUILayout.Button("X", GUILayout.Width(18f)))
                        {
                            if(EditorUtility.DisplayDialog("Are you sure?", "You are about to delete item number " + item.itemId.ToString() + " (" + item.itemName + ").\nAre you sure?", "Delete", "Cancel"))
                            {
                                db.DeleteItem(item.itemId);
                            }
                        }
                    }
                    else
                    {
                        GUILayout.Button("X", GUILayout.Width(18f));
                    }
                EditorGUILayout.EndVertical();

                EditorGUILayout.BeginVertical();

                    EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField(item.itemId.ToString(), GUILayout.Width(24f));
                        item.itemName = EditorGUILayout.TextField(item.itemName);
                    EditorGUILayout.EndHorizontal();
            
                    EditorGUILayout.BeginHorizontal();
                        item.itemType = (ItemType)EditorGUILayout.EnumPopup(item.itemType, GUILayout.Width(60f));
                        if (item.itemType == ItemType.StdEquip) item.eqSlot = (Equipment)EditorGUILayout.EnumPopup(item.eqSlot, GUILayout.Width(60f));
                        item.itemIcon = (Sprite)EditorGUILayout.ObjectField(item.itemIcon, typeof(Sprite), true);
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.BeginHorizontal();
                        item.description = EditorGUILayout.TextArea(item.description, GUILayout.Height(30f));
                    EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndVertical();

                EditorGUILayout.BeginVertical(GUILayout.Width(48f));

                    iconPos = EditorGUILayout.BeginHorizontal(GUILayout.Height(48f));
                        iconPos.width = 48f;
                        iconPos.height = 48f;
                        if (item.itemIcon != null)
                            EditorGUI.DrawPreviewTexture(iconPos, item.itemIcon.texture, new Material(Shader.Find("UI/Default")));
                        else
                            EditorGUI.DrawRect(iconPos, Color.grey);
                    EditorGUILayout.EndHorizontal();

                EditorGUILayout.EndVertical();

            EditorGUILayout.EndHorizontal();

            GUILayout.Space(12f);
            /*
             * End GUILayout formatting
             */
        }
        GUILayout.EndScrollView();
    }

    /*
     * An internal class for managing the database.
     */
    public class ItemDB
    {
        private string savePath = "Assets/Resources/Data/Item";

        public DatabaseInfo info;

        public List<Item> items = new List<Item>();

        public List<SerializedObject> sItems = new List<SerializedObject>();

        public int size
        {
            get
            {
                return items.Count;
            }
        }

        public ItemDB ()
        {
            ValidatePath(savePath);

            info = AssetDatabase.LoadAssetAtPath(savePath + "/itemDBInfo.asset", typeof(DatabaseInfo)) as DatabaseInfo;

            if (info == null)
            {
                // There is no database info present.

                // Delete old assets.
                DeleteAllSavedItems ();

                // Create a new one.
                info = CreateInstance<DatabaseInfo> ();

                // This will also overwrite an old one.
                AssetDatabase.CreateAsset(info, savePath + "/itemDBInfo.asset");

                // Create default null item.
                Item newItem = CreateItem();
                newItem.itemName = "Null";
                AssetDatabase.SaveAssets();
            }
            else
            {
                // Load items.
                LoadAllSavedItems ();
            }
        }
        
        void DeleteAllSavedItems ()
        {
            string[] existingItemAssets = AssetDatabase.FindAssets("t:Item", new string[] { savePath });
            int numExistingItems = existingItemAssets.Length;
            for (int i = 0; i < numExistingItems; i++) AssetDatabase.DeleteAsset(AssetDatabase.GUIDToAssetPath(existingItemAssets[i]));
        }

        void LoadAllSavedItems ()
        {
            string[] existingItemAssets = AssetDatabase.FindAssets("t:Item", new string[] { savePath });
            int numExistingItems = existingItemAssets.Length;

            for (int i = 0; i < numExistingItems; i++)
            {
                Item item = AssetDatabase.LoadAssetAtPath<Item>(AssetDatabase.GUIDToAssetPath(existingItemAssets[i]));

                items.Add(item);
                sItems.Add(new SerializedObject(item));
            }
        }

        /// <summary>
        /// Delete an item at the given Item ID.
        /// </summary>
        /// <param name="id">The id of the item to be deleted.</param>
        public void DeleteItem(int id)
        {
            int listId = items.FindIndex(x => x.itemId == id);

            // Delete item save asset.
            AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(items[listId]));

            // Delete from current list.
            items.RemoveAt(listId);
            sItems.RemoveAt(listId);
        }
        
        /// <summary>
        /// Create an item with the given name.
        /// </summary>
        /// <param name="name">The name of the new item.</param>
        public Item CreateItem ()
        {
            Item newItem = CreateInstance<Item>();
            int idNum = info.size;

            newItem.itemId = idNum;

            items.Add(newItem);
            sItems.Add (new SerializedObject (newItem));

            AssetDatabase.CreateAsset(newItem, savePath + "/" + idNum + ".asset");

            info.size++;

            EditorUtility.SetDirty (info);

            AssetDatabase.SaveAssets();
            
            Debug.Log("Added item to database");

            return newItem;
        }

        // Helper functions.
        void ValidatePath (string path)
        {
            int pivot;
            string targetFolder, pathRoot;

            if (!AssetDatabase.IsValidFolder (path))
            {
                pivot = path.LastIndexOf ("/");
                targetFolder = path.Substring(pivot + 1, path.Length - (pivot + 1));
                pathRoot = path.Remove(pivot, path.Length - pivot);

                ValidatePath (pathRoot);
                AssetDatabase.CreateFolder (pathRoot, targetFolder);
            }
        }
    }
}
