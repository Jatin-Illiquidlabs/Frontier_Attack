using System.Linq;
using UnityEditor;
using UnityEngine;

namespace WerewolfBearer {
    public static class MenuItems {
        public const string RootPath = "Werewolf/";

        [MenuItem(RootPath + "Fixup sprites")]
        private static void FixSpriteMaterials() {
            GameObject[] prefabs =
                Selection.GetFiltered<GameObject>(SelectionMode.Assets | SelectionMode.Editable | SelectionMode.TopLevel);

            Material defaultMaterial = Resources.FindObjectsOfTypeAll<Material>().First(m => m.name == "Sprites-Default");
            Material overlayMaterial = AssetDatabase.LoadAssetAtPath<Material>("Assets/Game/Media/Materials/Sprites-ColorOverlay.mat");

            foreach (GameObject prefab in prefabs) {
                foreach (Transform childTransform in prefab.transform) {
                    GameObject childGameObject = childTransform.gameObject;
                    if (childGameObject.name != "Sprite")
                        continue;

                    SpriteRenderer spriteRenderer = childGameObject.GetComponent<SpriteRenderer>();
                    spriteRenderer.sharedMaterial = defaultMaterial;
                    spriteRenderer.color = Color.white.WithA(1);

                    EditorUtility.SetDirty(prefab);
                }
            }
        }
    }
}
