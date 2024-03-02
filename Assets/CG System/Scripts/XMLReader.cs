using System;
using UnityEngine;
using System.Xml.Serialization;

namespace CG
{
    public class XMLReader : MonoBehaviour
    {
        private XMLData _XMLData;
        private int _lineIndex = 0;

        // TODO: 用 Addressable Asset System 替换
        private readonly string _folderPath = "Assets/CG System/Texts/";

        public XMLLine GetNextLine()
        {
            if (_lineIndex >= _XMLData.Lines.Length)
            {
                return null;
            }

            XMLLine line = _XMLData.Lines[_lineIndex];
            _lineIndex++;
            return line;
        }

        private void Awake()
        {
            LoadDialogFromXML(_folderPath + "Sample.xml");
        }

        private void LoadDialogFromXML(string filepath)
        {
            XmlSerializer serializer = new(typeof(XMLData));
            using System.IO.StreamReader streamReader = new(filepath);
            _XMLData = (XMLData)serializer.Deserialize(streamReader);
        }
    }

    [Serializable]
    public class XMLData
    {
        [XmlElement("Line")]
        public XMLLine[] Lines;
    }

    [Serializable]
    public class XMLLine
    {
        [XmlElement("Type")]
        public string Type;

        [XmlElement("Character")]
        public string Character;

        [XmlElement("Expression")]
        public string Expression;

        [XmlElement("Chinese")]
        public string Chinese;

        [XmlElement("English")]
        public string English;

        [XmlElement("OnFinished")]
        public string OnFinished;

        [XmlElement("SpecialEffect")]
        public string SpecialEffect;
    }
}
