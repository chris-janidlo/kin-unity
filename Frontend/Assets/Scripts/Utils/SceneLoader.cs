using UnityAtoms.SceneMgmt;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

namespace Utils
{
    public class SceneLoader : MonoBehaviour
    {
        [FormerlySerializedAs("Scene")] public SceneField scene;

        public void LoadScene()
        {
            SceneManager.LoadScene(scene);
        }
    }
}
