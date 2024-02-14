using System;
using System.IO;
using System.Xml.Serialization;
using UnityEngine;

namespace CG
{
    public class DialogManager : MonoBehaviour
    {
        private Dialog _dialogData;

        private void Start()
        {
            string testpath = "Assets/CG System/Scripts/narrations.xml";
            LoadDialogFromXML(testpath);
            foreach (var line in _dialogData.Lines)
            {
                Debug.Log(line.DialogBox);
            }
        }

        private void LoadDialogFromXML(string filepath)
        {
            XmlSerializer serializer = new(typeof(Dialog));
            using StreamReader streamReader = new(filepath);
            _dialogData = (Dialog)serializer.Deserialize(streamReader);
        }
    }

    [Serializable]
    public class Dialog
    {
        [XmlElement("Line")]
        public DialogLine[] Lines;
    }

    [Serializable]
    public class DialogLine
    {
        [XmlElement("DialogBox")]
        public string DialogBox;

        [XmlElement("Character")]
        public string Character;

        [XmlElement("Expression")]
        public string Expression;

        [XmlElement("Text")]
        public string Text;

        [XmlElement("SpecialEffect")]
        public string SpecialEffect;
    }
}