using UnityEngine;

[RequireComponent(typeof(Camera))]
public class GBGraphics : MonoBehaviour {
    [Range(1, 8)]
    public int mosaic = 2;
    public Material shader = null;
    private RenderTexture t = null;
    private Camera c;

    private void ReplaceRenderTexture(int w, int h) {
        t = new RenderTexture(w, h, 24);
        t.useMipMap = false;
        t.useDynamicScale = false;
        t.antiAliasing = 1;
        t.filterMode = FilterMode.Point;
    }

    private void Start() {
        c = gameObject.GetComponent<Camera>();
        c.depthTextureMode = DepthTextureMode.Depth;
        ReplaceRenderTexture(c.pixelWidth / mosaic, c.pixelHeight / mosaic);
    }

    private void OnPreRender() {
        int w = c.pixelWidth / mosaic;
        int h = c.pixelHeight / mosaic;
        if (t == null || (t.width != w || t.height != h)) {
            if (t != null) t.Release();
            ReplaceRenderTexture(w, h);
        }
        c.targetTexture = t;
    }

    private void OnPostRender() {
        if (c.targetTexture != null) {
            c.targetTexture = null;
            if (shader != null) {
                Graphics.Blit(t, null as RenderTexture, shader);
            } else {
                Graphics.Blit(t, null as RenderTexture);
            }
        }
    }
}
