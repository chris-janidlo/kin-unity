using System.Collections.Generic;
using TMPro;
using UnityAtoms.LDJ50;
using UnityEngine;
using UnityEngine.Serialization;

namespace Kin_UI
{
    public class GameLog : MonoBehaviour
    {
        [FormerlySerializedAs("Generator")]
        public GameLogGenerator generator;

        [FormerlySerializedAs("Text")]
        public TextMeshProUGUI text;

        private List<string> _logEntries;

        private void Start()
        {
            _logEntries = new List<string>();
            text.text = "";
        }

        public void OnDecisionMade(GameStatePair decision)
        {
            decision.Deconstruct(out var oldState, out var newState);
            var newLogEntry = generator.GetLogEntryForAction(oldState, newState);

            if (_logEntries.Count > 0)
                text.text += "\n\n";
            text.text += newLogEntry;
            _logEntries.Add(newLogEntry);
        }
    }
}
