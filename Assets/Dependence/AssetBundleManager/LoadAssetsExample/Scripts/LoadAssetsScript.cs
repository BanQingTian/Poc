using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.U2D;
using AssetBundles;

public class LoadAssetsScript : MonoBehaviour
{
    public Image Image;

    IEnumerator Start()
    {
        yield return ResourceManager.Instance.Initialize();

        //var loadOp = ResourceManager.LoadAssetAsync("examples/loadassetsexample/avatars", "avatar_012", typeof(Sprite));
        //yield return loadOp;

        //Image.sprite = loadOp.GetAsset<Sprite>();

        //LoadLeftCubeAsync();
    }

    public void LoadCube()
    {
        ResourceManager.LoadAssetAsync<GameObject>("examples/loadassetsexample/cube", "Cube", (GameObject prefab) =>
        {
            var cubeGo = GameObject.Instantiate(prefab);
            cubeGo.transform.position = Vector3.right * 4.0f;
        });
    }

    public async void LoadLeftCubeAsync()
    {
        var prefab = await ResourceManager.LoadAssetAsync<GameObject>("examples/loadassetsexample/cube", "Cube");

        var cubeGo = GameObject.Instantiate(prefab);
        cubeGo.transform.position = Vector3.right * -4.0f;
    }

    public async void LoadLight()
    {
        await ResourceManager.LoadLevelAsync("examples/loadassetsexample/light", "Light", true);

        //ResourceManager.Instance.LoadLevelAsync("examples/loadassetsexample/light", "Light", true, () =>
        //{

        //});
    }

    public void LoadSpriteAtlas()
    {
        // StartCoroutine(CoLoadSpriteAtlas());

        ResourceManager.LoadAssetAsync<SpriteAtlas>("examples/loadassetsexample/birds", "Birds.sd", (SpriteAtlas spriteAtlas) =>
        {
            var sprite = spriteAtlas.GetSprite("bird_01");

            Image.sprite = sprite;
        });
    }

    private IEnumerator CoLoadSpriteAtlas()
    {
        var loadOp = ResourceManager.LoadAssetAsync("examples/loadassetsexample/birds", "Birds.sd", typeof(SpriteAtlas));
        yield return loadOp;

        var spriteAtlas = loadOp.GetAsset<SpriteAtlas>();
        var sprite = spriteAtlas.GetSprite("bird_01");

        Image.sprite = sprite;
    }
}
