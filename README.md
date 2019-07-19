# Legacy-Asset-Management-System

 基于Unity-5.6.7实现的一个资源管理系统

```
特点：

	1. 引用计数，自动释放
	2. 异步加载，队列执行
	
```

```
Usage Example：

using LGamekit.LegacyAssetManagementSystem;

string json = Resources.Load<TextAsset>("AssetBundleManifest.txt");

AssetBundleManifest manifest = JsonUtility.FromJson<AssetBundleManifest>(json);

string assetPath = "Assets/Cube1.prefab";

List<string> assetPaths = new List<string> {
	assetPath,
	"Assets/Cube2.prefab",
	"Assets/Cube3.prefab"
};

AssetManager.Instance.SetAssetBundleManifest(manifest)
		.LoadAssetAsync<GameObject>(assetPath, asset => {
			if(asset == null) {
				// load failed.
			} else {
				Instantiate(asset);
			}
		})
		.LoadAssetsAsync<GameObject>(assetPaths, assets => {
			for(int i = 0, len = assets.Count; i < len; i++) {
				if(assets[i] == null) {
					// load failed.
				} else {
					Instantiate(assets[i]);
				}
			}
		})
		.UnloadAssetsAsync(assetPaths)
		.UnloadAssetAsync(assetPath);
		

```

```
Q&A:

	Q1. 怎样生成AssetBundleManifest.txt
	A1. 在Project面板右键选择菜单
		LGamekit->LegacyAssetManagementSystem->AssetBundleManifestBuilder
		在AssetBundleManifestBuilder.asset的面板上
		Input->选择打包生成的AssetBundleManifest文件
		Name->生成txt的名字（通常是AssetBundleManifest.txt）
		Output->生成txt文件的文件夹
		Export->生成txt文件
		

```

```
TODO:

	1. 完整的使用例子
	2. 配套的编辑器界面
	3. 真机测试

```

LICENSE 

[MIT](LICENSE)

