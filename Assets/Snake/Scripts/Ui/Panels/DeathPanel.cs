using UnityEngine;
using UnityEngine.SceneManagement;

namespace Ui.Panels
{
    public class DeathPanel : MonoBehaviour
    {
        public void Restart()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        public void Exit()
        {
            Application.Quit();
        }
    }
}