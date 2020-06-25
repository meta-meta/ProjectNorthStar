using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif


[RequireComponent(typeof(Camera))]
[ExecuteInEditMode]
public class NorthStarDistortion : MonoBehaviour {
    public Material _distortionMaterial;
    public Vector4 _offsetScaling;

//    float[]  left_uv_to_rect_x = { -0.7530364531010308f,  0.8806592947908687f, -0.8357813137161849f,   0.3013989721607643f,  0.9991764544369446f, -0.2578159567698274f,  0.3278667335649757f, -0.4602577277109663f,  -0.23980700925448195f, -0.056891370605734376f, -0.1248008903440144f,  0.7641381600051023f,   0.20935445281014292f, -0.06256983016261788f,  0.25844580123833516f, -0.5098143951663658f };
//    float[]  left_uv_to_rect_y = {  0.5612597403791647f, -1.1899589356849427f,  0.4652815794139322f,  -0.2737933233160801f,  0.3774305703820774f, -0.8110333901413378f,  1.2705775357104372f, -0.7461290557575936f,  -0.19222925521894155f,  0.936404121235537f,    -1.7109388784623627f,  0.9147182510080394f,   0.33073407860855586f, -1.1463700238163494f,   1.4965795269835196f,  -0.7090919632511286f };
//    float[] right_uv_to_rect_x = { -0.2117125319456463f, -0.432262579698108f,   0.41675063901331316f, -0.14650788483832153f, 1.0941580384494245f, -0.30628109185189906f, 0.109119134429531f,   0.11642874201014344f, -0.2761527408488216f,  -0.4335709010559027f,    0.9626491769528918f, -0.5572405188216735f,   0.18342869894719088f,  0.37981945016058366f, -0.8718621504058989f,   0.5218968716935535f };
//    float[] right_uv_to_rect_y = {  1.0129568069314265f, -2.110976542118192f,   1.4108474581893895f,  -0.7746290913232183f, -0.746419837008027f,   1.747642287758405f, - 1.5753294007072252f,  0.7143402603200871f,   0.5607717274125551f,  -1.5019493985594772f,    1.2539128525783017f, -0.42999735712430215f, -0.21517910830152714f,  0.5965062719847273f,  -0.5664205050494074f,   0.18545738302854597f};
    
// @meta-meta's calibration vals (right eye bends into the distance in top-left corner)    
    float[] left_uv_to_rect_x  = { -0.9535641466910926f, 1.3794895326313577f, -0.5437566325005336f, 0.1847672761600258f, -0.18117580257808052f, 1.048838846769232f, -1.2478774039686065f, 0.6275089303391955f, 0.039672306776154465f, -0.7501544193519004f, 1.5101663882731193f, -0.9298309746178907f, -0.05062019086421077f, 0.6538637855320495f, -1.1626519787380485f, 0.7228537498189695f };
    float[] left_uv_to_rect_y  = { -0.6576307532483832f, 1.0276985776273004f, -1.5650856229802803f, 0.6435418319384787f, 2.03239160664433f, -5.103451750352895f, 8.014652522724854f, -4.1595174881273955f, -2.027394081954163f, 8.933691845305628f, -14.959713463330008f, 7.988653474151336f, 1.2964655978919313f, -5.313176584805634f, 8.765024233592163f, -4.701587766223967f};
    float[] right_uv_to_rect_x  = { -0.9890676798923423f, 2.0682073434318227f, -1.2480373153551159f, 0.5614272395727617f, -0.04027426565449377f, -0.6071784508584728f, 0.6030592718231162f, -0.4176432208606997f, 0.31209646101908967f, -0.5427869602492096f, 0.6090877049377501f, -0.10976149668398162f, -0.23365995158563763f, 0.5599655687732469f, -0.6622593173681139f, 0.2387782467392823f};
    float[] right_uv_to_rect_y  = { -0.43202499520423154f, 0.1631804792865027f, 0.028403354415422882f, 0.14812043850538092f, 1.3509918678270576f, -1.352102711477393f, 1.8515817573506055f, -1.0917253728918563f, -0.8555043469486013f, 2.67747701539129f, -4.661865999275721f, 2.7894029338827586f, 0.6538241428419629f, -2.234848234652417f, 3.8438493550862334f, -2.194947675894772f};
    
    /// <summary> Open the Calibration.json and set the distortion parameters. </summary>
    protected void UpdateDistortion() {
        bool isLeft = transform.localPosition.x < 0f;
        _distortionMaterial.SetFloatArray("uv_to_rect_x", isLeft ? left_uv_to_rect_x : right_uv_to_rect_x);
        _distortionMaterial.SetFloatArray("uv_to_rect_y", isLeft ? left_uv_to_rect_y : right_uv_to_rect_y);
        _distortionMaterial.SetMatrix    ("camera_matrix", GetComponent<Camera>().projectionMatrix);
        _distortionMaterial.SetVector    ("offset_scaling", _offsetScaling);
        _distortionMaterial.SetFloat     ("is_left", isLeft ? 0.0f : 1.0f);
    }

    void OnEnable() { UpdateDistortion(); }


    public void OnRenderImage(RenderTexture source, RenderTexture destination) {
        Graphics.Blit(source, destination, _distortionMaterial);
    }

    void Update() {
        //If the Application is fullscreen, you don't need this... but if it isn't...
#if UNITY_EDITOR
        EditorWindow[] windows = (EditorWindow[])Resources.FindObjectsOfTypeAll(System.Type.GetType("UnityEditor.GameView,UnityEditor"));
        foreach(EditorWindow window in windows) {
            var serializedObject = new SerializedObject(window);
            var targetDisplay = serializedObject.FindProperty("m_TargetDisplay");

            if (targetDisplay.intValue == 1){//window.name.Equals("NorthStar Preview")) {
                Rect pos = window.position;

                // Correct for the Titlebar+Tab and Monitor offset
                pos = new Rect(pos.x - 1920f, pos.y + 44, pos.width, pos.height - 44);


                // Pass the fullscreen UV bottom left corner of each viewport in
                // Pass the fullscreen UV scale of each viewport in...
                // Remap the UVs inside the shader... GO!

                bool isLeft = transform.localPosition.x < 0f;

                // WORKING (FOR 1080P MAIN MONITOR UVS)
                //_offsetScaling = new Vector4((pos.x + (isLeft ? pos.width/2f : 0f)) / (pos.width/2f), (pos.y + pos.height) / pos.height,
                //                              pos.width/1920f,                                        (pos.height/1080f)/2f);

                // WORKING FOR 2880x1600 NORTH STAR UVS, OFFSET 1920 TO THE RIGHT
                _offsetScaling = new Vector4((pos.x + (isLeft ? pos.width / 2f : 0f)) / (pos.width / 2f), (pos.y + pos.height) / pos.height,
                                              pos.width / 2880f,                                                  (pos.height / 1600f) / 2f);
            }
        }
#endif
        UpdateDistortion();
    }
}
