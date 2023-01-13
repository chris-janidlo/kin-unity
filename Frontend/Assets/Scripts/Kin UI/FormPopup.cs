using System.Collections.Generic;
using System.Linq;
using Core_Rules;
using crass;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Kin_UI
{
    public class FormPopup : MonoBehaviour
    {
        public delegate void ClickCallback(Form form);

        [FormerlySerializedAs("ParentTransform")]
        public Transform parentTransform;

        [FormerlySerializedAs("FormButtons")]
        [Tooltip(
            "Assumes that there aren't any extra GameObjects between" +
            " buttons and the layout group that controls the button layout"
        )]
        public EnumMap<Form, Button> formButtons;

        public void RegisterCallback(ClickCallback callback)
        {
            foreach (var form in EnumUtil.AllValues<Form>())
                formButtons[form].onClick.AddListener(() => callback(form));

            TearDownPopup();
        }

        public void SetUpPopup(Cell cell, IEnumerable<Form> forms)
        {
            parentTransform.gameObject.SetActive(true);
            parentTransform.position = cell.transform.position;

            foreach (var form in EnumUtil.AllValues<Form>())
                formButtons[form].gameObject.SetActive(forms.Contains(form));
        }

        public void TearDownPopup()
        {
            parentTransform.gameObject.SetActive(false);
        }
    }
}
