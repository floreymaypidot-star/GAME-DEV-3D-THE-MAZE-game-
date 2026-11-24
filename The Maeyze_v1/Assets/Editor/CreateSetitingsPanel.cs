using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using TMPro;

public static class CreateSettingsPanel
{
    [MenuItem("Tools/Create Settings Panel")]
    public static void Create()
    {
        Transform parent = Selection.activeTransform;
        if (parent == null)
        {
            GameObject c = GameObject.Find("Canvas");
            if (c != null) parent = c.transform;
        }
        if (parent == null)
        {
            EditorUtility.DisplayDialog("Settings Panel", "Select your Canvas in the Hierarchy first.", "OK");
            return;
        }

        Sprite standard = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd");
        Sprite background = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/Background.psd");
        Sprite inputField = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/InputFieldBackground.psd");
        Sprite knob = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/Knob.psd");
        Sprite checkmark = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/Checkmark.psd");
        Sprite dropdown = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/DropdownArrow.psd");
        Sprite mask = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UIMask.psd");

        var res = new DefaultControls.Resources
        {
            standard = standard,
            background = background,
            inputField = inputField,
            knob = knob,
            checkmark = checkmark,
            dropdown = dropdown,
            mask = mask
        };
        var tmpRes = new TMP_DefaultControls.Resources
        {
            standard = standard,
            background = background,
            inputField = inputField,
            knob = knob,
            checkmark = checkmark,
            dropdown = dropdown,
            mask = mask
        };

        // Panel
        GameObject panel = new GameObject("SettingsPanel", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        Undo.RegisterCreatedObjectUndo(panel, "Create Settings Panel");
        panel.transform.SetParent(parent, false);
        RectTransform prt = panel.GetComponent<RectTransform>();
        prt.anchorMin = Vector2.zero;
        prt.anchorMax = Vector2.one;
        prt.offsetMin = Vector2.zero;
        prt.offsetMax = Vector2.zero;
        panel.GetComponent<Image>().color = new Color(0f, 0f, 0f, 0.92f);

        // Title
        TMP_Text title = MakeLabel(tmpRes, panel.transform, "Title", "SETTINGS", new Vector2(0, 250), new Vector2(600, 80), 60);
        title.color = new Color(0.8f, 0.1f, 0.1f);

        // Volume
        MakeLabel(tmpRes, panel.transform, "VolumeLabel", "Volume", new Vector2(-250, 130), new Vector2(250, 40), 30);
        Slider volume = MakeSlider(res, panel.transform, "VolumeSlider", new Vector2(120, 130), 0f, 1f);

        // Sensitivity
        MakeLabel(tmpRes, panel.transform, "SensitivityLabel", "Sensitivity", new Vector2(-250, 30), new Vector2(250, 40), 30);
        Slider sens = MakeSlider(res, panel.transform, "SensitivitySlider", new Vector2(120, 30), 0.1f, 5f);

        // Fullscreen toggle
        GameObject toggleGO = DefaultControls.CreateToggle(res);
        toggleGO.name = "FullscreenToggle";
        toggleGO.transform.SetParent(panel.transform, false);
        RectTransform trt = toggleGO.GetComponent<RectTransform>();
        trt.anchorMin = trt.anchorMax = trt.pivot = new Vector2(0.5f, 0.5f);
        trt.anchoredPosition = new Vector2(-100, -70);
        trt.sizeDelta = new Vector2(300, 40);
        Text oldLabel = toggleGO.GetComponentInChildren<Text>();
        oldLabel.text = "Fullscreen";
        oldLabel.fontSize = 30;
        oldLabel.color = Color.white;
        Toggle toggle = toggleGO.GetComponent<Toggle>();

        // Back button
        GameObject btnGO = TMP_DefaultControls.CreateButton(tmpRes);
        btnGO.name = "ButtonBack";
        btnGO.transform.SetParent(panel.transform, false);
        RectTransform brt = btnGO.GetComponent<RectTransform>();
        brt.anchorMin = brt.anchorMax = brt.pivot = new Vector2(0.5f, 0.5f);
        brt.anchoredPosition = new Vector2(0, -190);
        brt.sizeDelta = new Vector2(220, 55);
        TMP_Text btnText = btnGO.GetComponentInChildren<TMP_Text>();
        btnText.text = "BACK";
        btnText.fontSize = 30;
        Button back = btnGO.GetComponent<Button>();

        // Add the script and assign its references
        SettingsPanel sp = panel.AddComponent<SettingsPanel>();
        SerializedObject so = new SerializedObject(sp);
        so.FindProperty("volumeSlider").objectReferenceValue = volume;
        so.FindProperty("sensitivitySlider").objectReferenceValue = sens;
        so.FindProperty("fullscreenToggle").objectReferenceValue = toggle;
        so.FindProperty("backButton").objectReferenceValue = back;
        Transform gameOver = parent.Find("GameOver");
        if (gameOver != null) so.FindProperty("gameOverPanel").objectReferenceValue = gameOver.gameObject;
        so.ApplyModifiedProperties();

        Selection.activeGameObject = panel;
    }

    private static TMP_Text MakeLabel(TMP_DefaultControls.Resources r, Transform parent, string name,
        string text, Vector2 pos, Vector2 size, float fontSize)
    {
        GameObject go = TMP_DefaultControls.CreateText(r);
        go.name = name;
        go.transform.SetParent(parent, false);
        RectTransform rt = go.GetComponent<RectTransform>();
        rt.anchorMin = rt.anchorMax = rt.pivot = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = pos;
        rt.sizeDelta = size;
        TMP_Text t = go.GetComponent<TMP_Text>();
        t.text = text;
        t.fontSize = fontSize;
        t.alignment = TextAlignmentOptions.Center;
        t.color = Color.white;
        return t;
    }

    private static Slider MakeSlider(DefaultControls.Resources r, Transform parent, string name,
        Vector2 pos, float min, float max)
    {
        GameObject go = DefaultControls.CreateSlider(r);
        go.name = name;
        go.transform.SetParent(parent, false);
        RectTransform rt = go.GetComponent<RectTransform>();
        rt.anchorMin = rt.anchorMax = rt.pivot = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = pos;
        rt.sizeDelta = new Vector2(300, 25);
        Slider s = go.GetComponent<Slider>();
        s.minValue = min;
        s.maxValue = max;
        s.value = 1f;
        return s;
    }
}