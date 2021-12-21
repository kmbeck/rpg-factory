using Newtonsoft.Json.Linq;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* * * * *
 * Parent class for all ScriptableObject Libraries. Will use data found in
 * ObjectMetadata.json to load a directory of ScriptableObjects into a 
 * dictionary where every SO is defined by a unique ID (string).
 * Note: GetInstance<O> allows for a single SO to be used to instantiate many
 *   different types of prefabs.
 * * * * */

public class SOLib<D>
where D : SOOrigin
{
    // Maps uniqueIDs to lib list index.
    [HideInInspector]
    public Dictionary<string, D> lib;

    public void LoadLib(string SODir) {
        lib = new Dictionary<string, D>();

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
        
        // Generate Objects based on ScriptableObjects found in SOs/Equipment.
        foreach (string fname in tgtFiles) {
            D data = Resources.Load(SODir + "/" + fname, typeof(D)) as D;
            lib[data.uniqueID] = data;
        }
    }

    // Get an Instantated GameObject of type O using data at key.
    public O GetInstance<O>(string key) where O : ObjectOrigin<D> {
        O newInstance = MonoBehaviour.Instantiate(
            Resources.Load(SODB.GetMetadataField(typeof(O).ToString(), "default_prefab_fp"), 
            typeof(O))) as O;
        D data = MonoBehaviour.Instantiate(lib[key]) as D;
        newInstance.GetComponent<O>().data = data;
        return newInstance.GetComponent<O>();
    }
} 