using TMPro;
using UnityAtoms.LDJ50;
using UnityAtoms.SceneMgmt;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class GameModeButton : MonoBehaviour
{
    [FormerlySerializedAs("RedDeciderValue")]
    public ADecider redDeciderValue;

    [FormerlySerializedAs("BlueDeciderValue")]
    public ADecider blueDeciderValue;

    [FormerlySerializedAs("ButtonText")] public string buttonText;

    [FormerlySerializedAs("RedDeciderVariable")]
    public ADeciderVariable redDeciderVariable;

    [FormerlySerializedAs("BlueDeciderVariable")]
    public ADeciderVariable blueDeciderVariable;

    [FormerlySerializedAs("GameScene")] public SceneField gameScene;

    [FormerlySerializedAs("ButtonTextContainer")]
    public TextMeshProUGUI buttonTextContainer;

    private void Start()
    {
        buttonTextContainer.text = buttonText;
    }

    public void OnButtonClicked()
    {
        redDeciderVariable.Value = redDeciderValue;
        blueDeciderVariable.Value = blueDeciderValue;

        SceneManager.LoadScene(gameScene);
    }
}
