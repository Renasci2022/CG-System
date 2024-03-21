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
        public string TypeString;

        [XmlIgnore]
        public LineType Type => Enum.TryParse(TypeString, true, out LineType type) ? type : LineType.Narration;

        [XmlElement("DialogBox")]
        public string DialogBoxString;

        [XmlIgnore]
        public DialogBoxType DialogBox => Enum.TryParse(DialogBoxString, true, out DialogBoxType dialogBox) ? dialogBox : DialogBoxType.Normal;

        [XmlElement("Character")]
        public string Character;

        [XmlElement("Expression")]
        public string Expression;

        [XmlElement("Effect")]
        public string EffectString;

        [XmlIgnore]
        public EffectType Effect => Enum.TryParse(EffectString, true, out EffectType effect) ? effect : EffectType.None;

        [XmlElement("Interval")]
        public string IntervalString;

        [XmlIgnore]
        public float Interval => float.TryParse(IntervalString, out float interval) ? interval : -1f;

        [XmlElement("Chinese")]
        public string Chinese;

        [XmlElement("English")]
        public string English;

        // ? 是否存在性能问题
        [XmlIgnore]
        public string Text => GameObject.FindObjectOfType<CGPlayer>().Language == Language.Chinese ? Chinese : English;

        [XmlIgnore]
        public DialogInfo DialogInfo => new() { DialogBox = DialogBox, Character = Character, Expression = Expression, Effect = Effect };
    }

    public enum LineType
    {
        Scene,
        Narration,
        Dialog,
    }

    public enum DialogBoxType
    {
        Normal,
    }

    public enum EffectType
    {
        None,
        Shake,
        Fade,
    }

    public enum Language
    {
        Chinese,
        English,
    }

    public struct DialogInfo
    {
        public DialogBoxType DialogBox;
        public string Character;
        public string Expression;
        public EffectType Effect;
    }
}
