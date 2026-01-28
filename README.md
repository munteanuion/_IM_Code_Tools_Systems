Installing : Unity->Package Manager->Click Button + ->Select Git URL->Paste URL: https://github.com/munteanuion/_IM_Code_Tools_Systems.git

Ready Solutions To Use For Code:
* Zenject -> https://github.com/modesttree/Zenject.git?path=UnityProject/Assets/Plugins/Zenject
* vContainer -> https://github.com/hadashiA/VContainer.git?path=VContainer/Assets/VContainer#1.17.0
* UniTask -> https://github.com/Cysharp/UniTask.git?path=src/UniTask/Assets/Plugins/UniTask
* PrimeTween -> https://github.com/KyryloKuzyk/PrimeTween?tab=readme-ov-file#install-via-unity-package-manager-upm
* SerializedDictionary -> https://github.com/ayellowpaper/SerializedDictionary.git
* SerializeRefInterface -> https://github.com/munteanuion/ReferenceInterfaces.git

Ready Solutions To Use For Editor:
* NaughtyAttributes -> https://github.com/dbrizov/NaughtyAttributes.git#upm
* HeapExplorer -> https://github.com/pschraut/UnityHeapExplorer.git#4.3.0
* FastReload -> https://github.com/handzlikchris/FastScriptReload.git?path=Assets
* JumpTo -> https://github.com/improck/jumpto.git#upm

TODO List:

* init system abstract class for all abjects with spawners and without , int for order init, and try with dependence injection
* Add Script for Behaviour Animator State for send message ca sa chemam de exemplu sunet fmod (send message de facut prin manager static event bus si sa fie dynamic subscriber acolo in dictionary cu o cheita string sau mai bine Enum daca e posibil sa nu fie apiceatca)
* Static extensions methods
* scriptable object arhitecture with reactive var and scriptable variables (ca in AC)
* save system
* bootstraper -> single entry point for scenes without place in scene atached to scene asset or name and entry point without ref in inspector and initialize on load
* Player Spawn System in gameplay and for testing in editor mode from position when is set scene windfow camera(like that unreal)
* Actions events list class and call when in code exec action event(class contain fmod event sound, enable disable obj...)
* effects class container and play all vfx,sfx,shaderAnimators when event is exec from code example shoot...
