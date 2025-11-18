using UnityEngine;
using System.IO;

public class Capture : MonoBehaviour
{
    public RenderTexture renderTexture; // Render Texture를 드래그로 할당
    public string fileName = "CapturedImage.png"; // 저장될 파일 이름

    public void SaveRenderTextureToFile()
    {
        if (renderTexture == null)
        {
            Debug.LogError("RenderTexture is not assigned!");
            return;
        }

        // Render Texture의 크기와 동일한 Texture2D 생성
        Texture2D texture = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGB24, false);

        // Render Texture 활성화
        RenderTexture.active = renderTexture;

        // Render Texture의 내용을 Texture2D로 읽어오기
        texture.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        texture.Apply();

        // Render Texture 비활성화
        RenderTexture.active = null;

        // 감마 보정 적용
        ApplyGammaCorrection(texture);

        // Texture2D 데이터를 PNG 형식으로 변환
        byte[] bytes = texture.EncodeToPNG();

        // 저장 경로
        string path = Path.Combine(Application.dataPath, fileName);

        try
        {
            // 파일 저장
            File.WriteAllBytes(path, bytes);
            Debug.Log($"Render Texture saved to: {path}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to save Render Texture: {e.Message}");
        }

        // 메모리 해제
        Destroy(texture);
    }

    // 감마 보정 메서드
    private void ApplyGammaCorrection(Texture2D texture)
    {
        Color[] pixels = texture.GetPixels();
        for (int i = 0; i < pixels.Length; i++)
        {
            // Linear에서 Gamma로 변환
            pixels[i] = pixels[i].gamma;
        }
        texture.SetPixels(pixels);
        texture.Apply();
    }
}
