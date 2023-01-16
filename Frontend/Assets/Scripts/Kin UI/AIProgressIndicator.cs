using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using Decisions;

namespace Kin_UI
{
    public class AIProgressIndicator : MonoBehaviour
    {
        [FormerlySerializedAs("BaseText")]
        public string baseText;

        [FormerlySerializedAs("RepeatingText")]
        public string repeatingText;

        [FormerlySerializedAs("MaxRepetitions")]
        public int maxRepetitions;

        [FormerlySerializedAs("TextContainer")]
        public TextMeshProUGUI textContainer;

        [FormerlySerializedAs("AIDecider")]
        public AIDecider aiDecider;

        private void Update()
        {
            textContainer.text = aiDecider.Deciding
                ? baseText + Repeat(repeatingText, (int)(maxRepetitions * aiDecider.Progress))
                : "";
        }

        // https://stackoverflow.com/a/720915/5931898
        private static string Repeat(string value, int count)
        {
            return new StringBuilder(value.Length * count).Insert(0, value, count).ToString();
        }
    }
}
