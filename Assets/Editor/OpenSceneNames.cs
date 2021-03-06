//Generated by SceneFind.cs 
using UnityEditor; 
using UnityEngine; 
namespace Utility { 
public class EditorMapOpener : Editor { 
	[MenuItem("Open Scene/Menu", false, 20)]
	private static void Menu() {
		OpenIf("Assets/Scenes/Menu.unity");
	}
	[MenuItem("Open Scene/JG_Columnade", false, 20)]
	private static void JG_Columnade() {
		OpenIf("Assets/JG/JG_Scenes/JG_Columnade.unity");
	}
	private static void OpenIf(string level) {
		if (EditorApplication.SaveCurrentSceneIfUserWantsTo()){
			EditorApplication.OpenScene(level);
		}
	}
}
}