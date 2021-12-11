using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* * * * *
 * Generic class that can generate a library of Scriptable Objects using the
 * specified 
 * * * * */

public class ObjLib<T>
{
    //public static ObjLib<T> instance;
    protected string prefabFP;
    protected string SODir;

    // Dictionary that holds our objects.
    public Dictionary<string,SOItem> lib;

    public void Initialize(string _prefabFP, string _SODir) {
        prefabFP = _prefabFP;
        SODir = _SODir;
        LoadLib();
    }

    // Generate object prefabs and store in lib dict.
    public void LoadLib() {
        lib = new Dictionary<string,SOItem>();

        // Get filepaths & file names of equipment objects we want to load.
        string topDir = Directory.GetCurrentDirectory();

        string[] files = Directory.GetFiles(
            topDir + "/Assets/Resources/" + SODir, "*.asset", SearchOption.TopDirectoryOnly
        );
        List<string> tgtFiles = new List<string>();
        for (int i = 0; i < files.Length; i++) {
            string[] splitstr = files[i].Split('/');
            files[i] = splitstr[splitstr.Length - 1].Split('.')[0];
            tgtFiles.Add(files[i]);
        }
        
        // Generate GameObjects based on ScriptableObjects found in SOs/Equipment.
        foreach (string fname in tgtFiles) {
            //Debug.Log(fname);
            SOItem data = Resources.Load(SODir + "/" + fname, typeof(SOItem)) as SOItem;
            lib.Add(data.uniqueID, data);
        }
    }

    // Get an intantiated deep copy of an object in the lib.
    public T GetCopy<T>(string key) where T : ItemOrigin {
        T newInstance = MonoBehaviour.Instantiate(Resources.Load(prefabFP, typeof(T))) as T;
        SOItem data = MonoBehaviour.Instantiate(this.lib[key]);
        newInstance.GetComponent<T>().data = data;
        T retval = newInstance.GetComponent<T>();
        return retval;
    }
}