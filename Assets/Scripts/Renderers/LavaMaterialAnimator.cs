using System.Threading.Tasks;
using UnityEngine;

public class LavaMaterialAnimator : MonoBehaviour
{
    [SerializeField]
    private Material lavaMaterial;

    [SerializeField]
    private Texture2D[] textureVariants;

    [SerializeField]
    private int transitionTimeMs;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        AnimateMaterial();
    }

    private async void AnimateMaterial()
    {
        while (true)
        {
            foreach (Texture2D texture in textureVariants)
            {
                lavaMaterial.mainTexture = texture;
                await Task.Delay(transitionTimeMs);
            }
        }
    }
}
