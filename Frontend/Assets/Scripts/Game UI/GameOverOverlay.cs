using Core_Rules;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace GameUI
{
    public class GameOverOverlay : MonoBehaviour
    {
        [FormerlySerializedAs("TitleTemplate")]
        public string titleTemplate;

        [FormerlySerializedAs("Contents")] public GameObject contents;
        [FormerlySerializedAs("Title")] public TextMeshProUGUI title;

        private void Start()
        {
            contents.SetActive(false);
        }

        public void ShowOverlay(Player losingPlayer)
        {
            contents.SetActive(true);
            title.text = string.Format(titleTemplate, losingPlayer);
        }
    }
}
