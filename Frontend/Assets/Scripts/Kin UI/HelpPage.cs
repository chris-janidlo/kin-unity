using UnityEngine;
using UnityEngine.Serialization;

namespace Kin_UI
{
    public class HelpPage : MonoBehaviour
    {
        [FormerlySerializedAs("ContentsParent")]
        public GameObject contentsParent;

        private void Start()
        {
            Close();
        }

        public void Open()
        {
            contentsParent.SetActive(true);
        }

        public void Close()
        {
            contentsParent.SetActive(false);
        }
    }
}
