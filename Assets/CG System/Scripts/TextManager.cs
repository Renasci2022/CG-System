using System;
using System.IO;
using System.Xml.Serialization;
using UnityEngine;

namespace CG
{
    public class TextManager : MonoBehaviour
    {
        private TextData _textData;
        private int _lineIndex = 0;

        private readonly string _folderPath = "Assets/CG System/Texts/";

        public (Line line, bool isDialog) GetNextLine()
        {
            if (_lineIndex >= _textData.Lines.Length)
            {
                Debug.Log("没有更多文本");
                return (null, false);
            }

            Line line = _textData.Lines[_lineIndex];
            _lineIndex++;
            bool isDialog = !string.IsNullOrEmpty(line.DialogBox);
            return (line, isDialog);
        }

        private void Awake()
        {
            LoadDialogFromXML(_folderPath + "test.xml");    // TODO: 从外部传入文件路径
        }

        private void LoadDialogFromXML(string filepath)
        {
            XmlSerializer serializer = new(typeof(TextData));
            using StreamReader streamReader = new(filepath);
            _textData = (TextData)serializer.Deserialize(streamReader);
        }
    }

    [Serializable]
    public class TextData
    {
        [XmlElement("Line")]
        public Line[] Lines;
    }

    [Serializable]
    public class Line
    {
        [XmlElement("DialogBox")]
        public string DialogBox;

        [XmlElement("Character")]
        public string Character;

        [XmlElement("Expression")]
        public string Expression;

        [XmlElement("SpecialEffect")]
        public string SpecialEffect;

        [XmlElement("Chinese")]
        public string Chinese;

        [XmlElement("English")]
        public string English;
    }
}