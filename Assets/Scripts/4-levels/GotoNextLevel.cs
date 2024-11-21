using UnityEngine;
using UnityEngine.SceneManagement;

public class GotoNextLevel : MonoBehaviour {
    [SerializeField] string triggeringTag;
    [SerializeField] [Tooltip("Name of scene to move to when triggering the given tag")] string sceneName;
    //[SerializeField] NumberField scoreField;

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.tag == triggeringTag) {
            other.transform.position = Vector3.zero;
            SceneManager.LoadScene(sceneName);    // Input can either be a serial number or a name; here we use name.
        }
    }
}
