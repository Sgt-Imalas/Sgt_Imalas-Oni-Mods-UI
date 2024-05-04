using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ONIUIParent : MonoBehaviour
{
    [SerializeField]
    string _assetName;

    [SerializeField]
    List<StringEntry> _stringEntries;

    [SerializeField]
    public List<Dialog> dialogs;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    [Serializable]
    public class StringEntry
    {
        [SerializeField]
        public string key;

        [SerializeField]
        public Transform item;

        [SerializeField]
        public string defaultValue;
    }
}
