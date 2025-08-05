using UnityEngine;

public class GLLineTest : MonoBehaviour
{
    private static Material _lineMaterial;

    void CreateLineMaterial()
    {
        if (_lineMaterial == null)
        {
            // Built-in pipeline-friendly shader
            Shader shader = Shader.Find("Hidden/Internal-Colored");
            _lineMaterial = new Material(shader)
            {
                hideFlags = HideFlags.HideAndDontSave
            };
            _lineMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            _lineMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            _lineMaterial.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
            _lineMaterial.SetInt("_ZWrite", 0);
        }
    }

    void OnRenderObject()
    {
        CreateLineMaterial();

        _lineMaterial.SetPass(0);
        GL.PushMatrix();
        GL.MultMatrix(transform.localToWorldMatrix);
        GL.Begin(GL.LINES);
        GL.Color(Color.green);

        GL.Vertex3(-1, 0, 0);
        GL.Vertex3(1, 0, 0);

        GL.End();
        GL.PopMatrix();
    }
}
